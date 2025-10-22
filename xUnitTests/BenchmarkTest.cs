using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace PrimeFactor.Tests
{
	public class Test_CalculateStatisticalMode
	{
		private static MethodInfo GetModeMethod()
		{
			var method = typeof(Benchmark).GetMethod("CalculateStatisticalMode", BindingFlags.NonPublic | BindingFlags.Static);
			if (method == null)
				throw new InvalidOperationException("Could not find CalculateStatisticalMode method via reflection.");
			return method;
		}

		[Fact]
		public void CalculateStatisticalMode_ReturnsMostFrequentValue()
		{
			var series = new List<long> { 290, 280, 279, 280, 280 };

			var method = GetModeMethod();
			var obj = method.Invoke(null, new object[] { series });
			if (obj == null)
				throw new InvalidOperationException("CalculateStatisticalMode returned null.");
			var result = (long)obj;

			Assert.Equal(280L, result);
		}

		[Fact]
		public void CalculateStatisticalMode_AllUnique_ReturnsFirstElement()
		{
			var series = new List<long> { 5, 10, 15, 20 };

			var method = GetModeMethod();
			var obj = method.Invoke(null, new object[] { series });
			if (obj == null)
				throw new InvalidOperationException("CalculateStatisticalMode returned null.");
			var result = (long)obj;

			Assert.Equal(5L, result);
		}

		[Fact]
		public void CalculateStatisticalMode_SingleElement_ReturnsThatElement()
		{
			var series = new List<long> { 42 };

			var method = GetModeMethod();
			var obj = method.Invoke(null, new object[] { series });
			if (obj == null)
				throw new InvalidOperationException("CalculateStatisticalMode returned null.");
			var result = (long)obj;

			Assert.Equal(42L, result);
		}

		[Fact]
		public void CalculateStatisticalMode_EmptyList_ThrowsException()
		{
			var series = new List<long>();

			var method = GetModeMethod();
			var ex = Assert.Throws<TargetInvocationException>(() => method.Invoke(null, new object[] { series }));

			Assert.IsType<InvalidOperationException>(ex.InnerException);
		}
	}

	public class Test_CalculateMedian
	{
		private static MethodInfo GetMedianMethod()
		{
			var method = typeof(Benchmark).GetMethod("CalculateMedian", BindingFlags.NonPublic | BindingFlags.Static);
			if (method == null)
				throw new InvalidOperationException("Could not find CalculateMedian method via reflection.");
			return method;
		}

		[Fact]
		public void CalculateMedian_OddCount_ReturnsMiddleValue()
		{
			var series = new List<long> { 10, 20, 30 }; // sorted
			var method = GetMedianMethod();
			var obj = method.Invoke(null, new object[] { series });
			if (obj == null)
				throw new InvalidOperationException("CalculateMedian returned null.");
			var result = (long)obj;
			Assert.Equal(20L, result);
		}

		[Fact]
		public void CalculateMedian_EvenCount_ReturnsAverageOfMiddleTwo()
		{
			var series = new List<long> { 10, 20, 30, 40 }; // sorted
			var method = GetMedianMethod();
			var obj = method.Invoke(null, new object[] { series });
			if (obj == null)
				throw new InvalidOperationException("CalculateMedian returned null.");
			var result = (long)obj;
			Assert.Equal(25L, result);
		}

		[Fact]
		public void CalculateMedian_SingleElement_ReturnsThatElement()
		{
			var series = new List<long> { 42 };
			var method = GetMedianMethod();
			var obj = method.Invoke(null, new object[] { series });
			if (obj == null)
				throw new InvalidOperationException("CalculateMedian returned null.");
			var result = (long)obj;
			Assert.Equal(42L, result);
		}

		[Fact]
		public void CalculateMedian_EmptyList_ThrowsException()
		{
			var series = new List<long>();
			var method = GetMedianMethod();
			var ex = Assert.Throws<TargetInvocationException>(() => method.Invoke(null, new object[] { series }));
			Assert.IsType<InvalidOperationException>(ex.InnerException);
		}
	}

	public class Test_CalculateAverage
	{
		private static MethodInfo GetAverageMethod()
		{
			var method = typeof(Benchmark).GetMethod("CalculateAverage", BindingFlags.NonPublic | BindingFlags.Static);
			if (method == null)
				throw new InvalidOperationException("Could not find CalculateAverage method via reflection.");
			return method;
		}

		[Fact]
		public void CalculateAverage_ReturnsCorrectAverage()
		{
			var series = new List<long> { 9, 20, 7 };

			var method = GetAverageMethod();
			var obj = method.Invoke(null, new object[] { series });
			if (obj == null)
				throw new InvalidOperationException("CalculateAverage returned null.");
			var result = (long)obj;

			Assert.Equal(12L, result);
		}

		[Fact]
		public void CalculateAverage_NonIntegerAverage_TruncatesTowardsZero()
		{
			var series = new List<long> { 1, 2 }; // 3 / 2 => 1 (integer division)

			var method = GetAverageMethod();
			var obj = method.Invoke(null, new object[] { series });
			if (obj == null)
				throw new InvalidOperationException("CalculateAverage returned null.");
			var result = (long)obj;

			Assert.Equal(1L, result);
		}

		[Fact]
		public void CalculateAverage_SingleElement_ReturnsThatElement()
		{
			var series = new List<long> { 42 };

			var method = GetAverageMethod();
			var obj = method.Invoke(null, new object[] { series });
			if (obj == null)
				throw new InvalidOperationException("CalculateAverageTicks returned null.");
			var result = (long)obj;

			Assert.Equal(42L, result);
		}

		[Fact]
		public void CalculateAverage_EmptyList_ThrowsException()
		{
			var series = new List<long>();

			var method = GetAverageMethod();
			var ex = Assert.Throws<TargetInvocationException>(() => method.Invoke(null, new object[] { series }));

			Assert.IsType<InvalidOperationException>(ex.InnerException);
		}
	}

	public class Test_CalculateStandardDeviation
	{
		private static MethodInfo GetStdMethod()
		{
			var method = typeof(Benchmark).GetMethod("CalculateStandardDeviation", BindingFlags.NonPublic | BindingFlags.Static);
			if (method == null)
				throw new InvalidOperationException("Could not find CalculateStandardDeviation method via reflection.");
			return method;
		}

		[Fact]
		public void CalculateStandardDeviation_KnownDataset_ReturnsExpectedSD()
		{
			// population standard deviation for {2,4,4,4,5,5,7,9} with mean 5 is exactly 2
			var series = new List<long> { 2, 4, 4, 4, 5, 5, 7, 9 };
			var method = GetStdMethod();
			var obj = method.Invoke(null, new object[] { series, 5L });
			if (obj == null)
				throw new InvalidOperationException("CalculateStandardDeviation returned null.");
			var result = (long)obj;
			Assert.Equal(2L, result);
		}

		[Fact]
		public void CalculateStandardDeviation_SingleElement_ReturnsZero()
		{
			var series = new List<long> { 42 };
			var method = GetStdMethod();
			var obj = method.Invoke(null, new object[] { series, 42L });
			if (obj == null)
				throw new InvalidOperationException("CalculateStandardDeviation returned null.");
			var result = (long)obj;
			Assert.Equal(0L, result);
		}

		[Fact]
		public void CalculateStandardDeviation_NonIntegerResult_TruncatesTowardsZero()
		{
			// series {1,2} with integer average 1 produces sd = sqrt(0.5) ~ 0.707 -> cast to long yields 0
			var series = new List<long> { 1, 2 };
			var method = GetStdMethod();
			var obj = method.Invoke(null, new object[] { series, 1L });
			if (obj == null)
				throw new InvalidOperationException("CalculateStandardDeviation returned null.");
			var result = (long)obj;
			Assert.Equal(0L, result);
		}

		[Fact]
		public void CalculateStandardDeviation_EmptyList_ThrowsException()
		{
			var series = new List<long>();
			var method = GetStdMethod();
			var ex = Assert.Throws<TargetInvocationException>(() => method.Invoke(null, new object[] { series, 0L }));
			Assert.IsType<InvalidOperationException>(ex.InnerException);
		}
	}
}
