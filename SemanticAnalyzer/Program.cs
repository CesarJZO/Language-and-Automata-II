using Semantics;

var analyzer = new Analyzer();
analyzer.OnError += PrintError;

try
{
    analyzer.ReadTokenTable(args[0]);
}
catch (FileNotFoundException e)
{
    PrintError($"File not found: {e.FileName}");
}
catch (IndexOutOfRangeException)
{
    PrintError("Provide the path of the file containing the token table. \"SemanticAnalyzer.exe <input_file.csv>\"");
}

var identifierTokens = analyzer.GetIdentifierTokens();
analyzer.CheckForRepeatedIdentifiers(identifierTokens);
analyzer.CreateSymbolTable(identifierTokens);
analyzer.WriteFiles();
analyzer.CheckSymbolUsage();

Console.WriteLine($"Identifiers:\n{string.Join("\n", identifierTokens)}\n");
Console.WriteLine($"Symbols:\n{string.Join("\n", analyzer.Symbols)}");

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Semantic analysis successful.");
Console.ResetColor();

void PrintError(string message)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(message);
    Console.ResetColor();
    Environment.Exit(1);
}
