using Semantics;

var analyzer = new Analyzer();
analyzer.OnError += PrintError;

try
{
    // analyzer.ReadTokenTable(args[0]);
    analyzer.PerformFullAnalysis(args[0]);
}
catch (FileNotFoundException e)
{
    Console.WriteLine($"File not found: {e.FileName}");
    Environment.Exit(1);
}

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
