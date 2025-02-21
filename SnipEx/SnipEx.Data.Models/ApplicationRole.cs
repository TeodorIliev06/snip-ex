namespace SnipEx.Data.Models
{
    using Microsoft.AspNetCore.Identity;

    public class ApplicationRole : IdentityRole<Guid>
    {
        public ApplicationRole()
        {

        }

        public ApplicationRole(string roleName)
            : this()
        {
            this.Name = roleName;
        }
    }
}
