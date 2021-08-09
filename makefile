main:
	dotnet build writeGrammarHeader
	chmod u+x ./writeGrammarHeader/bin/Debug/net47/writeGrammarHeader.exe
	mono writeGrammarHeader/bin/Debug/net47/writeGrammarHeader.exe
	mv writeGrammarHeader/bin/Debug/net47/KnownGrammars.cs csgoslin/domain/KnownGrammars.cs
	dotnet build writeLipidEnums
	chmod u+x ./writeLipidEnums/bin/Debug/net47/writeLipidEnums.exe
	mono writeLipidEnums/bin/Debug/net47/writeLipidEnums.exe
	mv writeLipidEnums/bin/Debug/net47/ClassesEnum.cs csgoslin/parser/ClassesEnum.cs
	dotnet build csgoslin

clean:
	rm -rf writeGrammarHeader/bin
	rm -rf writeGrammarHeader/obj
	rm -rf writeLipidEnums/bin
	rm -rf writeLipidEnums/obj
	rm -rf csgoslin/bin
	rm -rf csgoslin/obj
	rm -rf csgoslin/domain/ClassesEnum.cs
	rm -rf csgoslin/parser/KnownGrammars.cs
