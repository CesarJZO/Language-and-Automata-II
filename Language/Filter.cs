namespace Language;

public static class Filter
{
    /// <summary>
    /// Returns all tokens between the var keyword and the first begin keyword.
    /// </summary>
    /// <param name="tokens">Token array containing all tokens</param>
    /// <returns>Token array containing all tokens between the var keyword and the first begin keyword</returns>
    public static Token[] GetVarTokens(Token[] tokens)
    {
        int varIndex = Array.FindIndex(tokens, t => t.Id == Lang.VarKeyword);
        int beginIndex = Array.FindIndex(tokens, t => t.Id == Lang.BeginKeyword);

        var varTokens = new Token[beginIndex - varIndex - 1];
        Array.Copy(tokens, varIndex + 1, varTokens, 0, beginIndex - varIndex - 1);
        return varTokens;
    }

    /// <summary>
    /// Returns all tokens between the first begin keyword and the last end keyword.
    /// </summary>
    /// <param name="tokens">Token array containing all tokens</param>
    /// <returns>Token array containing all tokens between the first begin keyword and the last end keyword</returns>
    public static Token[] GetBodyTokens(Token[] tokens)
    {
        int firstBeginIndex = Array.FindIndex(tokens, t => t.Id == Lang.BeginKeyword);
        int lastEndIndex = Array.FindLastIndex(tokens, t => t.Id == Lang.EndKeyword);

        var bodyTokens = new Token[lastEndIndex - firstBeginIndex - 1];
        Array.Copy(tokens, firstBeginIndex + 1, bodyTokens, 0, lastEndIndex - firstBeginIndex - 1);
        return bodyTokens;
    }
}
