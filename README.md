# csgoslin
C# implementation of the Goslin framework


## Building


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
