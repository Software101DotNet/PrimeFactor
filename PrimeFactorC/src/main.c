#include <stdio.h>
#include <stdlib.h>
#include <limits.h>
#include <time.h>
#include <float.h>

// example output: 2026-01-18 16:09:45
void PrintCurrentTime(void)
{
	time_t now = time(NULL);
	struct tm *local = localtime(&now);
	char buffer[64];
	strftime(buffer, sizeof(buffer), "%Y-%m-%d %H:%M:%S", local);
	printf("%s\n", buffer);
}

void *xmalloc(size_t n)
{
	void *p = malloc(n);
	if (!p)
	{
		fprintf(stderr, "fatal error: failed to allocate memory (%zu bytes)\n", n);
		exit(EXIT_FAILURE);
	}
	return p;
}

void GeneratePrimes(unsigned long limit)
{
	// 18446744073709551615 (on 64-bit systems)
	// 4294967295           (on 32-bit systems)
	unsigned long max_value = ULONG_MAX;
	unsigned long primeCount = 0;

	for (unsigned long num = 3; num <= limit && num < max_value; num += 2)
	{
		int is_prime = 1; // assume prime until proven otherwise

		for (unsigned long div = 3; div * div <= num; div += 2)
		{
			if (num % div == 0)
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

	printf("Number of primes found up to %lu: %lu\n", limit, primeCount + 1); // +1 for prime number 2
}

float Serial10M(void)
{
	const unsigned long limit = 10000000UL;

	printf("Benchmarking %lu primes... ", limit);
	clock_t start = clock();

	// using a null stream as there is no need to write the resulting calculations to a screen or file.
	GeneratePrimes(limit);

	clock_t end = clock();
	clock_t ticks = end - start;
	printf("Completed in %.3fs\n", ((float)(ticks) / CLOCKS_PER_SEC));

	return ticks;
}

typedef float (*fp)(void);

void MultipleRuns(fp action, int runs)
{
	if (runs < 1)
	{
		printf("Number of benchmark runs must be greater than zero.");
		return;
	}

	printf("Benchmarking %d runs, please wait...\n", runs);

	float minDuration = FLT_MAX;
	float maxDuration = 0.0f;
	for (int i = 1; i <= runs; i++)
	{
		printf("Run %d ", i);

		// run the task to be measured here
		float timeToCompute = action();

		if (timeToCompute < minDuration)
			minDuration = timeToCompute;
		if (timeToCompute > maxDuration)
			maxDuration = timeToCompute;
	}

	printf("Time to compute each run %.3fs ~ %.3fs\n", ((float)(minDuration) / CLOCKS_PER_SEC), ((float)(maxDuration) / CLOCKS_PER_SEC));
}

int main(void)
{
	PrintCurrentTime();

	fp action = &Serial10M;
	const int runs = 5;
	
	MultipleRuns(action, runs);

	return 0;
}
