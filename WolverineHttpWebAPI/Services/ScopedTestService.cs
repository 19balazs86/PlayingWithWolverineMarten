namespace WolverineHttpWebAPI.Services;

public sealed class ScopedTestService
{
    private readonly string _id = Guid.NewGuid().ToString();

    public string GetScopeId() => _id;
}