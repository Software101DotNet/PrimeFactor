// PrimeFactor
// A command line tool for finding prime factors of numbers, generating primes, and calculating GCD.
// The use of this source code file is governed by the license outlined in the License.txt file of this project.
// https://github.com/Software101DotNet/PrimeFactor

using System.Diagnostics;
using PrimeFactor;

namespace PrimeFactor.Tests;

// Note: The output from System.Diagnostics.Trace() is visable in VS Code in the Debug Console window in VS Code, 
// only when the tests are run in 'Debug Tests' profile. 
// The Trace output is not show when the Tests are run in the 'Run Tests' or 'Run Tests with Coverage' profile. 
public class TraceDisplay
{
	public static void Display(Factored n)
	{
		if (n == null)
			throw new ArgumentNullException(nameof(n));

		string elapsedTime = string.Empty; //ConsoleDisplay.FormatTimeSpan(n.ComputationTime);

		if (n.Value() <= 1)
		{
			Trace.WriteLine($"{n.Value()} is neither a prime or a composite number.");
		}
		else if (n.Factors().Count == 1)
		{
			DisplayAsPrime(n.Value(), elapsedTime);
		}
		else
		{
			DisplayAsPrimeFactors(n.Value(), n.Factors(), elapsedTime);
		}
	}

	private static void DisplayAsPrimeFactors(ulong n, IReadOnlyCollection<ulong> primeFactors, string computeTime)
	{
		Trace.Write($"{n} is a composite number, made up of the {primeFactors.Count} prime factors: ");
		foreach (var primeFactor in primeFactors)
		{
			Trace.Write($"{primeFactor} ");
		}

		Trace.WriteLine($"Calculation Time: {computeTime}");
	}

	private static void DisplayAsPrime(ulong n, string computeTime)
	{
		Trace.WriteLine($"{n} is a prime number. Calculation Time: {computeTime}");
	}
}

