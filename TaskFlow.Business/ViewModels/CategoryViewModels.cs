using System.ComponentModel.DataAnnotations;
using TaskFlow.Resources.Resources;
namespace TaskFlow.Business.ViewModels
{
    public class CategoryCreateVM
    {
        [Display(Name = "CategoryName_Display", ResourceType = typeof(ViewModelsResource))]
        [Required(ErrorMessageResourceName = "CategoryName_Required", ErrorMessageResourceType = typeof(ViewModelsResource))]
        [MaxLength(100, ErrorMessageResourceName = "CategoryName_MaxLength", ErrorMessageResourceType = typeof(ViewModelsResource))]
        public string Name { get; set; }

        [Display(Name = "Color_Display", ResourceType = typeof(ViewModelsResource))]
        [MaxLength(20)]
        public string Color { get; set; }
    }

    public class CategoryEditVM : CategoryCreateVM
    {
        [Required]
        public Guid Guid { get; set; }

        [Display(Name = "IsActive_Display", ResourceType = typeof(ViewModelsResource))]
        public bool IsActive { get; set; }
    }

    public class CategoryListItemVM
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public int TaskCount { get; set; }
    }
}
