// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using BenchmarkDotNet.Running;

namespace BenchmarkSuite5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var _ = BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }
}
