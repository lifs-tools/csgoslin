main:
	dotnet build csgoslin/csgoslin.csproj

	
test:
	dotnet build csgoslin.Tests/csgoslin.Tests.csproj -t:Test

	
clean:
	rm -rf writeGrammarHeader/bin
	rm -rf writeGrammarHeader/obj
	rm -rf writeLipidEnums/parser
	rm -rf writeLipidEnums/bin
	rm -rf writeLipidEnums/obj
	rm -rf csgoslin/bin
	rm -rf csgoslin/obj
	rm -rf csgoslin.Tests/bin
	rm -rf csgoslin.Tests/obj
	rm -rf csgoslin/domain/ClassesEnum.cs
	rm -rf csgoslin/domain/LipidClasses.cs
	rm -rf csgoslin/domain/KnownFunctionalGroups.cs
	rm -rf csgoslin/parser/KnownGrammars.cs
