// PrimeFactor
// A command line tool for finding prime factors of numbers, generating primes, and calculating GCD.
// The use of this source code file is governed by the license outlined in the License.txt file of this project.
// https://github.com/Software101DotNet/PrimeFactor

using System.Diagnostics;

namespace PrimeFactor;

public enum Modes
{
	Undefined,  // program run with either no commands or an invalid command
	Help,       // display usage information
	Version,    // display version information

	Factor,     // find the prime factors of the given number or indicate that the number is prime.
	GeneratePrimes,
	GCD         // find the greatest common divisor of the given set of numbers.
}

public enum LogLevel { Quite = 0, Error, Warning, Info, Diag }

public class CmdlineSettings
{
	private const string ConflictingModesErrorMsg = "Multiple mode commands given. See help for usage information.";
	private const string defaultCommand = "--default";

	public string DataFilename { get; private set; } = string.Empty;
	public bool DevMode { get; private set; } = false;// used to enable development mode features. Time expensive or destructive processing operations will be skipped.
	public Modes Mode { get; private set; } = Modes.Undefined;
	public LogLevel LogLevel { get; private set; } = LogLevel.Warning;

	// List of candidates to factor. 
	public List<ulong> candidates { get; private set; } = new List<ulong>();

	public ulong maxPrime { get; private set; } = ulong.MaxValue;
	public ulong minPrime { get; private set; } = 1;
	public int maxPrimeCount { get; private set; } = Int32.MaxValue / 2; // the upper limit of the memory cache of generated primes

	/// <summary>
	/// Parses the command line arguments and sets the appropriate properties.
	/// </summary>
	/// <param name="args"></param>
	/// <returns></returns>
	public CmdlineSettings(string[] args)
	{
		try
		{
			var result = SeparateParams(args);
			foreach (var param in result)
			{
				switch (param.Key)
				{
					case "--help":
						if (Mode != Modes.Undefined)
						{
							throw new ArgumentException(ConflictingModesErrorMsg);
						}
						Mode = Modes.Help;
						break;

					case "--version":
						if (Mode != Modes.Undefined)
						{
							throw new ArgumentException(ConflictingModesErrorMsg);
						}
						Mode = Modes.Version;
						break;

					case defaultCommand:
					case "--factor":
						if (Mode != Modes.Undefined)
						{
							throw new ArgumentException(ConflictingModesErrorMsg);
						}
						Mode = Modes.Factor;
						if (param.Value.Count < 2)
						{
							throw new ArgumentException("--factor value not specified.");
						}
						foreach (var s in param.Value[1..])
						{
							if (ulong.TryParse(s, out ulong n))
							{
								candidates.Add(n);
							}
							else
							{
								throw new ArgumentException($"Invalid number {s} specified for --factor.");
							}
						}
						break;

					case "--generate":
						if (Mode != Modes.Undefined)
						{
							throw new ArgumentException(ConflictingModesErrorMsg);
						}
						Mode = Modes.GeneratePrimes;
						break;

					case "--filename":
						if (param.Value.Count < 2)
						{
							throw new ArgumentException("--filename value not specified.");
						}
						DataFilename = param.Value[1];
						break;

					case "--min":
						if (param.Value.Count < 2)
						{
							throw new ArgumentException("--min value not specified.");
						}
						minPrime = ulong.Parse(param.Value[1]);
						break;

					case "--max":
						if (param.Value.Count < 2)
						{
							throw new ArgumentException("--max value not specified.");
						}
						maxPrime = ulong.Parse(param.Value[1]);
						break;

					case "--count":
						if (param.Value.Count < 2)
						{
							throw new ArgumentException("--count value not specified.");
						}
						try
						{
							maxPrimeCount = int.Parse(param.Value[1]);
							if (maxPrimeCount <= 0 || maxPrimeCount > int.MaxValue)
								throw new ArgumentOutOfRangeException($"--count has valid range of 1 to {int.MaxValue - 1}");
						}
						catch (OverflowException e)
						{
							throw new OverflowException($"parameter --count has valid range of 1 to {int.MaxValue - 1}", e);
						}
						break;

					case "--dev":
						DevMode = true;
						break;

					case "--loglevel":
						if (param.Value.Count == 2) 
							LogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), param.Value[1], true);
						break;

					default:
						throw new ArgumentException($"{param.Key} is not a recognised command.");
				}
			}
		}
		catch (ArgumentException )
		{
			if (LogLevel >= LogLevel.Info)
			{
				ConsoleDisplay.DisplayHelp();
			}
			throw;
		}
	}

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