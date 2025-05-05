// PrimeFactor
// A command line tool for finding prime factors of numbers, generating primes, and calculating GCD.
// The use of this source code file is governed by the license outlined in the License.txt file of this project.
// https://github.com/Software101DotNet/PrimeFactor

using System.Diagnostics;

namespace PrimeFactor;

public class Benchmark
{
	/// Method to messuring the Prime Generation performance of a computer platform.
	/// Past runs on the following platforms:
	/// MacBook Air M4, Benchmarking 10,000,000 primes. Time to compute 14s,878ms 
	/// Mac Mini M2P, Benchmarking 10,000,000 primes. Time to compute 17s 310ms ~ 17s 686ms
	/// Xeon E5-1650 v2, Benchmarking 10,000,000 primes. Time to compute 2m 26s 217ms ~ 2m 28s 41ms
	public static void Serial10M()
	{
		const uint limit = 10_000_000;
		const int runs = 5;
		Console.WriteLine($"Benchmarking {limit:N0} primes ({runs} runs)");

		var writer = new StreamWriter(Stream.Null);
		Stopwatch stopWatch = new Stopwatch();
		long min = long.MaxValue;
		long max = 0;

		for (var i = 1; i <= runs; i++)
		{
			Console.Write($"Run {i} ");
			stopWatch.Restart();

			// using a null stream as there is no need to write the resulting calculations to a screen or file.
			Prime.GeneratePrimes(writer, maxIndex: limit);

			stopWatch.Stop();
			min = long.Min(stopWatch.Elapsed.Ticks, min);
			max = long.Max(stopWatch.Elapsed.Ticks, max);
			Console.WriteLine($"time to compute ".FormatTimeSpan(stopWatch.Elapsed));
		}

		var minTS = new TimeSpan(min);
		var maxTS = new TimeSpan(max);
		Console.WriteLine($"Time to compute ".FormatTimeSpan(minTS) + "~ ".FormatTimeSpan(maxTS));
	}
}