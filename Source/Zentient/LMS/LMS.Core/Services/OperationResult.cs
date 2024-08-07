using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Services
{
    public class OperationResult
    {
        public bool Success { get; internal set; }
        public List<string> Messages { get; internal set; }

        public OperationResult Ok(IEnumerable<string> messages)
        {
            return new OperationResult
            {
                Success = true,
                Messages = messages.ToList()
            };
        }

        public OperationResult BadRequest(IEnumerable<string> messages)
        {
            return new OperationResult
            {
                Success = false,
                Messages = messages.ToList()
            };
        }
    }
}
