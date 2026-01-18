// PrimeFactor
// A command line tool for finding prime factors of numbers, generating primes, and calculating GCD.
// The use of this source code file is governed by the license outlined in the License.txt file of this project.
// https://github.com/Software101DotNet/PrimeFactor

namespace GenerateCacheSource;


// This program is to generate the prime cache class data during the software development of PrimeFactor
public class Program
{
	// generate 64bit Hex values of primality for a cache of odd numbers from 3 to 2^32.
	public static void GeneratePrimeCacheValues(StreamWriter writer, UInt64 maxValue = UInt32.MaxValue)
	{
		if (maxValue < 2)
		{
			return;
		}

		UInt64 primeCandidate = 3;

		// For performance reasons, we keep a running squared value of the current prime candidate instead of calculating the square root.
		UInt64 square = 3;
		UInt64 squared = 9;

		UInt64 qword = 0;
		UInt64 activeBit = 1;

		int newline = 0;

		do
		{
			while (squared <= primeCandidate)
			{
				square += 2;
				squared = square * square;
			}

			bool prime = true;  // assume primeCandidate is prime until a divider is found.

			for (var i = 3ul; i < primeCandidate && i < square; i += 2)
			{
				if ((primeCandidate % i) == 0)
				{
					prime = false;
					break;
				}
			}

			// if the primeCandidate value is prime, then set the bit true for the active bit in the qword.
			// for all non-primes, the bit remains false. 
			if (prime)
			{
				qword |= activeBit;
			}

			activeBit <<= 1;

			// when the active bit is shifted off the end, then the value of the qword is complete
			if (activeBit == 0)
			{
				// all qwords have a comma except the very last qword
				if (primeCandidate <= (maxValue - 64))
					writer.Write($"{qword:X16},");
				else
					writer.Write($"{qword:X16}");

				// insert a newline every 8 qwords
				if (++newline >= 8)
				{
					newline = 0;
					writer.WriteLine();
				}

				// reset the qword value and the active bit
				qword = 0;
				activeBit = 1;
			}


			// increase to the next odd value
			primeCandidate += 2;

		} while (primeCandidate <= maxValue);

		writer.WriteLine();
		writer.Flush();

		return;
	}


	enum ExitCode : int { Success = 0, CmdLineError = 1, Exception = 2 };

	static int Main(string[] args)
	{
		var exitCode = ExitCode.Success;

		//UInt64 maxPrimeCount = 64 * 101;  // 64bit *8 = 512 bits -> cache result of values from 3 to (2*512)+3 = 1,027
		string filename = "result.txt";

		try
		{
			using var writer = string.IsNullOrEmpty(filename)
				? new StreamWriter(Console.OpenStandardOutput())
				: new StreamWriter(filename);

			GeneratePrimeCacheValues(writer);
		}
		catch (Exception e)
		{
			Console.WriteLine($"{e.Message}");
			exitCode = ExitCode.Exception;
		}

		return (int)exitCode;
	}
}
