# Adens.DevToys.RsaGenerator
A rsa key pair generator for DevToys.

## License
This extension is licensed under the GPL License - see the LICENSE file for details.

## Installation
1. Download the [Adens.DevToys.RsaGenerator](https://www.nuget.org/packages/Adens.DevToys.RsaGenerator/) NuGet package from NuGet.org.
2. For DevToys, open Manager Extensions, click on Install and select the downloaded NuGet package.

## Limitations

Not support for DevToys CLI (for now).

## Benchmark

```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4317/23H2/2023Update/SunValley3)
Intel Core i7-10700 CPU 2.90GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.403
  [Host]     : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2


```
| Method    | Mean         | Error        | StdDev        |
|---------- |-------------:|-------------:|--------------:|
| Key2048   |     48.72 ms |     1.306 ms |      3.810 ms |
| Key4096   |    445.01 ms |    50.035 ms |    146.744 ms |
| Key6144   |  1,547.98 ms |   189.676 ms |    553.293 ms |
| Key8192   |  4,467.49 ms |   691.113 ms |  2,016.010 ms |
| Key10240  |  8,943.09 ms | 1,301.756 ms |  3,755.863 ms |
| Key12288  | 16,429.40 ms | 2,308.950 ms |  6,550.109 ms |
| Key14336  | 33,067.80 ms | 5,580.311 ms | 16,366.088 ms |
| Key16384  | 56,593.69 ms | 9,743.628 ms | 28,268.033 ms |

