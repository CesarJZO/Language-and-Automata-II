namespace Language;

public static class FileParser
{
    private const string Separator = ",";

    /// <summary>
    /// Reads the token table from a csv file and returns an array of all tokens.
    /// </summary>
    /// <param name="path">The path of the csv file</param>
    /// <returns>Array of tokens</returns>
    public static Token[] GetTokensFromFile(string path)
    {
        string[] lines = File.ReadAllLines(path);
        var tokens = new Token[lines.Length];

        for (var i = 0; i < lines.Length; i++)
        {
            string[] line = lines[i].Split(Separator);
            try
            {
                tokens[i] = new Token(
                    lexeme: line[0],
                    id: int.Parse(line[1]),
                    tablePosition: int.Parse(line[2]),
                    line: int.Parse(line[3])
                );
            }
            catch (FormatException)
            {
                throw new FormatException($"Table had not the correct format. Line {i + 1}: {lines[i]}");
            }
            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException($"Not enough arguments. Line {i + 1}: {lines[i]}");
            }
        }

        return tokens;
    }

    /// <summary>
    /// Reads the symbol table from a csv file and returns an array of all symbols.
    /// </summary>
    /// <param name="path">The path of the csv file</param>
    /// <returns>Array of symbols</returns>
    /// <exception cref="FormatException">If csv file has not the correct format</exception>
    /// <exception cref="IndexOutOfRangeException">If csv file has not enough columns</exception>
    public static Symbol[] GetSymbolsFromFile(string path)
    {
        string[] lines = File.ReadAllLines(path);
        var symbols = new Symbol[lines.Length];

        for (var i = 0; i < lines.Length; i++)
        {
            string[] line = lines[i].Split(Separator);
            try
            {
                symbols[i] = new Symbol(
                    id: line[0],
                    token: int.Parse(line[1]),
                    value: line[2]
                );
            }
            catch (FormatException)
            {
                throw new FormatException($"Table had not the correct format. Line {i + 1}: {lines[i]}");
            }
            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException($"Not enough arguments. Line {i + 1}: {lines[i]}");
            }
        }

        return symbols;
    }

    /// <summary>
    /// Writes the token table to a csv file. Each token is written with the following format:
    /// <code>lexeme,id,tablePosition,line</code>
    /// </summary>
    /// <param name="path">Csv file path</param>
    /// <param name="tokens">Array of tokens</param>
    public static void WriteTokenTable(string path, Token[] tokens)
    {
        var lines = new string[tokens.Length];

        for (var i = 0; i < tokens.Length; i++)
        {
            Token token = tokens[i];
            lines[i] = $"{token.Lexeme}{Separator}{token.Id}{Separator}{token.TablePosition}{Separator}{token.Line}";
        }

        File.WriteAllLines(path, lines);
    }

    /// <summary>
    /// Writes the tokens to a csv file. Each token is written with the following format:
    /// <code>index,lexeme</code>
    /// </summary>
    /// <param name="path">Csv file path</param>
    /// <param name="tokens">Array of tokens</param>
    public static void WriteTokensAsArray(string path, Token[] tokens)
    {
        var lines = new string[tokens.Length];

        for (var i = 0; i < tokens.Length; i++)
            lines[i] = $"{i}{Separator}{tokens[i].Lexeme}";

        File.WriteAllLines(path, lines);
    }

    /// <summary>
    /// Writes the symbol table to a csv file. Each symbol is written with the following format:
    /// <code>id,token,value</code>
    /// </summary>
    /// <param name="path">Path of the csv file to be written</param>
    /// <param name="symbols">Array of symbols</param>
    public static void WriteSymbolTable(string path, Symbol[] symbols)
    {
        var lines = new string[symbols.Length];

        for (var i = 0; i < symbols.Length; i++)
        {
            Symbol symbol = symbols[i];
            lines[i] = $"{symbol.Id}{Separator}{symbol.Token}{Separator}{symbol.Value}";
        }

        File.WriteAllLines(path, lines);
    }
}
