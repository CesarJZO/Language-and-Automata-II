using System.Data;
using Language;

namespace IntermediateCode;

public class IcvExecutable
{
    private static readonly DataTable DataTable = new();

    private readonly IntermediateCodeVector _vector;
    private readonly Stack<Token> _executionStack;

    private readonly Symbol[] _symbols;

    private readonly Func<string?> _readFunction;
    private readonly Action<string> _writeFunction;

    /// <summary>
    /// Creates a new instance of <see cref="IcvExecutable"/>.
    /// </summary>
    /// <param name="vector">Intermediate code vector to be executed.</param>
    /// <param name="symbols">Symbols table from semantic analysis.</param>
    /// <param name="readFunction">Predicate to be used as read function.</param>
    /// <param name="writeFunction">Action to be used as write function.</param>
    public IcvExecutable(IntermediateCodeVector vector, Symbol[] symbols, Func<string?> readFunction, Action<string> writeFunction)
    {
        _vector = vector;
        _symbols = symbols;
        _readFunction = readFunction;
        _writeFunction = writeFunction;
        _executionStack = new Stack<Token>();
    }

    /// <summary>
    /// Executes the intermediate code vector.
    /// </summary>
    /// <returns>The symbols table with the updated values.</returns>
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

    /// <summary>
    /// Assigns the result of the input function to the next identifier token in vector.
    /// </summary>
    /// <param name="index">The index of the current token.</param>
    private void AssignToNext(int index)
    {
        Token nextToken = _vector[index + 1];

        Symbol symbol = _symbols[nextToken.TablePosition];
        symbol.Value = _readFunction() ?? string.Empty;
    }

    /// <summary>
    /// Writes the value of the next token of icv to the output functions specified in the constructor.
    /// </summary>
    /// <param name="index">The index of the current token.</param>
    private void WriteNext(int index)
    {
        Token nextToken = _vector[index + 1];

        Symbol symbol = _symbols[nextToken.TablePosition];
        _writeFunction(symbol.Value);
    }

    /// <summary>
    /// Assigns the right operand's value to the left operand's identifier.
    /// </summary>
    /// <param name="leftOperand">Should be an Identifier token.</param>
    /// <param name="rightOperand">Identifier or Literal token.</param>
    private void AssignRightToLeft(Token leftOperand, Token rightOperand)
    {
        string rightValue = Lang.IsIdentifier(rightOperand)
            ? _symbols[rightOperand.TablePosition].Value
            : rightOperand.Lexeme;

        Symbol symbolOfLeftOperand = _symbols[leftOperand.TablePosition];
        symbolOfLeftOperand.Value = rightValue;
    }

    /// <summary>
    /// Pops the last two operands from the stack.
    /// </summary>
    /// <returns>Returns a tuple of the two operands (left, right).</returns>
    private (Token, Token) PopOperands()
    {
        Token rightOperand = _executionStack.Pop();
        Token leftOperand = _executionStack.Pop();

        return (leftOperand, rightOperand);
    }

    /// <summary>
    /// Evaluates the operation and returns the result.
    /// </summary>
    /// <param name="leftOperand">If operand is an identifier, then it's value is taken from the symbol table. Otherwise, the lexeme is taken.</param>
    /// <param name="rightOperand">If operand is an identifier, then it's value is taken from the symbol table. Otherwise, the lexeme is taken.</param>
    /// <param name="op">The operator to be evaluated.</param>
    /// <returns>The result of the operation.</returns>
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
