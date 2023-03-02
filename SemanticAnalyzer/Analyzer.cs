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

    /// <summary>
    /// Performs the full analysis of the token table and creates the symbol table
    /// </summary>
    public void PerformFullAnalysis()
    {
        ReadTokenTable();
        GetIdentifierTokens();
        if (CheckForRepeatedIdentifiers()) return;
        CreateSymbolTable();
    }

    /// <summary>
    /// Reads the token table from the file and stores it in the Tokens list
    /// </summary>
    public void ReadTokenTable()
    {
        var lines = File.ReadAllLines(_tokenTableFilePath);

        foreach (var t in lines)
        {
            var columns = t.Split(",");
            var token = new Token(
                lexeme: columns[0],
                id: int.Parse(columns[1]),
                tablePosition: int.Parse(columns[2]),
                line: int.Parse(columns[3])
            );
            Tokens.Add(token);
        }
    }

    /// <summary>
    /// Creates the symbol table from the identifiers in the token table. Should be called after CheckForRepeatedIdentifiers()
    /// </summary>
    public void CreateSymbolTable()
    {
        foreach (var token in Identifiers)
        {
            var symbol = new Symbol
            {
                id = token.Lexeme,
                token = token.Id,
                value = GetDefaultValueForToken(token)
            };
            UpdateTokenInTable(token, Symbols.Count);
            Symbols.Add(symbol);
        }
    }

    /// <summary>
    /// Checks if there are repeated identifiers in the token table
    /// </summary>
    /// <returns>True if there are repeated identifiers, false otherwise</returns>
    public bool CheckForRepeatedIdentifiers()
    {
        // If there's a repeat, it's an error. Print the repeated identifier and the line it was found
        var hasRepeated = Identifiers.GroupBy(x => x.Lexeme).Any(g => g.Count() > 1);

        if (!hasRepeated) return false;

        var repeated = Identifiers.GroupBy(x => x.Lexeme).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        foreach (var identifier in repeated)
        {
            var line = Identifiers.Find(token => token.Lexeme == identifier).Line;
            OnError?.Invoke($"Identifier {identifier} is already defined in line {line}");
        }
        return true;
    }

    /// <summary>
    /// Gets the identifiers from the token table and stores them in the Identifiers list
    /// </summary>
    /// <returns></returns>
    public List<Token> GetIdentifierTokens()
    {
        var varKeywordIndex = Tokens.FindIndex(token => token.Id == -15);
        var beginKeywordIndex = Tokens.FindIndex(token => token.Id == -2);

        var definitionTokens = Tokens.GetRange(varKeywordIndex, beginKeywordIndex);
        Identifiers = definitionTokens.Where(token => token.TablePosition == -2).ToList();
        return Identifiers;
    }

    /// <summary>
    /// Updates the table position of a token in the token table
    /// </summary>
    /// <param name="token">The token to be updated</param>
    /// <param name="tablePosition">The new position of the specified token</param>
    private void UpdateTokenInTable(Token token, int tablePosition)
    {
        var index = Tokens.FindIndex(t => t.Lexeme == token.Lexeme);
        var aux = Tokens[index];
        aux.TablePosition = tablePosition;
        Tokens[index] = aux;
    }

    /// <summary>
    /// Gets the default value for a token
    /// </summary>
    /// <param name="token">The identifier token</param>
    /// <returns>Depending on the type of the token, returns a default value</returns>
    private string GetDefaultValueForToken(Token token)
    {
        switch (token.Id)
        {
            case -51: return "0";
            case -52: return "0.0";
            case -53: return "null";
            case -54: return "false";
            default:
                OnError?.Invoke($"Invalid token: [{token}]");
                return "";
        }
    }

    public string GetFormattedTokenTable()
    {
        var sb = new StringBuilder();

        foreach (var token in Tokens)
            sb.AppendLine($"{token.Lexeme},{token.Id},{token.TablePosition},{token.Line}");
        return sb.ToString();
    }

    public string GetFormattedSymbolTable()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Id,Token,Valor");
        foreach (var symbol in Symbols)
            sb.AppendLine($"{symbol.id},{symbol.token},{symbol.value}");
        return sb.ToString();
    }

    public override string ToString() => $@"Tokens
    {string.Join("\n", Tokens)}

Symbols
    {string.Join("\n", Symbols.Count == 0 ? "No symbols" : Symbols)}";
}
