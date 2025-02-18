﻿// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace CodeOfChaos.CliArgsParser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[AttributeUsage(AttributeTargets.Property)]
#pragma warning disable CS9113// Parameter is unread.
public class CliArgsParameterAttribute(string name, string shortName, ParameterType type = ParameterType.Value) : Attribute {
#pragma warning restore CS9113// Parameter is unread.
}

public enum ParameterType : uint {
    Value = 0,
    Flag = 1
}
