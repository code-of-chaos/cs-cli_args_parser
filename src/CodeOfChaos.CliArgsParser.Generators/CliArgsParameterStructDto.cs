// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace CodeOfChaos.CliArgsParser.Generators;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CliArgsParameterStructDto {
    public bool IsPartial { get; private set; }
    public ISymbol Symbol { get; private set; } = null!;
    public bool HasEmptyConstructor { get; private set; }

    public string ClassName { get; private set; } = null!;
    public string Namespace { get; private set; } = null!;

    public CliArgsParameterPropertyDto[] PropertyDtos { get; private set; } = [];

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public static CliArgsParameterStructDto FromSyntax(GeneratorSyntaxContext context, StructDeclarationSyntax structSyntax) {
        ISymbol structSymbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, structSyntax)!;

        CliArgsParameterPropertyDto[] properties = structSyntax.Members
            .OfType<PropertyDeclarationSyntax>()
            .Where(prop => prop.AttributeLists.Any(attrs => attrs.Attributes.Any(attr => attr.Name.ToString().Contains("CliArgsParameter"))))
            .Select(syntax => CliArgsParameterPropertyDto.FromContext(context, syntax))
            .ToArray();
        
        bool hasEmptyConstructor = structSyntax.ParameterList?.Parameters.Count == 0;
        ValidateProperties(properties);

        return new CliArgsParameterStructDto {
            Symbol = structSymbol,
            IsPartial = structSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword)),
            ClassName = structSymbol.Name,
            Namespace = structSymbol.ContainingNamespace.ToDisplayString(),
            PropertyDtos = properties,
            HasEmptyConstructor = hasEmptyConstructor,
        };
    }

    private static void ValidateProperties(CliArgsParameterPropertyDto[] properties) {
        // Validate duplicate ParameterName
        CliArgsParameterPropertyDto[] duplicateNames = properties
            .GroupBy(p => p.ParameterName)
            .Where(group => group.Count() > 1)
            .SelectMany(group => group)
            .ToArray();

        foreach (CliArgsParameterPropertyDto property in duplicateNames) {
            property.SetInvalid("Duplicate parameter name");
        }

        // Validate duplicate ParameterShortName
        CliArgsParameterPropertyDto[] duplicateShortNames = properties
            .GroupBy(p => p.ParameterShortName)
            .Where(group => group.Count() > 1)
            .SelectMany(group => group)
            .ToArray();

        foreach (CliArgsParameterPropertyDto property in duplicateShortNames) {
            property.SetInvalid("Duplicate parameter short name");
        }
    }

    public string ToStructDeclarationName () {
        string constructor = HasEmptyConstructor ? string.Empty : "()";
        return $"{ClassName}{constructor}";
    }
}
