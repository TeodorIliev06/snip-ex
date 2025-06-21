namespace SnipEx.Services.Data.Contracts
{
    using SnipEx.Web.ViewModels.Admin;

    public interface IManagerService
    {
        Task<IEnumerable<ManageUserViewModel>> GetAllUsersWithBanStatusAsync();

        Task<bool> ToggleBanStatusAsync(string userId);
    }
}
