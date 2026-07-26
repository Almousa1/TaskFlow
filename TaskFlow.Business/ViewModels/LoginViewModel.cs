using System.ComponentModel.DataAnnotations;
using TaskFlow.Resources.Resources;

public class LoginViewModel
{
    [Display(Name = "Email_Display", ResourceType = typeof(ViewModelsResource))]
    [Required(ErrorMessageResourceName = "Email_Required", ErrorMessageResourceType = typeof(ViewModelsResource))]
    [EmailAddress(ErrorMessageResourceName = "Email_Invalid", ErrorMessageResourceType = typeof(ViewModelsResource))]
    public string Email { get; set; }

    [Display(Name = "Password_Display", ResourceType = typeof(ViewModelsResource))]
    [Required(ErrorMessageResourceName = "Password_Required", ErrorMessageResourceType = typeof(ViewModelsResource))]
    public string Password { get; set; }
}
