# Demonstração Prática: Teste de Desempenho (sem sobreposição com Teste de Carga)

Este pacote contém três projetos C# independentes:

```
TesteDesempenho/
├── BenchmarkDemo/       -> Nível de CÓDIGO (BenchmarkDotNet) — comportamento no tempo de algoritmos
├── MockCheckoutApi/     -> API local "de mentira", que reporta suas próprias métricas de recurso
└── ResponseTimeDemo/    -> Nível de API — tempo de resposta e uso de recursos, 1 usuário, sem concorrência
```

Pré-requisito: .NET 8 SDK instalado (`dotnet --version` deve mostrar 8.x).

---

## Por que essa versão não é um teste de carga

Teste de carga usa **vários usuários/requisições simultâneas** para descobrir a capacidade
máxima de um sistema. Esta demonstração faz o oposto de propósito: **um único usuário,
chamando a API em sequência**, sem nenhuma requisição em paralelo. O objetivo não é achar
o limite do sistema — é medir, com precisão, dois dos três pilares de desempenho definidos
pela ISO 25010:

1. **Comportamento no tempo** (time behavior) — quanto tempo cada operação leva.
2. **Utilização de recursos** (resource utilization) — quanta memória e CPU cada operação consome.

(O terceiro pilar, **capacidade**, é justamente o que testes de carga/stress investigam —
por isso ele fica de fora aqui, de propósito.)

---

## 1. BenchmarkDemo — nível de código

Compara `List.Contains`, um loop manual em `Array` e `HashSet.Contains`, buscando um
elemento inexistente em coleções de 100, 10.000 e 1.000.000 de itens.

```bash
cd BenchmarkDemo
dotnet run -c Release
```

Resultado em: `BenchmarkDemo/BenchmarkDotNet.Artifacts/results/`

---

## 2. MockCheckoutApi + ResponseTimeDemo — nível de API (eficiência, sem carga)

### O que a API faz

A cada requisição, além de simular o processamento de um checkout (50-150ms de trabalho
+ 2% de chance de falha aleatória, simulando uma dependência externa instável), ela mede
e devolve, no próprio corpo da resposta:
- `tempoProcessamentoMs` — quanto tempo o processamento levou, medido no servidor.
- `memoriaGerenciadaMb` — memória gerenciada pelo .NET (heap do Garbage Collector).
- `memoriaFisicaMb` — memória física (RAM) que o processo da API está usando.
- `cpuTotalMs` — tempo total de CPU consumido pelo processo desde que ele iniciou.

### O que o cliente faz

`ResponseTimeDemo` chama essa API 300 vezes, **uma de cada vez, esperando cada resposta
antes de disparar a próxima** — nunca em paralelo. Para cada chamada, registra o tempo de
resposta do lado do cliente e as métricas de recurso que a API devolveu. No final, calcula
estatísticas agregadas (mínimo, média, máximo, P50/P95/P99) e compara a memória das 20
primeiras chamadas com a das 20 últimas — se a memória crescer de forma consistente, é
sinal de vazamento de memória (memory leak), um problema clássico de desempenho que só
aparece em execuções longas.

### Como rodar (precisa de DOIS terminais)

**Terminal 1 — suba a API:**
```bash
cd MockCheckoutApi
dotnet run -c Release
```

**Terminal 2 — rode o cliente de medição:**
```bash
cd ResponseTimeDemo
dotnet run -c Release
```

O terminal 2 imprime o progresso e, ao final, um resumo com todas as estatísticas. Também
salva um CSV detalhado (uma linha por chamada) em:
```
ResponseTimeDemo/relatorios_tempo_resposta/
```

---

## Conceitos cobertos por essa demonstração

| Conceito (ISO 25010 — Desempenho e Eficiência) | Onde aparece |
|---|---|
| Comportamento no tempo (algoritmo) | BenchmarkDemo |
| Complexidade O(1) vs O(n) | BenchmarkDemo |
| Comportamento no tempo (sistema/API) | ResponseTimeDemo (tempo de resposta por chamada) |
| Utilização de recursos (memória) | ResponseTimeDemo (memória gerenciada e física) |
| Utilização de recursos (CPU) | ResponseTimeDemo (CPU total do processo) |
| Detecção de vazamento de memória | ResponseTimeDemo (comparação início vs fim) |
| Taxa de erro de fundo (não relacionada a carga) | MockCheckoutApi (2% de falha aleatória) |

**Fora do escopo, de propósito** (território do teste de carga): número de usuários
simultâneos, requisições por segundo (throughput), ponto de saturação/capacidade máxima.

## Solução de problemas comuns

- **"contém mais de um arquivo de projeto"**: rode `dotnet run` de dentro de uma das três
  subpastas, nunca na pasta raiz `TesteDesempenho`.
- **`Connection refused` no ResponseTimeDemo**: a `MockCheckoutApi` não está rodando —
  confira o Terminal 1 antes de rodar o Terminal 2.
