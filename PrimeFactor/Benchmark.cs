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
	/// MacBook Air M4 Benchmarking 10,000,000 primes. Time to compute 14s,878ms 
	/// Mac Mini M2P Benchmarking 10,000,000 primes. Time to compute 17s,457ms 
	public static void Serial10M()
	{
		const uint limit = 10_000_000;
		Console.Write($"Benchmarking {limit:N0} primes. ");

		Stopwatch stopWatch = new Stopwatch();
		stopWatch.Start();

		// using a null stream as there is no need to write the resulting calculations to a screen or file.
		Prime.GeneratePrimes(writer: new StreamWriter(Stream.Null), maxIndex: limit);

		stopWatch.Stop();
		Console.WriteLine("Time to compute ".FormatTimeSpan(stopWatch.Elapsed));
	}
}