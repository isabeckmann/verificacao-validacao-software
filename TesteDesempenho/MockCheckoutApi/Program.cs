var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var random = new Random();

const int capacidadeMaxima = 15;
var semaforo = new SemaphoreSlim(capacidadeMaxima, capacidadeMaxima);
var requisicoesEmAndamento = 0;

app.MapGet("/api/checkout", async () => {
    if (random.NextDouble() < 0.02) {
        return Results.StatusCode(500);
    }

    var conseguiuVaga = await semaforo.WaitAsync(TimeSpan.FromSeconds(2));
    if (!conseguiuVaga) {
        return Results.StatusCode(503);
    }

    try {
        Interlocked.Increment(ref requisicoesEmAndamento);

        var tempoBase = random.Next(50, 150);
        var penalidadeDeFila = Math.Max(0, requisicoesEmAndamento - 5) * 15;
        await Task.Delay(tempoBase + penalidadeDeFila);

        return Results.Ok(new { status = "aprovado" });
    } finally {
        Interlocked.Decrement(ref requisicoesEmAndamento);
        semaforo.Release();
    }
});

app.Run("http://localhost:5000");