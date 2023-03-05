using Semantics;

Analyzer analyzer;
try
{
    analyzer = new Analyzer();
    analyzer.ReadTokenTable(args[0]);
}
catch (FileNotFoundException e)
{
    Console.WriteLine("File not found: " + e.FileName);
    return;
}

analyzer.OnError += PrintError;

var identifiers = analyzer.GetIdentifierTokens();
Console.WriteLine($"Identifiers:\n{string.Join(Environment.NewLine, identifiers)}");

analyzer.CreateSymbolTable(identifiers);
Console.WriteLine($"Symbol table:\n{string.Join(Environment.NewLine, analyzer.Symbols)}");

void PrintError(string message)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(message);
    Console.ResetColor();
    Environment.Exit(1);
}
