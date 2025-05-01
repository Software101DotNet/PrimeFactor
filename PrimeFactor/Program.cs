// PrimeFactor
// A command line tool for finding prime factors of numbers, generating primes, and calculating GCD.
// The use of this source code file is governed by the license outlined in the License.txt file of this project.
// https://github.com/Software101DotNet/PrimeFactor

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

							if (factored.Factors().Count == 1)
								Console.WriteLine($"{factored.Value()} is prime");
							else
								Console.WriteLine($"{factored.Value()} has prime factors {string.Join(" ", factored.Factors())}");
						}
					}
					break;

				case Modes.PerfectNumber:
					throw new NotImplementedException("Perfect Number calculation will be implemented in a later version");

				case Modes.GCD:
					throw new NotImplementedException("Greatest Command Factor calculation will be implemented in a later version.");

				case Modes.Benchmark:
					Benchmark.SquareRoot();
					//Benchmark.Serial10M();
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
