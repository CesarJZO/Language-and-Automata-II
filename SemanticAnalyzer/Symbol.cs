namespace Semantics;

public struct Symbol
{
    public string id;
    public int token;
    public string value;

    public override string ToString() => $"Id: {id}, Token: {token}, Value: {value}";
}
