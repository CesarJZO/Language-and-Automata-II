using System.Text;

namespace Semantics;
public class Analyzer
{
    public event Action<string>? OnError;

    private readonly string _tokenTableFilePath;

    public List<Token> Tokens { get; }

    public List<Token> Identifiers { get; private set; }

    public List<Symbol> Symbols { get; }

    public Analyzer(string tokenTableFilePath)
    {
        _tokenTableFilePath = tokenTableFilePath;
        Tokens = new List<Token>();
        Identifiers = new List<Token>();
        Symbols = new List<Symbol>();
    }

    public void FullAnalysis()
    {
        ReadTokenTable();
        GetIdentifierTokens();
        if (CheckForRepeatedIdentifiers()) return;
        CreateSymbolTable();
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
            Tokens.Add(token);
        }
    }

    public void CreateSymbolTable()
    {
        foreach (var token in Identifiers)
        {
            var symbol = new Symbol
            {
                id = token.lexeme,
                token = token.id,
                valor = GetDefaultValueForToken(token)
            };
            UpdateTokenInTable(token, Symbols.Count);
            Symbols.Add(symbol);
        }
    }

    public bool CheckForRepeatedIdentifiers()
    {
        // If there's a repeat, it's an error. Print the repeated identifier and the line it was found
        var hasRepeated = Identifiers.GroupBy(x => x.lexeme).Any(g => g.Count() > 1);

        if (!hasRepeated) return false;

        var repeated = Identifiers.GroupBy(x => x.lexeme).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        foreach (var identifier in repeated)
        {
            var line = Identifiers.Find(token => token.lexeme == identifier).line;
            OnError?.Invoke($"Identifier {identifier} is already defined in line {line}");
        }
        return true;
    }

    public List<Token> GetIdentifierTokens()
    {
        int varKeywordIndex = Tokens.FindIndex(token => token.id == -15);
        int beginKeywordIndex = Tokens.FindIndex(token => token.id == -2);

        List<Token> definitionTokens = Tokens.GetRange(varKeywordIndex, beginKeywordIndex);
        Identifiers = definitionTokens.Where(token => token.tablePosition == -2).ToList();
        return Identifiers;
    }

    private void UpdateTokenInTable(Token token, int tablePosition)
    {
        int index = Tokens.FindIndex(t => t.lexeme == token.lexeme);
        Tokens[index].UpdateTablePosition(tablePosition);
    }

    private string GetDefaultValueForToken(Token token)
    {
        switch (token.id)
        {
            case -51: return "0";
            case -52: return "0.0";
            case -53: return "\"\"";
            case -54: return "false";
            default:
                OnError?.Invoke($"Invalid token: [{token}]");
                return "";
        }
    }

    public string GetFormattedTokenTable()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Lexeme,Id,Table Position,Line");
        foreach (var token in Tokens)
        {
            sb.AppendLine($"{token.lexeme},{token.id},{token.tablePosition},{token.line}");
        }
        return sb.ToString();
    }

    public string GetFormattedSymbolTable()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Id,Token,Valor");
        foreach (var symbol in Symbols)
        {
            sb.AppendLine($"{symbol.id},{symbol.token},{symbol.valor}");
        }
        return sb.ToString();
    }

    public override string ToString() => $@"Tokens
{string.Join("\n", Tokens)}

Symbols
{string.Join("\n", Symbols.Count == 0 ? "No symbols" : Symbols)}";
}
