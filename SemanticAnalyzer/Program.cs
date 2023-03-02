using Semantics;

Analyzer analyzer;

try
{
    analyzer = new Analyzer(args[0]);
    analyzer.ReadTokenTable();
}
catch (FileNotFoundException e)
{
    Console.WriteLine("File not found: " + e.FileName);
    return;
}

analyzer.OnError += PrintError;
analyzer.ReadTokenTable();

var identifiers = analyzer.GetIdentifierTokens();
Console.WriteLine($"Identifiers:\n{string.Join(Environment.NewLine, identifiers)}");

analyzer.CreateSymbolTable();
Console.WriteLine($"Symbol table:\n{string.Join(Environment.NewLine, analyzer.Symbols)}");
identifiers = analyzer.GetIdentifierTokens();
Console.WriteLine($"Identifiers updated:\n{string.Join(Environment.NewLine, identifiers)}");

void PrintError(string message)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(message);
    Console.ResetColor();
    Environment.Exit(1);
}
