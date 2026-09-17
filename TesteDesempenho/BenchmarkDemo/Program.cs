using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Order;

BenchmarkRunner.Run<BuscaDeElementoBenchmark>();

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class BuscaDeElementoBenchmark {
    [Params(100, 10_000, 1_000_000)]
    public int TamanhoDaColecao;

    private List<int> _lista = null!;
    private int[] _array = null!;
    private HashSet<int> _hashSet = null!;
    private const int ValorProcurado = -1;

    [GlobalSetup]
    public void Setup() {
        _lista = Enumerable.Range(0, TamanhoDaColecao).ToList();
        _array = _lista.ToArray();
        _hashSet = new HashSet<int>(_lista);
    }

    [Benchmark(Baseline = true)]
    public bool Busca_Com_List_Contains() {
        return _lista.Contains(ValorProcurado);
    }

    [Benchmark]
    public bool Busca_Com_Array_Loop_Manual() {
        foreach (var item in _array) {
            if (item == ValorProcurado) return true;
        }
        return false;
    }

    [Benchmark]
    public bool Busca_Com_HashSet_Contains() {
        return _hashSet.Contains(ValorProcurado);
    }
}