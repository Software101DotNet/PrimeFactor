// PrimeFactor
// A command line tool for finding prime factors of numbers, generating primes, and calculating GCD.
// The use of this source code file is governed by the license outlined in the License.txt file of this project.
// https://github.com/Software101DotNet/PrimeFactor

namespace PrimeFactor.Tests;

using PrimeFactor;

public class MathTests
{
	// Test the FastSquareRoot method against the Math.Sqrt library method
	// performance advisory: this test takes a long time to run. Approximaty 135~148 seconds on an MBA M4 cpu.
	[Fact]
	public void Test_FastSquareRoot()
	{
		for (UInt64 i = 0; i < int.MaxValue; i++)
		{
			var actual = Primes.FastSquareRoot(i);
			var expected = (UInt64)Math.Sqrt(i);
			Assert.True(actual == expected);
		}
	}
}