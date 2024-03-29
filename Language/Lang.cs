﻿namespace Language;

public static class Lang
{
    public const int IntKeyword = -11;
    public const int IntIdentifier = -51;
    public const int IntLiteral = -61;

    public const int RealKeyword = -12;
    public const int RealIdentifier = -52;
    public const int RealLiteral = -62;

    public const int StringKeyword = -13;
    public const int StringIdentifier = -53;
    public const int StringLiteral = -63;

    public const int LogicKeyword = -14;
    public const int LogicIdentifier = -54;
    public const int TrueLiteral = -64;
    public const int FalseLiteral = -65;

    public const int BeginKeyword = -2;
    public const int EndKeyword = -3;
    public const int VarKeyword = -15;
    public const int GeneralIdentifier = -55;

    public const int ReadFunction = -4;
    public const int WriteFunction = -5;

    public const int MulOperator = -21;
    public const int DivOperator = -22;
    public const int ModOperator = -23;
    public const int SumOperator = -24;
    public const int SubOperator = -25;

    public const int AssignmentOperator = -26;

    public const int LowerThanOperator = -31;
    public const int LowerOrEqualOperator = -32;
    public const int GreaterThanOperator = -33;
    public const int GreaterOrEqualOperator = -34;
    public const int EqualToOperator = -35;
    public const int NotEqualsOperator = -36;
    public const int AndOperator = -41;
    public const int OrOperator = -42;
    public const int NotOperator = -43;

    public const int OpenParenthesis = -73;
    public const int CloseParenthesis = -74;

    public const int Semicolon = -75;
    public const int Comma = -76;

    public const int DefaultTablePosition = -2;

    public const int RepeatKeyword = -9;
    public const int UntilKeyword = -10;

    public const int Address = -99;

    public static bool IsLiteral(int id) => id is IntLiteral or RealLiteral or StringLiteral or TrueLiteral or FalseLiteral;

    public static bool IsIdentifier(int id) => id is IntIdentifier or RealIdentifier or StringIdentifier or LogicIdentifier or GeneralIdentifier;

    public static bool IsOperator(int id) => id is
        MulOperator or DivOperator or ModOperator or SumOperator
        or SubOperator or LowerThanOperator or LowerOrEqualOperator or GreaterThanOperator or GreaterOrEqualOperator
        or EqualToOperator or NotEqualsOperator or AndOperator or OrOperator or NotOperator or AssignmentOperator;

    public static bool IsStatement(int id) => id is RepeatKeyword or UntilKeyword;

    public static bool IsFunction(int id) => id is ReadFunction or WriteFunction;

    public static int GetPriority(Token token) => token.Id switch
    {
        MulOperator => Priority.Mul,
        DivOperator => Priority.Div,
        SumOperator => Priority.Sum,
        SubOperator => Priority.Sub,
        LowerThanOperator => Priority.LowerThan,
        GreaterThanOperator => Priority.GreaterThan,
        GreaterOrEqualOperator => Priority.GreaterThan,
        LowerOrEqualOperator => Priority.LowerThan,
        EqualToOperator => Priority.EqualTo,
        NotOperator => Priority.Not,
        AndOperator => Priority.And,
        OrOperator => Priority.Or,
        AssignmentOperator => Priority.Assign,
        OpenParenthesis => Priority.Assign,
        CloseParenthesis => Priority.Assign,
        _ => throw new Exception("Invalid operator")
    };

    /// <summary>
    /// Gets the default value for a token
    /// </summary>
    /// <param name="identifier">The identifier token</param>
    /// <returns>Depending on the type of the token, returns a default value</returns>
    public static string DefaultValueOf(Token identifier)
    {
        return identifier.Id switch
        {
            IntIdentifier => "0",
            RealIdentifier => "0.0",
            StringIdentifier => "null",
            LogicIdentifier => "false",
            _ => throw new Exception("-1")
        };
    }

    private static class Priority
    {
        public const int Mul = 60;
        public const int Div = 60;

        public const int Sum = 50;
        public const int Sub = 50;

        public const int LowerThan = 40;
        public const int GreaterThan = 40;
        public const int EqualTo = 40;

        public const int Not = 30;
        public const int And = 20;
        public const int Or = 10;

        public const int Assign = 0;
    }
}
