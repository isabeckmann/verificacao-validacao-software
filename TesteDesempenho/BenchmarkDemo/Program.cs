// ==============================================================
// DEMONSTRAÇÃO 1 — Teste de Desempenho no nível de CÓDIGO/UNIDADE
// Ferramenta: BenchmarkDotNet
//
// Objetivo didático: mostrar que a escolha da estrutura de dados
// tem impacto direto e MENSURÁVEL no desempenho, e que "achismo"
// não é uma estratégia válida em engenharia de software.
// ==============================================================

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Order;

BenchmarkRunner.Run<BuscaDeElementoBenchmark>();

[MemoryDiagnoser]                 // mostra quanto de memória cada abordagem aloca
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class BuscaDeElementoBenchmark
{
    // [Params] faz o BenchmarkDotNet rodar o teste inteiro para CADA valor,
    // simulando "níveis de carga de dados" diferentes.
    [Params(100, 10_000, 1_000_000)]
    public int TamanhoDaColecao;

    private List<int> _lista = null!;
    private int[] _array = null!;
    private HashSet<int> _hashSet = null!;
    private const int ValorProcurado = -1; // pior caso: elemento não existe (força varredura completa)

    [GlobalSetup]
    public void Setup()
    {
        _lista = Enumerable.Range(0, TamanhoDaColecao).ToList();
        _array = _lista.ToArray();
        _hashSet = new HashSet<int>(_lista);
    }

    [Benchmark(Baseline = true)]
    public bool Busca_Com_List_Contains()
    {
        return _lista.Contains(ValorProcurado);
    }

    [Benchmark]
    public bool Busca_Com_Array_Loop_Manual()
    {
        foreach (var item in _array)
        {
            if (item == ValorProcurado) return true;
        }
        return false;
    }

    [Benchmark]
    public bool Busca_Com_HashSet_Contains()
    {
        return _hashSet.Contains(ValorProcurado);
    }
}

/*
 * O QUE OBSERVAR NO RELATÓRIO GERADO:
 *
 * - Colunas "Mean" (tempo médio) e "Allocated" (memória alocada por operação).
 * - Para TamanhoDaColecao = 100, a diferença entre as 3 abordagens é quase
 *   irrelevante — ilustra a ideia de "otimização prematura é a raiz de todo mal".
 * - Para TamanhoDaColecao = 1.000.000, HashSet.Contains (O(1)) deve ser ORDENS
 *   DE MAGNITUDE mais rápido que List/Array (O(n)) — prova concreta de que
 *   a complexidade algorítmica importa em escala.
 * - Isso é "teste de desempenho no nível de código": garante que uma peça
 *   isolada do sistema se comporta dentro do esperado ANTES de ela virar
 *   gargalo em um teste de carga (nível de sistema, ver Demonstração 2).
 */
