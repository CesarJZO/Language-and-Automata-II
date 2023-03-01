namespace Semantics;
public class SemanticAnalyzer
{
    private readonly string _tokenTableFilePath;
    private readonly List<Token> _allTokens;
    private readonly List<Symbol> _symbols;

    public SemanticAnalyzer(string tokenTableFilePath)
    {
        _tokenTableFilePath = tokenTableFilePath;
        _allTokens = new List<Token>();
        _symbols = new List<Symbol>();
    }

    /// <summary>
    /// Reads the token table from a csv file and stores it in a list
    /// </summary>
    public void ReadTokenTable()
    {
        string[] lines = File.ReadAllLines(_tokenTableFilePath);

        foreach (var t in lines)
        {
            string[] columns = t.Split(",");
            var token = new Token
            {
                lexeme = columns[0],
                id = int.Parse(columns[1]),
                tablePosition = int.Parse(columns[2]),
                line = int.Parse(columns[3])
            };
            _allTokens.Add(token);
        }
    }

    public void CreateSymbolTable(List<Token> identifiers)
    {
        // If there's a repeat, it's an error. Print the repeated identifier and the line it was found
        if (identifiers.GroupBy(x => x.lexeme).Any(g => g.Count() > 1))
        {
            var repeated = identifiers.GroupBy(x => x.lexeme).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            foreach (var identifier in repeated)
            {
                int line = identifiers.Find(token => token.lexeme == identifier).line;
                PrintError($"Error: Identifier {identifier} is already defined in line {line}");
            }
            return;
        }


        for (var i = 0; i < identifiers.Count; i++)
        {
            var token = identifiers[i];
            var symbol = new Symbol
            {
                id = token.lexeme,
                token = token.id,
                valor = GetDefaultValueForToken(token.id)
            };
            UpdateTokenInTable(token, _symbols.Count);
            _symbols.Add(symbol);
        }
    }

    public List<Token> GetIdentifierTokens()
    {
        int varKeywordIndex = _allTokens.FindIndex(token => token.id == -15);
        int beginKeywordIndex = _allTokens.FindIndex(token => token.id == -2);

        List<Token> definitionTokens = _allTokens.GetRange(varKeywordIndex, beginKeywordIndex);
        List<Token> identifierTokens = definitionTokens.Where(token => token.tablePosition == -2).ToList();
        return identifierTokens;
    }

    private void UpdateTokenInTable(Token token, int tablePosition)
    {
        int index = _allTokens.FindIndex(t => t.lexeme == token.lexeme);
        _allTokens[index].UpdateTablePosition(tablePosition);
    }

    private string GetDefaultValueForToken(int token)
    {
        return token switch {
            -51 => "0",
            -52 => "0.0",
            -53 => "\"\"",
            -54 => "false",
            _ => Convert.ToString(PrintError("Error: Type mismatch"))
        };
    }

    public int PrintError(string error)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(error);
        Console.ResetColor();
        return -1;
    }

    public override string ToString()
    {
        string result = "";
        foreach (var token in _allTokens)
        {
            result += token.lexeme + " " + token.id + " " + token.tablePosition + " " + token.line + "\n";
        }

        return result;
    }
}
