// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Linq;

namespace CodeOfChaos.CliArgsParser.Generators;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public struct CliArgsParameterPropertyDto() {
    public PropertyDeclarationSyntax PropertyDeclarationSyntax { get; set; } = null!;
    
    public string AccessModifier { get; set; } = "public";
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string? Description { get; set; } = null;
    public bool IsFlag { get; set; } = false;
    
    public TypeSyntax PropertyType => PropertyDeclarationSyntax.Type;
    public bool IsNullable => PropertyType is NullableTypeSyntax;

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public static CliArgsParameterPropertyDto FromPropertyDeclarationSyntax(PropertyDeclarationSyntax propertySyntax, ISymbol propertySymbol) {
        string accessModifier = propertySyntax.Modifiers switch {
            var modifiers when modifiers.Any(SyntaxKind.PrivateKeyword) => "private",
            var modifiers when modifiers.Any(SyntaxKind.ProtectedKeyword) => "protected",
            var modifiers when modifiers.Any(SyntaxKind.InternalKeyword) => "internal",
            _ => "public"
        };
        
        ImmutableArray<AttributeData> attributes = propertySymbol.GetAttributes();
        AttributeData parameterAttribute = attributes.FirstOrDefault(attr => attr.AttributeClass!.Name.ToString().Contains("CliArgsParameter"))!;
        AttributeData descriptionAttribute = attributes.FirstOrDefault(attr => attr.AttributeClass!.Name.ToString().Contains("CliArgsDescription"))!;
        
        // Get the actual values from the symbol
        string name = parameterAttribute.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString() ?? string.Empty;
        string shortName = parameterAttribute.ConstructorArguments.ElementAtOrDefault(1).Value?.ToString() ?? string.Empty;
        bool isFlag = parameterAttribute.ConstructorArguments.ElementAtOrDefault(2).Value as int? == 1;
        string? description = descriptionAttribute.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString();
        
        return new CliArgsParameterPropertyDto {
            PropertyDeclarationSyntax = propertySyntax,
            AccessModifier = accessModifier,
            Name = name,
            ShortName = shortName,
            Description = description,
            IsFlag = isFlag
        };
    }
}
