namespace Zentient.Tests;
public class TestLogEntry
{
    public string TestName { get; set; }
    public string TestResult { get; set; }
    public TimeSpan Duration { get; set; }
    public string Exception { get; set; }
    public string StackTrace { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Environment { get; set; }
    public string TestOutput { get; set; }
}
