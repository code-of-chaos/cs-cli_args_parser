// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.CodeAnalysis;

namespace CodeOfChaos.CliArgsParser.Generators;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public struct CliArgsParameterStructDto() {
    public bool IsNotPartial { get; set; } = false;
    public ISymbol Symbol { get; set; } = null!;
    public CliArgsParameterPropertyDto[] PropertyDtos { get; set; } = [];
}
