[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![GPL License][license-shield]][license-url]

<div align="center">

<h3 align="center">Prime Factor</h3>

  <p align="center">
    A command line tool for assisting number theory research, teaching and cryptography subjects. 
    <br>Runs on Windows, macOS and Linux. Native support for x64 and ARM platforms<br/>
    <br />
    <a href="https://github.com/Software101DotNet/PrimeFactor">View Demo</a>
    ·
    <a href="https://github.com/Software101DotNet/PrimeFactor/issues">Report Bug</a>
    ·
    <a href="https://github.com/Software101DotNet/PrimeFactor/issues">Request Feature</a>
  </p>
</div>

<!-- TABLE OF CONTENTS, can be removed if readme is small -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

[Prime Factor](https://www.software101.net/PrimeFactor)

A command line tool for assisting number theory research, teaching and cryptography subjects:
  * Factoring a composite given number into its prime factors
  * Generating a series of prime numbers of a given range from 1 to 18,446,744,073,709,551,616.
  * Calculate the Greatest Common Divisor for a given series of numbers


### Built With

* Dot Net 9
* VS Code


## Getting Started

* Install DotNet if your platform does not already have it. 
* Clone this repo.
* Build the project.
* Run "./PrimeFactor --help" for usage instructions. 


### Prerequisites

  Install DotNet 9.0 for the platform Windows, macOS or Linux. Intel x64 and Apple Silicon support. 
  
  If unsure if Dot Net is install or which version you already have, then open a terminal and run the following command.

  ```
  dotnet --version
  ```

  Download and install from:

  ```sh
  https://dotnet.microsoft.com/en-us/download
  ```

### Installation

1. Clone the project's repository
   ```sh
   git clone https://github.com/Software101DotNet/PrimeFactor.git
   ```
2. Build the project
   ```sh
   dotnet build -c Release PrimeFactor/PrimeFactor.csproj
   ```
3. Optionally, run the project's unit tests. Optionally, build the debug version and run the project's unit tests. Depending on the computer, these tests can take a few seconds to several minutes. In the unlikely event that any tests fail, please create a [new issue](https://github.com/Software101DotNet/CreateDotNetProject/issues) ticket.
   ```sh
   dotnet build -c debug PrimeFactor.sln
   dotnet test xUnitTests/bin/Debug/net9.0/xUnitTests.dll
   ```


## Usage

For upto date usage, Run 
```
./PrimeFactor --help 
```

## Roadmap

- [completed v1.0] Prime factoring of given number
- [completed v1.0] Prime number series generation
    - [Complated v1.0] support for directing output to a file stream. Usesful for very large series generation.
- [ v2 ] Support for Greatest Common Divisor (supporting a series of more than two values)
- [ v3 ] Support for Perfect Numbers
- [ v4 ] Support for number range 1 to 340,282,366,920,938,463,463,374,607,431,768,211,455 (2^128 bits) 

See the [open issues](https://github.com/Software101DotNet/CreateDotNetProject/issues) for a full list of proposed features (and known issues).


## Notes On Performance

### benchmark3
Building PrimeFactor for release using the command line `dotnet build --configuration Release` and running the benchmark3 100 times with the command `./bin/Release/net9.0/PrimeFactor --benchmark3 1000000 100` reveals a performance side effect. between run 61 and 62, a step change in performance occurs. The median value of run 1 to 61 is 600ms, the median value of run 62 to 100 is 479ms. This is a 20% reduction in time taken to compute the same calculation.
```
Benchmarking primality test for values between 1 and 1,000,000
Run 1 completed in 632ms 
Run 2 completed in 583ms 
 ...
Run 61 completed in 604ms 
Run 62 completed in 479ms 
 ...
Run 99 completed in 480ms 
Run 100 completed in 478ms 
Time to compute each run 478ms ~ 632ms
```

Running a longer benchmark of 250 runs of a primality test from 1 to 10 million also results in a step change occurring between runs 61 and 62. This is believed to be due to the JIT compiler and the .NET runtime performing hot-path optimisations. 
```
Benchmarking 250 runs, please wait...
Run 1 Benchmarking 10,000,000 primes... Completed in 15s 334ms
Run 2 Benchmarking 10,000,000 primes... Completed in 15s 305ms
...
Run 61 Benchmarking 10,000,000 primes... Completed in 15s 302ms
Run 62 Benchmarking 10,000,000 primes... Completed in 11s 446ms
...
Run 249 Benchmarking 10,000,000 primes... Completed in 11s 471ms
Run 250 Benchmarking 10,000,000 primes... Completed in 11s 502ms
Time to compute 11s 412ms ~ 15s 334ms , median 11s 464ms
```

This anommally was explored further by creating a C language implementing of the same benchmarks. This sub project can be found in the subfolder *PrimeFactorC*


## Contributing

If you have any additions or improvements that would make this project better, please fork the repository and create a pull request. You can also simply open an issue with the tag "enhancement". Thanks.


1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request


## License 

Distributed under the GPL License. See `LICENSE.txt` for more information.


## Contact

Anthony Ransley - software101.net@gmail.com

Project Link: [https://github.com/Software101DotNet/PrimeFactor](https://github.com/Software101DotNet/PrimeFactor)


## Acknowledgments

* [Choose an Open Source License](https://choosealicense.com)
* [GitHub Pages](https://pages.github.com)
* [DotNet Download](https://dotnet.microsoft.com/en-us/download)
* [DotNet Documentation](https://learn.microsoft.com/en-us/dotnet)





<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/Software101DotNet/PrimeFactor.svg?style=for-the-badge
[contributors-url]: https://github.com/Software101DotNet/PrimeFactor/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/Software101DotNet/PrimeFactor.svg?style=for-the-badge
[forks-url]: https://github.com/Software101DotNet/PrimeFactor/network/members
[stars-shield]: https://img.shields.io/github/stars/Software101DotNet/PrimeFactor.svg?style=for-the-badge
[stars-url]: https://github.com/Software101DotNet/PrimeFactor/stargazers

[issues-shield]: https://img.shields.io/github/issues/Software101DotNet/PrimeFactor.svg?style=for-the-badge
[issues-url]: https://github.com/Software101DotNet/CreateDotNetProject/issues

[license-shield]: https://img.shields.io/github/license/Software101DotNet/PrimeFactor.svg?style=for-the-badge
[license-url]: https://github.com/Software101DotNet/PrimeFactor/blob/main/license.txt

[product-screenshot]: images/screenshot.png
[Next.js]: https://img.shields.io/badge/next.js-000000?style=for-the-badge&logo=nextdotjs&logoColor=white
[Next-url]: https://nextjs.org/
[React.js]: https://img.shields.io/badge/React-20232A?style=for-the-badge&logo=react&logoColor=61DAFB
[React-url]: https://reactjs.org/
[Vue.js]: https://img.shields.io/badge/Vue.js-35495E?style=for-the-badge&logo=vuedotjs&logoColor=4FC08D
[Vue-url]: https://vuejs.org/
[Angular.io]: https://img.shields.io/badge/Angular-DD0031?style=for-the-badge&logo=angular&logoColor=white
[Angular-url]: https://angular.io/
[Svelte.dev]: https://img.shields.io/badge/Svelte-4A4A55?style=for-the-badge&logo=svelte&logoColor=FF3E00
[Svelte-url]: https://svelte.dev/
[Laravel.com]: https://img.shields.io/badge/Laravel-FF2D20?style=for-the-badge&logo=laravel&logoColor=white
[Laravel-url]: https://laravel.com
[Bootstrap.com]: https://img.shields.io/badge/Bootstrap-563D7C?style=for-the-badge&logo=bootstrap&logoColor=white
[Bootstrap-url]: https://getbootstrap.com
[JQuery.com]: https://img.shields.io/badge/jQuery-0769AD?style=for-the-badge&logo=jquery&logoColor=white
[JQuery-url]: https://jquery.com 
