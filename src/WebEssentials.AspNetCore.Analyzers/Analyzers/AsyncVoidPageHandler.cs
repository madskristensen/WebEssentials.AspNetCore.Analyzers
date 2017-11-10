using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace WebEssentials.AspNetCore.Analyzers
{
    /// <summary>
    /// Validates that Razor Pages handlers aren't async void methods.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AsyncVoidPageHandler : DiagnosticAnalyzer
    {
        private DiagnosticDescriptor _descriptor = Descriptors.AsyncVoidPageHandler;

        public AsyncVoidPageHandler()
        {
            SupportedDiagnostics = ImmutableArray.Create(_descriptor);
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(syntaxContext =>
            {
                var invocation = (MethodDeclarationSyntax)syntaxContext.Node;

                if (invocation.Parent is ClassDeclarationSyntax cls &&
                    cls.BaseList?.Types.FirstOrDefault()?.Type?.ToString() == "PageModel" &&
                    invocation.Identifier.Text.StartsWith("On") &&
                    invocation.Modifiers.Any(SyntaxKind.PublicKeyword) &&
                    invocation.Modifiers.Any(SyntaxKind.AsyncKeyword) &&
                    invocation.ReturnType?.ToString() == "void")
                {
                    syntaxContext.ReportDiagnostic(Diagnostic.Create(_descriptor, invocation.Identifier.GetLocation()));
                }


            }, SyntaxKind.MethodDeclaration);
        }
    }
}
