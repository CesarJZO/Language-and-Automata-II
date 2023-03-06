using Semantics;

var analyzer = new Analyzer();
analyzer.OnError += PrintError;

try
{
    analyzer.PerformFullAnalysis(args[0]);
}
catch (FileNotFoundException e)
{
    PrintError($"File not found: {e.FileName}");
}
catch (IndexOutOfRangeException)
{
    PrintError("Provide the path of the file containing the token table. \"SemanticAnalyzer.exe <input_file.csv>\"");
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
