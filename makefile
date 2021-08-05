main:
	dotnet build writeSourceFiles
	./writeSourceFiles/bin/Debug/net5.0/writeSourceFiles
	dotnet build csgoslin

clean:
	rm -rf csgoslin/domain/ClassesEnum.cs
