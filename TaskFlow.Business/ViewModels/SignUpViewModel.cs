using System.ComponentModel.DataAnnotations;
using TaskFlow.Resources.Resources;

namespace TaskFlow.Business.ViewModels
{
    public class SignUpViewModel
    {
        [Display(Name = "Name_Display", ResourceType = typeof(ViewModelsResource))]
        [Required(ErrorMessageResourceName = "Name_Required", ErrorMessageResourceType = typeof(ViewModelsResource))]
        [MaxLength(100, ErrorMessageResourceName = "Name_MaxLength", ErrorMessageResourceType = typeof(ViewModelsResource))]
        public string Name { get; set; }

        [Display(Name = "NameAr_Display", ResourceType = typeof(ViewModelsResource))]
        [MaxLength(100, ErrorMessageResourceName = "NameAr_MaxLength", ErrorMessageResourceType = typeof(ViewModelsResource))]
        public string NameAr { get; set; }

        [Display(Name = "Email_Display", ResourceType = typeof(ViewModelsResource))]
        [Required(ErrorMessageResourceName = "Email_Required", ErrorMessageResourceType = typeof(ViewModelsResource))]
        [EmailAddress(ErrorMessageResourceName = "Email_Invalid", ErrorMessageResourceType = typeof(ViewModelsResource))]
        public string Email { get; set; }

        [Display(Name = "Password_Display", ResourceType = typeof(ViewModelsResource))]
        [Required(ErrorMessageResourceName = "Password_Required", ErrorMessageResourceType = typeof(ViewModelsResource))]
        [MinLength(8, ErrorMessageResourceName = "Password_MinLength", ErrorMessageResourceType = typeof(ViewModelsResource))]
        public string Password { get; set; }

        [Display(Name = "ConfirmPassword_Display", ResourceType = typeof(ViewModelsResource))]
        [Compare("Password", ErrorMessageResourceName = "ConfirmPassword_NotMatch", ErrorMessageResourceType = typeof(ViewModelsResource))]
        public string ConfirmPassword { get; set; }
    }
}