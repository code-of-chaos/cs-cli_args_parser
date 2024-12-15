// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace CodeOfChaos.CliArgsParser.Generators.Content.CommandGenerator;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ClassDto(ISymbol symbol, ClassDeclarationSyntax syntax) {
    private readonly bool _isPartial = syntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword));
    private readonly bool _hasEmptyConstructor = syntax.ParameterList?.Parameters.Count == 0;
    private readonly Location _location = symbol.Locations.First();

    public readonly string ClassName = symbol.Name;
    public readonly string Namespace = symbol.ContainingNamespace.ToDisplayString();
    
    public readonly ITypeSymbol? GenericTypeArgument = (symbol as ITypeSymbol)?.AllInterfaces.FirstOrDefault(i => i.OriginalDefinition.ToDisplayString().EndsWith("ICommand<T>"))?.TypeArguments.FirstOrDefault();
    
    
    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public static ClassDto FromSyntax(GeneratorSyntaxContext context, ClassDeclarationSyntax classSyntax) {
        ISymbol symbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, classSyntax)!;
        return new ClassDto(symbol, classSyntax);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public string ToDeclarationName () {
        string constructor = _hasEmptyConstructor ? string.Empty : "()";
        return $"{ClassName}{constructor}";
    }

    public void ReportDiagnostics(SourceProductionContext context) {
        if (!_isPartial) context.ReportCommandClassMustBePartial(_location, ClassName);
        if (GenericTypeArgument is null) context.ReportCommandClassMustImplementICommand(_location, ClassName);
    }
}
