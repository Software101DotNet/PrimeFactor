// PrimeFactor
// A command line tool for finding prime factors of numbers, generating primes, and calculating GCD.
// The use of this source code file is governed by the license outlined in the License.txt file of this project.
// https://github.com/Software101DotNet/PrimeFactor

#define statistics

using System.Diagnostics;

namespace PrimeFactor;

public partial class Prime
{
	public static IEnumerable<ulong> Primes(ulong limit = ulong.MaxValue)
	{

#if cached
			for (var i = 0; i < primes.Length && primes[i] <= limit; i++)
				yield return primes[i];

			Console.WriteLine($"Warning! {limit} is beyond cached value {primes[primes.Length - 1]}, and will have to be calculated...");

			for (var i = HighestCachedPrimeValue + 2; i <= limit; i += 2)
				if (IsPrime_TrialDivisionMethod(i))
					yield return i;
#else
		if (limit >= 2)
		{
			yield return 2;

			for (ulong i = 3; i <= limit; i += 2)
				if (IsPrime_TrialDivisionMethod(i))
					yield return i;
		}
#endif
	}

	public ulong GratestCommonFactor(List<ulong> values)
	{
		ulong result = 0;

		foreach (var n in values)
		{
			//Factor(n);
		}
		return result;
	}

	public List<ulong> LeastCommonMultiple()
	{
		var result = new List<ulong>();

		return result;
	}

	/// <summary>
	/// calculates (in parallel) list of Prime values from minLimit to maxLimit
	/// </summary>
	/// <param name="maxLimit"></param>
	/// <param name="minLimit"></param>
	/// <returns>an ascending ordered list of prime values</returns>
	public static List<ulong> Primes(ulong maxLimit, ulong minLimit = 0)
	{
		var results = new List<ulong>();

		int degreeOfParallelism = Environment.ProcessorCount;

		var q = new ValueTuple<ulong, bool, Task>[degreeOfParallelism]; // value being tested for primeality, result, task 

		// handle edge case minLimit > maxLimit
		if (minLimit > maxLimit)
			return results;

		// handle edge cases of n starting with 0,1. As neither are prime, just advance to 2
		ulong n = minLimit;
		if (n < 2)
			n = 2;

		// Handle edge case of n starting with 2, by adding 2 to the result and moving on to the odds for which the algorithm is designed. 
		if (n == 2 && n < maxLimit)
		{
			results.Add(2);
			n++;
		}
		// Handle edge case of n being any even number greater than 2, by increasing to the next odd value.
		if (n % 2 == 0)
			n++;

		// populate the task array
		for (int i = 0; i < degreeOfParallelism && n <= maxLimit; i++)
		{
			var id = i;

			q[id].Item1 = n;
			q[id].Item3 = new Task(() => q[id].Item2 = Prime.IsPrime_TrialDivisionMethod(q[id].Item1));
			q[id].Item3.Start();

			n += 2;
		}

		int id2 = 0;

		// wait for any of the parallel tasks to complate and add the results to the list,
		// start new calc work until the range limit is reached
		while (n <= maxLimit)
		{
			q[id2].Item3.Wait();

			if (q[id2].Item2)
			{
				results.Add(q[id2].Item1);
			}

			// assign new work
			q[id2].Item1 = n;
			var id3 = id2;
			q[id2].Item3 = new Task(() => q[id3].Item2 = Prime.IsPrime_TrialDivisionMethod(q[id3].Item1));
			q[id2].Item3.Start();

			n += 2;

			id2 = (id2 + 1) % degreeOfParallelism;
		}

		// wait for the remaing task to complete, and add the results to the list.
		for (var i = 0; i < degreeOfParallelism; i++)
		{
			if (q[id2].Item3 == null)
				break;

			q[id2].Item3.Wait();

			if (q[id2].Item2)
			{
				results.Add(q[id2].Item1);
			}

			id2 = (id2 + 1) % degreeOfParallelism;
		}

		return results;
	}

	public static bool IsPrime_TrialDivisionMethod_Serial(ulong value)
	{
		if (value <= 1)
			return false;

		if (value == 2)
			return true;

		if (value % 2 == 0)
			return false;

		var r = (ulong)Math.Sqrt(value) + 1;

		for (ulong i = 3; i <= r; i += 2)
			if (value % i == 0)
				return false;

		return true;
	}

	public static bool IsPrime_TrialDivisionMethod(ulong value)
	{
		if (value <= 1) // 
			return false;

		if (value == 2) //the only even prime
			return true;

		if (value % 2 == 0) // all evens above 2 are composit
			return false;

		if (value == 3)
			return true;

		var r = (ulong)Math.Sqrt(value) + 1;

		int degreeOfParallelism = 16; // use Environment.ProcessorCount

		ulong degreeOfParallelismLimit = ((value - 3ul) / 2ul) + 1ul;
		if (degreeOfParallelismLimit < (ulong)degreeOfParallelism)
			degreeOfParallelism = (int)degreeOfParallelismLimit;

		var tokenSource = new CancellationTokenSource();
		var token = tokenSource.Token;

		var tasks = new Task[degreeOfParallelism];
		var results = new bool[degreeOfParallelism];

		for (int tid = 0; tid < degreeOfParallelism; tid++)
		{
			ulong initial = 3ul + (2ul * (ulong)tid);

			var tid1 = tid;
			tasks[tid] = new Task(() => results[tid1] = Prime.IsPrime(value, r, initial, (ulong)degreeOfParallelism), token);
			tasks[tid].Start();
		}

		for (int i = 0; i < degreeOfParallelism; i++)
		{
			var tid = Task.WaitAny(tasks);
			if (results[tid] == false)
			{
				// one of the tasks has found that Value is not prime, so no need to continue

				// clean up
				tokenSource.Cancel();

				return false;
			}

			//tasks[tid] = null;
		}

		return true;
	}

	private static bool IsPrime(ulong value, ulong limit, ulong initial, ulong inc)
	{
		for (ulong i = initial; i <= limit; i += inc)
			if (value % i == 0)
				return false;

		return true;
	}

#if cached
	public static bool IsPrime_TrialDivisionMethodCached(ulong value)
	{
		if (value <= 1)
			return false;

		if (value == 2)
			return true;

		if (value % 2 == 0)
			return false;

		// search through the list of cached primes
		for (var i = 0; i < primes.Length; i++)
		{
			if (value % primes[i] == 0)
			{
				return (value == primes[i]);
			}
		}

		var r = (ulong)Math.Sqrt(value) + 1;
		var lastCachedPrime = primes[primes.Length - 1];

		// use the trial division method, stating from the next odd number after the last cached prime
		for (ulong i = lastCachedPrime + 2; i <= r; i += 2)
		{
			if (value % i == 0)
				return false;
		}
		return true;
	}
#endif

	// generate prime numbers with optimisation support from a prime cache.
	public static UInt64[] GeneratePrimes(StreamWriter writer, UInt64 maxIndex = UInt32.MaxValue, UInt64 maxValue = UInt64.MaxValue)
	{
		if (maxValue < 2)
		{
			return Array.Empty<ulong>();
		}

		// if maxIndex is a value that is too high for the memory of the platform, 
		// an exception is thrown to the out most block to explain to the user the limits
		var primes = new UInt64[maxIndex];

		UInt64 primeIndex = 0;
		UInt64 primeCandidate = 2;

		// For performance reasons, we keep a running squared value of the current prime candidate instead of calculating the square root.
		UInt64 square = 2;
		UInt64 squared = 4;
		UInt64 squareIdx = 0;

		// the first prime is 2
		writer.WriteLine($"{primeCandidate:N0}");
		primes[primeIndex++] = primeCandidate;

		// The second prime is 3. We must start the loop on odd values, as we increase by odd values after 2.
		primeCandidate++;

		do
		{
			bool prime = true;

			Debug.Assert(squared >= primeCandidate, $"squared {squared} >= primeCandidate {primeCandidate}");

			// check if the primeCandidate is divisible by any of the smaller prime values cached.
			// we only need to check primes that are less than or equal to the square root of the primeCandidate.
			for (var i = 0ul; (primes[i] <= square) && (i < primeIndex); i++)
			{
				if ((primeCandidate % primes[i]) == 0)
				{
					prime = false;
					break;
				}
			}

			if (prime)
			{
				writer.WriteLine($"{primeCandidate:N0}");

				// add the prime to the cache of primes.
				primes[primeIndex++] = primeCandidate;

				// if we have reached the limit of the cache, then
				if (primeIndex >= maxIndex)
					break;
			}

			// increase to the next odd value
			primeCandidate += 2;

			while (squared < primeCandidate)
			{
				squareIdx++;
				square = primes[squareIdx];
				squared = square * square;
			}

		} while (primeIndex < maxIndex && primeCandidate <= maxValue);

		// trim the array to the actual number of primes found
		// this is needed if the maxValue is reached before the maxIndex is reached
		if (primeIndex < maxIndex && primeIndex<=int.MaxValue)
		{
			Array.Resize(ref primes, (int)primeIndex);
		}
		return primes;
	}
}
