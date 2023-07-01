namespace WolverineHttpWebAPI.Services;

public interface IScopedTestService
{
    string GetScopeId();
}

public sealed class ScopedTestService : IScopedTestService
{
    private readonly string _id = Guid.NewGuid().ToString();

    public string GetScopeId() => _id;
}