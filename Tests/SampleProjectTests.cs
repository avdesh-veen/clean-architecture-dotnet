/// <summary>
/// Illustrative example showing how handlers could be exercised in a test.
/// Replace with real xUnit/NUnit tests and Testcontainers usage in the real codebase.
/// </summary>
using SampleProject.Application;
using SampleProject.Application.Requests;
using SampleProject.Infrastructure.Persistence;

public class SampleProjectTests
{
    /// <summary>
    /// Example test that creates and retrieves a project using handlers directly.
    /// </summary>
    public async Task CreateAndGetProject()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("test").Options;
        await using var db = new ApplicationDbContext(options);

        var createHandler = new SampleProject.Application.Handlers.CreateProjectHandler(db);
        var response = await createHandler.Handle(new CreateProjectRequest("Test", "desc"), CancellationToken.None);

        var getHandler = new SampleProject.Application.Handlers.GetProjectHandler(db);
        var fetched = await getHandler.Handle(new SampleProject.Application.Requests.GetProjectRequest(response.Uuid), CancellationToken.None);
    }
}
