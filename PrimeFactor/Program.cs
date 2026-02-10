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
// along with this program.  If not, see <https://www.gnu.org/licenses/>

namespace PrimeFactor;

public class Program
{
	enum ExitCode : int { Success = 0, CmdLineError = 1, Exception = 2 };

	static Settings settings;

	static int Main(string[] args)
	{
		var exitCode = ExitCode.Success;
		try
		{
			settings = CmdlineSettings.Parse(args);
			
			switch (settings.Mode)
			{
				case Modes.GeneratePrimes:
					{
						using var writer = string.IsNullOrEmpty(settings.DataFilename)
							? new StreamWriter(Console.OpenStandardOutput())
							: new StreamWriter(settings.DataFilename);

						Prime.GeneratePrimes(writer, settings.maxPrimeCount, settings.maxPrime);
					}
					break;

				case Modes.Factor:
					if (settings.candidates != null)
					{
						foreach (var candidateFactor in settings.candidates)
						{
							var factored = new Factored(candidateFactor);

							if (factored.Factors().Count == 0)
								Console.WriteLine($"{factored.Value()} is neither prime nor composite");
							else if (factored.Factors().Count == 1)
								Console.WriteLine($"{factored.Value()} is prime");
							else
								Console.WriteLine($"{factored.Value()} has prime factors {string.Join(" ", factored.Factors())}");
						}
					}
					break;

				case Modes.IsPrime:
					foreach (var candidateFactor in settings.candidates)
					{
						if (Prime.IsPrime(candidateFactor))
							Console.WriteLine($"true");
						else
							Console.WriteLine($"false");
					}
					break;

				case Modes.PerfectNumber:
					throw new NotImplementedException("Perfect Number calculation will be implemented in a later version");

				case Modes.GCD:
					throw new NotImplementedException("Greatest Command Factor calculation will be implemented in a later version.");

				case Modes.Benchmark1:
					Benchmark.MultipleRuns(Benchmark.SerialBenchmark, settings.benchmarkLimit, settings.benchmarkRuns);
					break;

				case Modes.Benchmark2:
					throw new NotImplementedException("Benchmark2 not implemented in this version.");

				case Modes.Benchmark3:
					Benchmark.MultipleRuns(Benchmark.Benchmark3, settings.benchmarkLimit, settings.benchmarkRuns);
					break;

				case Modes.Version:
					ConsoleDisplay.DisplayVersion();
					break;

				case Modes.Undefined:
				case Modes.Help:
				default:
					ConsoleDisplay.DisplayHelp();
					break;
			}
		}
		catch (Exception e)
		{
			ConsoleExtended.WriteLine($"{e.Message}", ConsoleColor.DarkRed);

			if (e.InnerException != null)
				ConsoleExtended.WriteLine(e.InnerException.Message, ConsoleColor.DarkYellow);

			exitCode = ExitCode.Exception;
		}

		return (int)exitCode; // Application exit code for use with commandline chaining or scripting
	}

	internal static Version SoftwareVerison()
	{
		var assemblyName = typeof(CmdlineSettings).Assembly.GetName();
		var version = assemblyName?.Version ?? new Version(0, 0, 0, 0);
		return version;
	}
}
