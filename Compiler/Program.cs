using Semantics;
using IntermediateCode;

var analyzer = new Analyzer();
analyzer.OnError += PrintError;

try
{
    // analyzer.ReadTokenTable(args[0]);
    analyzer.PerformFullAnalysis(args[0]);
    return;
}
catch (FileNotFoundException e)
{
    PrintError($"File not found: {e.FileName}");
}
catch (IndexOutOfRangeException)
{
    PrintError("Provide the path of the file containing the token table. \"SemanticAnalyzer.exe <input_file.csv>\"");
}

var vector = new IntermediateCodeVector();

Token[] bodyTokens = vector.GetBodyTokens(analyzer.Tokens);

vector.GenerateIcv(bodyTokens);

Console.WriteLine($"""
VCI:
[{string.Join(", ", vector.Select(t => t.Lexeme))}]
"""
);

void PrintError(string message)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(message);
    Console.ResetColor();
    Environment.Exit(1);
}
