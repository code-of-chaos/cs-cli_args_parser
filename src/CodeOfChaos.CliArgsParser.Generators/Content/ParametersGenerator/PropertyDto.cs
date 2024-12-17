// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace CodeOfChaos.CliArgsParser.Generators.Content.ParametersGenerator;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class PropertyDto(IPropertySymbol symbol, PropertyDeclarationSyntax syntax, AttributeData parameterAttribute) {

    private readonly AttributeData? _descriptionAttribute = symbol.GetAttributes().FirstOrDefault(attr => attr.AttributeClass?.Name.ToString().Contains("CliArgsDescription") == true);
    private readonly bool _hasInit = syntax.AccessorList?.Accessors.Any(a => a.Keyword.IsKind(SyntaxKind.InitKeyword)) ?? false;
    private readonly Location _location = symbol.Locations.First();

    public readonly bool IsFlag = parameterAttribute.ConstructorArguments.ElementAtOrDefault(2).Value as uint? == 1u;
    public readonly bool IsNullableAnnotated = symbol.Type.NullableAnnotation == NullableAnnotation.Annotated;
    public readonly bool IsPropertyValueType = symbol.Type.IsValueType;
    public readonly bool IsRequiredProperty = syntax.Modifiers.Any(SyntaxKind.RequiredKeyword);

    public readonly string ParameterName = parameterAttribute.ConstructorArguments.ElementAt(0).Value?.ToString() ?? string.Empty;
    public readonly string? ParameterShortName = parameterAttribute.ConstructorArguments.ElementAtOrDefault(1).Value?.ToString();

    public readonly string PropertyDefaultValue = ToPropertyDefaultValue(syntax, symbol);
    public readonly string PropertyName = syntax.Identifier.ToString();
    public readonly TypeSyntax PropertyType = syntax.Type;
    public string Description => _descriptionAttribute?.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString() ?? string.Empty;

    // diagnostics
    public bool IsDuplicateName { get; set; }
    public bool IsDuplicateShortName { get; set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public static PropertyDto FromContext(GeneratorSyntaxContext context, PropertyDeclarationSyntax propertySyntax) {
        IPropertySymbol propertySymbol = context.SemanticModel.GetDeclaredSymbol(propertySyntax)!;

        return new PropertyDto(
            propertySymbol,
            propertySyntax,
            // Instead of having a lot of computer properties, just do the computation once,
            // to limit errors with parsing later on
            propertySymbol.GetAttributes().First(attr => attr.AttributeClass?.Name.ToString().Contains("CliArgsParameter") == true)
        );
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

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public string ToPropertyInitialization() {
        return this switch {
            { IsRequiredProperty: true }
                => $"{PropertyName} = registry.GetParameterByPossibleNames<{PropertyType}>(\"--{ParameterName}\", \"-{ParameterShortName}\"),",
            { IsPropertyValueType: true, IsNullableAnnotated: false }
                => $"{PropertyName} = registry.GetOptionalParameterByPossibleNames<{PropertyType}?>(\"--{ParameterName}\", \"-{ParameterShortName}\") ?? {PropertyDefaultValue},",
            _
                => $"{PropertyName} = registry.GetOptionalParameterByPossibleNames<{PropertyType}>(\"--{ParameterName}\", \"-{ParameterShortName}\") ?? {PropertyDefaultValue},"
        };
    }


    public void ReportDiagnostics(SourceProductionContext context) {
        if (IsDuplicateName) context.ReportParameterPropertyDuplicateNames(_location, PropertyName, ParameterName);
        if (IsDuplicateShortName) context.ReportParameterPropertyDuplicateShortNames(_location, PropertyName, ParameterShortName ?? "UNDEFINED");
        if (!_hasInit) context.ReportParameterPropertyMustHaveInit(_location, PropertyName);
    }
}
