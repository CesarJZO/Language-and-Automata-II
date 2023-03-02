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
Console.WriteLine($"Identifiers updated:\n{string.Join(Environment.NewLine, analyzer.Tokens)}");

void PrintError(string message)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(message);
    Console.ResetColor();
    Environment.Exit(1);
}
