// PrimeFactor
// A command line tool for finding prime factors of numbers, generating primes, and calculating GCD.
// The use of this source code file is governed by the license outlined in the License.txt file of this project.
// https://github.com/Software101DotNet/PrimeFactor

using System.Diagnostics;

namespace PrimeFactor;

public class Primes
{
	public static UInt64 FastSquareRoot(UInt64 value)
	{
		if (value == 0 || value == 1)
			return value;

		// Initial guess: half of the value (or any heuristic)
		UInt64 guess = value / 2;
		UInt64 nextGuess;

		while (true)
		{
			nextGuess = (guess + value / guess) / 2;

			// Stop if the guesses stabilize
			if (nextGuess >= guess)
				return guess;

			guess = nextGuess;
		}
	}

	public static bool IsPrime_TrialDivisionMethod(UInt64 n)
	{
		if (n <= 1)
			return false;

		if (n == 2)
			return true;

		if (n % 2 == 0)
			return false;

		var limit = 1 + FastSquareRoot(n);

		for (UInt64 i = 3; i <= limit; i += 2)
		{
			if (n % i == 0)
				return false; // found a i as a factor of n, so n is not prime 
		}

		// no factors found, so n is prime.
		return true;
	}

	public static void GeneratePrimes(bool quite = false, UInt64 minValue = 1, UInt64 maxValue = UInt64.MaxValue)
	{
		Stopwatch sw = new Stopwatch();
		int primesFound = 0;

		if (!quite)
			Console.WriteLine($"Generating Prime Numbers in the range {minValue:N0} to {maxValue:N0}");

		sw.Start();

		for (UInt64 i = minValue; i <= maxValue; i += 2) // count through odd numbers only
		{
			if (IsPrime_TrialDivisionMethod(i))
			{
				primesFound++;
				if (!quite)
					Console.WriteLine($"{sw.ElapsedMilliseconds:N0}ms {i:N0}");
				else
					Console.Write($"{i},");
			}
		}

		sw.Stop();
		if (!quite)
			Console.WriteLine($"{sw.ElapsedMilliseconds:N0}ms {primesFound:N0} primes found.");
		else
			Console.WriteLine();
	}
}
