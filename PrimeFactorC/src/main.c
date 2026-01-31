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

unsigned long GeneratePrimes(unsigned long limit)
{
	unsigned long max_value = ULONG_MAX;
	unsigned long primeCount = 1; // +1 for prime number 2
	unsigned long primeCandidate = 3; // the algorithm assumes starting on an odd number due to the +=2 step

	for (; (primeCandidate <= limit) && (primeCandidate < max_value); primeCandidate += 2)
	{
		int is_prime = 1; // assume prime until proven otherwise

		for (unsigned long div = 3; (div * div) <= primeCandidate; div += 2)
		{
			if (primeCandidate % div == 0)
			{
				is_prime = 0; // found a divisor, not prime
				break;
			}
		}

		if (is_prime)
		{
			primeCount++;
		}
	}
	return primeCount;
}

typedef float (*fp)(void);

float Benchmark(void)
{
	// 18446744073709551615 (on 64-bit systems)
	// 4294967295           (on 32-bit systems)
	const unsigned long limit = 4294967296UL; // max value for UInt32
	printf("Benchmarking primality test for values between 1 and %lu ... ", limit);
	clock_t start = clock();

	// using a null stream as there is no need to write the resulting calculations to a screen or file.
	unsigned long primeCount = GeneratePrimes(limit);

	clock_t end = clock();
	clock_t ticks = end - start;
	printf("%lu primes found in %.3fs\n", primeCount,((float)(ticks) / CLOCKS_PER_SEC));

	return ticks;
}

void MultipleRuns(fp action, int runs)
{
	if (runs < 1)
	{
		printf("Number of benchmark runs should be greater than zero.");
		return;
	}

	printf("Benchmarking %d runs, please wait...\n", runs);

	float minDuration = FLT_MAX;
	float maxDuration = 0.0f;
	float avgDuration = 0.0f;

	for (int i = 1; i <= runs; i++)
	{
		printf("Run %d ", i);

		// run the task to be measured here
		float timeToCompute = action();

		avgDuration += timeToCompute;
		if (timeToCompute < minDuration)
			minDuration = timeToCompute;
		if (timeToCompute > maxDuration)
			maxDuration = timeToCompute;
	}

	avgDuration /= runs;
	printf("Time to compute each run %.3fs ~ %.3fs, average %.3fs\n", ((float)(minDuration) / CLOCKS_PER_SEC), ((float)(maxDuration) / CLOCKS_PER_SEC), ((float)(avgDuration) / CLOCKS_PER_SEC));
}

int main(void)
{
	fp action = &Benchmark;
	const int runs = 1;

	MultipleRuns(action, runs);

	return 0;
}
