# Demonstração Prática: Teste de Desempenho (Performance Testing)

## 1. O que é o "nível de teste" de desempenho?

Teste de desempenho é um **teste não-funcional**: não valida *o que* o sistema faz, mas *quão bem* ele faz. Ele aparece em diferentes granularidades da pirâmide de testes:

| Nível | O que mede | Ferramenta usada aqui |
|---|---|---|
| **Micro (unidade/código)** | Tempo de execução e alocação de memória de um método/algoritmo isolado | `BenchmarkDotNet` |
| **Macro (API/sistema)** | Comportamento sob carga simultânea de usuários (throughput, latência, erros) | `NBomber` |
| **Ponta a ponta (produção)** | Escalabilidade real, infraestrutura, banco de dados | JMeter, k6, Gatling (mencionados, não implementados aqui) |

### Subtipos de teste de desempenho (nível macro)
- **Load Testing**: carga esperada/normal (ex: 100 usuários simultâneos).
- **Stress Testing**: além do limite, para achar o ponto de ruptura.
- **Spike Testing**: pico repentino de requisições.
- **Soak/Endurance Testing**: carga moderada por longo período (detecta vazamento de memória).

## 2. Demonstração 1 — Nível de código (Micro-benchmark)

Arquivo: `BenchmarkDemo/Program.cs`

Compara 3 formas de buscar um elemento em uma coleção (`List<T>.Contains`, `Array` com loop manual, `HashSet<T>.Contains`), medindo tempo de execução e memória alocada com **BenchmarkDotNet**, a ferramenta padrão de mercado para isso em .NET.

Como rodar:
```bash
cd BenchmarkDemo
dotnet run -c Release
```
> Sempre em modo `Release` — benchmark em `Debug` gera números irreais.

O relatório gerado mostra colunas como `Mean`, `Error`, `StdDev` e `Allocated`, permitindo comparar visualmente qual estrutura de dados escala melhor conforme o volume de dados cresce (o exemplo testa com 100, 10.000 e 1.000.000 de itens).

## 3. Demonstração 2 — Nível de API (Load Test)

Arquivo: `LoadTestDemo/Program.cs`

Simula 50 usuários virtuais fazendo requisições simultâneas contra um endpoint HTTP durante 30 segundos, usando **NBomber**, um framework de load testing nativo em .NET (equivalente ao k6/JMeter, mas escrito em C#).

Como rodar:
```bash
cd LoadTestDemo
dotnet run -c Release
```

Ao final, o NBomber gera um relatório (HTML/CSV/Markdown) com métricas essenciais:
- **RPS** (Requests Per Second / throughput)
- **Latência** (mín, média, p50, p95, p99)
- **Taxa de erro**
- **Requisições completadas vs falhas**

## 4. Estudo de caso para apresentação

**Cenário:** API de checkout de um e-commerce (`POST /api/checkout`).

**Objetivo do teste:** validar se o endpoint suporta a carga esperada na Black Friday sem degradar a experiência do usuário.

**Critérios de aceite definidos previamente (SLA):**
- p95 de latência < 500 ms
- Taxa de erro < 1%
- Suportar 200 requisições/segundo sustentadas por 5 minutos (load test)
- Identificar o ponto de saturação ao dobrar a carga a cada 2 minutos (stress test)

**Passos da demonstração em aula:**
1. Rodar o benchmark de código (Demo 1) para mostrar que otimizações de algoritmo/estrutura de dados já reduzem custo antes mesmo de chegar à carga de rede.
2. Rodar o load test (Demo 2) contra uma API de exemplo (pode ser uma rota fake local, tipo `https://jsonplaceholder.typicode.com/posts` para fins didáticos).
3. Analisar o relatório do NBomber e comparar com os critérios de aceite do SLA.
4. Discutir: o que fazer se o SLA falhar? (cache, escalonamento horizontal, otimização de query, filas assíncronas).

## 5. Outras ferramentas do mercado (para citar na apresentação)
- **JMeter** (Java, GUI, muito usado em empresas tradicionais)
- **k6** (JavaScript, moderno, ótimo para CI/CD)
- **Gatling** (Scala, relatórios ricos)
- **Locust** (Python, testes distribuídos)
- **Azure Load Testing / AWS Distributed Load Testing** (nuvem, larga escala)
