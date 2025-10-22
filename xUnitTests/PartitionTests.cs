// PrimeFactor
// A command line tool for finding prime factors of numbers, generating primes, and calculating GCD.
// The use of this source code file is governed by the license outlined in the License.txt file of this project.
// https://github.com/Software101DotNet/PrimeFactor

namespace PrimeFactor.Tests;

public class Tests_Partition
{
	[Fact]
	public void PartitionTest_100to600by6()
	{
		// Arrange
		ulong firstValue = 100;
		ulong lastValue = 600;
		ulong partitions = 6;

		var expected = new (ulong, ulong)[] // (first, last) value in a partition
		{
			(101,184),(185,268),(269,352), (353,436),(437,520),(521,600)
		};

		CheckContiguousSeries(expected);

		// Act
		var ranges = Prime.Partition(partitions, lastValue, firstValue);

		// Assert
		CheckResult(expected, ranges);
	}

	private static void CheckResult((ulong, ulong)[] expected, List<(ulong firstValue, ulong lastValue)> ranges)
	{
		// Assert
		Assert.Equal(expected.Count(), ranges.Count);
		for (var partition = 0; partition < ranges.Count; partition++)
		{
			Assert.Equal(expected[partition].Item1, ranges[partition].firstValue);
			Assert.Equal(expected[partition].Item2, ranges[partition].lastValue);
		}
	}

	private static void CheckContiguousSeries((ulong, ulong)[] expected)
	{
		// check the expected data is a contiguous integer series
		for (int i = 1; i < expected.Count(); i++)
		{
			var difference = expected[i].Item1 - expected[i - 1].Item2;
			Assert.Equal(1UL, (expected[i].Item1 - expected[i - 1].Item2));
		}
	}

	[Fact]
	public void PartitionTest_1to21by20()
	{
		// Arrange
		ulong firstValue = 1;
		ulong lastValue = 21;
		ulong partitions = 20;

		var expected = new (ulong, ulong)[] // (first, last) value in a partition
		{
			(1,2),(3,4),(5,6),(7,8),(9,10),(11,12),(13,14),(15,16),(17,18),(19,20),(21,21)
		};
		CheckContiguousSeries(expected);

		// Act
		var ranges = Prime.Partition(partitions, lastValue, firstValue);

		// Assert
		CheckResult(expected, ranges);
	}

	[Fact]
	public void PartitionTest_1to21by10()
	{
		// Arrange
		ulong firstValue = 1;
		ulong lastValue = 21;
		ulong partitions = 10;

		var expected = new (ulong, ulong)[] // (first, last) value in a partition
		{
			(1,2),(3,4),(5,6),(7,8),(9,10),(11,12),(13,14),(15,16),(17,18),(19,21)
		};
		CheckContiguousSeries(expected);

		// Act
		var ranges = Prime.Partition(partitions, lastValue, firstValue);

		// Assert
		CheckResult(expected, ranges);
	}

	[Fact]
	public void PartitionTest_1to21by5()
	{
		// Arrange
		ulong firstValue = 1;
		ulong lastValue = 21;
		ulong partitions = 5;

		var expected = new (ulong, ulong)[] // (first, last) value in a partition
		{
			(1,4),(5,8),(9,12),(13,16),(17,21)
		};
		CheckContiguousSeries(expected);

		// Act
		var ranges = Prime.Partition(partitions, lastValue, firstValue);

		// Assert
		CheckResult(expected, ranges);
	}


	[Fact]
	public void PartitionTest_2to21by20()
	{
		// Arrange
		ulong firstValue = 2;
		ulong lastValue = 21;
		ulong partitions = 20;

		var expected = new (ulong, ulong)[] // (first, last) value in a partition
		{
			(3,4),(5,6),(7,8),(9,10),(11,12),(13,14),(15,16),(17,18),(19,20),(21,21)

		};
		CheckContiguousSeries(expected);

		// Act
		var ranges = Prime.Partition(partitions, lastValue, firstValue);

		// Assert
		CheckResult(expected, ranges);
	}

	[Fact]
	public void PartitionTest_2to21by10()
	{
		// Arrange
		ulong firstValue = 2;
		ulong lastValue = 21;
		ulong partitions = 10;

		var expected = new (ulong, ulong)[] // (first, last) value in a partition
		{
			(3,4),(5,6),(7,8),(9,10),(11,12),(13,14),(15,16),(17,18),(19,20),(21,21)
		};
		CheckContiguousSeries(expected);

		// Act
		var ranges = Prime.Partition(partitions, lastValue, firstValue);

		// Assert
		CheckResult(expected, ranges);
	}


	// Range 2-21, Partitions=5 -> PartitionSize=4 
	// Odd boundary start 	- 3 4 5 6 | 7 8 9 10 | 11 12 13 14 | 15 16 17 18 | 19 20 21 	
	// 						2 3 4 5 | 6 7 8 9 | 10 11 12 13 | 14 15 16 17 | 18 19 20 21

	[Fact]
	public void PartitionTest_2to21by5()
	{
		// Arrange
		ulong firstValue = 2;
		ulong lastValue = 21;
		ulong partitions = 5;

		var expected = new (ulong, ulong)[] // (first, last) value in a partition
		{
			(3,6),(7,10),(11,14),(15,18),(19,21)
		};
		CheckContiguousSeries(expected);

		// Act
		var ranges = Prime.Partition(partitions, lastValue, firstValue);

		// Assert
		CheckResult(expected, ranges);
	}

	[Fact]
	public void PartitionTest_1to100by1()
	{
		// Arrange
		ulong firstValue = 1;
		ulong lastValue = 100;
		ulong partitions = 1;

		var expected = new (ulong, ulong)[] // (first, last) value in a partition
		{
			(1,100)
		};
		CheckContiguousSeries(expected);

		// Act
		var ranges = Prime.Partition(partitions, lastValue, firstValue);

		// Assert
		CheckResult(expected, ranges);
	}

	[Fact]
	public void PartitionTest_1to200by2()
	{
		// Arrange
		ulong firstValue = 1;
		ulong lastValue = 200;
		ulong partitions = 2;

		var expected = new (ulong, ulong)[] // (first, last) value in a partition
		{
			(1,100),(101,200)
		};
		CheckContiguousSeries(expected);

		// Act
		var ranges = Prime.Partition(partitions, lastValue, firstValue);

		// Assert
		CheckResult(expected, ranges);
	}
}