// PrimeFactor
// A command line tool for finding prime factors of numbers, generating primes, and calculating GCD.
// The use of this source code file is governed by the license outlined in the License.txt file of this project.
// https://github.com/Software101DotNet/PrimeFactor


using System.Diagnostics;

namespace PrimeFactor.Tests;

public class FactorTests
{
	[Fact]
	public void FactorTest()
	{
		// test a series of values to be factorised 
		for (ulong testValue = 0; testValue <= 1000; testValue++)
		{
			FactorTest_part2(testValue);
		}
	}

	private static void FactorTest_part2(ulong testValue)
	{
		// factorise the test value into an a list of factors
		var factors = Factored.Factor(testValue);
		Assert.NotNull(factors);

		if (testValue <= 1)
		{
			//  0,1 are not prime or have integer factors, so factors should be an empty list.
			Assert.True(factors.Count == 0);
		}
		else
		{
			// assert that there is at least one or more factors for test values >= 2
			// when the test value is prime, the list of factors will be just the test value itself
			Assert.True(factors.Count >= 1);

			var product = 1ul;
			foreach (var factor in factors)
			{
				// for each of the factors, assert that the factor is prime. 
				Assert.True(Prime.IsPrime(factor));

				// Calculate the product as we progress through the factors 
				product *= factor;
			}

			// assert that the product of the factors are equal to the test value.
			Assert.Equal(testValue, product);
		}
	}

	[Fact]
	public void FactorTest_Max()
	{
		// Arrange
		ulong testValue = ulong.MaxValue - 2;

		// Act
		var factors = Factored.Factor(testValue);

		// Assert
		var product = 1ul;
		foreach (var factor in factors)
		{
			// Calculate the product of the factors 
			product *= factor;
		}

		// assert that the product of the factors are equal to the test value.
		Assert.Equal(testValue, product);
	}

}