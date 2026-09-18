using System.Diagnostics;
using System.Text.Json;
using ScottPlot;

const string urlAlvo = "http://localhost:5000/api/checkout";
const int chamadasDeAquecimento = 20;
const int totalDeChamadas = 300;

using var httpClient = new HttpClient();

Console.WriteLine("=== TESTE DE DESEMPENHO — TEMPO DE RESPOSTA E RECURSOS (1 usuário, sequencial) ===\n");

Console.WriteLine($"Iniciando com {chamadasDeAquecimento} chamadas...");
for (var i = 0; i < chamadasDeAquecimento; i++) {
    await httpClient.GetAsync(urlAlvo);
}

var resultados = new List<Resultado>();
Console.WriteLine($"Executando {totalDeChamadas} chamadas sequenciais...\n");

for (var i = 0; i < totalDeChamadas; i++) {
    var cronometroCliente = Stopwatch.StartNew();
    var response = await httpClient.GetAsync(urlAlvo);
    cronometroCliente.Stop();

    var sucesso = response.IsSuccessStatusCode;
    double tempoProcessamentoServidorMs = 0, memoriaGerenciadaMb = 0, memoriaFisicaMb = 0, cpuTotalMs = 0;

    if (sucesso) {
        var corpo = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(corpo);
        tempoProcessamentoServidorMs = json.RootElement.GetProperty("tempoProcessamentoMs").GetDouble();
        memoriaGerenciadaMb = json.RootElement.GetProperty("memoriaGerenciadaMb").GetDouble();
        memoriaFisicaMb = json.RootElement.GetProperty("memoriaFisicaMb").GetDouble();
        cpuTotalMs = json.RootElement.GetProperty("cpuTotalMs").GetDouble();
    }

    resultados.Add(new Resultado(
        i + 1, sucesso, (int)response.StatusCode,
        cronometroCliente.Elapsed.TotalMilliseconds,
        tempoProcessamentoServidorMs, memoriaGerenciadaMb, memoriaFisicaMb, cpuTotalMs));

    if ((i + 1) % 50 == 0)
        Console.WriteLine($"  {i + 1}/{totalDeChamadas} chamadas concluídas...");
}

var tempos = resultados.Select(r => r.TempoRespostaClienteMs).OrderBy(t => t).ToList();

double Percentil(List<double> valoresOrdenados, double p) {
    var indice = (int)Math.Ceiling(p / 100.0 * valoresOrdenados.Count) - 1;
    return valoresOrdenados[Math.Clamp(indice, 0, valoresOrdenados.Count - 1)];
}

var falhas = resultados.Count(r => !r.Sucesso);
var taxaDeErro = falhas / (double)totalDeChamadas * 100;

Console.WriteLine("\n=== RESUMO ===");
Console.WriteLine($"Total de chamadas: {totalDeChamadas}");
Console.WriteLine($"Falhas: {falhas} ({taxaDeErro:F2}%)");
Console.WriteLine($"Tempo de resposta — mín: {tempos.Min():F2}ms | média: {tempos.Average():F2}ms | máx: {tempos.Max():F2}ms");
Console.WriteLine($"Tempo de resposta — P50: {Percentil(tempos, 50):F2}ms | P95: {Percentil(tempos, 95):F2}ms | P99: {Percentil(tempos, 99):F2}ms");

var primeiras = resultados.Take(20).Where(r => r.Sucesso).Select(r => r.MemoriaFisicaMb).DefaultIfEmpty(0).Average();
var ultimas = resultados.TakeLast(20).Where(r => r.Sucesso).Select(r => r.MemoriaFisicaMb).DefaultIfEmpty(0).Average();
Console.WriteLine($"Memória física do servidor — início: {primeiras:F2}MB | fim: {ultimas:F2}MB | variação: {(ultimas - primeiras):+0.00;-0.00}MB");

var pastaSaida = "relatorios_tempo_resposta";
Directory.CreateDirectory(pastaSaida);
var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
var caminhoCsv = Path.Combine(pastaSaida, $"tempo_resposta_{timestamp}.csv");

await using (var writer = new StreamWriter(caminhoCsv)) {
    await writer.WriteLineAsync("indice,sucesso,status_code,tempo_resposta_cliente_ms,tempo_processamento_servidor_ms,memoria_gerenciada_mb,memoria_fisica_mb,cpu_total_ms");
    foreach (var r in resultados) {
        await writer.WriteLineAsync(
            $"{r.Indice},{r.Sucesso},{r.StatusCode}," +
            $"{r.TempoRespostaClienteMs.ToString("F3", System.Globalization.CultureInfo.InvariantCulture)}," +
            $"{r.TempoProcessamentoServidorMs.ToString("F3", System.Globalization.CultureInfo.InvariantCulture)}," +
            $"{r.MemoriaGerenciadaMb.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}," +
            $"{r.MemoriaFisicaMb.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}," +
            $"{r.CpuTotalMs.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}");
    }
}

Console.WriteLine($"\nCSV salvo em: {caminhoCsv}");

var x = resultados.Select(r => (double)r.Indice).ToArray();
var y = resultados.Select(r => r.TempoRespostaClienteMs).ToArray();

var plt = new ScottPlot.Plot();
var scatter = plt.Add.Scatter(x, y);
scatter.LineWidth = 1.5f;
scatter.MarkerSize = 3;

plt.Title("Tempo de Resposta por Requisição");
plt.XLabel("Número da Requisição");
plt.YLabel("Tempo de Resposta (ms)");

var caminhoPng = Path.Combine(pastaSaida, $"grafico_tempo_resposta_{timestamp}.png");
plt.SavePng(caminhoPng, 1000, 500);

Console.WriteLine($"Gráfico salvo em: {caminhoPng}");

record Resultado(
    int Indice,
    bool Sucesso,
    int StatusCode,
    double TempoRespostaClienteMs,
    double TempoProcessamentoServidorMs,
    double MemoriaGerenciadaMb,
    double MemoriaFisicaMb,
    double CpuTotalMs
);