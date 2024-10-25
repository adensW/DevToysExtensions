using Adens.DevToys.RsaGenerator.Helpers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Security.Cryptography;

namespace Adens.DevToys.RsaGenerator.Benchmarks;

public class Program
{
    public class RsaGeneratorBenchmark
    {
        [Benchmark]
        public async Task Key2048()
        {
             await RsaGeneratorHelper.GenerateRsaKeyPairAsync(2048);
        }
        [Benchmark]
        public async Task Key4096()
        {
            await RsaGeneratorHelper.GenerateRsaKeyPairAsync(2048*2);
        }
        [Benchmark]
        public async Task Key6144()
        {
            await RsaGeneratorHelper.GenerateRsaKeyPairAsync(2048 * 3);
        }
        [Benchmark]
        public async Task Key8192()
        {
            await RsaGeneratorHelper.GenerateRsaKeyPairAsync(2048 * 4);
        }
        [Benchmark]
        public async Task Key10240()
        {
            await RsaGeneratorHelper.GenerateRsaKeyPairAsync(2048 * 5);
        }
        [Benchmark]
        public async Task Key12288()
        {
            await RsaGeneratorHelper.GenerateRsaKeyPairAsync(2048 * 6);
        }
        [Benchmark]
        public async Task Key14336()
        {
            await RsaGeneratorHelper.GenerateRsaKeyPairAsync(2048 * 7);
        }
        [Benchmark]
        public async Task Key16384()
        {
            await RsaGeneratorHelper.GenerateRsaKeyPairAsync(2048 * 8);
        }
    }
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<RsaGeneratorBenchmark>();
    }
}
