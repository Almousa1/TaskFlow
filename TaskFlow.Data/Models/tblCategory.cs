using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Data.Models
{
    public class tblCategory : Common
    {
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(20)]
        public string Color { get; set; }
        public int UserId { get; set; }
        public tblSystemUser User { get; set; }
    }
}
