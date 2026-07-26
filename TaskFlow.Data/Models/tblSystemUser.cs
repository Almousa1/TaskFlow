using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Data.Models
{
    public class tblSystemUser : Common
    {
        [StringLength(100)]
        public string Email { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(100)]
        public string NameAr { get; set; }
        [StringLength(500)]
        public string Password { get; set; }
        public int UserRoleId { get; set; }
        public tblUserRole UserRole { get; set; }
    }
}
