using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var random = new Random();
var processoAtual = Process.GetCurrentProcess();

app.MapGet("/api/checkout", async () => {
    if (random.NextDouble() < 0.02) {
        return Results.StatusCode(500);
    }

    var cronometro = Stopwatch.StartNew();

    var tempoDeProcessamento = random.Next(50, 150);
    await Task.Delay(tempoDeProcessamento);

    cronometro.Stop();
    processoAtual.Refresh();

    var memoriaGerenciadaMb = Math.Round(GC.GetTotalMemory(false) / 1024.0 / 1024.0, 2);
    var memoriaFisicaMb = Math.Round(processoAtual.WorkingSet64 / 1024.0 / 1024.0, 2);
    var cpuTotalMs = Math.Round(processoAtual.TotalProcessorTime.TotalMilliseconds, 2);

    return Results.Ok(new {
        status = "aprovado",
        tempoProcessamentoMs = cronometro.Elapsed.TotalMilliseconds,
        memoriaGerenciadaMb,
        memoriaFisicaMb,
        cpuTotalMs
    });
});

app.Run("http://localhost:5000");
