using System.ComponentModel.DataAnnotations;
using TaskFlow.Resources.Resources;
namespace TaskFlow.Business.ViewModels
{
    public class StatusCreateVM
    {
        [Display(Name = "StatusNameEn_Display", ResourceType = typeof(ViewModelsResource))]
        [Required(ErrorMessageResourceName = "StatusNameEn_Required", ErrorMessageResourceType = typeof(ViewModelsResource))]
        [MaxLength(20, ErrorMessageResourceName = "StatusNameEn_MaxLength", ErrorMessageResourceType = typeof(ViewModelsResource))]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessageResourceName = "CompanyNameEn_EnglishOnly", ErrorMessageResourceType = typeof(ViewModelsResource))]
        public string StatusName { get; set; }

        [Display(Name = "StatusNameAr_Display", ResourceType = typeof(ViewModelsResource))]
        [Required(ErrorMessageResourceName = "StatusNameAr_Required", ErrorMessageResourceType = typeof(ViewModelsResource))]
        [MaxLength(20, ErrorMessageResourceName = "StatusNameAr_MaxLength", ErrorMessageResourceType = typeof(ViewModelsResource))]
        [RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessageResourceName = "CompanyNameAr_ArabicOnly", ErrorMessageResourceType = typeof(ViewModelsResource))]
        public string StatusNameAr { get; set; }
    }

    public class StatusEditVM : StatusCreateVM
    {
        [Required]
        public Guid Guid { get; set; }

        [Display(Name = "Approved_Display", ResourceType = typeof(ViewModelsResource))]
        public bool Approved { get; set; }
    }

    public class StatusListItemVM : StatusCreateVM
    {
        public Guid Guid { get; set; }
        [Display(Name = "Approved_Display", ResourceType = typeof(ViewModelsResource))]
        public bool Approved { get; set; }
        [Display(Name = "CreationDate_Display", ResourceType = typeof(ViewModelsResource))]
        public DateTime UpdatedAt { get; set; }
    }
}
