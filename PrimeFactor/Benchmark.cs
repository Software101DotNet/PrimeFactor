// PrimeFactor
// A command line tool for finding prime factors of numbers, generating primes, and calculating GCD.
// The use of this source code file is governed by the license outlined in the License.txt file of this project.
// https://github.com/Software101DotNet/PrimeFactor

using System.Diagnostics;

namespace PrimeFactor;

public class Benchmark
{
	/// Method for measuring the Prime Generation performance of a computer platform.
	/// 5 runs benchmarking 10,000,000 primes on the following platforms:
	/// MacBook Air M4. Time to compute 14s 920ms ~ 15s 316ms (DotNet 9.0)
	/// Mac Mini M2P. Time to compute 17s 310ms ~ 17s 686ms
	/// Xeon E5-1650 v2, Time to compute 2m 26s 217ms ~ 2m 28s 41ms
	/// Xeon E5-2697 v2, Time to compute 3m 12s 490ms ~ 3m 14s 434ms
	/// 

	/// <param name="action">The action to be measured</param>
	public static void MultipleRuns(Func<TimeSpan> action, int runs = 5)
	{
		if (runs < 3)
			runs = 5; // default to 5 runs if invalid number provided
		else if (runs % 2 == 0)
			runs++; // Enforce odd number of runs for simpler median calculation

		var runDurations = new List<long>(runs);
		Console.WriteLine($"Benchmarking {runs} runs, please wait...");

		for (var i = 1; i <= runs; i++)
		{
			Console.Write($"Run {i} ");

			// run the task to be measured here
			var timeToCompute = action();

			runDurations.Add(timeToCompute.Ticks);
		}

		runDurations.Sort();
		var medianTS = new TimeSpan(runDurations[runs / 2]);
		var minTS = new TimeSpan(runDurations[0]);
		var maxTS = new TimeSpan(runDurations[runs - 1]);
		Console.WriteLine($"Time to compute each run ".FormatTimeSpan(minTS) + "~ ".FormatTimeSpan(maxTS) + ", median " + "".FormatTimeSpan(medianTS));
	}

	/// measure the Prime Generation performance of a computer platform for 10 million primes.
	/// returns the TimeSpan taken to complete the task.
	public static TimeSpan Serial10M()
	{
		const uint limit = 10_000_000;
		var writer = new StreamWriter(Stream.Null);

		Console.Write($"Benchmarking {limit:N0} primes... ");
		Stopwatch stopWatch = new Stopwatch();
		stopWatch.Restart();

		// using a null stream as there is no need to write the resulting calculations to a screen or file.
		Prime.GeneratePrimes(writer, maxIndex: limit);

		stopWatch.Stop();
		Console.WriteLine($"Completed in ".FormatTimeSpan(stopWatch.Elapsed));

		return new TimeSpan(stopWatch.Elapsed.Ticks);
	}

	/// measure the Prime Generation performance of a computer platform for 2^64.
	public static void SerialMax()
	{
		const ulong limit = ulong.MaxValue;
		Console.WriteLine($"Benchmarking {limit:N0} primes...");

		var writer = new StreamWriter(Stream.Null);
		Stopwatch stopWatch = new Stopwatch();
		stopWatch.Restart();

		// using a null stream as there is no need to write the resulting calculations to a screen or file.
		Prime.GeneratePrimes(writer, maxValue: limit);

		stopWatch.Stop();
		Console.WriteLine($"Completed in ".FormatTimeSpan(stopWatch.Elapsed));
	}
}


