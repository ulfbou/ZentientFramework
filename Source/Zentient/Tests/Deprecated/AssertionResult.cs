namespace Zentient.Tests.Deprecated;

public partial class AssertionResult<T> where T : new()
{
    private readonly T subject;

    public AssertionResult(T subject)
    {
        this.subject = subject ?? throw new ArgumentNullException("Subject must be non-null");
    }

    // Allows chaining of more assertions
    public AssertionResult<T> And
    {
        get { return this; }
    }

    public AssertionResult<T> BeEqualTo(T expected)
    {
        if (!Equals(subject, expected))
        {
            throw new AssertionException($"Expected {subject} to be equal to {expected}.");
        }

        return this;
    }

    public AssertionResult<T> Throws<TException>() where TException : Exception
    {
        if (!(subject is Action action))
        {
            throw new AssertionException($"Subject is of type `{subject.GetType().Name}` and cannot be tested to throw an action.");
        }

        try
        {
            action();
        }
        catch (Exception ex)
        {
            if (ex.GetType() != typeof(TException))
            {
                throw new AssertionException($"Expected `{GetTypeName<TException>()}` to be thrown, but {ex.GetType().Name} was thrown instead.");
            }

            return this;
        }

        throw new AssertionException($"Expected `{GetTypeName<TException>()}` to be thrown, but no exception was actually thrown.");
    }

    public AssertionResult<T> ThrowsDerived<TException>() where TException : Exception
    {
        if (!(subject is Action action))
        {
            throw new AssertionException($"Subject is of type `{subject.GetType().Name}` and cannot be tested to throw an action.");
        }

        try
        {
            action();
        }
        catch (Exception ex)
        {
            if (!IsDerivedFrom<TException>(ex.GetType()))
            {
                throw new AssertionException($"Expected `{GetTypeName<TException>()}` to be thrown, but {ex.GetType().Name} was thrown instead.");
            }

            return this;
        }

        throw new AssertionException($"Expected `{GetTypeName<TException>()}` to be thrown, but no exception was actually thrown.");
    }

    private bool IsDerivedFrom<T>(Type? subType) where T : class
    {
        return subType != null && subType.IsSubclassOf(typeof(T));
    }

    private static string GetTypeName<T>() where T : Exception
    {
        T? instance = CreateInstance<T>();

        return (instance is null ? typeof(T) : instance.GetType()).Name;
    }

    private static T? CreateInstance<T>() where T : Exception
    {
        T? instance = null;

        try
        {
            instance = Activator.CreateInstance(typeof(T)) as T;
        }
        catch
        { }

        return instance;
    }

}
