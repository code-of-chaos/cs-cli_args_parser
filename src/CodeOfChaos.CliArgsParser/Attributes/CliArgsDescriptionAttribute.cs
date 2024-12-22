// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace CodeOfChaos.CliArgsParser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
#pragma warning disable CS9113// Parameter is unread.
public class CliArgsDescriptionAttribute(string description) : Attribute;
#pragma warning restore CS9113// Parameter is unread.
