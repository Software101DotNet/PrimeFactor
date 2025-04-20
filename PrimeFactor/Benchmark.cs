// PrimeFactor
// A command line tool for finding prime factors of numbers, generating primes, and calculating GCD.
// The use of this source code file is governed by the license outlined in the License.txt file of this project.
// https://github.com/Software101DotNet/PrimeFactor

using System.Diagnostics;

namespace PrimeFactor;

public class Benchmark
{
    /// Method to messuring the perform of a computer platform
    /// MacBook Air M4 runtime 00:05:18.641
    public static void Serial10M()
    {
        uint limit = 10_000_000;
        Console.WriteLine($"Benchmarking {limit:N0} primes.");

        // no need to write the resulting calculations to the screen or a file
        StreamWriter writer = new StreamWriter(Stream.Null);

        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        var results = Prime.GeneratePrimes(writer, limit);

        stopWatch.Stop();
        Console.WriteLine("Time to compute ".FormatTimeSpan(stopWatch.Elapsed));
    }
}