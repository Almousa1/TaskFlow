using System.ComponentModel.DataAnnotations;
using TaskFlow.Resources.Resources;
namespace TaskFlow.Business.ViewModels
{
    public class UserCreateVM
    {
        [Display(Name = "Email_Display", ResourceType = typeof(ViewModelsResource))]
        [Required(ErrorMessageResourceName = "Email_Required", ErrorMessageResourceType = typeof(ViewModelsResource))]
        [EmailAddress(ErrorMessageResourceName = "Email_Invalid", ErrorMessageResourceType = typeof(ViewModelsResource))]
        [MaxLength(100)]
        public string Email { get; set; }

        [Display(Name = "Name_Display", ResourceType = typeof(ViewModelsResource))]
        [Required(ErrorMessageResourceName = "Name_Required", ErrorMessageResourceType = typeof(ViewModelsResource))]
        [MaxLength(100, ErrorMessageResourceName = "Name_MaxLength", ErrorMessageResourceType = typeof(ViewModelsResource))]
        public string Name { get; set; }

        [Display(Name = "NameAr_Display", ResourceType = typeof(ViewModelsResource))]
        [Required(ErrorMessageResourceName = "NameAr_Required", ErrorMessageResourceType = typeof(ViewModelsResource))]
        [MaxLength(100, ErrorMessageResourceName = "NameAr_MaxLength", ErrorMessageResourceType = typeof(ViewModelsResource))]
        public string NameAr { get; set; }

        [Display(Name = "Password_Display", ResourceType = typeof(ViewModelsResource))]
        [Required(ErrorMessageResourceName = "Password_Required", ErrorMessageResourceType = typeof(ViewModelsResource))]
        [MinLength(8, ErrorMessageResourceName = "Password_MinLength", ErrorMessageResourceType = typeof(ViewModelsResource))]
        public string Password { get; set; }

        [Compare("Password", ErrorMessageResourceName = "Required_Field", ErrorMessageResourceType = typeof(ViewModelsResource))]
        public string ConfirmPassword { get; set; }

        public int UserRoleId { get; set; }
    }

    public class UserEditVM
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }

        [Display(Name = "Email_Display", ResourceType = typeof(ViewModelsResource))]
        [Required(ErrorMessageResourceName = "Email_Required", ErrorMessageResourceType = typeof(ViewModelsResource))]
        [EmailAddress(ErrorMessageResourceName = "Email_Invalid", ErrorMessageResourceType = typeof(ViewModelsResource))]
        public string Email { get; set; }

        [Display(Name = "Name_Display", ResourceType = typeof(ViewModelsResource))]
        [Required(ErrorMessageResourceName = "Name_Required", ErrorMessageResourceType = typeof(ViewModelsResource))]
        public string Name { get; set; }

        [Display(Name = "NameAr_Display", ResourceType = typeof(ViewModelsResource))]
        [Required(ErrorMessageResourceName = "NameAr_Required", ErrorMessageResourceType = typeof(ViewModelsResource))]
        public string NameAr { get; set; }

        [Display(Name = "IsActive_Display", ResourceType = typeof(ViewModelsResource))]
        public bool IsActive { get; set; }

        public int UserRoleId { get; set; }
    }

    public class UserListItemVM
    {
        public Guid Guid { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public int? UserRoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleNameAr { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
