// PrimeFactor
// A command line tool for finding prime factors of numbers, generating primes, and calculating GCD.
// The use of this source code file is governed by the license outlined in the License.txt file of this project.
// https://github.com/Software101DotNet/PrimeFactor

using System.Diagnostics;
using System.Reflection;

namespace PrimeFactor;

public partial class PrimeCache
{
	public static ulong GetMaxValue()
	{
		return ((ulong)cache.Length * 128UL) + 2UL;
	}

	public static bool? GetBitValue(ulong bitIndex)
	{
		if (bitIndex >= ((ulong)cache.Length * 64))
			return null;

		int arrayIndex = (int)(bitIndex / 64);
		int bitPosition = (int)(bitIndex % 64);

		ulong value = cache[arrayIndex];

		// Shift and mask the desired bit
		bool isSet = (value & (1UL << bitPosition)) != 0;
		return isSet;
	}

	public static bool? IsPrime(ulong value)
	{
		// cache supports values <3 or even.
		if (value < 2)
			return false;
		if (value == 2)
			return true;
		if (value % 2 == 0)
			return false;

		// map value to bit index in the cache. The cache stores only odd numbers from the value 3 onwards. 
		// Value 3 maps to index 0, Value 5 maps to index 1, Value 7 maps to index 2, etc.
		ulong bitIndex = value - 3;
		bitIndex /= 2;
		Debug.Assert(bitIndex <= int.MaxValue, "Given number must map into the cache array index [int]");

		return GetBitValue(bitIndex);
	}
}
