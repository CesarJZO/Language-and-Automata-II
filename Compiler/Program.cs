using Language;
using IntermediateCode;

var tokensFilePath = string.Empty;
var symbolsFilePath = string.Empty;
try
{
    tokensFilePath = args[0];
    symbolsFilePath = args[1];
}
catch (IndexOutOfRangeException)
{
    PrintError($"Usage: {System.Diagnostics.Process.GetCurrentProcess().ProcessName} <icv_file> <symbols_file>");
}

Token[] tokens = Array.Empty<Token>();
Symbol[] symbols = Array.Empty<Symbol>();
try
{
    tokens = FileParser.GetTokensFromFile(tokensFilePath);
    symbols = FileParser.GetSymbolsFromFile(symbolsFilePath);
}
catch (FileNotFoundException e)
{
    PrintError($"File not found: {e.FileName}");
}
catch (Exception e)
{
    PrintError(e.Message);
}

var vector = new IntermediateCodeVector();
Token[] bodyTokens = Filter.GetBodyTokens(tokens);
vector.Generate(bodyTokens);

Console.WriteLine($"""
VCI: {vector.Count()}
{vector}
"""
);

IcvExecutable executable = new(
    vector: vector,
    symbols: symbols,
    readFunction: Console.ReadLine,
    writeFunction: Console.WriteLine
);

Symbol[] updatedSymbols = executable.ExecuteIcv();

Console.WriteLine($"""
Symbol table:
{"\t"}{string.Join("\n\t", (IEnumerable<Symbol>)updatedSymbols)}
"""
);

const string outputDir = "output_files";

if (!Directory.Exists(outputDir))
    Directory.CreateDirectory(outputDir);

FileParser.WriteTokensAsArray($"{outputDir}/vci.csv", vector.ToArray());
FileParser.WriteSymbolTable($"{outputDir}/symbols.csv", updatedSymbols);

void PrintError(string message)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(message);
    Console.ResetColor();
    Environment.Exit(1);
}
