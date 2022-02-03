# C# implementation for Goslin
[![.NET](https://github.com/lifs-tools/csgoslin/actions/workflows/dotnet.yml/badge.svg)](https://github.com/lifs-tools/csgoslin/actions/workflows/dotnet.yml)

This is the Goslin reference implementation for C#.

> **_NOTE:_**  Please report issues to help improve it!

csgoslin has been developed with regard the following general issues:

1. Facilitate the parsing of shorthand lipid names dialects.
2. Provide a structural representation of the shorthand lipid after parsing.
3. Use the structural representation to generate normalized names, following the latest shorthand nomenclature.

## Related Projects

- [This project](https://github.com/lifs-tools/csgoslin)
- [Goslin grammars and test files](http://github.com/lifs-tools/goslin)
- [C++ implementation](https://github.com/lifs-tools/cppgoslin) 
- [Java implementation](https://github.com/lifs-tools/jgoslin)
- [Python implementation](https://github.com/lifs-tools/pygoslin)
- [R implementation](https://github.com/lifs-tools/rgoslin)
- [Webapplication and REST API](https://github.com/lifs-tools/goslin-webapp)

## Installation
C# implementation of the Goslin framework


## Prerequisites

In order to build csgoslin, please install the lastest [DotNet 5.0.x](https://dotnet.microsoft.com/en-us/download/dotnet/5.0) release for your operating system.

For up to date instructions, please check our CI script here: https://github.com/lifs-tools/csgoslin/blob/main/.github/workflows/dotnet.yml

### dotnet

To install any external dependencies, run:

```
dotnet restore
```

To compile the code, run:

```
dotnet build
```

To clean everything up, enter:

```
dotnet clean
```

You can run the unit tests as follows:

```
dotnet build -t:Test --verbosity normal csgoslin.Tests/csgoslin.Tests.csproj
```

### Legacy msbuild

With msbuild

On Windows

```
msbuild.exe /t:restore /t:build
```

On Linux

```
msbuild /t:restore /t:build
```
