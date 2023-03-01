namespace Semantics;
public struct Token
{
    public string lexeme;
    public int id;
    public int tablePosition;
    public int line;

    public void UpdateTablePosition(int position)
    {
        tablePosition = position;
    }

    public override string ToString()
    {
        return $"Lexeme: {lexeme}, Id: {id}, Table Position: {tablePosition}, Line: {line}";
    }
}
