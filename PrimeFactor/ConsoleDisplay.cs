// Copyright (C) 2025-2026 Anthony Ransley
// https://github.com/Software101DotNet/PrimeFactor
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

using System.Text;
using System.Globalization;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;
using System.Runtime.InteropServices;
using System.Drawing;

namespace PrimeFactor;

public static class StringExtensions
{
	public static string FormatTimeSpan(this string message, TimeSpan ts)
	{
		StringBuilder sb = new StringBuilder(256);

		if (!string.IsNullOrEmpty(message))
		{
			sb.Append(message);
		}
		if (ts.Days > 0)
		{
			sb.Append($"{ts.Days}d {ts.Hours}h {ts.Minutes}m {ts.Seconds}s {ts.Milliseconds}ms ");
		}
		else if (ts.Hours > 0)
		{
			sb.Append($"{ts.Hours}h {ts.Minutes}m {ts.Seconds}s {ts.Milliseconds}ms ");
		}
		else if (ts.Minutes > 0)
		{
			sb.Append($"{ts.Minutes}m {ts.Seconds}s {ts.Milliseconds}ms ");
		}
		else if (ts.Seconds > 0)
		{
			sb.Append($"{ts.Seconds}s {ts.Milliseconds}ms ");
		}
		else if (ts.Milliseconds > 0)
		{
			sb.Append($"{ts.Milliseconds}ms ");
		}
		else
		{
			sb.Append($"{ts.TotalMilliseconds}ms ");
		}

		return sb.ToString();
	}
}

// Using ConsoleExtended as a wrapper class because extending WriteLine as an extension method would require 
// passing Console as a parameter, but his is not possible because Console is a Static Type.
// See error CS0721: 'Console': static types cannot be used as parameters
public static class ConsoleExtended
{
	public static void WriteLine(string message, ConsoleColor color)
	{
		var originalForegroundColour = Console.ForegroundColor;
		Console.ForegroundColor = color;
		Console.WriteLine(message);
		Console.ForegroundColor = originalForegroundColour;
	}
	public static void Write(string message, ConsoleColor color)
	{
		var originalForegroundColour = Console.ForegroundColor;
		Console.ForegroundColor = color;
		Console.Write(message);
		Console.ForegroundColor = originalForegroundColour;
	}
	public static void WriteLine(string message)
	{
		var originalForegroundColour = Console.ForegroundColor;
		var originalBackgroundColour = Console.BackgroundColor;
		Console.ForegroundColor = originalBackgroundColour;
		Console.BackgroundColor = originalForegroundColour;
		Console.Write(message);
		Console.ForegroundColor = originalForegroundColour;
		Console.BackgroundColor = originalBackgroundColour;
		Console.WriteLine();
	}
}

public class ConsoleDisplay
{
	public static void Display(Factored n, bool quite = false)
	{
		if (quite)
		{
			foreach (var factors in n.Factors())
			{
				Console.Write($"{factors} ");
			}
			Console.WriteLine();
		}
		else
		{
			string elapsedTime = string.Empty; //FormatTimeSpan(n.ComputationTime);

			if (n.Value() <= 1)
			{
				Console.WriteLine($"{n} is neither a prime or a composite number!");
			}
			else if (n.Factors().Count == 1)
			{
				DisplayAsPrime(n.Value(), elapsedTime);
			}
			else
			{
				DisplayAsPrimeFactors(n.Value(), n.Factors(), elapsedTime);
			}
		}
	}

	public static void DisplayAsPrimeFactors(ulong n, IReadOnlyCollection<ulong> primeFactors, string computeTime)
	{
		var originalColour = Console.ForegroundColor;
		Console.ForegroundColor = ConsoleColor.Cyan;
		Console.Write($"{n}");
		Console.ForegroundColor = originalColour;
		Console.Write(" is a ");
		Console.ForegroundColor = ConsoleColor.Cyan;
		Console.Write("composite");
		Console.ForegroundColor = originalColour;
		Console.Write($" number made up of the {primeFactors.Count} prime factors: ");
		Console.ForegroundColor = ConsoleColor.Red;
		foreach (var primeFactor in primeFactors)
		{
			Console.Write($"{primeFactor} ");
		}

		Console.ForegroundColor = ConsoleColor.Gray;
		Console.Write($"Calculation Time {computeTime}");

		Console.ForegroundColor = originalColour;
		Console.WriteLine();
	}

	public static void DisplayAsPrime(ulong n, string computeTime)
	{
		var originalColour = Console.ForegroundColor;
		Console.ForegroundColor = ConsoleColor.Red;
		Console.Write($"{n}");
		Console.ForegroundColor = originalColour;
		Console.Write(" is a ");
		Console.ForegroundColor = ConsoleColor.Red;
		Console.Write("prime");
		Console.ForegroundColor = originalColour;
		Console.Write(" number. ");

		Console.ForegroundColor = ConsoleColor.Gray;
		Console.Write($"Calculation Time {computeTime}");
		Console.ForegroundColor = originalColour;
		Console.WriteLine();
	}

	public static void DisplayVersion()
	{
		var softwareVersion = Program.SoftwareVerison();
		var assemblyName = typeof(CmdlineSettings).Assembly.GetName();
		var supported = "is supported";
		var unsupported = "is not supported";

#if DEBUG
		ConsoleExtended.WriteLine($"{assemblyName?.Name} version {softwareVersion} debug build.", ConsoleColor.DarkRed);
#else
		Console.WriteLine($"{assemblyName?.Name} version {softwareVersion}");
#endif

#if NET6_0
		Console.WriteLine("Target runtime .NET 6.0");
#elif NET7_0
		Console.WriteLine("Target runtime .NET 7.0");
#elif NET8_0
		Console.WriteLine("Target runtime. NET 8.0");
#elif NET9_0
		Console.WriteLine("Target runtime .NET 9.0");
#elif NET10_0
		Console.WriteLine("Target runtime .NET 10.0");
#endif

		Console.WriteLine($"CLR version: {Environment.Version}, {(Environment.Is64BitProcess ? "64" : "32")} bit process.");
		Console.WriteLine($"Running on {Environment.OSVersion}");
		Console.WriteLine($"Maximum degree of parallelisum: {Environment.ProcessorCount} processors.");

		Console.WriteLine($"OS Architecture: {RuntimeInformation.OSArchitecture}");
		Console.WriteLine($"Process Architecture: {RuntimeInformation.ProcessArchitecture}");

		if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
		{
			Console.WriteLine("Intel SSE {0}.", Sse.IsSupported ? supported : unsupported);
			Console.WriteLine("Intel AVX {0}.", Avx.IsSupported ? supported : unsupported);
			Console.WriteLine("Intel AVX2 {0}.", Avx2.IsSupported ? supported : unsupported);
		}

		Console.WriteLine();
	}

	/// <summary>
	///     Displays the command line parameters
	/// </summary>
	public static void DisplayHelp()
	{
		const string owl1 = "\n" + @" /\_/\" + "\n" + " (O,O)\n" + " (:::)\n" + @"--""-""--" + "\t";
		Console.WriteLine(owl1);

		var colour = ConsoleColor.Blue;
		var originalForegroundColour = Console.ForegroundColor;

		Console.WriteLine();
		Console.WriteLine("PrimeFactor [commands] [data]\n");

		ConsoleExtended.WriteLine("Display this help screen.");
		ConsoleExtended.WriteLine("\t--help\n", colour);

		Console.WriteLine("Display the program version and poignuant platform information.");
		ConsoleExtended.WriteLine("\t--version\n", colour);

		ConsoleExtended.WriteLine("Calculate the prime factors of the given number n.");
		Console.WriteLine("If the number is prime, then the output will be the single value n that is itself prime.");
		Console.WriteLine("If the number is composite, then the output will be the calculated prime factors of n.");
		Console.WriteLine("If a list of numbers are given, then the output will be the calculated prime factors of each number.");
		ConsoleExtended.WriteLine("\t--factor [n | n .. nk] \n", colour);

		Console.WriteLine("Primality test of a given number, returns true or false");
		Console.WriteLine("Intended for use with scripting languages or in programs that are chained together using the pipe operator.");
		Console.WriteLine("If a list of numbers are given n..nk, then the output will be true or false for each number.");
		ConsoleExtended.WriteLine("\t--IsPrime [n | n .. nk] \n", colour);

		Console.WriteLine("Generate a list of prime numbers.");
		Console.WriteLine("The default is to generate all primes from 0 to 2^64-1.");
		Console.WriteLine("The --min and --max options are used to set the range of prime numbers to generate.");
		Console.WriteLine("The --count option is used to set the maximum number of primes to generate.");
		Console.WriteLine("If both --max and --count are specified, then the generation will stop when the first of the two limits is reached.");
		Console.WriteLine("Use the --filename option when you want the generated prime values to be saved to the given file instead of the terminal window.");
		ConsoleExtended.WriteLine("\t--generate [--min value] [--max value] [--count value] [--filename result.txt]\n", colour);

		Console.WriteLine("Benchmark1, platform performance measurement of primality testing from values from 1 to limit. The default limit is 10,000,000, and the maximum limit value that the implementation will accept is 4,294,967,296. Specifying a limit of 0 will indicate that the maximum limit should be used. This benchmark does not use caching of previously found primes. The default number of runs is 1. When more than 1 run is specified, each benchmark is run in turn, with additional statistical information calculated. Minimum, Maximum, Median and Average.");
		ConsoleExtended.WriteLine("\t--benchmark1 [limit | limit runs]\n", colour);

		Console.WriteLine("Benchmark2, same as Benchmark1, except that it partitions the number range 1 to limit by the number of available processor cores and performs the primality test in parallel.");
		ConsoleExtended.WriteLine("\t--benchmark2 [limit | limit runs]\n", colour);

		Console.WriteLine("Benchmark3, same as Benchmark1 except that it uses memory to cache the prime values as it proceeds through the number range 1 to limit.");
		ConsoleExtended.WriteLine("\t--benchmark3 [limit | limit runs]\n", colour);

		Console.ForegroundColor = originalForegroundColour;
	}
}
