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

void *xmalloc(size_t n)
{
	void *p = malloc(n);
	if (!p)
	{
		fprintf(stderr, "fatal error: failed to allocate memory (%zu bytes)\n", n);
		exit(EXIT_FAILURE);
	}
	return p;

#if nop
#define CACHE_BLOCK_SIZE 3
#define CACHE_BLOCKS 7

	// allocate memory for cache
	size_t allocationSize = CACHE_BLOCKS * sizeof(unsigned long *);
	unsigned long *pCache[CACHE_BLOCKS] = xmalloc(allocationSize);

	allocationSize = CACHE_BLOCK_SIZE * sizeof(unsigned long);
	for (int i = 0; i < CACHE_BLOCKS; i++)
	{
		pCache[i] = xmalloc(allocationSize);
	}

	// free memory for cache
	for (int i = 0; i < CACHE_BLOCKS; i++)
	{
		free(pCache[i]);
		pCache[i] = (unsigned long *)0;
	}
	free(*pCache);
	*pCache = (unsigned long *)0;
#endif
}

void GeneratePrimes(unsigned long limit, int displayProgress)
{
	clock_t start = clock();

	// 18446744073709551615 (on 64-bit systems)
	// 4294967295           (on 32-bit systems)
	unsigned long max_value = ULONG_MAX;
	unsigned long primeCount = 1; // +1 for prime number 2
	unsigned long primeCandidate = 3;

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

		// print progress every 16 million numbers checked
		if (displayProgress && (primeCandidate % 16777216UL == 1))
		{
			clock_t end = clock();
			clock_t ticks = end - start;
			printf("%.3fs %lu primes found from 1 to %lu\n", ((float)(ticks) / CLOCKS_PER_SEC), primeCount, primeCandidate);
		}
	}

	clock_t end = clock();
	clock_t ticks = end - start;
	printf("%.3fs %lu primes found from 1 to %lu\n", ((float)(ticks) / CLOCKS_PER_SEC), primeCount, primeCandidate);
}

typedef float (*fpSerial10M)(void);

float Serial10M(void)
{
	const unsigned long limit = 4294967296UL; // 10000000UL;
	const int displayProgress = 1;
	printf("Benchmarking primality test for values between 1 and %lu ... ", limit);
	clock_t start = clock();

	// using a null stream as there is no need to write the resulting calculations to a screen or file.
	GeneratePrimes(limit, displayProgress);

	clock_t end = clock();
	clock_t ticks = end - start;
	printf("Completed in %.3fs\n", ((float)(ticks) / CLOCKS_PER_SEC));

	return ticks;
}

void MultipleRuns(fpSerial10M action, int runs)
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
	fpSerial10M action = &Serial10M;
	const int runs = 1;

	MultipleRuns(action, runs);

	return 0;
}
