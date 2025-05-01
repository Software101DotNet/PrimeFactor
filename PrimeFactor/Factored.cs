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
		if (value < 2)
			return factors;

		ulong remainder = value;

		foreach (var factor in Prime.Primes(value))
		{
			// sometimes a factor value is used more than once, such as 45 = 3*3*5 
			while (remainder % factor == 0)
			{
				factors.Add(factor);
				remainder /= factor;
				if (remainder <= 1)
					return factors;
			}
		}

		// the value is prime, so return it as the only factor
		if (remainder > 1 && factors.Count <= 0)
			factors.Add(value);

		return factors;
	}
}