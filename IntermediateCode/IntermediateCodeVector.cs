using System.Collections;
using Language;
using static System.String;

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

    public void Generate(Token[] tokens)
    {
        Token temp = null!;
        Token currentStatement = null!;
        foreach (Token token in tokens)
        {
            if (Lang.IsIdentifier(token) || Lang.IsLiteral(token) || Lang.IsFunction(token))
            {
                AddToIcv(token);
            }
            else if (Lang.IsOperator(token))
            {
                AddOperator(new Operator(
                    token: token,
                    priority: Lang.GetPriority(token)
                ));
            }
            else if (token.Id is Lang.OpenParenthesis)
            {
                _operators.Push(new Operator(token, Lang.GetPriority(token)));
            }
            else if (token.Id is Lang.CloseParenthesis)
            {
                RemovePairExpression();
            }
            else if (token.Id is Lang.Semicolon)
            {
                EmptyOperatorsStack();

                if (temp?.Id is not Lang.UntilKeyword) continue;
                // Until condition evaluated

                var address = _addresses.Pop().ToString();
                AddToIcv(new Token(address, 0, 0, 0));
                AddToIcv(temp);
                temp = null!;
            }
            else if (token.Id is Lang.EndKeyword)
            {
                currentStatement = _statements.Pop();
            }
            else if (token.Id is Lang.RepeatKeyword)
            {
                _statements.Push(token);
                _addresses.Push(_intermediateCodeVector.Count);
            }
            else if (token.Id is Lang.UntilKeyword)
            {
                if (currentStatement.Id is Lang.RepeatKeyword)
                    temp = token;
            }
        }
    }

    /// <summary>
    /// Removes all operators from the stack until the first open parenthesis is found.
    /// </summary>
    private void RemovePairExpression()
    {
        while (_operators.Peek().Id != Lang.OpenParenthesis)
            AddToIcv(_operators.Pop());
        _operators.Pop();
    }

    /// <summary>
    /// Removes all operators from the stack and adds them to the intermediate code vector.
    /// </summary>
    private void EmptyOperatorsStack()
    {
        while (_operators.Count > 0)
            AddToIcv(_operators.Pop());
    }

    /// <summary>
    /// Adds an operator to the stack based on its priority. If the operator has a higher priority
    /// than the top of the stack, it is added to the stack. Otherwise, all operators with a higher
    /// or equal priority are removed from the stack and added to the intermediate code vector.
    /// </summary>
    /// <param name="op">Operator to add</param>
    private void AddOperator(Operator op)
    {
        if (_operators.Count == 0)
            _operators.Push(op);
        else if (_operators.Peek().Priority < op.Priority)
            _operators.Push(op);
        else
        {
            while (_operators.Count > 0 && _operators.Peek().Priority >= op.Priority)
                AddToIcv(_operators.Pop());
            _operators.Push(op);
        }
    }

    /// <summary>
    /// Adds a token to the intermediate code vector.
    /// </summary>
    /// <param name="token">Token to add</param>
    private void AddToIcv(Token token) => _intermediateCodeVector.Add(token);

    public IEnumerator<Token> GetEnumerator() => _intermediateCodeVector.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString()
    {
        const string separator = " | ";
        int digits = _intermediateCodeVector.Count.ToString().Length;

        string icv = "[ " + Join(separator,
            _intermediateCodeVector.Select(t => Format($$"""{0,{{digits}}}""", t.Lexeme))) + " ]";

        var indexes = new string[_intermediateCodeVector.Count];

        for (var i = 0; i < _intermediateCodeVector.Count; i++)
        {
            string lexeme = _intermediateCodeVector[i].Lexeme;
            string formattedIndex = Format($$"""{0,{{lexeme.Length}}}""", i);
            indexes[i] = Format($$"""{0,{{digits}}}""", formattedIndex);
        }

        return $"{icv}\n[ {Join(separator, indexes)} ]";
    }
}
