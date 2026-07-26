using System.ComponentModel.DataAnnotations;
using TaskFlow.Resources.Resources;
namespace TaskFlow.Business.ViewModels
{
    public class DashboardVM
    {
        [Display(Name = "TotalTasks", ResourceType = typeof(ViewModelsResource))]
        public int TotalTasks { get; set; }

        [Display(Name = "CompletedTasks", ResourceType = typeof(ViewModelsResource))]
        public int CompletedTasks { get; set; }

        [Display(Name = "PendingTasks", ResourceType = typeof(ViewModelsResource))]
        public int PendingTasks { get; set; }

        [Display(Name = "OverdueTasks", ResourceType = typeof(ViewModelsResource))]
        public int OverdueTasks { get; set; }

        public List<TodoItemListItemVM> RecentTasks { get; set; }
        public List<ProjectListItemVM> Projects { get; set; }
        public List<CategoryListItemVM> Categories { get; set; }
    }
}
