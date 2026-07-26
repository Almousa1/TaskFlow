using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskFlow.Resources.Resources;
namespace TaskFlow.Business.ViewModels
{
    public class TodoItemCreateVM
    {
        [Display(Name = "Title_Display", ResourceType = typeof(ViewModelsResource))]
        [Required(ErrorMessageResourceName = "Title_Required", ErrorMessageResourceType = typeof(ViewModelsResource))]
        [MaxLength(200, ErrorMessageResourceName = "Title_MaxLength", ErrorMessageResourceType = typeof(ViewModelsResource))]
        public string Title { get; set; }

        [Display(Name = "Description_Display", ResourceType = typeof(ViewModelsResource))]
        [MaxLength(2000, ErrorMessageResourceName = "Description_MaxLength", ErrorMessageResourceType = typeof(ViewModelsResource))]
        public string Description { get; set; }

        [Display(Name = "DueDate_Display", ResourceType = typeof(ViewModelsResource))]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        [Display(Name = "Priority_Display", ResourceType = typeof(ViewModelsResource))]
        [Required(ErrorMessageResourceName = "Priority_Required", ErrorMessageResourceType = typeof(ViewModelsResource))]
        [Range(1, 5, ErrorMessageResourceName = "Priority_Range", ErrorMessageResourceType = typeof(ViewModelsResource))]
        public int Priority { get; set; }

        [Display(Name = "Project_Display", ResourceType = typeof(ViewModelsResource))]
        public int? ProjectId { get; set; }

        [Display(Name = "Category_Display", ResourceType = typeof(ViewModelsResource))]
        public int? CategoryId { get; set; }

        [Display(Name = "Status_Display", ResourceType = typeof(ViewModelsResource))]
        [Required(ErrorMessageResourceName = "Status_Required", ErrorMessageResourceType = typeof(ViewModelsResource))]
        public int StatusId { get; set; }

        public IEnumerable<SelectListItem> ProjectOptions { get; set; }
        public IEnumerable<SelectListItem> CategoryOptions { get; set; }
        public IEnumerable<SelectListItem> StatusOptions { get; set; }
    }

    public class TodoItemEditVM : TodoItemCreateVM
    {
        [Required]
        public Guid Guid { get; set; }

        [Display(Name = "IsCompleted_Display", ResourceType = typeof(ViewModelsResource))]
        public bool IsCompleted { get; set; }

        [Display(Name = "IsActive_Display", ResourceType = typeof(ViewModelsResource))]
        public bool IsActive { get; set; }
    }

    public class TodoItemListItemVM
    {
        public Guid Guid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public int Priority { get; set; }
        public string ProjectName { get; set; }
        public string CategoryName { get; set; }
        public string StatusName { get; set; }
        public string StatusNameAr { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
