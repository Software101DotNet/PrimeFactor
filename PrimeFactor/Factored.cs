// PrimeFactor
// A command line tool for finding prime factors of numbers, generating primes, and calculating GCD.
// The use of this source code file is governed by the license outlined in the License.txt file of this project.
// https://github.com/Software101DotNet/PrimeFactor

using System.Diagnostics;

namespace PrimeFactor;

/// <summary>
/// A tuple representing a number and its factors. There the number is prime, the list will be empty
/// </summary>
public class Factored
{
	private readonly Tuple<ulong, List<ulong>> data;

	public Factored(ulong item1)
	{
		data = new Tuple<ulong, List<ulong>>(item1, Factor(item1));
	}

	public Factored(ulong value, List<ulong> factors)
	{
		data = new Tuple<ulong, List<ulong>>(value, factors);
	}

	public ulong Value()
	{
		return data.Item1;
	}

	public List<ulong> Factors()
	{
		return data.Item2;
	}


	/// <returns>List of prime factors, or if it has no factors then just the value itself as now confirmeed to be prime.</returns>
	public static List<ulong> Factor(ulong value, bool ForceSerial = false)
	{
		var factors = new List<ulong>();

		if (value <= 1)
			return factors; //  0 and 1 are not prime or have any integer factors, so return an empty list.

		ulong remainder = value;
		ulong factor = 2;

		// find (if any) all the factors of 2,
		while (remainder % factor == 0)
		{
			factors.Add(factor);
			remainder /= factor;
			if (remainder <= 1)
				return factors;
		}

		// find (if any) all the odd prime factors of the remaining value 
		var ranges = Prime.Partition((ulong)Environment.ProcessorCount, remainder);

		var tokenSource = new CancellationTokenSource();
		var token = tokenSource.Token;
		var tasks = new List<Task<List<ulong>>>();

		for (int tid = 0; tid < ranges.Count; tid++)
		{
			int taskId = tid;
			tasks.Add(Task.Run(() => FindFactorsInPartition(remainder, ranges[taskId].firstValue, ranges[taskId].lastValue, token), token));
		}

		try
		{
			while (tasks.Count > 0)
			{
				int completedIndex = Task.WaitAny(tasks.ToArray());

				var completedTask = tasks[completedIndex];

				// if the task found in the partition, one or more factors that are not the original value, add them to the factor collection 
				if (completedTask.Result.Count > 0)
				{
					foreach (var f in completedTask.Result)
					{
						// if the factor is not the original value, add it to the factor collection 
						if (f != value)
						{
							factors.Add(f);
						}
					}
				}

				tasks.RemoveAt(completedIndex);
			}
		}
		finally
		{
			tokenSource.Dispose();
		}

		// if no factors found, then we have found a prime. So add it as the single result.
		if (factors.Count == 0)
			factors.Add(value);

#if DEBUG // debug build has a varification set that factorised value is equal to the product of the calculated factors

		if (factors.Count > 0)
		{
			ulong product = 1;
			foreach (var f in factors)
			{
				// Calculate the product of the factors 
				product *= f;
			}

			// assert that the product of the factors are equal to the value given to be factorised.
			Debug.Assert(value == product, $"Calculation Error! The product of the factors do not equal the factored value.");
		}
#endif
		return factors;
	}

	static List<ulong> FindFactorsInPartition(ulong value, ulong firstValue, ulong lastValue, CancellationToken token)
	{
		var factors = new List<ulong>();

		var remainder = value;
		for (ulong factor = firstValue; (!token.IsCancellationRequested) && (remainder > 2) && (factor <= lastValue) && (factor <= value); factor += 2)
		{
			if (Prime.IsPrime(factor, true))
			{
				// if the factor is prime, check if it divides the remainder
				while (remainder > 1 && (remainder % factor == 0))
				{
					factors.Add(factor);
					remainder /= factor;
				}
			}
		}

		return factors;
	}
}