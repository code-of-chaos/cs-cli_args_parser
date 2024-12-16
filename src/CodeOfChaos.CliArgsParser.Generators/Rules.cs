// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.CodeAnalysis;

namespace CodeOfChaos.CliArgsParser.Generators;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class Rules {
    public static void ReportParameterStructMustBePartial(this SourceProductionContext context, Location location, string structName)
        => context.ReportDiagnostic(
            Diagnostic.Create(
                new DiagnosticDescriptor(
                    "CLI001",
                    "Parameter struct must be partial",
                    "The struct '{0}' must be declared as partial to support CLI argument parsing.",
                    "Usage",
                    DiagnosticSeverity.Error,
                    true
                ),
                location,
                structName
            )
        );

    public static void ReportCommandClassMustBePartial(this SourceProductionContext context, Location location, string className)
        => context.ReportDiagnostic(
            Diagnostic.Create(
                new DiagnosticDescriptor(
                    "CLI002",
                    "Command class must be partial",
                    "The class '{0}' must be declared as partial to be supported as a CLI command.",
                    "Usage",
                    DiagnosticSeverity.Error,
                    true
                ),
                location,
                className
            )
        );

    public static void ReportCommandClassMustImplementICommand(this SourceProductionContext context, Location location, string className)
        => context.ReportDiagnostic(
            Diagnostic.Create(
                new DiagnosticDescriptor(
                    "CLI003",
                    "Command class must implement ICommand",
                    "The class '{0}' must implement the ICommand interface to be supported as a CLI command.",
                    "Usage",
                    DiagnosticSeverity.Error,
                    true
                ),
                location,
                className
            )
        );

    public static void ReportParameterPropertyDuplicateNames(this SourceProductionContext context, Location location, string propertyName, string parameterName)
        => context.ReportDiagnostic(
            Diagnostic.Create(
                new DiagnosticDescriptor(
                    "CLI004",
                    "Parameter property duplicates",
                    "The property '{0}' has a parameter name of '{1}', that is duplicate to another property",
                    "Usage",
                    DiagnosticSeverity.Error,
                    true
                ),
                location,
                propertyName,
                parameterName
            )
        );

    public static void ReportParameterPropertyDuplicateShortNames(this SourceProductionContext context, Location location, string propertyName, string parameterName)
        => context.ReportDiagnostic(
            Diagnostic.Create(
                new DiagnosticDescriptor(
                    "CLI005",
                    "Parameter property duplicates",
                    "The property '{0}' has a parameter short-name of '{1}', that is duplicate to another property",
                    "Usage",
                    DiagnosticSeverity.Error,
                    true
                ),
                location,
                propertyName,
                parameterName
            )
        );

    public static void ReportParameterPropertyMustHaveInit(this SourceProductionContext context, Location location, string propertyName)
        => context.ReportDiagnostic(
            Diagnostic.Create(
                new DiagnosticDescriptor(
                    "CLI006",
                    "Parameter property must have an Initializer",
                    "The property '{0}' is missing an initializer.",
                    "Usage",
                    DiagnosticSeverity.Error,
                    true
                ),
                location,
                propertyName
            )
        );
}
