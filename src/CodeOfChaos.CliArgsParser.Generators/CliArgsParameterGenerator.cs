// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

namespace CodeOfChaos.CliArgsParser.Generators;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[Generator(LanguageNames.CSharp)]
public class CliArgsParameterGenerator : IIncrementalGenerator{

    public void Initialize(IncrementalGeneratorInitializationContext context) {
        IncrementalValueProvider<ImmutableArray<CliArgsParameterStructDto>> parameterStructs = context.SyntaxProvider
            .CreateSyntaxProvider(
                IsParameterStruct,
                GatherParameterStruct
            ).Collect();
        
        context.RegisterSourceOutput(context.CompilationProvider.Combine(parameterStructs), GenerateSources);
    }

    private static bool IsParameterStruct(SyntaxNode node, CancellationToken _) {
        if (node is not StructDeclarationSyntax { BaseList: { Types.Count: > 0 } baseList, Members: var members }) return false;

        if (!baseList.Types.Any(
                type => type.Type is IdentifierNameSyntax genericNameSyntax
                    && genericNameSyntax.Identifier.ValueText.Contains("IParameters")
            )) return false;

        return members.OfType<PropertyDeclarationSyntax>()
            .SelectMany(prop => prop.AttributeLists.SelectMany(attrlist => attrlist.Attributes))
            .Any(attr => attr.Name.ToString().Contains("CliArgsParameter")
            );

    }

    private static CliArgsParameterStructDto GatherParameterStruct(GeneratorSyntaxContext context, CancellationToken cancellationToken) {
        var structSyntax = (StructDeclarationSyntax)context.Node;
        ISymbol structSymbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, structSyntax)!;

        CliArgsParameterPropertyDto[] properties = structSyntax.Members
            .OfType<PropertyDeclarationSyntax>()
            .Where(prop => prop.AttributeLists.Any(attrlist => attrlist.Attributes.Any(attr => attr.Name.ToString().Contains("CliArgsParameter"))))
            .Select(syntax => CliArgsParameterPropertyDto.FromPropertyDeclarationSyntax(syntax, ModelExtensions.GetDeclaredSymbol(context.SemanticModel, syntax)!))
            .ToArray();

        return new CliArgsParameterStructDto {
            PropertyDtos = properties,
            Symbol = structSymbol,
            IsNotPartial = !structSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword))
        };
    }
    
    private static void GenerateSources(SourceProductionContext context, (Compilation Left, ImmutableArray<CliArgsParameterStructDto> Right) source) {
        (_, ImmutableArray<CliArgsParameterStructDto> cliArgsParameterStructDtos) = source;
        StringBuilder builder = new();
    }
}
