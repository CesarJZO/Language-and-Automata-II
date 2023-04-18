using Language;
using IntermediateCode;

var filePath = string.Empty;
try
{
    filePath = args[0];
}
catch (IndexOutOfRangeException)
{
    PrintError("No file provided.");
}

Token[] tokens = Array.Empty<Token>();
try
{
    tokens = FileParser.GetTokensFromFile(filePath);
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

const string outputDir = "output_files";

if (!Directory.Exists(outputDir))
    Directory.CreateDirectory(outputDir);

FileParser.WriteTokensAsArray($"{outputDir}/vci.csv", vector.ToArray());

void PrintError(string message)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(message);
    Console.ResetColor();
    Environment.Exit(1);
}
