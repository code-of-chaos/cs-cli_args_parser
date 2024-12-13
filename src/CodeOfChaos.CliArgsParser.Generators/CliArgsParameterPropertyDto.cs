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
public class CliArgsParameterPropertyDto {
    public PropertyDeclarationSyntax PropertyDeclarationSyntax { get; private set; } = null!;
    public IPropertySymbol Symbol { get; private set; } = null!;
    
    public string ParameterName { get; private set; } = null!;
    public string ParameterShortName { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsFlag { get; private set; }
    public bool IsValid { get; private set; } = true;
    public List<string> InvalidReason { get; private set; } = [];
    
    public string PropertyDefaultValue { get; private set; } = null!;
    public TypeSyntax PropertyType { get; private set; } = null!; 
    public string PropertyName { get; private set; } = null!; 
    public bool IsRequiredProperty { get; private set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public static CliArgsParameterPropertyDto FromContext(GeneratorSyntaxContext context, PropertyDeclarationSyntax propertySyntax) {
        IPropertySymbol propertySymbol = context.SemanticModel.GetDeclaredSymbol(propertySyntax)!;
        
        ImmutableArray<AttributeData> attributes = propertySymbol.GetAttributes();
        AttributeData parameterAttribute = attributes.FirstOrDefault(attr => attr.AttributeClass!.Name.ToString().Contains("CliArgsParameter"))!;
        AttributeData descriptionAttribute = attributes.FirstOrDefault(attr => attr.AttributeClass!.Name.ToString().Contains("CliArgsDescription"))!;
        
        // Get the actual values from the symbol
        string name = parameterAttribute.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString() ?? string.Empty;
        string shortName = parameterAttribute.ConstructorArguments.ElementAtOrDefault(1).Value?.ToString() ?? string.Empty;
        bool isFlag = parameterAttribute.ConstructorArguments.ElementAtOrDefault(2).Value as uint? == 1u;
        string? description = descriptionAttribute.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString();
        
        // Instead of having a lot of computer properties, just do the computation once,
        // to limit errors with parsing later on
        return new CliArgsParameterPropertyDto {
            PropertyDeclarationSyntax = propertySyntax,
            Symbol = propertySymbol,
            ParameterName = name,
            ParameterShortName = shortName,
            Description = description,
            IsFlag = isFlag,
            PropertyDefaultValue = ToPropertyDefaultValue(propertySyntax, propertySymbol),
            PropertyType = propertySyntax.Type,
            PropertyName = propertySyntax.Identifier.ToString(),
            IsRequiredProperty = propertySyntax.Modifiers.Any(SyntaxKind.RequiredKeyword),
        };
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public string ToPropertyInitialization() {
        return IsRequiredProperty 
            ? $"{PropertyName} = registry.GetParameter<{PropertyType}>(\"{ParameterName}\")," 
            : $"{PropertyName} = registry.GetOptionalParameter<{PropertyType}>(\"{ParameterName}\") ?? {PropertyDefaultValue},";
    }

    private static string ToPropertyDefaultValue(PropertyDeclarationSyntax propertySyntax, IPropertySymbol symbol) {
        if (propertySyntax.Initializer?.Value.ToString() is {} predefinedDefault) return predefinedDefault;
        // Check if the type of the property symbol is a collection
        // If so, return an empty collection.
        if (symbol.Type.AllInterfaces.Any(i => i.Name.Contains("ICollection"))) return "[]";
        return propertySyntax.Type is NullableTypeSyntax 
            ? "null"
            : "default";
    }

    public void SetInvalid(string reason) {
        IsValid = false;
        InvalidReason.Add(reason);
    }
}
