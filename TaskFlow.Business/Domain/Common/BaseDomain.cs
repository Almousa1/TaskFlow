using TaskFlow.Data.Models;
using TaskFlow.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Business.Domain.Common
{
    public class BaseDomain
    {
        private TaskFlowContext context;
        internal TaskFlowContext _context
        {
            get
            {
                if (context == null)
                {
                    context = new TaskFlowContext();
                    return context;
                }
                else
                    return context;
            }
        }
    }
}
