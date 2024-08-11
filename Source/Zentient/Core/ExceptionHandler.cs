using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zentient.Core
{
    public delegate Task ExceptionHandler(Exception ex, CancellationToken cancellation = default);
}
