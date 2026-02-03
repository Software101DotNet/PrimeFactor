// Copyright (C) 2025-2026 Anthony Ransley
// https://github.com/Software101DotNet/PrimeFactor
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3
// as published by the Free Software Foundation.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

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
	public static void MultipleRuns(Func<ulong, TimeSpan> action, ulong limit, int runs)
	{
		if (runs <= 1)
		{
			var timeToCompute = action(limit);
		}
		else
		{
			var runDurations = new List<long>(runs);
			Console.WriteLine($"Benchmarking {runs} runs, please wait...");

			for (var i = 1; i <= runs; i++)
			{
				Console.Write($"Run {i} ");

				// run the task to be measured here
				var timeToCompute = action(limit);

				runDurations.Add(timeToCompute.Ticks);
			}

			runDurations.Sort();
			var minTS = new TimeSpan(runDurations[0]);
			var maxTS = new TimeSpan(runDurations[runs - 1]);

			var medianTS = new TimeSpan(CalculateMedian(runDurations));

			long averageTicks = CalculateAverage(runDurations);
			var averageTS = new TimeSpan(averageTicks);

			var sdTS = new TimeSpan(CalculateStandardDeviation(runDurations, averageTicks));
			var modeTS = new TimeSpan(CalculateStatisticalMode(runDurations));
			Console.WriteLine($"Time to compute each run " + "".FormatTimeSpan(minTS) + "~ ".FormatTimeSpan(maxTS) + $", median ".FormatTimeSpan(medianTS) + $", average ".FormatTimeSpan(averageTS) + $", standard deviation ".FormatTimeSpan(sdTS) + $", mode ".FormatTimeSpan(modeTS));
		}
	}

	private static long CalculateMedian(List<long> series)
	{
		if (series == null)
			throw new ArgumentNullException(nameof(series));
		if (series.Count == 0)
			throw new InvalidOperationException("Cannot compute the median of an empty list."); // Matches the mathematical definition (mean is undefined for an empty set).

		// calculate the median time taken
		// odd number of runs, take the middle value for the median
		var median = series[series.Count / 2];
		if (series.Count % 2 == 0)
		{
			// When the number of runs is even, average the middle two values for the median
			median += series[series.Count / 2 - 1];
			median /= 2;
		}

		return median;
	}

	private static long CalculateAverage(List<long> series)
	{
		if (series == null)
			throw new ArgumentNullException(nameof(series));
		if (series.Count == 0)
			throw new InvalidOperationException("Cannot compute the average of an empty list."); // Matches the mathematical definition (mean is undefined for an empty set).

		var total = 0L;
		for (var i = 0; i < series.Count; i++)
			total += series[i];
		var average = total / series.Count;
		return average;
	}

	private static long CalculateStandardDeviation(List<long> series, long average)
	{
		if (series == null)
			throw new ArgumentNullException(nameof(series));
		if (series.Count == 0)
			throw new InvalidOperationException("Cannot compute the standard deviation of an empty list."); // Matches the mathematical definition (mean is undefined for an empty set).

		double sumOfSquaresOfDifferences = 0;
		for (var i = 0; i < series.Count; i++)
		{
			var difference = series[i] - average;
			sumOfSquaresOfDifferences += difference * difference;
		}
		var standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / series.Count);
		return (long)standardDeviation;
	}

	// calculate the statistical Mode - the most frequently occurring value.
	private static long CalculateStatisticalMode(List<long> series)
	{
		if (series == null)
			throw new ArgumentNullException(nameof(series));
		if (series.Count == 0)
			throw new InvalidOperationException("Cannot compute the statisical mode of an empty list."); // Matches the mathematical definition (mean is undefined for an empty set).

		var modeDict = new Dictionary<long, int>();
		for (var i = 0; i < series.Count; i++)
		{
			if (modeDict.ContainsKey(series[i]))
				modeDict[series[i]]++;
			else
				modeDict[series[i]] = 1;
		}
		var mode = series[0];
		var modeCount = 1;
		foreach (var kvp in modeDict)
		{
			if (kvp.Value > modeCount)
			{
				mode = kvp.Key;
				modeCount = kvp.Value;
			}
		}
		return mode;
	}

	/// measure the Primality test performance of a computer platform for values from 1 to limit.
	public static TimeSpan SerialBenchmark(ulong limit)
	{
		Console.Write($"Benchmarking primality test for values between 1 and {limit:N0} ... ");

		Stopwatch stopWatch = new Stopwatch();
		stopWatch.Restart();

		// using a null stream as there is no need to write the resulting calculations to a screen or file.
		UInt64 primeCount = Prime.GeneratePrimes(limit);

		stopWatch.Stop();
		Console.WriteLine($"{primeCount:N0} primes found in ".FormatTimeSpan(stopWatch.Elapsed));
		return new TimeSpan(stopWatch.Elapsed.Ticks);
	}
}


