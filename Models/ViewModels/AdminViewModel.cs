using Microsoft.AspNetCore.Identity;

namespace JV_Imprest_Payment.Models.ViewModels
{
    public class AdminViewModel
    {
        public List<IdentityUser> Users { get; set; }
        public List<IdentityRole> Roles { get; set; }
        public Dictionary<string, List<string>> UserRoles { get; set; }
    }


}
