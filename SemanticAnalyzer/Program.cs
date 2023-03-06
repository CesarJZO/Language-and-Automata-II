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
catch (IndexOutOfRangeException)
{
    Console.WriteLine("Provide the path of the file containing the token table.\nUsage: SemanticAnalyzer.exe <input_file.csv>");
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
