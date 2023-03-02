namespace Semantics;
public class Token
{
    public string lexeme;
    public int id;
    public int tablePosition;
    public int line;

    public void UpdateTablePosition(int position)
    {
        tablePosition = position;
    }

    public override string ToString() => $"Lexeme: {lexeme}, Id: {id}, Table Position: {tablePosition}, Line: {line}";
}
