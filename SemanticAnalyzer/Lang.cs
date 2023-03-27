namespace Semantics;

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
    public const int EndKeyword = 50;
    public const int VarKeyword = -15;
    public const int GeneralIdentifier = -55;

    public const int Assignment = -26;
    public const int Semicolon = -75;
    public const int Comma = -76;

    public const int DefaultTablePosition = -2;

    public const int RepeatKeyword = 30;
    public const int UntilKeyword = 69;
}

public static class Priority
{
    public const int Mul = 60;
    public const int Div = 60;

    public const int Sum = 50;
    public const int Sub = 50;

    public const int LowerThan = 40;
    public const int GreaterThan = 40;
    public new const int Equals = 40;

    public const int Not = 30;
    public const int And = 20;
    public const int Or = 10;

    public const int Assignment = 0;
}
