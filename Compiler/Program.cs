using Language;
using Semantics;
using IntermediateCode;

var analyzer = new Analyzer();

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

var vector = new IntermediateCodeVector();
Token[] bodyTokens = vector.GetBodyTokens(analyzer.Tokens);
vector.GenerateIcv(bodyTokens);

Console.WriteLine($"""
VCI: {vector.Count()}
{vector}
"""
);

void PrintError(string message)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(message);
    Console.ResetColor();
    Environment.Exit(1);
}
