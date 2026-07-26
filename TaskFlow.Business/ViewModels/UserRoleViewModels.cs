using System.ComponentModel.DataAnnotations;
using TaskFlow.Resources.Resources;
namespace TaskFlow.Business.ViewModels
{
    public class UserRoleCreateVM
    {
        [Display(Name = "RoleNameEn_Display", ResourceType = typeof(ViewModelsResource))]
        [Required(ErrorMessageResourceName = "User_Role_NameRequired", ErrorMessageResourceType = typeof(ViewModelsResource))]
        [MaxLength(25, ErrorMessageResourceName = "RoleNameEn_MaxLength", ErrorMessageResourceType = typeof(ViewModelsResource))]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessageResourceName = "CompanyNameEn_EnglishOnly", ErrorMessageResourceType = typeof(ViewModelsResource))]
        public string RoleName { get; set; }

        [Display(Name = "RoleNameAr_Display", ResourceType = typeof(ViewModelsResource))]
        [Required(ErrorMessageResourceName = "User_Role_NameRequiredAr", ErrorMessageResourceType = typeof(ViewModelsResource))]
        [MaxLength(25, ErrorMessageResourceName = "RoleNameAr_MaxLength", ErrorMessageResourceType = typeof(ViewModelsResource))]
        [RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessageResourceName = "CompanyNameAr_ArabicOnly", ErrorMessageResourceType = typeof(ViewModelsResource))]
        public string RoleNameAr { get; set; }
    }

    public class UserRoleEditVM : UserRoleCreateVM
    {
        public int Id { get; set; }
        [Required]
        public Guid Guid { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserRoleListItemVM
    {
        public Guid Guid { get; set; }
        [Display(Name = "RoleNameEn_Display", ResourceType = typeof(ViewModelsResource))]
        public string RoleName { get; set; }
        [Display(Name = "RoleNameAr_Display", ResourceType = typeof(ViewModelsResource))]
        public string RoleNameAr { get; set; }
        [Display(Name = "CreationDate_Display", ResourceType = typeof(ViewModelsResource))]
        public DateTime CreationDate { get; set; }
        [Display(Name = "IsActive_Display", ResourceType = typeof(ViewModelsResource))]
        public bool IsActive { get; set; }
    }
}
