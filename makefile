main: csgoslin/parser/KnownGrammars.cs csgoslin/domain/ClassesEnum.cs
	dotnet build csgoslin
	
csgoslin/parser/KnownGrammars.cs:
	dotnet build writeGrammarHeader
	chmod u+x ./writeGrammarHeader/bin/Debug/net47/writeGrammarHeader.exe
	mono writeGrammarHeader/bin/Debug/net47/writeGrammarHeader.exe

	
csgoslin/domain/ClassesEnum.cs: csgoslin/parser/KnownGrammars.cs
	mkdir -p writeLipidEnums/parser
	cp csgoslin/parser/BaseParserEventHandler.cs writeLipidEnums/parser/.
	cp csgoslin/parser/KnownGrammars.cs writeLipidEnums/parser/.
	cp csgoslin/parser/Parser.cs writeLipidEnums/parser/.
	cp csgoslin/parser/ParserClasses.cs writeLipidEnums/parser/.
	cp csgoslin/parser/SumFormulaParser.cs writeLipidEnums/parser/.
	cp csgoslin/parser/SumFormulaParserEventHandler.cs writeLipidEnums/parser/.
	cp csgoslin/domain/Element.cs writeLipidEnums/parser/.
	cp csgoslin/domain/LipidExceptions.cs writeLipidEnums/parser/.
	cp csgoslin/domain/StringFunctions.cs writeLipidEnums/parser/.
	dotnet build writeLipidEnums
	chmod u+x ./writeLipidEnums/bin/Debug/net47/writeLipidEnums.exe
	mono writeLipidEnums/bin/Debug/net47/writeLipidEnums.exe
	
	
clean:
	rm -rf writeGrammarHeader/bin
	rm -rf writeGrammarHeader/obj
	rm -rf writeLipidEnums/parser
	rm -rf writeLipidEnums/bin
	rm -rf writeLipidEnums/obj
	rm -rf csgoslin/bin
	rm -rf csgoslin/obj
	rm -rf csgoslin/domain/ClassesEnum.cs
	rm -rf csgoslin/parser/KnownGrammars.cs
