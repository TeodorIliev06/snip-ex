// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

namespace SnipEx.Web.Areas.Identity.Pages.Account
{
    using System.Security.Claims;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    using SnipEx.Common;
    using SnipEx.Data.Models;
    using SnipEx.Services.Data.Contracts.Utils;

    public class LoginModel(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ILogger<LoginModel> logger,
        ITokenService tokenService) : PageModel
    {
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var user = await userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid Email!");
                    await PopulateExternalLoginsAsync(returnUrl);
                    return Page();
                }

                if (user.IsBanned)
                {
                    Response.StatusCode = 403;
                    return RedirectToAction("Error", "Error", new { statusCode = 403 });
                }

                var result = await signInManager.PasswordSignInAsync(user.UserName!, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var token = await tokenService.GenerateJwtTokenAsync(user);

                    Response.Cookies.Append("JwtToken", token, new CookieOptions
                    {
                        HttpOnly = false,
                        Expires = DateTime.Now.AddMinutes(JwtSettings.ExpiryMinutes),
                        Secure = true,
                        SameSite = SameSiteMode.None
                    });

                    logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    await PopulateExternalLoginsAsync(returnUrl);
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            await PopulateExternalLoginsAsync(returnUrl);
            return Page();
        }

        public async Task<IActionResult> OnPostExternalLoginAsync(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Page("./Login", pageHandler: "Callback", values: new { returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                await PopulateExternalLoginsAsync(returnUrl);

                return Page();
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information.");
                await PopulateExternalLoginsAsync(returnUrl);

                return Page();
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await signInManager
                .ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
                    isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                var email = GetEmailFromExternalLogin(info);
                if (email != null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true
                    };
                    var createResult = await userManager.CreateAsync(user);

                    if (createResult.Succeeded)
                    {
                        createResult = await userManager.AddLoginAsync(user, info);
                        if (createResult.Succeeded)
                        {
                            await signInManager.SignInAsync(user, isPersistent: false);
                            logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                            return LocalRedirect(returnUrl);
                        }
                    }

                    foreach (var error in createResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    await PopulateExternalLoginsAsync(returnUrl);

                    return Page();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email claim not received from external provider.");
                    await PopulateExternalLoginsAsync(returnUrl);

                    return Page();
                }
            }
        }

        private async Task PopulateExternalLoginsAsync(string returnUrl = null)
        {
            ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ReturnUrl = returnUrl ?? Url.Content("~/");
        }

        private string GetEmailFromExternalLogin(ExternalLoginInfo info)
        {
            if (info.LoginProvider == "GitHub")
            {
                return info.Principal.FindFirstValue(ClaimTypes.Email)
                       ?? info.Principal.FindFirstValue("urn:github:email");
            }

            return info.Principal.FindFirstValue(ClaimTypes.Email);
        }
    }
}
