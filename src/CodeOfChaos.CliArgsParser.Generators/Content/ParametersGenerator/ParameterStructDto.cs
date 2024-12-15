// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace CodeOfChaos.CliArgsParser.Generators.Content.ParametersGenerator;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ParameterStructDto(ISymbol symbol, StructDeclarationSyntax syntax) {
    private readonly bool _isPartial = syntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword));
    private readonly bool _hasEmptyConstructor = syntax.ParameterList?.Parameters.Count == 0;
    private readonly Location _location = symbol.Locations.First();

    public readonly string ClassName = symbol.Name;
    public readonly string Namespace = symbol.ContainingNamespace.ToDisplayString();

    public PropertyDto[] PropertyDtos { get; private set; } = [];

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public static ParameterStructDto FromSyntax(GeneratorSyntaxContext context, StructDeclarationSyntax structSyntax) {
        ISymbol structSymbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, structSyntax)!;

        return new ParameterStructDto(structSymbol, structSyntax) {
            PropertyDtos = structSyntax.Members
                .OfType<PropertyDeclarationSyntax>()
                .Where(prop => prop.AttributeLists.Any(attrs => attrs.Attributes.Any(attr => attr.Name.ToString().Contains("CliArgsParameter"))))
                .Select(syntax => PropertyDto.FromContext(context, syntax))
                .ToArray()
        };
    }
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public string ToDeclarationName() {
        string constructor = _hasEmptyConstructor ? string.Empty : "()";
        return $"{ClassName}{constructor}";
    }
    
    public void ReportDiagnostics(SourceProductionContext context) {
        // Check the variables of this class
        if (!_isPartial) context.ReportParameterStructMustBePartial(_location, ClassName);

        // Technically these errors pertain to the individual properties,
        //      but it is easier to check and assign them in this level
        GatherDuplicateNames().ForEach(dto => dto.IsDuplicateName = true);
        GatherDuplicateShortNames().ForEach(dto => dto.IsDuplicateShortName = true);
        
        // check all the nested propertyDtos as well for individual errors
        foreach (PropertyDto dto in PropertyDtos) {
            dto.ReportDiagnostics(context);
        }
    }

    private List<PropertyDto> GatherDuplicateNames() {
        return PropertyDtos
            .GroupBy(p => p.ParameterName)
            .Where(group => group.Count() > 1)
            .SelectMany(group => group)
            .ToList();
    }

    private List<PropertyDto> GatherDuplicateShortNames() {
        return PropertyDtos
            .Where(p => p.ParameterShortName != null)
            .GroupBy(p => p.ParameterShortName)
            .Where(group => group.Count() > 1)
            .SelectMany(group => group)
            .ToList();
    }
}
