// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace CodeOfChaos.CliArgsParser.Attributes;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[AttributeUsage(AttributeTargets.Property)]
public class CliArgsParameterAttribute(string name, string? shortName = null, ParameterType type = ParameterType.Value) : Attribute {

}

public enum ParameterType : ulong {
    Value = 0,
    Flag = 1
}
