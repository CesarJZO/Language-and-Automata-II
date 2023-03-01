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
Console.WriteLine(analyzer);
analyzer.FullAnalysis();

void PrintError(string message)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(message);
    Console.ResetColor();
}
