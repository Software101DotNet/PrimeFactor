// filepath: /Users/anthony/Developer/Repos/GitHub/Software101DotNet/PrimeFactor/PrimeFactor/PrimeCacheTest.cs
using Xunit;

namespace PrimeFactor.Tests;

public class PrimeCacheTest
{
	[Fact]
	public void GetBitValue_ValidIndex_Bit0()
	{
		// Arrange
		int bitIndex = 0; // First bit of the first ulong
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
		int bitIndex = 1; // Second bit of the first ulong
		bool expected = false; // Second bit is 0 in 0x96DD96DD96DD96DD

		// Act
		bool? result = PrimeCache.GetBitValue(bitIndex);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(expected, result);
	}

	[Fact]
	public void GetBitValue_NegativeIndex()
	{
		// Arrange
		int bitIndex = -1;

		// Act
		bool? result = PrimeCache.GetBitValue(bitIndex);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public void GetBitValue_OutOfBoundsIndex()
	{
		// Arrange
		int bitIndex = PrimeCache.cache.Length * 64; // Out of bounds

		// Act
		bool? result = PrimeCache.GetBitValue(bitIndex);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public void GetBitValue_LastValidIndex()
	{
		// Arrange
		int bitIndex = PrimeCache.cache.Length * 64 - 1; // Last valid bit
		bool expected = true; // Last bit of 0xFFFFFFFFFFFFFFFF is 1

		// Act
		bool? result = PrimeCache.GetBitValue(bitIndex);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(expected, result);
	}
}