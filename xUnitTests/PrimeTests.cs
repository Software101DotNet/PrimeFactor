// PrimeFactor
// A command line tool for finding prime factors of numbers, generating primes, and calculating GCD.
// The use of this source code file is governed by the license outlined in the License.txt file of this project.
// https://github.com/Software101DotNet/PrimeFactor

#define statistics

using System.Diagnostics;

namespace PrimeFactor.Tests;

public class PrimeTests
{
#if cached
		[Fact]
		public void IsPrime()
		{
			for (ulong i = 0; i <= 250000; i++)
			{
				var result1 = Prime.IsPrime_TrialDivisionMethod(i);
				var result2 = Prime.IsPrime_TrialDivisionMethodCached(i);
				Assert.AreEqual(result1, result2);
			}
		}
#endif

	[Fact]
	public void IsPrimeTest_Value0()
	{
		var result = Prime.IsPrime_TrialDivisionMethod(0uL);
		Assert.False(result);
	}

	[Fact]
	public void IsPrimeTest_Value1()
	{
		var result = Prime.IsPrime_TrialDivisionMethod(1uL);
		Assert.False(result);
	}

	[Fact]
	public void IsPrimeTest_Value2()
	{
		var result = Prime.IsPrime_TrialDivisionMethod(2uL);
		Assert.True(result);
	}

	[Fact]
	public void IsPrimeTest_Value3()
	{
		var result = Prime.IsPrime_TrialDivisionMethod(3uL);
		Assert.True(result);
	}

	[Fact]
	public void IsPrimeTest_Value4()
	{
		var result = Prime.IsPrime_TrialDivisionMethod(4uL);
		Assert.False(result);
	}

	[Fact]
	public void IsPrimeTest_Value13()
	{
		var result = Prime.IsPrime_TrialDivisionMethod(13uL);
		Assert.True(result);
	}

	[Fact]
	public void IsPrimeTest_Max_Serial()
	{
		const ulong testValue = ulong.MaxValue - 82;
		Trace.WriteLine($"Testing serial trial division method with the prime value {testValue.ToString()}");
		var result = Prime.IsPrime_TrialDivisionMethod_Serial(testValue);
		Assert.True(result);
	}

	[Fact]
	public void IsPrimeTest_Max_Parallel()
	{
		const ulong testValue = ulong.MaxValue - 82;
		Trace.WriteLine($"Testing parallel trial division method with the prime value {testValue.ToString()}");
		var result = Prime.IsPrime_TrialDivisionMethod(testValue);
		Assert.True(result);
	}

	[Fact]
	public void IsPrimeTest_ValueUlongMax_Serial()
	{
		const ulong testValue = ulong.MaxValue;
		Trace.WriteLine($"Testing serial trial division method with the composit value {testValue.ToString()}");
		var result = Prime.IsPrime_TrialDivisionMethod_Serial(testValue);
		Assert.False(result);
	}

	[Fact]
	public void IsPrimeTest_ValueUlongMax_Parallel()
	{
		const ulong testValue = ulong.MaxValue;
		Trace.WriteLine($"Testing parallel trial division method with the composit value {testValue.ToString()}");
		var result = Prime.IsPrime_TrialDivisionMethod(testValue);
		Assert.False(result);
	}


	[Fact]
	public void FactorTest()
	{
		Stopwatch stopWatch = new Stopwatch();
		stopWatch.Start();

		FactorTest_part2(0);

		ulong limit = 10;
		ulong inc = 1;

		for (var j = 1; j <= 6; j++)
		{
			limit *= 10;
			inc *= 10;
			for (ulong i = 1 + inc; i < limit; i += inc)
			{
				FactorTest_part2(i);
			}
		}

		stopWatch.Stop();
		TimeSpan computationTime = stopWatch.Elapsed;
		var computeTime = ConsoleDisplay.FormatTimeSpan(computationTime);
		Trace.WriteLine($"Factor Test calculation time: {computeTime}");
	}

	private static void FactorTest_part2(ulong i)
	{
		var factors = Prime.Factor(i, out TimeSpan computationTime);
		var factored = new Factored(i, factors, computationTime);
		TraceDisplay.Display(factored);

		Assert.NotNull(factors);
		if (i >= 2)
		{
			Assert.True(factors.Count >= 1);

			var sum = 1ul;
			foreach (var factor in factors)
			{
				Assert.True(Prime.IsPrime_TrialDivisionMethod_Serial(factor));
				sum *= factor;
			}

			Assert.Equal(i, sum);
		}
		else
		{
			Assert.True(factors.Count == 0);
		}
	}

	[Fact]
	public void GeneratePrimes_WithMaxValue()
	{
		// Arrange
		int maxIndex = 10;
		ulong maxValue = 15;
		var expectedPrimes = new ulong[] { 2, 3, 5, 7, 11, 13 };

		using var memoryStream = new MemoryStream();
		using var writer = new StreamWriter(memoryStream);
		writer.AutoFlush = true;

		// Act
		var primes = Prime.GeneratePrimes(writer, maxIndex, maxValue);

		// Assert
		Assert.Equal(expectedPrimes.Length, primes.Length);
		for (int i = 0; i < expectedPrimes.Length; i++)
		{
			Assert.Equal(expectedPrimes[i], primes[i]);
		}

		// Optional: Check the written data
		memoryStream.Position = 0;
		using var reader = new StreamReader(memoryStream);
		string output = reader.ReadToEnd();

		foreach (var prime in expectedPrimes)
		{
			Assert.Contains(prime.ToString("N0"), output);
		}
	}

	[Fact]
	public void GeneratePrimes_WithMaxIndex10_ReturnsFirst10Primes()
	{
		// Arrange
		int maxIndex = 10;
		ulong maxValue = ulong.MaxValue;
		var expectedPrimes = new ulong[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 };

		using var memoryStream = new MemoryStream();
		using var writer = new StreamWriter(memoryStream);
		writer.AutoFlush = true;

		// Act
		var primes = Prime.GeneratePrimes(writer, maxIndex, maxValue);

		// Assert
		Assert.Equal(expectedPrimes.Length, primes.Length);
		for (int i = 0; i < expectedPrimes.Length; i++)
		{
			Assert.Equal(expectedPrimes[i], primes[i]);
		}

		// Optional: Check the written data
		memoryStream.Position = 0;
		using var reader = new StreamReader(memoryStream);
		string output = reader.ReadToEnd();

		foreach (var prime in expectedPrimes)
		{
			Assert.Contains(prime.ToString("N0"), output);
		}
	}

	[Fact]
	public void GeneratePrimes_WithMaxIndex0_ReturnsEmptyArray()
	{
		using var memoryStream = new MemoryStream();
		using var writer = new StreamWriter(memoryStream);
		writer.AutoFlush = true;

		var primes = Prime.GeneratePrimes(writer, 0, 100);

		Assert.Empty(primes);
	}
}

