using Semantics;

SemanticAnalyzer analyzer;

try
{
    analyzer = new SemanticAnalyzer(args[0]);
}
catch (FileNotFoundException e)
{
    Console.WriteLine("File not found: " + e.FileName);
    return;
}

analyzer.ReadTokenTable();
