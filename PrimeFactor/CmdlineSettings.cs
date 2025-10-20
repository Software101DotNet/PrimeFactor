// PrimeFactor
// A command line tool for finding prime factors of numbers, generating primes, and calculating GCD.
// The use of this source code file is governed by the license outlined in the License.txt file of this project.
// https://github.com/Software101DotNet/PrimeFactor

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
	IsPrime,	// less processing required then Factor. Intended for script languages
	Benchmark,  // benchmark performance 10M single threaded
	Benchmark2,	// benchmark performance 18,446,744,073,709,551,615 (2^64-1)
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

					case "--benchmark":
						SetMode(ref settings, Modes.Benchmark);
						break;

					case "--benchmark2":
						SetMode(ref settings, Modes.Benchmark2);
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