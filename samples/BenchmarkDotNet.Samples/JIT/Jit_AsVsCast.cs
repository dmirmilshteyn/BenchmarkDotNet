﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;

namespace BenchmarkDotNet.Samples.JIT
{
    [AllJitsJob]
    public class Jit_AsVsCast
    {
        public class Foo
        {
        }

        private object foo = new Foo();

        [Benchmark]
        public Foo As()
        {
            return foo as Foo;
        }

        [Benchmark]
        public object Cast()
        {
            return (Foo)foo;
        }
    }
}