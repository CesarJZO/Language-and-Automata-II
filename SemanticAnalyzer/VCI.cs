namespace Semantics;

public class VCI
{
    public List<string> IntermediateCodeVector { get; }
    public Stack<Operator> Operators { get; }
    public Stack<int> Addresses { get; }
    public Stack<Statement> Statements { get; }
    public Stack<Token> Cloud { get; }

    public VCI()
    {
        IntermediateCodeVector = new List<string>();
        Operators = new Stack<Operator>();
        Addresses = new Stack<int>();
        Statements = new Stack<Statement>();
        Cloud = new Stack<Token>();
    }

    public void ReadTokens(List<Token> tokens)
    {
        foreach (var token in tokens)
        {
            var id = token.Id;
            if (Lang.IsIdentifier(id) || Lang.IsLiteral(id))
            {
                IntermediateCodeVector.Add(token.Lexeme);
            }
            else if (Lang.IsOperator(id))
            {
                AddOperator(new Operator(
                    name: token.Lexeme,
                    priority: Lang.GetPriority(token)
                ));
            }
            else if (Lang.IsStatement(id))
            {

            }
        }
    }

    public void AddOperator(Operator op)
    {
        // Push to operators stack if it's empty
        if (Operators.Count == 0)
        {
            Operators.Push(op);
        }
        else
        {
            // If the operator has higher priority than the top of the stack, push it
            var top = Operators.Peek();
            if (op.Priority > top.Priority)
            {
                Operators.Push(op);
            }
            else
            {
                // If the operator has lower or equal priority than the top of the stack, pop the stack until
                while (Operators.Count > 0 && op.Priority <= top.Priority)
                {
                    IntermediateCodeVector.Add(top.Name);
                    Operators.Pop();
                    top = Operators.Peek();
                }
                Operators.Push(op);
            }
        }
    }

    public void AddStatement(Statement statement)
    {
        if (statement is Statement.Repeat)
        {
            
        }
    }
}

public struct Operator
{
    public string Name { get; }
    public int Priority { get; }

    public Operator(string name, int priority)
    {
        Name = name;
        Priority = priority;
    }
}

public enum Statement
{
    Repeat,
    Until,
    If,
    Else,
    While
}
