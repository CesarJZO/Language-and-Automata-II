namespace Language;

public class Operator : Token
{
    public int Priority { get; }

    public Operator(Token token, int priority) : base(token.Lexeme, token.Id, token.TablePosition, token.Line)
    {
        Priority = priority;
    }
}
