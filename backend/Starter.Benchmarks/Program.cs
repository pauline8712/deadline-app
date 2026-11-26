using BenchmarkDotNet.Running;
using Starter.Benchmarks;

BenchmarkSwitcher.FromAssembly(typeof(WorkListOrderingBenchmarks).Assembly).Run(args);
