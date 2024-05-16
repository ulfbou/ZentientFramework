using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zentient.Tests.Deprecated
{
    public static class TestExtensions
    {
        public static AssertionResult<object> AssertThat(this object instance)
        {
            return new AssertionResult<object>(instance);
        }

    }
}
