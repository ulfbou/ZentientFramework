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
