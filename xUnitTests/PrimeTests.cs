// PrimeFactor
// A command line tool for finding prime factors of numbers, generating primes, and calculating GCD.
// The use of this source code file is governed by the license outlined in the License.txt file of this project.
// https://github.com/Software101DotNet/PrimeFactor


using System.Diagnostics;

namespace PrimeFactor.Tests;

public class PrimeTests
{
	[Fact]
	public void IsPrimeTest_Enumerable()
	{
		// Arrange
		int[] primes = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251 };

		// Act
		var primeIndex = 0;
		foreach (var prime in Prime.Primes((ulong)primes.Length))
		{
			// Assert
			Assert.Equal(prime, (ulong)primes[primeIndex++]);
		}
	}

	[Fact]
	public void IsPrimeTest_Value0()
	{
		var result = Prime.IsPrime(0uL);
		Assert.False(result);
	}

	[Fact]
	public void IsPrimeTest_Value1()
	{
		var result = Prime.IsPrime(1uL);
		Assert.False(result);
	}

	[Fact]
	public void IsPrimeTest_Value2()
	{
		var result = Prime.IsPrime(2uL);
		Assert.True(result);
	}

	[Fact]
	public void IsPrimeTest_Value3()
	{
		var result = Prime.IsPrime(3uL);
		Assert.True(result);
	}

	[Fact]
	public void IsPrimeTest_Value4()
	{
		var result = Prime.IsPrime(4uL);
		Assert.False(result);
	}

	[Fact]
	public void IsPrimeTest_SmallValues()
	{
		bool[] isPrime = new bool[256];

		// initialise the test results array with all the primes in the number range 0 to 255
		int[] primes = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251 };
		foreach (var p in primes)
			isPrime[p] = true;

		for (int i = 0; i < isPrime.Length; i++)
		{
			var actual = Prime.IsPrime((ulong)i);
			Assert.Equal(isPrime[i], actual);
		}
	}

	[Fact]
	public void IsPrimeTest_UInt64_MaxPrime()
	{
		const ulong testValue = ulong.MaxValue - 82; // largest prime in a 2^64 
		ulong squareRoot = 1 + (ulong)Math.Sqrt(testValue);

		Stopwatch stopWatch = new Stopwatch();

		stopWatch.Restart();
		var result = Prime.IsPrime(testValue);
		stopWatch.Stop();
		Trace.WriteLine($"IsPrime({testValue})={result} Time to compute ".FormatTimeSpan(stopWatch.Elapsed));
		Assert.True(result);

		stopWatch.Restart();
		result = Prime.IsPrime_Serial(testValue, squareRoot);
		stopWatch.Stop();
		Trace.WriteLine($"IsPrimeSerial({testValue})={result} Time to compute ".FormatTimeSpan(stopWatch.Elapsed));
		Assert.True(result);

		stopWatch.Restart();
		result = Prime.IsPrime_Parallel(testValue, squareRoot);
		stopWatch.Stop();
		Trace.WriteLine($"IsPrimeParallel({testValue})={result} Time to compute ".FormatTimeSpan(stopWatch.Elapsed));
		Assert.True(result);
	}

	[Fact]
	public void IsPrimeTest_Serial_UInt64_Max()
	{
		const ulong testValue = ulong.MaxValue; // value is not prime
		ulong squareRoot = 1 + (ulong)Math.Sqrt(testValue);

		Stopwatch stopWatch = new Stopwatch();
		stopWatch.Restart();
		var result = Prime.IsPrime_Serial(testValue, squareRoot);
		stopWatch.Stop();

		Assert.False(result);

		Trace.WriteLine($"{result} Time to compute ".FormatTimeSpan(stopWatch.Elapsed));
	}

	[Fact]
	public void IsPrimeTest_Parallel_UInt64_Max()
	{
		const ulong testValue = ulong.MaxValue; // value is not prime
		ulong squareRoot = 1 + (ulong)Math.Sqrt(testValue);

		Stopwatch stopWatch = new Stopwatch();
		stopWatch.Restart();
		var result = Prime.IsPrime_Parallel(testValue, squareRoot);
		stopWatch.Stop();

		Assert.False(result);

		Trace.WriteLine($"{result} Time to compute ".FormatTimeSpan(stopWatch.Elapsed));
	}

	[Fact]
	public void IsPrimeTest_TimeTop100()
	{
		Stopwatch stopWatch = new Stopwatch();

		ulong threshold = 1_000_000_000_000;

		for (ulong candidate = threshold - 21; candidate < threshold + 50; candidate += 2)
		{
			Trace.Write($"Prime testing {candidate}: ");

			stopWatch.Restart();
			var pResult = Prime.IsPrime(candidate);
			stopWatch.Stop();
			Trace.WriteLine($"{pResult} Time to compute ".FormatTimeSpan(stopWatch.Elapsed));
		}
	}

	[Fact]
	public void GeneratePrimes_WithMaxValue()
	{
		// Arrange
		uint maxIndex = 10;
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
		uint maxIndex = 10;
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

