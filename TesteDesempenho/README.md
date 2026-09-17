# Demonstração Prática: Teste de Desempenho (Performance Testing)

Este pacote contém três projetos C# independentes:

```
TesteDesempenho/
├── BenchmarkDemo/       -> Teste de desempenho no nível de CÓDIGO (BenchmarkDotNet)
├── MockCheckoutApi/     -> API local "de mentira", com um gargalo proposital
└── LoadTestDemo/        -> Teste de CARGA/STRESS contra a MockCheckoutApi (NBomber)
```

Pré-requisito: .NET 8 SDK instalado (`dotnet --version` deve mostrar 8.x).

---

## 1. BenchmarkDemo — nível de código

Compara `List.Contains`, um loop manual em `Array` e `HashSet.Contains`, buscando um
elemento inexistente em coleções de 100, 10.000 e 1.000.000 de itens.

**Como rodar:**
```bash
cd BenchmarkDemo
dotnet run -c Release
```

**Onde fica o resultado:** `BenchmarkDemo/BenchmarkDotNet.Artifacts/results/` (HTML, Markdown e CSV).

---

## 2. MockCheckoutApi + LoadTestDemo — nível de API/sistema (stress test)

Esses dois projetos trabalham em conjunto: o `MockCheckoutApi` é o "sistema sob teste"
(simula um endpoint de checkout com capacidade limitada de propósito), e o `LoadTestDemo`
é quem gera a carga contra ele.

### Por que uma API local em vez de uma API pública?

Uma API pública robusta (como usamos numa primeira versão) absorve qualquer carga
didática sem esforço — o que é ótimo para provar que "não há erro", mas ruim para
ilustrar o que acontece quando um sistema *é* sobrecarregado. A `MockCheckoutApi` tem
uma capacidade conhecida e controlada (~100-150 requisições/segundo), então o teste de
carga consegue ultrapassá-la de propósito e gerar uma curva real de degradação.

A API simula:
- **Capacidade limitada** (15 "vagas" de processamento simultâneas, como um pool de conexões de banco).
- **Fila com timeout**: se não sobrar vaga em 2 segundos, responde `503` (sistema sobrecarregado).
- **Falha aleatória de dependência externa**: 2% de chance de `500`, independente da carga (simula algo como um gateway de pagamento instável).

### Como rodar (precisa de DOIS terminais abertos ao mesmo tempo)

**Terminal 1 — suba a API:**
```bash
cd MockCheckoutApi
dotnet run -c Release
```
Deixe rodando. Ela escuta em `http://localhost:5000`.

**Terminal 2 — rode o teste de carga:**
```bash
cd LoadTestDemo
dotnet run -c Release
```

O teste sobe a carga em 5 estágios (20 → 80 → 150 → 400 → 800 req/s), cada um durando
de 10 a 15 segundos. Os três primeiros estágios devem ficar estáveis; a partir do
quarto (400 req/s, bem acima da capacidade da API), devem aparecer erros `503` de
saturação, além dos `500` aleatórios.

**Onde fica o resultado:** `LoadTestDemo/relatorios_desempenho_stress/` (HTML e CSV,
com nome contendo a data/hora da execução).

### O que observar no relatório
- **Latência (P50/P95/P99) subindo** conforme os estágios avançam — evidência de fila/contenção.
- **Taxa de erro baixa (~2%) nos primeiros estágios**, subindo consideravelmente nos últimos — evidência do ponto de saturação.
- **RPS efetivo menor que o RPS "oferecido"** nos estágios mais agressivos — sinal de que o sistema não consegue processar tudo que está sendo empurrado para ele.

---

## Conceitos cobertos por essa demonstração

| Conceito | Onde aparece |
|---|---|
| Complexidade O(1) vs O(n) | BenchmarkDemo |
| Alocação de memória (Garbage Collector) | BenchmarkDemo |
| Load Testing (carga esperada) | Estágios 1-2 do LoadTestDemo |
| Stress Testing (além do limite) | Estágios 3-4 do LoadTestDemo |
| Spike Testing (pico repentino) | Estágio 5 do LoadTestDemo (salto de 400 para 800 req/s) |
| Latência (P50/P95/P99) | Relatório do NBomber |
| Taxa de erro e tipos de erro (saturação vs. falha externa) | Relatório do NBomber + lógica da MockCheckoutApi |
| Throughput (RPS) | Relatório do NBomber |

## Solução de problemas comuns

- **"contém mais de um arquivo de projeto"**: você está rodando `dotnet run` na pasta raiz `TesteDesempenho`, não dentro de uma das três subpastas. Entre na subpasta correta primeiro.
- **Erro de conflito de versão do NBomber.Contracts**: já corrigido neste pacote — o `LoadTestDemo.csproj` referencia apenas o pacote `NBomber` (sem `NBomber.Http`).
- **`Connection refused` ao rodar o LoadTestDemo**: a `MockCheckoutApi` não está rodando ou não terminou de subir — confira o Terminal 1 antes de rodar o Terminal 2.
