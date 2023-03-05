using Semantics;

Analyzer analyzer;
try
{
    analyzer = new Analyzer();
    // analyzer.ReadTokenTable(args[0]);
    analyzer.PerformFullAnalysis(args[0]);
}
catch (FileNotFoundException e)
{
    Console.WriteLine("File not found: " + e.FileName);
    return;
}

analyzer.OnError += PrintError;

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Semantic analysis successful.");

void PrintError(string message)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(message);
    Console.ResetColor();
    Environment.Exit(1);
}
