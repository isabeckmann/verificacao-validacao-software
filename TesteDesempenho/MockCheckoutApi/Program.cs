// ==============================================================
// API DE CHECKOUT "DE MENTIRA" — versão de EFICIÊNCIA (sem carga)
//
// Diferença em relação à versão anterior: não existe mais fila,
// semáforo ou limite de concorrência — porque aqui não estamos
// testando "quantos usuários simultâneos ela aguenta" (isso é
// teste de CARGA). Estamos testando "quanto tempo uma operação
// leva e quanto recurso ela consome", que é teste de DESEMPENHO
// no sentido estrito (tempo de resposta + utilização de recursos,
// os pilares de "eficiência de desempenho" da ISO 25010).
//
// Por isso a própria API devolve, em cada resposta, informações
// sobre o próprio consumo de memória e CPU no momento em que
// processou aquela requisição.
// ==============================================================

using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var random = new Random();
var processoAtual = Process.GetCurrentProcess();

app.MapGet("/api/checkout", async () =>
{
    // Falha aleatória de "dependência externa" (ex: gateway de pagamento instável).
    // Mantido de propósito: mesmo em um cenário de 1 usuário só, sistemas reais
    // têm uma taxa de erro de fundo que não depende de volume de requisições.
    if (random.NextDouble() < 0.02) // 2%
    {
        return Results.StatusCode(500);
    }

    var cronometro = Stopwatch.StartNew();

    // Simula processamento real de um checkout (validação de pedido,
    // cálculo de frete, etc.) — tempo variável, como na vida real.
    var tempoDeProcessamento = random.Next(50, 150);
    await Task.Delay(tempoDeProcessamento);

    cronometro.Stop();

    // Métricas de recurso do PRÓPRIO processo da API, coletadas no instante
    // da resposta. Isso é "utilização de recursos" — não tem nenhuma relação
    // com concorrência ou número de usuários simultâneos.
    processoAtual.Refresh();
    var memoriaGerenciadaMb = Math.Round(GC.GetTotalMemory(false) / 1024.0 / 1024.0, 2);
    var memoriaFisicaMb = Math.Round(processoAtual.WorkingSet64 / 1024.0 / 1024.0, 2);
    var cpuTotalMs = Math.Round(processoAtual.TotalProcessorTime.TotalMilliseconds, 2);

    return Results.Ok(new
    {
        status = "aprovado",
        tempoProcessamentoMs = cronometro.Elapsed.TotalMilliseconds,
        memoriaGerenciadaMb,
        memoriaFisicaMb,
        cpuTotalMs
    });
});

app.Run("http://localhost:5000");
