using System.Collections;
using Semantics;

namespace IntermediateCode;

public class IntermediateCodeVector : IEnumerable<Token>
{
    private readonly List<Token> _intermediateCodeVector;
    public Stack<Operator> Operators { get; }
    public Stack<int> Addresses { get; }
    public Stack<Token> Statements { get; }

    public IntermediateCodeVector()
    {
        _intermediateCodeVector = new List<Token>();
        Operators = new Stack<Operator>();
        Addresses = new Stack<int>();
        Statements = new Stack<Token>();
    }

    public Token[] GetBodyTokens(List<Token> tokens)
    {
        int firstBeginIndex = tokens.FindIndex(t => t.Id == Lang.BeginKeyword);
        int lastEndIndex = tokens.FindLastIndex(t => t.Id == Lang.EndKeyword);

        return tokens.GetRange(firstBeginIndex + 1, lastEndIndex - firstBeginIndex - 1).ToArray();
    }

    public void GenerateIcv(Token[] tokens)
    {
        for (var i = 0; i < tokens.Length; i++)
        {
            Token token = tokens[i];
            int id = token.Id;
            if (Lang.IsIdentifier(id) || Lang.IsLiteral(id))
            {
                _intermediateCodeVector.Add(token);
            }
            else if (Lang.IsOperator(id))
            {
                AddOperator(new Operator(
                    token: token,
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
            Operator top = Operators.Peek();
            if (op.Priority > top.Priority)
            {
                Operators.Push(op);
            }
            else
            {
                // If the operator has lower or equal priority than the top of the stack, pop the stack until
                while (Operators.Count > 0 && op.Priority <= top.Priority)
                {
                    _intermediateCodeVector.Add(top);
                    top = Operators.Pop();
                }
                Operators.Push(op);
            }
        }
    }

    public void AddStatement(Token statement)
    {

    }

    public IEnumerator<Token> GetEnumerator()
    {
        return _intermediateCodeVector.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Operator : Token
{
    public int Priority { get; }

    public Operator(Token token, int priority) : base(token.Lexeme, token.Id, token.TablePosition, token.Line)
    {
        Priority = priority;
    }
}
