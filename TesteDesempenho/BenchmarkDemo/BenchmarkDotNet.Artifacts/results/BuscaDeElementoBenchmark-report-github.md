```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26200.8875)
Unknown processor
.NET SDK 10.0.300
  [Host]     : .NET 8.0.27 (8.0.2726.22922), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.27 (8.0.2726.22922), X64 RyuJIT AVX2


```
| Method                      | TamanhoDaColecao | Mean           | Error         | StdDev        | Median         | Ratio | RatioSD | Allocated | Alloc Ratio |
|---------------------------- |----------------- |---------------:|--------------:|--------------:|---------------:|------:|--------:|----------:|------------:|
| Busca_Com_HashSet_Contains  | 100              |       4.388 ns |     0.1198 ns |     0.2129 ns |       4.390 ns |  0.38 |    0.02 |         - |          NA |
| Busca_Com_List_Contains     | 100              |      11.528 ns |     0.2266 ns |     0.2009 ns |      11.543 ns |  1.00 |    0.02 |         - |          NA |
| Busca_Com_Array_Loop_Manual | 100              |      45.470 ns |     0.5451 ns |     0.4832 ns |      45.375 ns |  3.95 |    0.08 |         - |          NA |
|                             |                  |                |               |               |                |       |         |           |             |
| Busca_Com_HashSet_Contains  | 10000            |       4.021 ns |     0.0545 ns |     0.0483 ns |       4.017 ns | 0.003 |    0.00 |         - |          NA |
| Busca_Com_List_Contains     | 10000            |   1,226.350 ns |    23.9306 ns |    35.8181 ns |   1,227.520 ns | 1.001 |    0.04 |         - |          NA |
| Busca_Com_Array_Loop_Manual | 10000            |   4,014.965 ns |    79.9815 ns |   150.2246 ns |   3,953.761 ns | 3.277 |    0.15 |         - |          NA |
|                             |                  |                |               |               |                |       |         |           |             |
| Busca_Com_HashSet_Contains  | 1000000          |       4.005 ns |     0.0695 ns |     0.0580 ns |       3.988 ns | 0.000 |    0.00 |         - |          NA |
| Busca_Com_List_Contains     | 1000000          | 131,295.300 ns | 2,397.4453 ns | 2,242.5719 ns | 130,887.341 ns | 1.000 |    0.02 |         - |          NA |
| Busca_Com_Array_Loop_Manual | 1000000          | 414,978.895 ns | 4,482.2820 ns | 3,742.9084 ns | 414,999.561 ns | 3.162 |    0.06 |         - |          NA |
