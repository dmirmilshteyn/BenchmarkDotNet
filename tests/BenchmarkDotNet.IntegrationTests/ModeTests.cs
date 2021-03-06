﻿using System;
using System.Linq;
using System.Threading;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Tests.Loggers;
using Xunit;
using Xunit.Abstractions;

namespace BenchmarkDotNet.IntegrationTests
{
    public class ModeTests : BenchmarkTestExecutor
    {
        public ModeTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void ModesAreSupported()
        {
            var logger = new OutputLogger(Output);
            var config = ManualConfig.CreateEmpty()
                .With(DefaultConfig.Instance.GetColumns().ToArray())
                .With(logger)
                .With(Job.Dry.With(Mode.SingleRun))
                .With(Job.Dry.With(Mode.Throughput));

            var results = CanExecute<ModeBenchmarks>(config);

            Assert.Equal(4, results.Benchmarks.Count());

            Assert.Equal(1, results.Benchmarks.Count(b => b.Job.Mode == Mode.SingleRun && b.Target.Method.Name == "BenchmarkWithVoid"));
            Assert.Equal(1, results.Benchmarks.Count(b => b.Job.Mode == Mode.SingleRun && b.Target.Method.Name == "BenchmarkWithReturnValue"));

            Assert.Equal(1, results.Benchmarks.Count(b => b.Job.Mode == Mode.Throughput && b.Target.Method.Name == "BenchmarkWithVoid"));
            Assert.Equal(1, results.Benchmarks.Count(b => b.Job.Mode == Mode.Throughput && b.Target.Method.Name == "BenchmarkWithReturnValue"));

            var testLog = logger.GetLog();
            Assert.Contains("// ### Benchmark with void called ###", testLog);
            Assert.Contains("// ### Benchmark with return value called ###", testLog);
            Assert.DoesNotContain("No benchmarks found", logger.GetLog());
        }

        public class ModeBenchmarks
        {
            public bool FirstTime { get; set; }

            [Setup]
            public void Setup()
            {
                // Ensure we only print the diagnostic messages once per run in the tests, otherwise it fills up the output log!!
                FirstTime = true;
            }

            [Benchmark]
            public void BenchmarkWithVoid()
            {
                Thread.Sleep(10);
                if (FirstTime)
                {
                    Console.WriteLine("// ### Benchmark with void called ###");
                    FirstTime = false;
                }
            }

            [Benchmark]
            public string BenchmarkWithReturnValue()
            {
                Thread.Sleep(10);
                if (FirstTime)
                {
                    Console.WriteLine("// ### Benchmark with return value called ###");
                    FirstTime = false;
                }
                return "okay";
            }
        }
    }
}