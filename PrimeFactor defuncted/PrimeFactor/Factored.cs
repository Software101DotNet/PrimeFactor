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


	/// <returns>List of prime factors, or a single element of value (which is proven to be prime)</returns>
	public static List<ulong> Factor(ulong value)
	{
		var factors = new List<ulong>();
		if (value >= 2)
		{
			var remainder = value;
			//var r = (ulong)Math.Sqrt(remainder) + 1;
			bool continueEvaluation = false;
			do
			{
#if cached
				foreach (var prime in Primes(remainder))
				{
					//Debug.Assert(prime <= remainder);
					if (remainder % prime == 0)
					{
						factors.Add(prime);
						remainder /= prime;
						continueEvaluation = (remainder > 1);
						break;
					}
				}
#else
				ulong i = 2;
				if (remainder % i == 0)
				{
					factors.Add(i);
					remainder /= i;
					continueEvaluation = (remainder > 1);
					continue;
				}

				i++;

				for (; i <= remainder; i += 2)
				{
					if (Prime.IsPrime_TrialDivisionMethod(i))
					{
						if (remainder % i == 0)
						{
							factors.Add(i);
							remainder /= i;
							continueEvaluation = (remainder > 1);
							break;
						}
					}
				}
#endif
				if (continueEvaluation == false && remainder > 1)
				{
					factors.Add(remainder);
				}

			} while (continueEvaluation);

		}

		return factors;
	}
}