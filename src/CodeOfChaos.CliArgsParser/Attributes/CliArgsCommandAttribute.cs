﻿// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace CodeOfChaos.CliArgsParser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[AttributeUsage(AttributeTargets.Class)]
#pragma warning disable CS9113 // Parameter is unread.
public class CliArgsCommandAttribute(string name) : Attribute;
#pragma warning restore CS9113 // Parameter is unread.
