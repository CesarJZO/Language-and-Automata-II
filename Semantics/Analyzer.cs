using Language;

namespace Semantics;

public class Analyzer
{
    private readonly Token[] _tokens;

    public List<Symbol> Symbols { get; }

    public Analyzer(Token[] tokens)
    {
        _tokens = tokens;
        Symbols = new List<Symbol>();
    }

    /// <summary>
    /// Performs the full analysis of the token table and creates the symbol table
    /// </summary>
    public void PerformFullAnalysis()
    {
        Token[] identifierTokens = GetIdentifierTokens(Filter.GetVarTokens(_tokens));
        CheckForRepeatedIdentifiers(identifierTokens);
        CreateSymbolTable(identifierTokens);
        WriteFiles();
        CheckSymbolUsage(Filter.GetBodyTokens(_tokens));
    }

    /// <summary>
    /// Gets identifier tokens from Token list. If tokens have a table position of -2 and they are stored in
    /// declaration and initialization blocks, they are considered identifiers.
    /// </summary>
    /// <returns>An enumerable containing only identifier tokens</returns>
    public Token[] GetIdentifierTokens(Token[] varTokens)
    {
        return varTokens.Where(token => token.TablePosition == Lang.DefaultTablePosition).ToArray();
    }


    /// <summary>
    /// Checks if there are repeated identifiers in the token table
    /// </summary>
    /// <returns>True if there are repeated identifiers, false otherwise</returns>
    public bool CheckForRepeatedIdentifiers(Token[] identifiers)
    {
        // If there's a repeat, it's an error. Print the repeated identifier and the line it was found
        bool hasRepeated = identifiers.GroupBy(x => x.Lexeme).Any(g => g.Count() > 1);

        if (!hasRepeated) return false;

        IEnumerable<Token> repeated = identifiers.GroupBy(x => x.Lexeme).Where(g => g.Count() > 1).Select(g => g.Last());
        foreach (Token identifier in repeated)
            throw new SemanticException($"Identifier [{identifier.Lexeme}] is already defined: line {identifier.Line}.");
        return true;
    }

    /// <summary>
    /// Creates the symbol table from the identifiers in the token table. Should be called after CheckForRepeatedIdentifiers()
    /// </summary>
    public void CreateSymbolTable(Token[] identifiers)
    {
        foreach (Token token in identifiers)
        {
            try
            {
                var symbol = new Symbol(
                    id: token.Lexeme,
                    token: token.Id,
                    value: Lang.DefaultValueOf(token)
                );
                UpdateTokenInTable(token, Symbols.Count);
                token.TablePosition = Symbols.Count;
                Symbols.Add(symbol);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("-1"))
                    throw new SemanticException($"[{token.Lexeme}] is not a valid type: line {token.Line}.");
            }
        }
    }

    /// <summary>
    /// Updates the table position of a token in the token table
    /// </summary>
    /// <param name="token">The token to be updated</param>
    /// <param name="tablePosition">The new position of the specified token</param>
    private void UpdateTokenInTable(Token token, int tablePosition)
    {
        foreach (Token t in _tokens.Where(t => t.Lexeme == token.Lexeme))
            t.TablePosition = tablePosition;
    }

    /// <summary>
    /// Writes the token table and symbol table to the output_files folder
    /// </summary>
    public void WriteFiles()
    {
        const string directory = "./output_files/";
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        File.WriteAllLines($"{directory}token_table.csv", _tokens.Select(t => $"{t.Lexeme},{t.Id},{t.TablePosition},{t.Line}"));
        File.WriteAllLines($"{directory}symbol_table.csv", Symbols.Select(s => $"{s.Id},{s.Token},{s.Value}"));
    }

    /// <summary>
    /// Once symbol list is created, checks Program body if usage of identifiers is correct by checking if they exist on symbol table.
    /// Invokes OnError if a token has not a corresponding symbol.
    /// </summary>
    public void CheckSymbolUsage(IEnumerable<Token> bodyTokens)
    {
        IEnumerable<Token> identifiers = bodyTokens.Where(t =>
            t.Id is Lang.IntIdentifier or Lang.RealIdentifier or Lang.StringIdentifier or Lang.LogicIdentifier);
        foreach (Token identifier in identifiers)
        {
            if (!IsSymbol(identifier))
                throw new SemanticException($"[{identifier.Lexeme}] is not defined: line {identifier.Line}.");
            if (!HasCorrectType(identifier))
                throw new SemanticException($"[{identifier.Lexeme}] was defined with another type: line {identifier.Line}.");
            if (!HasCorrectAssignment(identifier))
                throw new SemanticException($"[{identifier.Lexeme}] type mismatch on line {identifier.Line}.");
        }
    }

    private bool IsSymbol(Token token) => Symbols.Any(symbol => token.Lexeme == symbol.Id);

    private bool HasCorrectType(Token token)
    {
        Symbol symbol = Symbols.First(s => s.Id == token.Lexeme);
        return token.Id == symbol.Token;
    }

    private bool HasCorrectAssignment(Token token)
    {
        IEnumerable<Token> tokens = _tokens.Where(t => t.Line == token.Line)
            .Where(t => t.Id is not (Lang.AssignmentOperator or Lang.Comma or Lang.Semicolon
                or <= -21 and >= -43 or <= -73 and >= -76));
        return token.Id switch
        {
            Lang.IntIdentifier => tokens.All(t =>
                t.Id is Lang.IntLiteral or Lang.IntIdentifier or Lang.LogicIdentifier),
            Lang.RealIdentifier => tokens.All(t => t.Id is Lang.RealLiteral or Lang.RealIdentifier),
            Lang.StringIdentifier => tokens.All(t => t.Id is Lang.StringLiteral or Lang.StringIdentifier),
            Lang.LogicIdentifier => tokens.All(t =>
                t.Id is Lang.TrueLiteral or Lang.FalseLiteral or Lang.LogicIdentifier or Lang.LogicKeyword
                    or Lang.IntLiteral or Lang.IntIdentifier),
            _ => false
        };
    }

    public override string ToString() => $"""
        Tokens
            {string.Join("\n", _tokens as IEnumerable<Token>)}

        Symbols
            {string.Join("\n", Symbols.Count == 0 ? "No symbols" : Symbols)}
        """;
}
