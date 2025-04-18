// PrimeFactor
// A command line tool for finding prime factors of numbers, generating primes, and calculating GCD.
// The use of this source code file is governed by the license outlined in the License.txt file of this project.
// https://github.com/Software101DotNet/PrimeFactor

namespace PrimeFactor.Tests;

using PrimeFactor;

public class MathTests
{
	// Test the FastSquareRoot method against the Math.Sqrt library method
	[Fact]
	public void Test_FastSquareRoot()
	{
		for (UInt64 i = 1_000_000_000; i > 10; i /= 3)
		{
			var actual = Primes.FastSquareRoot(i);
			var expected = (UInt64)(Math.Sqrt(i));
			Assert.True(actual == expected);
		}
	}
}