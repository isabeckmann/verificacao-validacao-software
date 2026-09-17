using NBomber.CSharp;

using var httpClient = new HttpClient();

var scenario = Scenario.Create("carga_api_checkout_local", async context => {
    try {
        var httpResponse = await httpClient.GetAsync("http://localhost:5000/api/checkout");

        return httpResponse.IsSuccessStatusCode
            ? Response.Ok(statusCode: ((int)httpResponse.StatusCode).ToString())
            : Response.Fail(statusCode: ((int)httpResponse.StatusCode).ToString());
    } catch (Exception ex) {
        return Response.Fail(message: ex.Message);
    }
}) 
.WithLoadSimulations(
    Simulation.Inject(rate: 20, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(15)),

    Simulation.Inject(rate: 80, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(15)),

    Simulation.Inject(rate: 150, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(15)),

    Simulation.Inject(rate: 400, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(15)),

    Simulation.Inject(rate: 800, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(10))
);

NBomberRunner
    .RegisterScenarios(scenario)
    .WithReportFolder("relatorios_desempenho_stress")
    .WithReportFormats(NBomber.Contracts.Stats.ReportFormat.Html, NBomber.Contracts.Stats.ReportFormat.Csv)
    .Run();