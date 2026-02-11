<br />
<div align="center">

<h3 align="center">Prime Factor</h3>

  <p align="center">
    A C language implementation of the Primality Test used in <i>PrimeFactor</i>. 
  <br/>
  <br/>
    <a href="https://github.com/Software101DotNet/PrimeFactor">View Project</a>
    ·
    <a href="https://github.com/Software101DotNet/PrimeFactor/issues">Report Bug</a>
    ·
    <a href="https://github.com/Software101DotNet/PrimeFactor/issues">Request Feature</a>
  </p>
</div>


This C23 implementation was written later than the parent PrimeFactor project, for the purpose of a performance comparison between C and C# on the macOS ARM platform. The areas of interest affecting performance between the two implementations are: 
<br/>
- benchmark 1: raw single cpu core computation performance between C and C# implementations.
<br/>
<br/>
- benchmark 2: C# memory-managed versus C unmanaged memory caching of prime values as the primality search increased in value. 
<br/>
<br/>
- benchmark 3: Split the number range into partitions and use separate cpu cores to perform the calculations of each partition in parallel.
<br/>

### benchmark1 
Taking Dotnet memory allocation out of the equation, benchmark1 does not allocate memory to cache the prime values found as it processes the range 1 to limit. It just uses the simpler technique of testing with each odd value each time, up to the square root of the limit of the range. 

Benchmark 1. Primality test for values between 1 and 4,294,967,296 -> 203,280,221 of these values are prime.

#### Apple M4 (macbook air), Tahoe 26.1
-  C language implementation <br/>
`./build/release/PrimeFactorC --benchmark1 0` <br/>
Time to compute was 2,952 to 2,959 seconds per execution run, averaging 2,958 seconds over 5 runs.


- C# language implementation on DotNet 9.0.202 <br/>
`./PrimeFactor/bin/Release/net9.0/PrimeFactor --benchmark1 0` <br/>
Time to compute was 3,255 to 3,416 seconds per execution run, averaging 3,289 seconds over 5 runs. 

  The C# implementation took an average of 11% longer to compute the same calculation than the C implementation. 

#### Intel Xeon E5-2697 v2, Windows 10
- todo








