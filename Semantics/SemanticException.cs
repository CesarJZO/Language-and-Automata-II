﻿namespace Semantics;

public class SemanticException : Exception
{
    public SemanticException(string? message) : base(message) { }
}
