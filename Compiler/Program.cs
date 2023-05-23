using Language;
using IntermediateCode;

var icvFilePath = string.Empty;
var symbolsFilePath = string.Empty;
try
{
    icvFilePath = args[0];
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
    tokens = FileParser.GetTokensFromFile(icvFilePath);
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

IcvExecutable executable = new(
    vector: tokens,
    symbols: symbols,
    readFunction: s =>
    {
        Console.Write($"Read {s}: ");
        return Console.ReadLine();
    },
    writeFunction: (value, identifier) =>
    {
        Console.WriteLine(identifier is not null
            ? $"Write {identifier}: {value}"
            : $"Write: {value}"
        );
    });

Symbol[] updatedSymbols = executable.ExecuteIcv();

Console.WriteLine($"""
{"\n"}Symbol table:
{"\t"}{string.Join("\n\t", (IEnumerable<Symbol>)updatedSymbols)}
"""
);

const string outputDir = "output_files";

if (!Directory.Exists(outputDir))
    Directory.CreateDirectory(outputDir);

FileParser.WriteTokenTable($"{outputDir}/vci.csv", tokens);
FileParser.WriteSymbolTable($"{outputDir}/symbols.csv", updatedSymbols);

void PrintError(string message)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(message);
    Console.ResetColor();
    Environment.Exit(1);
}
