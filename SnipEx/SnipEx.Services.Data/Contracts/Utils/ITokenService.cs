namespace SnipEx.Services.Data.Contracts.Utils
{
    using SnipEx.Data.Models;

    public interface ITokenService
    {
        Task<string> GenerateJwtTokenAsync(ApplicationUser user);
    }
}
