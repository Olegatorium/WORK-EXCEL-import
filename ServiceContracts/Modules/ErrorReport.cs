using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.Modules
{
    public class ErrorReport
    {
        public string? SenderWorkCode { get; set; }
        public string Error { get; set; }
    }
}
