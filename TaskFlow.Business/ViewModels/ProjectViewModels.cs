using System.ComponentModel.DataAnnotations;
using TaskFlow.Resources.Resources;
namespace TaskFlow.Business.ViewModels
{
    public class ProjectCreateVM
    {
        [Display(Name = "ProjectName_Display", ResourceType = typeof(ViewModelsResource))]
        [Required(ErrorMessageResourceName = "ProjectName_Required", ErrorMessageResourceType = typeof(ViewModelsResource))]
        [MaxLength(100, ErrorMessageResourceName = "ProjectName_MaxLength", ErrorMessageResourceType = typeof(ViewModelsResource))]
        public string Name { get; set; }

        [Display(Name = "Description_Display", ResourceType = typeof(ViewModelsResource))]
        [MaxLength(500)]
        public string Description { get; set; }

        [Display(Name = "Color_Display", ResourceType = typeof(ViewModelsResource))]
        [MaxLength(20)]
        public string Color { get; set; }
    }

    public class ProjectEditVM : ProjectCreateVM
    {
        [Required]
        public Guid Guid { get; set; }

        [Display(Name = "IsActive_Display", ResourceType = typeof(ViewModelsResource))]
        public bool IsActive { get; set; }
    }

    public class ProjectListItemVM
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public int TaskCount { get; set; }
    }
}
