namespace Semantics;

public class VCI
{
    public Stack<string> Operators { get; private set; }
    public Stack<int> Adresses { get; private set; }
    public Stack<string> Statements { get; private set; }
}