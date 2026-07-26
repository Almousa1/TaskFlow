using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Data.Models
{
    public class tblSystemLog : Common
    {
        [StringLength(20)]
        public string Name { get; set; }
        [StringLength(20)]
        public string NameAr { get; set; }
        public int? UserId { get; set; }
        public int? StudentId { get; set; }
        [StringLength(10)]
        public string Action { get; set; }
        [StringLength(20)]
        public string TableName { get; set; }
        public int AffectedRecord { get; set; }
        [StringLength(5000)]
        public string OldValues { get; set; }
        [StringLength(5000)]
        public string NewValues { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
