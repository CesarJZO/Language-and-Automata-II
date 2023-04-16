namespace Semantics;

public class Token
{
    public string Lexeme { get; }
    public int Id { get; }
    public int TablePosition { get; set; }
    public int Line { get; }

    public Token(string lexeme, int id, int tablePosition, int line)
    {
        Lexeme = lexeme;
        Id = id;
        TablePosition = tablePosition;
        Line = line;
    }

    public override string ToString() => $"Lexeme: {Lexeme,6}, Id: {Id,3}, Table Position: {TablePosition,2}, Line: {Line,2}";
}
