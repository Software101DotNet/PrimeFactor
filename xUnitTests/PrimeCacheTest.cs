// filepath: /Users/anthony/Developer/Repos/GitHub/Software101DotNet/PrimeFactor/PrimeFactor/PrimeCacheTest.cs
using Xunit;

namespace PrimeFactor.Tests;

public class PrimeCacheTest
{
	[Fact]
	public void GetBitValue_ValidIndex_Bit0()
	{
		// Arrange
		ulong bitIndex = 0;     // prime candidate value 3, is prime
		bool expected = true;

		// Act
		bool? result = PrimeCache.GetBitValue(bitIndex);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(expected, result);
	}

	[Fact]
	public void GetBitValue_ValidIndex_Bit1()
	{
		// Arrange
		ulong bitIndex = 1; // prime candidate value 5, is prime
		bool expected = true;

		// Act
		bool? result = PrimeCache.GetBitValue(bitIndex);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(expected, result);
	}

	[Fact]
	public void GetBitValue_ValidIndex_Bit3()
	{
		// Arrange
		ulong bitIndex = 3; // prime candidate value 9, is not prime
		bool expected = false;

		// Act
		bool? result = PrimeCache.GetBitValue(bitIndex);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(expected, result);
	}

	[Fact]
	public void GetBitValue_OutOfBoundsIndex()
	{
		// Arrange
		ulong bitIndex = (ulong)PrimeCache.cache.Length * 64; // Out of bounds

		// Act
		bool? result = PrimeCache.GetBitValue(bitIndex);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public void IsPrime_0to7()
	{
		bool? result = PrimeCache.IsPrime(0);
		Assert.Equal(false, result);

		result = PrimeCache.IsPrime(1);
		Assert.Equal(false, result);

		result = PrimeCache.IsPrime(2);
		Assert.Equal(true, result);

		result = PrimeCache.IsPrime(3);
		Assert.Equal(true, result);

		result = PrimeCache.IsPrime(4);
		Assert.Equal(false, result);

		result = PrimeCache.IsPrime(5);
		Assert.Equal(true, result);

		result = PrimeCache.IsPrime(6);
		Assert.Equal(false, result);

		result = PrimeCache.IsPrime(7);
		Assert.Equal(true, result);
	}

	[Fact]
	public void IsPrime_0to4000()
	{
		for (ulong i = 0; i <= 4000; i++)
		{
			var expected = Prime.IsPrime_TrialDivisionMethod_Serial(i);
			bool? result = PrimeCache.IsPrime(i);
			Assert.NotNull(result);
			Assert.Equal(expected, result);
		}

	}
}