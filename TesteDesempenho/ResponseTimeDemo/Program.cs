// ==============================================================
// DEMONSTRAÇÃO 2 (versão EFICIÊNCIA) — Teste de Desempenho
// Mede tempo de resposta e consumo de recursos com UM ÚNICO
// usuário, fazendo chamadas em SEQUÊNCIA (nunca em paralelo).
//
// Isso propositalmente NÃO é um teste de carga: não há
// concorrência, não há "requisições por segundo alvo", não há
// múltiplos usuários virtuais. É a medição pura dos dois outros
// pilares de desempenho da ISO 25010: comportamento no tempo
// (time behavior) e utilização de recursos (resource utilization).
//
// Sem dependências externas via NuGet — usa só HttpClient e
// System.Text.Json, que já vêm no .NET. Isso elimina de vez
// qualquer risco de conflito de pacotes.
// ==============================================================

using System.Diagnostics;
using System.Text.Json;

const string urlAlvo = "http://localhost:5000/api/checkout";
const int chamadasDeAquecimento = 20;
const int totalDeChamadas = 300;

using var httpClient = new HttpClient();

Console.WriteLine("=== TESTE DE DESEMPENHO — TEMPO DE RESPOSTA E RECURSOS (1 usuário, sequencial) ===\n");

// ---------- Aquecimento (warm-up de JIT e conexão HTTP) ----------
Console.WriteLine($"Aquecendo com {chamadasDeAquecimento} chamadas...");
for (var i = 0; i < chamadasDeAquecimento; i++)
{
    await httpClient.GetAsync(urlAlvo);
}

// ---------- Medição real: uma chamada de cada vez, em sequência ----------
var resultados = new List<Resultado>();
Console.WriteLine($"Executando {totalDeChamadas} chamadas sequenciais...\n");

for (var i = 0; i < totalDeChamadas; i++)
{
    var cronometroCliente = Stopwatch.StartNew();
    var response = await httpClient.GetAsync(urlAlvo);
    cronometroCliente.Stop();

    var sucesso = response.IsSuccessStatusCode;
    double tempoProcessamentoServidorMs = 0, memoriaGerenciadaMb = 0, memoriaFisicaMb = 0, cpuTotalMs = 0;

    if (sucesso)
    {
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

// ---------- Estatísticas agregadas ----------
var tempos = resultados.Select(r => r.TempoRespostaClienteMs).OrderBy(t => t).ToList();

double Percentil(List<double> valoresOrdenados, double p)
{
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

// Compara memória média das 20 primeiras vs 20 últimas chamadas.
// Se a memória física crescer de forma consistente ao longo do tempo,
// é um indício de vazamento de memória (memory leak) — um problema
// de desempenho clássico que só aparece em execuções longas (soak test).
var primeiras = resultados.Take(20).Where(r => r.Sucesso).Select(r => r.MemoriaFisicaMb).DefaultIfEmpty(0).Average();
var ultimas = resultados.TakeLast(20).Where(r => r.Sucesso).Select(r => r.MemoriaFisicaMb).DefaultIfEmpty(0).Average();
Console.WriteLine($"Memória física do servidor — início: {primeiras:F2}MB | fim: {ultimas:F2}MB | variação: {(ultimas - primeiras):+0.00;-0.00}MB");

// ---------- Exportar CSV ----------
var pastaSaida = "relatorios_tempo_resposta";
Directory.CreateDirectory(pastaSaida);
var caminhoCsv = Path.Combine(pastaSaida, $"tempo_resposta_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.csv");

await using (var writer = new StreamWriter(caminhoCsv))
{
    await writer.WriteLineAsync("indice,sucesso,status_code,tempo_resposta_cliente_ms,tempo_processamento_servidor_ms,memoria_gerenciada_mb,memoria_fisica_mb,cpu_total_ms");
    foreach (var r in resultados)
    {
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
