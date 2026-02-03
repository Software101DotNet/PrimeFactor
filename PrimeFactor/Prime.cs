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
// along with this program.  If not, see <https://www.gnu.org/licenses/>

using System.Diagnostics;

namespace PrimeFactor;

public class Prime
{
	// Primality test of value primeCandidate
	public static bool IsPrime(ulong primeCandidate, bool ForceSerial = false)
	{
		if (primeCandidate <= 1)
			return false;   // integer values 1 or less are not prime

		if (primeCandidate == 2)
			return true;    // 2 is prime, and the only even prime

		if (primeCandidate % 2 == 0)
			return false;   // value is not prime because is have a factor of value 2

		ulong parallelThreshhold = 1_001; // value must be odd.
		ulong squareRoot = 1 + (ulong)Math.Sqrt(primeCandidate);

		// only employ parallel tasks over a given threshold due task overhead.
		if (ForceSerial || squareRoot < parallelThreshhold)
			return IsPrime_Serial(primeCandidate, squareRoot);
		else
			return IsPrime_Parallel(primeCandidate, squareRoot);
	}

	// Assumes that the given prime candidate value is both odd and >= 3
	public static bool IsPrime_Serial(ulong primeCandidate, ulong squareRoot)
	{
		Debug.Assert(primeCandidate >= 3 && primeCandidate % 2 != 0);

		for (ulong i = 3; i <= squareRoot; i += 2)
			if (primeCandidate % i == 0)
				return false;

		return true;
	}

	// Assumes that the given prime candidate value is both odd and >= 3
	public static bool IsPrime_Parallel(ulong primeCandidate, ulong squareRoot)
	{
		Debug.Assert(primeCandidate >= 3 && primeCandidate % 2 != 0);

		// Test for primality using parallel tasks, each with an allocated partition of the serise.
		var ranges = Prime.Partition((ulong)Environment.ProcessorCount, squareRoot);

		var tokenSource = new CancellationTokenSource();
		var token = tokenSource.Token;

		List<Task<bool>> tasks = new List<Task<bool>>();

		bool isPrime = true;    // true unless factors are found
		for (int tid = 0; tid < ranges.Count; tid++)
		{
			int taskId = tid;
			tasks.Add(Task.Run(() => HasFactor(primeCandidate, ranges[taskId].firstValue, ranges[taskId].lastValue, token), token));
		}

		try
		{
			while (tasks.Count > 0)
			{
				int completedIndex = Task.WaitAny(tasks.ToArray());

				Task<bool> completedTask = tasks[completedIndex];
				bool hasFactors = completedTask.Result;
				tasks.RemoveAt(completedIndex);

				// if one of the tasks has found that Value is not prime, so no need to continue
				if (hasFactors)
				{
					// terminate remain tasks
					tokenSource.Cancel();
					isPrime = false;
					break;
				}
			}
		}
		catch (AggregateException ae)
		{
			foreach (var e in ae.InnerExceptions)
			{
				if (e is TaskCanceledException)
					Console.WriteLine("A task was canceled.");
				else
					Console.WriteLine($"Unexpected exception: {e}");
			}
		}
		finally
		{
			tokenSource.Dispose();
		}

		return isPrime;


		static bool HasFactor(ulong primeCandidate, ulong firstValue, ulong lastValue, CancellationToken token)
		{
			Debug.Assert(firstValue % 2 != 0);

			for (ulong i = firstValue; i <= lastValue && !token.IsCancellationRequested; i += 2)
				if (primeCandidate % i == 0)
					return true;    // found a factor

			return false; // no factors found in the range firstValue ... lastValue, or task cancelled.
		}
	}


	// The actual number of partitions that the list is separated into may be less than the requested number of partitions.
	public static List<(ulong firstValue, ulong lastValue)> Partition(ulong partitions, ulong lastValue, ulong firstValue = 3)
	{
		if (partitions <= 0)
			throw new ArgumentException($"The number of partitions {partitions} must be 1 or greater.");

		if (firstValue > lastValue)
			throw new ArgumentException($"The first value {firstValue} must be less than or equal to the last value {lastValue}.");

		// if the given first value is even, adjust the start value to the next odd value.
		if (firstValue % 2 == 0)
			firstValue++;

		// the number of terms is the number of integers in a series from the first to last value inclusive.
		ulong terms = 1 + lastValue - firstValue;

		var result = new List<(ulong, ulong)>();

		var partionSize = terms / partitions;

		// to maintain the odd or even boundary, the partition size must always be an even number.
		if (partionSize <= 0)
			partionSize = 2;
		else if (partionSize % 2 != 0)
			partionSize++;

		ulong partitionFirst = firstValue;
		ulong partitionLast;
		ulong partition = 1;
		do
		{
			partitionLast = partitionFirst + partionSize - 1;
			if (partitionLast > lastValue || partition >= partitions)
				partitionLast = lastValue;

			result.Add((partitionFirst, partitionLast));

			partitionFirst += partionSize;
			partition++;
		} while (partitionLast < lastValue);

		return result;
	}

	// returns enumerable of prime numbers up (but not including) to the given limit.
	public static IEnumerable<ulong> Primes(ulong limit = ulong.MaxValue - 1)
	{
		if (limit >= ulong.MaxValue)
			throw new ArgumentOutOfRangeException($"limit needs to me less than {ulong.MaxValue:N0}");

		if (limit >= 2)
		{
			yield return 2;

			for (ulong i = 3; i <= limit; i += 2)
				if (IsPrime(i))
					yield return i;
		}
	}

	// Generates a series of prime numbers without a prebuilt cache.
	// The length of the series is determinied by maxIndex or maxValue depending which is reached first.
	public static UInt64[] GeneratePrimes(StreamWriter writer, UInt64 maxIndex = 0, UInt64 maxValue = 0)
	{
		if (maxValue < 2 && maxIndex <= 0)
		{
			return Array.Empty<ulong>();
		}

		// At this point, either maxIndex or maxValue will have been supplied with a positive value
		// If maxIndex has been given as zero, provision the cache with an estimated maximum number of primes based on maxValue
		// If maxValue has been given as zero, assign it the theoretical maximum value to let maxIndex become the limiting value.
		if (maxIndex <= 0)
			maxIndex = maxValue / 2;
		else if (maxValue <= 0)
			maxValue = UInt64.MaxValue;

		// if maxIndex is a value that is too high for the memory of the platform, 
		// an exception is thrown to the out most block to explain to the user the limits
		var primes = new UInt64[maxIndex];

		UInt64 primeIndex = 0;
		UInt64 primeCandidate = 2;

		// For performance reasons, we keep a running squared value of the current prime candidate instead of calculating the square root.
		UInt64 squareRoot = 2;
		UInt64 squared = 4;
		UInt64 squareIdx = 0;

		// the first prime is 2
		writer.Write($"{primeCandidate:N0}");
		primes[primeIndex++] = primeCandidate;

		// The second prime is 3. Sets main loop to start and iterate on odd values.
		primeCandidate++;

		do
		{
			// update square
			while (squared < primeCandidate)
			{
				squareIdx++;
				Debug.Assert(squareIdx <= primeIndex, $"squareIdx {squareIdx} <= primeIndex {primeIndex}");
				squareRoot = primes[squareIdx];
				squared = squareRoot * squareRoot;
			}

			bool prime = true;

			// check if the primeCandidate is divisible by any of the smaller prime values, cached.
			// we only need to check primes that are less than or equal to the square root of the primeCandidate.
			for (var i = 0ul; (primes[i] <= squareRoot) && (i < primeIndex); i++)
			{
				if ((primeCandidate % primes[i]) == 0)
				{
					prime = false;
					break;
				}
			}

			if (prime)
			{
				writer.Write($", {primeCandidate:N0}");

				// add the prime to the cache of primes.
				primes[primeIndex++] = primeCandidate;
			}

			// increase to the next odd value
			primeCandidate += 2;

		} while (primeIndex < maxIndex && primeCandidate <= maxValue);

		writer.WriteLine();
		writer.Flush();

		// trim the array to the actual number of primes found
		// this is needed if the maxValue is reached before the maxIndex is reached
		if (primeIndex < maxIndex && primeIndex <= int.MaxValue)
		{
			Array.Resize(ref primes, (int)primeIndex);
		}
		return primes;
	}

	// Generate primes for the benchmark function
	public static UInt64 GeneratePrimes(UInt64 limit)
	{
		if (limit < 2 )
		{
			return 0;
		}

		UInt64 primeCount = 1; // +1 for prime number 2
		UInt64 primeCandidate = 3; // the algorithm assumes starting on an odd number due to the +=2 step

		for (; primeCandidate <= limit; primeCandidate += 2)
		{
			bool isPrime = true; // assume prime until proven otherwise

			for (UInt64 div = 3; (div * div) <= primeCandidate; div += 2)
			{
				if (primeCandidate % div == 0)
				{
					isPrime = false; // found a divisor, not prime
					break;
				}
			}

			if (isPrime)
			{
				primeCount++;
			}
		}
		return primeCount;
	}
}
