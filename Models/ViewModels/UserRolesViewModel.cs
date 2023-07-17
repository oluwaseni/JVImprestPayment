using Microsoft.AspNetCore.Mvc.Rendering;

namespace JV_Imprest_Payment.Models.ViewModels
{
    public class UserRolesViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public IEnumerable<SelectListItem> Roles { get; set; }
        public int RoleId { get; set; }
    }

}
