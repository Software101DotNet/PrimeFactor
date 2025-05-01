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
		for (UInt64 i = UInt32.MaxValue; i >= 2; i /= 3)
		{
			var actual = Prime.FastSquareRoot(i);

			var expectedFloor = (UInt64)(Math.Sqrt(i));
			var expectedCeiling = expectedFloor + 1;

			Assert.True(actual >= expectedFloor);
			Assert.True(actual <= expectedCeiling);
		}
	}

	[Fact]
	public void Test_IntegerSquareRoot()
	{
		for (UInt64 i = UInt32.MaxValue; i >= 2; i /= 3)
		{
			var actual = Prime.IntegerSquareRoot(i);

			var expectedFloor = (UInt64)(Math.Sqrt(i));
			var expectedCeiling = expectedFloor + 1;

			Assert.True(actual >= expectedFloor);
			Assert.True(actual <= expectedCeiling);
		}
	}

}