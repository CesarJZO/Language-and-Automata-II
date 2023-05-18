namespace Semantics;

public class Symbol
{
    public string Id { get; }
    public int Token { get; }
    public string Value { get; set; }

    public Symbol(string id, int token, string value)
    {
        Id = id;
        Token = token;
        Value = value;
    }

    public override string ToString() => $"Id: {Id,6}, Token: {Token,3}, Value: {Value,5}";
}
