// Copyright (C) 2025-2026 Anthony Ransley
// https://github.com/Software101DotNet
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3
// as published by the Free Software Foundation.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

#include <stdio.h>
#include <stdlib.h>
#include <limits.h>
#include <time.h>
#include <float.h>
#include <string.h>
#include <locale.h>

// Program version information for this implementation. The version number is in the format of Major.Year.Month.DayOfMonth
const char *version = "1.26.02.10";

unsigned long GeneratePrimes(unsigned long limit)
{
	if (limit < 2)
	{
		return 0;
	}

	unsigned long primeCount = 1;	  // +1 for prime number 2
	unsigned long primeCandidate = 3; // the algorithm assumes starting on an odd number due to the +=2 step

	for (; (primeCandidate < limit); primeCandidate += 2)
	{
		int isPrime = 1; // assume prime until proven otherwise

		for (unsigned long div = 3; (div * div) <= primeCandidate; div += 2)
		{
			if (primeCandidate % div == 0)
			{
				isPrime = 0; // found a divisor, not prime
				break;
			}
		}

		if (isPrime)
		{
			primeCount++;
		}
	}
	return primeCount;
}

typedef float (*fp)(unsigned long);

float Benchmark(unsigned long limit)
{
	// 18446744073709551615UL (on 64-bit systems)
	// 4294967295UL           (on 32-bit systems)
	printf("Benchmarking primality test for values between 1 and %'lu ... ", limit);
	fflush(stdout);
	clock_t start = clock();

	// using a null stream as there is no need to write the resulting calculations to a screen or file.
	unsigned long primeCount = GeneratePrimes(limit);

	clock_t end = clock();
	clock_t ticks = end - start;
	printf("%'lu primes found in %.3fs\n", primeCount, ((float)(ticks) / CLOCKS_PER_SEC));

	return ticks;
}

void MultipleRuns(fp action, unsigned long limit, int runs)
{
	if (runs < 1)
	{
		printf("Number of benchmark runs should be greater than zero.");
	}
	else if (runs == 1)
	{
		action(limit);
	}
	else
	{
		printf("Benchmarking %d runs, please wait...\n", runs);

		float minDuration = FLT_MAX;
		float maxDuration = 0.0f;
		float avgDuration = 0.0f;

		for (int i = 1; i <= runs; i++)
		{
			printf("Run %d ", i);

			// run the task to be measured here
			float timeToCompute = action(limit);

			avgDuration += timeToCompute;
			if (timeToCompute < minDuration)
				minDuration = timeToCompute;
			if (timeToCompute > maxDuration)
				maxDuration = timeToCompute;
		}

		avgDuration /= runs;
		printf("Time to compute each run %.3fs ~ %.3fs, average %.3fs\n", ((float)(minDuration) / CLOCKS_PER_SEC), ((float)(maxDuration) / CLOCKS_PER_SEC), ((float)(avgDuration) / CLOCKS_PER_SEC));
	}
}

void DisplayHelp(void)
{
#if defined NDEBUG
	const char *owl1 = "\n /\\_/\\\n (O,O)\n (:::)\n--\"-\"--\t\n";
#else
	const char *owl1 = "\n /\\_/\\\n (O,O)\n (:::)\n--\"-\"--\t (Debug build)\n";
#endif

	printf("%s\n", owl1);
	printf("This is a C23 language implementation of the benchmark part of the PrimeFactorCS version. It is intended only for benchmarking a C# implementation against a C implementation.\n\n");

	printf("Benchmark1, platform performance measurement of primality testing from values from 1 to limit. The default limit is 10,000,000, and the maximum limit value that the implementation will accept is 4,294,967,296. Specifying a limit of 0 will indicate that the maximum limit should be used. This benchmark does not use caching of previously found primes. The default number of runs is 1. When more than 1 run is specified, each benchmark is run in turn, with additional statistical information calculated. Minimum, Maximum, Median and Average.");
	printf("\t--benchmark [limit | limit runs]\n\n");

	printf("Benchmark2, same as Benchmark1, except that it partitions the number range 1 to limit by the number of available processor cores and performs the primality test in parallel.");
	printf("\t--benchmark2 [limit | limit runs]\n\n");

	printf("Benchmark3, same as Benchmark1 except that it uses memory to cache the prime values as it proceeds through the number range 1 to limit.");
	printf("\t--benchmark3 [limit | limit runs]\n\n");

	printf("display version information");
	printf("\t--version\n\n");

	printf("display this help information");
	printf("\t--help\n\n");
}

typedef enum
{
	None,
	Version,
	Help,
	Benchmark1,
	Benchmark2,
	Benchmark3
} Command;

typedef struct {
	Command command;
	long parameter1;
	long parameter2;
} Settings;

Settings ParseArgs(int argc, char *argv[])
{
	Settings s = { .command = None, .parameter1 = 0, .parameter2 = 0 };

	if (argc > 1)
	{
		if (strcmp(argv[1], "--help") == 0 || strcmp(argv[1], "-h") == 0)
		{
			s.command = Help;
		}
		else if (strcmp(argv[1], "--version") == 0 || strcmp(argv[1], "-v") == 0)
		{
			s.command = Version;
		}
		else if ((strcmp(argv[1], "--benchmark") == 0) || (strcmp(argv[1], "--benchmark1") == 0))
		{
			s.command = Benchmark1;
			if (argc > 2)
			{
				s.parameter1 = atol(argv[2]);	// limit

				// if the limit is 0, then use the maximum limit value that the implementation will accept.
				if (s.parameter1 <= 0) {
					s.parameter1 = UINT_MAX;
				}

				if (argc > 3)
					s.parameter2 = atol(argv[3]); // runs
				else
					s.parameter2 = 1;	// default runs if not specified.

			}
			else
			{
				// No limit or Runs specified for benchmark. Using default limit of 10,000,000 and default runs of 1.
				s.parameter1 = 10000000;
				s.parameter2 = 1;
			}
		}
		else if (strcmp(argv[1], "--benchmark2") == 0)
		{
			s.command = Benchmark2;
			if (argc > 2)
			{
				s.parameter1 = atol(argv[2]);
				if (argc > 3)
				{
					s.parameter2 = atol(argv[3]);
				}
			}
		}
		else if (strcmp(argv[1], "--benchmark3") == 0)
		{
			s.command = Benchmark3;
			if (argc > 2)
			{
				s.parameter1 = atol(argv[2]);
				if (argc > 3)
				{
					s.parameter2 = atol(argv[3]);
				}
			}
		}
		
	}

	return s;
}

int main(int argc, char *argv[])
{
	// Set the locale to give comma thousands separators in the output when printing large numbers.
	setlocale(LC_ALL, "en_US.UTF-8");

	Settings s = ParseArgs(argc, argv);
	switch (s.command)
	{
	case Help:
		DisplayHelp();
		break;
	case Version:
		printf("version %s\n", version);
		break;
	case Benchmark1:
		unsigned long limit = s.parameter1;
		int runs = s.parameter2;
		MultipleRuns(Benchmark, limit, runs);
		break;
	case Benchmark2:
		printf("Benchmark2 is not yet implemented.\n");
		break;
	case Benchmark3:
		printf("Benchmark3 is not yet implemented.\n");
		break;
	default:
		printf("No command specified. Use --help to see available commands.\n");
		break;
	}

	return 0;
}
