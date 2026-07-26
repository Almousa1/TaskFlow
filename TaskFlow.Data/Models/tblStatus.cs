using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Data.Models
{
    public class tblStatus : Common
    {
        [StringLength(20)]
        public string StatusName { get; set; }
        [StringLength(20)]
        public string StatusNameAr { get; set; }
    }
}
