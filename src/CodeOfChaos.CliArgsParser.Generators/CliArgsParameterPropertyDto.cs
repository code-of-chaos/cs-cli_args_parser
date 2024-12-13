// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CodeOfChaos.CliArgsParser.Generators;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public struct CliArgsParameterPropertyDto() {
    public PropertyDeclarationSyntax PropertyDeclarationSyntax { get; set; } = null!;
    public ISymbol Symbol { get; set; } = null!;
    
    
    
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string? Description { get; set; } = null;
    public bool IsFlag { get; set; } = false;
    
    public TypeSyntax PropertyType => PropertyDeclarationSyntax.Type;
    public bool IsNullable => PropertyType is NullableTypeSyntax;
    public string PropertyName => PropertyDeclarationSyntax.Identifier.ToString();
    public bool IsRequiredProperty => PropertyDeclarationSyntax.Modifiers.Any(SyntaxKind.RequiredKeyword);
    public string PropertyDefaultValue {
        get {
            if (PropertyDeclarationSyntax.Initializer?.Value.ToString() is {} predefinedDefault) return predefinedDefault;
            // Check if the type of the property symbol is a collection
            // If so, return an empty array.
            if (Symbol is IPropertySymbol { Type: {} typeSymbol } 
                && typeSymbol.AllInterfaces.Any(i => i.Name.Contains("ICollection"))) return "[]";
            return "default";
        }
    }
    
    public string AccessModifier =>  PropertyDeclarationSyntax.Modifiers switch {
        var modifiers when modifiers.Any(SyntaxKind.PrivateKeyword) => "private",
        var modifiers when modifiers.Any(SyntaxKind.ProtectedKeyword) => "protected",
        var modifiers when modifiers.Any(SyntaxKind.InternalKeyword) => "internal",
        _ => "public"
    };

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public static CliArgsParameterPropertyDto FromPropertyDeclarationSyntax(PropertyDeclarationSyntax propertySyntax, ISymbol propertySymbol) {
        ImmutableArray<AttributeData> attributes = propertySymbol.GetAttributes();
        AttributeData parameterAttribute = attributes.FirstOrDefault(attr => attr.AttributeClass!.Name.ToString().Contains("CliArgsParameter"))!;
        AttributeData descriptionAttribute = attributes.FirstOrDefault(attr => attr.AttributeClass!.Name.ToString().Contains("CliArgsDescription"))!;
        
        // Get the actual values from the symbol
        string name = parameterAttribute.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString() ?? string.Empty;
        string shortName = parameterAttribute.ConstructorArguments.ElementAtOrDefault(1).Value?.ToString() ?? string.Empty;
        bool isFlag = parameterAttribute.ConstructorArguments.ElementAtOrDefault(2).Value as uint? == 1u;
        string? description = descriptionAttribute.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString();
        
        return new CliArgsParameterPropertyDto {
            PropertyDeclarationSyntax = propertySyntax,
            Symbol = propertySymbol,
            Name = name,
            ShortName = shortName,
            Description = description,
            IsFlag = isFlag,
        };
    }
}
