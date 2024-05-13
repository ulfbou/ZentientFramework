using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zentient.Tests;

public enum TestResultType
{
    Fail = 1, Success = 2, Stopped = 3
}

public class TestResult(TestResultType type, string message)
{
    public TestResultType resultType { get; set; } = type;
    public string message { get; set; } = message;
}
