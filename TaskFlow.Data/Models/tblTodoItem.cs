using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Data.Models
{
    public class tblTodoItem : Common
    {
        [StringLength(200)]
        public string Title { get; set; }
        [StringLength(2000)]
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public int Priority { get; set; }
        public int? ProjectId { get; set; }
        public tblProject Project { get; set; }
        public int? CategoryId { get; set; }
        public tblCategory Category { get; set; }
        public int UserId { get; set; }
        public tblSystemUser User { get; set; }
        public int StatusId { get; set; }
        public tblStatus Status { get; set; }
    }
}
