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
        Token temp = null!;
        for (var i = 0; i < tokens.Length; i++)
        {
            Token token = tokens[i];
            int id = token.Id;
            if (Lang.IsIdentifier(id) || Lang.IsLiteral(id))
            {
                AddToIcv(token);
            }
            else if (Lang.IsOperator(id))
            {
                AddOperator(new Operator(
                    token: token,
                    priority: Lang.GetPriority(token)
                ));
            }
            else if (token.Id is Lang.CloseParenthesis)
            {
                while (_operators.Peek().Id != Lang.OpenParenthesis)
                {
                    AddToIcv(_operators.Pop());
                }
                _operators.Pop();
            }
            else if (token.Id is Lang.Semicolon)
            {
                EmptyOperatorsStack();
                if (temp != null)
                {
                    var address = _addresses.Pop().ToString();
                    AddToIcv(new Token(address, 0, 0, 0));
                    AddToIcv(temp);
                }
            }
            else if (token.Id is Lang.RepeatKeyword)
            {
                _statements.Push(token);
                _addresses.Push(_intermediateCodeVector.Count - 1);
            }
            else if (token.Id is Lang.UntilKeyword)
            {
                temp = token;
            }
        }
    }

    public void EmptyOperatorsStack()
    {
        while (_operators.Count > 0)
        {
            Operator op = _operators.Pop();
            if (op.Id is Lang.OpenParenthesis or Lang.CloseParenthesis)
                continue;
            AddToIcv(op);
        }
    }

    private void AddOperator(Operator op)
    {
        // Push to operators stack if it's empty
        if (_operators.Count == 0 || op.Id == Lang.OpenParenthesis)
        {
            _operators.Push(op);
        }
        else
        {
            // If the operator has a higher priority than the top of the stack, push it
            if (_operators.Peek().Priority < op.Priority)
            {
                _operators.Push(op);
            }
            else
            {
                // Pop until the top of the stack has a lower priority than the operator
                while (_operators.Count > 0 && _operators.Peek().Priority >= op.Priority)
                {
                    AddToIcv(_operators.Pop());
                }
                _operators.Push(op);
            }
        }
    }

    public void AddToIcv(Token token)
    {
        if (token.Id is Lang.OpenParenthesis or Lang.CloseParenthesis)
            return;
        _intermediateCodeVector.Add(token);
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
