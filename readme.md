# Semantic Analyzer

This is a simple semantic analyzer, it takes a comma separated file (token table) as input and outputs a list of symbols and semantic errors.

## Usage

This project is written in **C#** and uses `.NET Core 7.0`.

```powershell
dotnet run <input file>
```

Input file should be a comma separated file with the following format:

```csv
<lexeme>,<token>,<table_position>,<line>
```

Example:

```csv
a,-52,-2,3
```

## Language rules

The language rules are defined in the `SemanticAnalyzer/Lang.cs` file. List of tokens

### Keywords

- Begin: `-2`
- Integer: `-11`
- Real: `-12`
- Text: `-13`
- Logical: `-14`
- Var: `-15`

### Identifiers

- Integer: `-51`
- Real: `-52`
- Text: `-53`
- Logical: `-54`

### Literals

- Integer: `-61`
- Real: `-62`
- Text: `-63`
- True: `-64`
- False: `-65`

### Operators

- Assignment: `-26`
- Semicolon: `-75`
