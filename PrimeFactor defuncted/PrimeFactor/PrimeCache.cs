// PrimeFactor
// A command line tool for finding prime factors of numbers, generating primes, and calculating GCD.
// The use of this source code file is governed by the license outlined in the License.txt file of this project.
// https://github.com/Software101DotNet/PrimeFactor

namespace PrimeFactor;

public class PrimeCache
{
	// 4 x 64-bit = 256 bits
	public static readonly ulong[] cache = new ulong[]
	{	// test data for unit tests
		0x96DD96DD96DD96DDUL,
		0x1234567890ABCDEFUL,
		0x0F0F0F0F0F0F0F0FUL,
		0x8000000000000000UL
	};

	public static bool? GetBitValue(int bitIndex)
	{
		if (bitIndex < 0 || bitIndex >= cache.Length * 64)
			return null;

		int arrayIndex = bitIndex / 64;
		int bitPosition = bitIndex % 64;

		ulong value = cache[arrayIndex];

		// Shift and mask the desired bit
		bool isSet = (value & (1UL << bitPosition)) != 0;
		return isSet;
	}
}
