using NBomber.CSharp;
using NBomber.Contracts.Stats;

using var httpClient = new HttpClient();

var scenario = Scenario.Create("carga_api_checkout_simulado", async context =>
{
    try
    {
        var httpResponse = await httpClient.GetAsync("https://jsonplaceholder.typicode.com/posts/1");

        return httpResponse.IsSuccessStatusCode
            ? Response.Ok(statusCode: ((int)httpResponse.StatusCode).ToString())
            : Response.Fail(statusCode: ((int)httpResponse.StatusCode).ToString());
    }
    catch (Exception ex)
    {
        return Response.Fail(message: ex.Message);
    }
})
.WithLoadSimulations(
    Simulation.Inject(rate: 10,
    interval: TimeSpan.FromSeconds(1),
    during: TimeSpan.FromSeconds(10)),

    Simulation.Inject(rate: 50,
    interval: TimeSpan.FromSeconds(1),
    during: TimeSpan.FromSeconds(20)),

    Simulation.Inject(rate: 100,
    interval: TimeSpan.FromSeconds(1),
    during: TimeSpan.FromSeconds(15))
);

NBomberRunner
    .RegisterScenarios(scenario)
    .WithReportFolder("relatorios_desempenho")
    .WithReportFormats(ReportFormat.Html, ReportFormat.Csv)
    .Run();
