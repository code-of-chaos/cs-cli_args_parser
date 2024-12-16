// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser.Generators.Helpers;
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

    private readonly ITypeSymbol? _genericTypeArgument = (symbol as ITypeSymbol)?.AllInterfaces.FirstOrDefault(i => i.OriginalDefinition.ToDisplayString().EndsWith("ICommand<T>"))?.TypeArguments.FirstOrDefault();
    private string GenericTypeDisplayName => _genericTypeArgument?.ToDisplayString() ?? "UNDEFINED";
    private readonly AttributeData? _commandNameAttribute = symbol.GetAttributes().FirstOrDefault(attr => attr.AttributeClass?.Name.ToString().Contains("CliArgsCommand") == true);
    private string CommandName => 
        _commandNameAttribute?.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString() 
        ?? ClassName.Replace("Command", "").ToLowerInvariant(); // Maybe create a ToKebabCase method?
    
    private readonly AttributeData? _descriptionAttribute = symbol.GetAttributes().FirstOrDefault(attr => attr.AttributeClass?.Name.ToString().Contains("CliArgsDescription") == true);
    private string Description => _descriptionAttribute?.ConstructorArguments.ElementAtOrDefault(0).Value?.ToString() ?? string.Empty;
    
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

    public void ToCommandData(GeneratorStringBuilder builder) {
        builder.AppendLine("public CommandData CommandData { get; } = new CommandData(")
            .Indent()
            .AppendLine($"\"{CommandName}\",")
            .AppendLine($"\"{Description}\",")
            .AppendLine($"typeof({symbol.ToDisplayString()})")
            .UnIndent()
            .AppendLine(");");
    }

    public void ToCommandInitialization(GeneratorStringBuilder builder) {
        builder.AppendLine("public Task InitializeAsync(IUserInputRegistry registry) {")
            .Indent()
            .AppendLine($"return ExecuteAsync({GenericTypeDisplayName}.FromRegistry(registry));")
            .UnIndent()
            .AppendLine("}");
    }

    public void ReportDiagnostics(SourceProductionContext context) {
        if (!_isPartial) context.ReportCommandClassMustBePartial(_location, ClassName);
        if (_genericTypeArgument is null) context.ReportCommandClassMustImplementICommand(_location, ClassName);
    }
}
