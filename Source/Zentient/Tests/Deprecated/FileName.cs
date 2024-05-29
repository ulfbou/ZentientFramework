using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zentient.Tests.Deprecated
{
    public static class FileName
    {
        public static void FileTester()
        {
            // Arrange
            Func<object> methodToTest = () => throw new ArgumentException("Expected exception");

            object value = methodToTest.AssertThat()
                ;
        }

        public static void FileTested<T>() where T : Exception, new()
        {
            throw new T();
        }
    }
}
