using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Data.Models
{
    public class tblUserRole : Common
    {
        [StringLength(25)]
        public string RoleName { get; set; }
        [StringLength(25)]
        public string RoleNameAr { get; set; }
    }
}
