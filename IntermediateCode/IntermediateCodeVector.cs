using System.Collections;
using Language;

namespace IntermediateCode;

public class IntermediateCodeVector : IEnumerable<Token>
{
    private readonly List<Token> _intermediateCodeVector;

    private readonly Stack<Operator> _operators;
    private readonly Stack<int> _addresses;
    private readonly Stack<Token> _statements;

    public IntermediateCodeVector()
    {
        _intermediateCodeVector = new List<Token>();
        _operators = new Stack<Operator>();
        _addresses = new Stack<int>();
        _statements = new Stack<Token>();
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
            else if (token.Id == Lang.Semicolon)
            {
                EmptyOperatorsStack();
            }
        }
    }

    public void EmptyOperatorsStack()
    {
        while (_operators.Count > 0)
        {
            _intermediateCodeVector.Add(_operators.Pop());
        }
    }

    public void AddOperator(Operator op)
    {
        // Push to operators stack if it's empty
        if (_operators.Count == 0)
        {
            _operators.Push(op);
        }
        else
        {
            // If the operator has higher priority than the top of the stack, push it
            Operator top = _operators.Peek();
            if (op.Priority > top.Priority)
            {
                _operators.Push(op);
            }
            else
            {
                // If the operator has lower or equal priority than the top of the stack, pop the stack until
                while (_operators.Count > 0 && op.Priority <= top.Priority)
                {
                    _intermediateCodeVector.Add(top);
                    top = _operators.Pop();
                }
                _operators.Push(op);
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
