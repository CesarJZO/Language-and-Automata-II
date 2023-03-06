using System.Text;

namespace Semantics;

public class Analyzer
{
    public event Action<string>? OnError;

    public List<Token> Tokens { get; }

    public List<Symbol> Symbols { get; }

    public Analyzer()
    {
        Tokens = new List<Token>();
        Symbols = new List<Symbol>();
    }

    /// <summary>
    /// Performs the full analysis of the token table and creates the symbol table
    /// </summary>
    /// <param name="filePath">The path of the file containing the token table. It should be a csv file.</param>
    public void PerformFullAnalysis(string filePath)
    {
        ReadTokenTable(filePath);
        var identifierTokens = GetIdentifierTokens();
        if (CheckForRepeatedIdentifiers(identifierTokens)) return;
        CreateSymbolTable(identifierTokens);
        WriteFiles();
        CheckSymbolUsage();
    }

    /// <summary>
    /// Reads the token table from the file and stores it in the Tokens list
    /// </summary>
    /// <param name="filePath">The path of the file containing the token table. It should be a csv file.</param>
    public void ReadTokenTable(string filePath)
    {
        var lines = File.ReadAllLines(filePath);

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
    /// Gets the identifiers from the token table and stores them in the Identifiers list
    /// </summary>
    /// <returns></returns>
    public List<Token> GetIdentifierTokens()
    {
        var varKeywordIndex = Tokens.FindIndex(token => token.Id == Lang.VarKeyword);
        var beginKeywordIndex = Tokens.FindIndex(token => token.Id == Lang.BeginKeyword);

        var definitionTokens = Tokens.GetRange(varKeywordIndex, beginKeywordIndex - varKeywordIndex);
        return definitionTokens.Where(token => token.TablePosition == Lang.DefaultTablePosition).ToList();
    }

    /// <summary>
    /// Checks if there are repeated identifiers in the token table
    /// </summary>
    /// <returns>True if there are repeated identifiers, false otherwise</returns>
    private bool CheckForRepeatedIdentifiers(List<Token> identifiers)
    {
        // If there's a repeat, it's an error. Print the repeated identifier and the line it was found
        var hasRepeated = identifiers.GroupBy(x => x.Lexeme).Any(g => g.Count() > 1);

        if (!hasRepeated) return false;

        var repeated = identifiers.GroupBy(x => x.Lexeme).Where(g => g.Count() > 1).Select(g => g.Last()).ToList();
        foreach (var identifier in repeated)
            OnError?.Invoke($"Identifier [{identifier.Lexeme}] is already defined in line {identifier.Line}");
        return true;
    }

    /// <summary>
    /// Creates the symbol table from the identifiers in the token table. Should be called after CheckForRepeatedIdentifiers()
    /// </summary>
    public void CreateSymbolTable(IEnumerable<Token> identifiers)
    {
        foreach (var token in identifiers)
        {
            var symbol = new Symbol(
                id: token.Lexeme,
                token: token.Id,
                value: GetDefaultValueForToken(token)
            );
            UpdateTokenInTable(token, Symbols.Count);
            Symbols.Add(symbol);
        }
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
    /// Writes the token table and symbol table to the output_files folder
    /// </summary>
    public void WriteFiles()
    {
        const string directory = "./output_files/";
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        File.WriteAllText($"{directory}token_table.csv", GetFormattedTokenTable());
        File.WriteAllText($"{directory}symbol_table.csv", GetFormattedSymbolTable());
        string GetFormattedTokenTable()
        {
            var sb = new StringBuilder();
            foreach (var token in Tokens)
                sb.AppendLine($"{token.Lexeme},{token.Id},{token.TablePosition},{token.Line}");
            return sb.ToString();
        }

        string GetFormattedSymbolTable()
        {
            var sb = new StringBuilder();
            foreach (var symbol in Symbols)
                sb.AppendLine($"{symbol.Id},{symbol.Token},{symbol.Value}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// Once symbol list is created, checks Program body if usage of identifiers is correct by checking if they exist on symbol table.
    /// Invokes OnError if a token has not a corresponding symbol.
    /// </summary>
    public void CheckSymbolUsage()
    {
        var beginIndex = Tokens.FindIndex(t => t.Id == Lang.BeginKeyword);
        var tokens = Tokens.GetRange(beginIndex, Tokens.Count - beginIndex);

        var identifiers = tokens.Where(t =>
            t.Id is Lang.IntIdentifier or Lang.RealIdentifier or Lang.StringIdentifier or Lang.LogicIdentifier);
        foreach (var identifier in identifiers)
        {
            if (!IsSymbol(identifier))
                OnError?.Invoke($"[{identifier.Lexeme}] is not defined: line {identifier.Line}.");
            if (!HasCorrectAssignment(identifier))
                OnError?.Invoke($"[{identifier.Lexeme}] type mismatch on line {identifier.Line}.");
        }
    }

    private bool IsSymbol(Token token) => Symbols.Any(symbol => token.Lexeme == symbol.Id);

    private bool HasCorrectAssignment(Token token)
    {
        var tokens = Tokens.Where(t => t.Line == token.Line).Where(t => t.Id is not (Lang.Assignment or Lang.Comma or Lang.Semicolon));
        return token.Id switch
        {
            Lang.IntIdentifier => tokens.All(t => t.Id is Lang.IntLiteral or Lang.IntIdentifier or Lang.IntKeyword),
            Lang.RealIdentifier => tokens.All(t => t.Id is Lang.RealLiteral or Lang.RealIdentifier or Lang.RealKeyword),
            Lang.StringIdentifier => tokens.All(t => t.Id is Lang.StringLiteral or Lang.StringIdentifier or Lang.StringKeyword),
            Lang.LogicIdentifier => tokens.All(t => t.Id is Lang.TrueLiteral or Lang.FalseLiteral or Lang.LogicIdentifier or Lang.LogicKeyword),
            _ => false
        };
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
            case Lang.IntIdentifier: return "0";
            case Lang.RealIdentifier: return "0.0";
            case Lang.StringIdentifier: return "null";
            case Lang.LogicIdentifier: return "false";
            default:
                OnError?.Invoke($"[{token.Lexeme}] is not a valid type: line {token.Line}.");
                return "";
        }
    }

    public override string ToString() => $@"Tokens
    {string.Join("\n", Tokens)}

Symbols
    {string.Join("\n", Symbols.Count == 0 ? "No symbols" : Symbols)}";
}
