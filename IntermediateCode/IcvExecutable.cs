using System.Data;
using Language;
using Semantics;

namespace IntermediateCode;

public class IcvExecutable
{
    private static readonly DataTable DataTable = new();

    private readonly IntermediateCodeVector _vector;
    private readonly Stack<Token> _executionStack;

    private readonly Symbol[] _symbols;

    private readonly Func<string> _readFunction;
    private readonly Action<string> _writeFunction;

    public IcvExecutable(IntermediateCodeVector vector, Symbol[] symbols, Func<string> readFunction,
        Action<string> writeFunction)
    {
        _vector = vector;
        _symbols = symbols;
        _readFunction = readFunction;
        _writeFunction = writeFunction;
        _executionStack = new Stack<Token>();
    }

    public Symbol[] ExecuteIcv()
    {
        for (var i = 0; i < _vector.Count(); i++)
        {
            Token token = _vector[i];
            if (Lang.IsIdentifier(token) || Lang.IsLiteral(token))
            {
                _executionStack.Push(token);
            }
            else if (token.Id is Lang.AssignmentOperator)
            {
                (Token leftOperand, Token rightOperand) = PopOperands();
                AssignRightToLeft(leftOperand, rightOperand);
            }
            else if (Lang.IsOperator(token))
            {
                (Token leftOperand, Token rightOperand) = PopOperands();
                var op = token as Operator;

                object result = Evaluate(leftOperand, rightOperand, op!);

                _executionStack.Push(new Token(result.ToString()!, 0, 0, 0));
            }
            else if (token.Id is Lang.ReadFunction)
            {
                AssignToNext(i);
            }
            else if (token.Id is Lang.WriteFunction)
            {
                WriteNext(i);
            }
        }

        return _symbols;
    }

    private void AssignToNext(int index)
    {
        Token nextToken = _vector[index + 1];

        Symbol symbol = _symbols[nextToken.TablePosition];
        symbol.Value = _readFunction();
    }

    private void WriteNext(int index)
    {
        Token nextToken = _vector[index + 1];

        Symbol symbol = _symbols[nextToken.TablePosition];
        _writeFunction(symbol.Value);
    }

    private void AssignRightToLeft(Token leftOperand, Token rightOperand)
    {
        string rightValue = Lang.IsIdentifier(rightOperand)
            ? _symbols[rightOperand.TablePosition].Value
            : rightOperand.Lexeme;

        Symbol symbolOfLeftOperand = _symbols[leftOperand.TablePosition];
        symbolOfLeftOperand.Value = rightValue;
    }

    private (Token, Token) PopOperands()
    {
        Token rightOperand = _executionStack.Pop();
        Token leftOperand = _executionStack.Pop();

        return (leftOperand, rightOperand);
    }

    private object Evaluate(Token leftOperand, Token rightOperand, Token op)
    {
        string leftValue = Lang.IsIdentifier(leftOperand)
            ? _symbols[leftOperand.TablePosition].Value
            : leftOperand.Lexeme;
        string rightValue = Lang.IsIdentifier(rightOperand)
            ? _symbols[rightOperand.TablePosition].Value
            : rightOperand.Lexeme;

        var operation = $"{leftValue}{op.Lexeme}{rightValue}";

        return DataTable.Compute(operation, string.Empty);
    }
}
