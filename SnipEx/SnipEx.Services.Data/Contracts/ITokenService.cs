namespace SnipEx.Services.Data.Contracts
{
    using SnipEx.Data.Models;

    public interface ITokenService
    {
        Task<string> GenerateJwtTokenAsync(ApplicationUser user);
    }
}
