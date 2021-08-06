main:
	dotnet build writeGrammarHeader
	./writeGrammarHeader/bin/Debug/net5.0/writeGrammarHeader
	dotnet build writeLpiidEnum
	./writeLpiidEnum/bin/Debug/net5.0/writeSourceFiles
	dotnet build csgoslin

clean:
	rm -rf writeGrammarHeader/bin
	rm -rf writeGrammarHeader/obj
	rm -rf writeLpiidEnum/bin
	rm -rf writeLpiidEnum/obj
	rm -rf csgoslin/bin
	rm -rf csgoslin/obj
	rm -rf csgoslin/domain/ClassesEnum.cs
	rm -rf csgoslin/parser/KnownGrammars.cs
