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


using System.Globalization;
using System.Runtime.CompilerServices;

namespace PrimeFactor;

public enum Modes
{
	Undefined,  // program run with either no commands or an invalid command
	Help,       // display usage information
	Version,    // display version information
	Factor,     // find the prime factors of the given number or indicate that the number is prime.
	GeneratePrimes,
	IsPrime,    // less processing required then Factor. Intended for script languages
	Benchmark1, // benchmark primality test of values in the range 1..limit 
	Benchmark2, // same as benchmark1, except employs parallelism.
	Benchmark3, // same as Benchmark1 except that it uses memory to cache the prime values as it proceeds through the number range 1 to limit
	PerfectNumber,
	GCD         // find the greatest common divisor of the given set of numbers.
}

public enum LogLevel { Quite = 0, Error, Warning, Info, Diag }

public struct Settings
{
	public Settings() { }
	public string DataFilename { get; set; } = string.Empty;
	public bool DevMode { get; set; } = false;// used to enable development mode features such as time expensive or destructive processing operations will be skipped.
	public Modes Mode { get; set; } = Modes.Undefined;
	public LogLevel LogLevel { get; set; } = LogLevel.Warning;
	public int benchmarkRuns { get; set; } = 1; // default number of benchmark runs to perform
	public ulong benchmarkLimit { get; set; } = 10_000_000; // the default limit to benchmark

	// List of candidates to factor. 
	public List<ulong> candidates { get; set; } = new List<ulong>();

	public ulong maxPrime { get; set; } = 100; // the default max value of a series of generate primes
	public ulong minPrime { get; set; } = 1;
	public ulong maxPrimeCount { get; set; } = 100; // the default maximum limit of the number of primes to generate or cache
}

public static class CmdlineSettings
{
	private const string ConflictingModesErrorMsg = "Multiple mode commands given. See help for usage information.";
	private const string defaultCommand = "--default";
	private const string MaxPrimeCountCommand = "--count";
	private const string MaxPrimeCommand = "--max";
	private const string MinPrimeCommand = "--min";

	/// <summary>
	/// Parses the command line arguments and sets the appropriate properties of the CmdlineSettings object.
	/// The command line arguments are expected to be in the form of "--<command> <value1> <value2> ...".
	/// </summary>
	/// <param name="args"></param>
	/// <returns></returns>	
	public static Settings Parse(string[] args)
	{
		Settings settings = new();

		try
		{
			var result = SeparateParams(args);
			foreach (var param in result)
			{
				switch (param.Key)
				{
					case "--help":
						SetMode(ref settings, Modes.Help);
						break;

					case "--version":
						SetMode(ref settings, Modes.Version);
						break;

					case defaultCommand:
					case "--factor":
						SetMode(ref settings, Modes.Factor);
						settings.candidates = ParseCommandValues(param.Value, "--factor");
						break;

					case "--generate":
						SetMode(ref settings, Modes.GeneratePrimes);
						break;

					case "--isprime":
						SetMode(ref settings, Modes.IsPrime);
						settings.candidates = ParseCommandValues(param.Value, "--isprime");
						break;

					case "--benchmark1":
						SetMode(ref settings, Modes.Benchmark1);
						settings = ParseBenchmarkValues(settings, param);
						break;

					case "--benchmark2":
						SetMode(ref settings, Modes.Benchmark2);
						settings = ParseBenchmarkValues(settings, param);
						break;

					case "--benchmark3":
						SetMode(ref settings, Modes.Benchmark3);
						settings = ParseBenchmarkValues(settings, param);
						break;

					case "--filename":
						if (param.Value.Count < 2)
						{
							throw new ArgumentException("--filename value not specified.");
						}
						settings.DataFilename = param.Value[1];
						break;

					case MinPrimeCommand:
						settings.minPrime = ParseCommandValue(param.Value, MinPrimeCommand);
						break;

					case MaxPrimeCommand:
						settings.maxPrime = ParseCommandValue(param.Value, MaxPrimeCommand);
						break;

					case MaxPrimeCountCommand:
						settings.maxPrimeCount = ParseCommandValue(param.Value, MaxPrimeCountCommand);
						break;

					case "--dev":
						settings.DevMode = true;
						break;

					case "--loglevel":
						if (param.Value.Count == 2)
							settings.LogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), param.Value[1], true);
						break;

					default:
						throw new ArgumentException($"{param.Key} is not a recognised command.");
				}
			}


		}
		catch (ArgumentException)
		{
			if (settings.LogLevel >= LogLevel.Info)
			{
				ConsoleDisplay.DisplayHelp();
			}
			throw;
		}

		return settings;

		static void SetMode(ref Settings settings, Modes mode)
		{
			if (settings.Mode != Modes.Undefined)
			{
				throw new ArgumentException(ConflictingModesErrorMsg);
			}
			settings.Mode = mode;
		}

		static ulong ParseCommandValue(List<string> param, string CommandName)
		{
			if (param.Count < 2)
			{
				throw new ArgumentException($"Value not specified for command {CommandName}");
			}
			if (ulong.TryParse(param[1], NumberStyles.AllowThousands, CultureInfo.CurrentUICulture, out ulong n))
			{
				return n;
			}
			else
			{
				throw new ArgumentException($"Invalid number {n} specified for command {CommandName}");
			}
		}

		static List<ulong> ParseCommandValues(List<string> param, string CommandName)
		{
			List<ulong> serise = new();
			if (param.Count < 2)
			{
				throw new ArgumentException($"Value not specified for command {CommandName}");
			}
			foreach (var s in param[1..])
			{
				if (ulong.TryParse(s, NumberStyles.AllowThousands, CultureInfo.CurrentUICulture, out ulong n))
				{
					serise.Add(n);
				}
				else
				{
					throw new ArgumentException($"Invalid number {s} specified for command {CommandName}");
				}
			}
			return serise;
		}

		static Settings ParseBenchmarkValues(Settings settings, KeyValuePair<string, List<string>> param)
		{
			try // benchmark limit and runs are optional values. The first value is interprated as the limit and the second as the number of runs.
			{
				var options = ParseCommandValues(param.Value, "--benchmark");
				if (options.Count >= 1)
					settings.benchmarkLimit = options[0] <= 0 ? 4294967296 : options[0];    // a given value of 0 is to be interprated to use the max value 2^32
				if (options.Count >= 2)
					settings.benchmarkRuns = (int)options[1];
			}
			catch (ArgumentException)
			{
				// In this case, this is not an error because benchmark runs is optional, so
				// leave the value as the default value, because an optional value was not specified.
			}

			return settings;
		}
	}

	/// <summary>
	/// Separates the command line arguments into a dictionary of parameters and their values.
	/// Each parameter is a key in the dictionary, and its values are stored in a list.	
	/// </summary>
	/// <param name="args"> 
	/// The command line arguments passed to the program.
	/// Each argument is expected to be a string, and parameters are expected to start with "--".
	/// </param>
	/// <returns>
	/// A dictionary where the key is the parameter name (e.g., "--factor") and the value is a list of strings representing the values for that parameter.
	/// The first value in the list is the parameter name itself, and subsequent values are the arguments associated with that parameter.
	/// </returns>
	public static Dictionary<string, List<string>> SeparateParams(string[] args)
	{
		var d = new Dictionary<string, List<string>>();
		string currentParam;

		// group arguments with each -- <SomeCommand> that is found.
		for (int i = 0; i < args.Length; i++)
		{
			currentParam = args[i].Trim();
			if (currentParam.StartsWith("--", StringComparison.Ordinal))
			{
				var subParams = new List<string> { currentParam };

				if ((i + 1) < args.Length)
				{
					var next = args[i + 1].Trim();
					if (!next.StartsWith("--", StringComparison.Ordinal))
					{
						i++;
						for (; i < args.Length; i++)
						{
							next = args[i].Trim();
							if (next.StartsWith("--", StringComparison.Ordinal))
							{
								i--;
								break;
							}
							subParams.Add(next);
						}
					}
				}
				d.Add(currentParam.ToLowerInvariant(), subParams);
			}
			else
			{
				// handle the case when the first group of arguments do not start with a -- <SomeCommand>  
				// Therefore, the --default command is assumed and the given arguments are group along with it. 
				var subParams = new List<string> { defaultCommand };
				subParams.Add(currentParam);
				if ((i + 1) < args.Length)
				{
					var next = args[i + 1].Trim();
					if (!next.StartsWith("--", StringComparison.Ordinal))
					{
						i++;
						for (; i < args.Length; i++)
						{
							next = args[i].Trim();
							if (next.StartsWith("--", StringComparison.Ordinal))
							{
								i--;
								break;
							}
							subParams.Add(next);
						}
					}
				}
				d.Add(defaultCommand, subParams);
			}
		}

		return d;
	}
}