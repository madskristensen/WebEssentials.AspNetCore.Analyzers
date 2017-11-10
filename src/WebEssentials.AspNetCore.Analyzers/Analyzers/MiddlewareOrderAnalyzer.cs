using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace WebEssentials.AspNetCore.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MiddlewareOrderAnalyzer : DiagnosticAnalyzer
    {
        private static Dictionary<string, IEnumerable<string>> _rules = new Dictionary<string, IEnumerable<string>> {
            { "UseBrowserLink", new [] { "UseDeveloperExceptionPage", "UseMvc", "UseMvcWithDefaultRoute" } },
            { "UseDeveloperExceptionPage",  new [] { "UseMvc", "UseMvcWithDefaultRoute" } },
            { "UseAuthentication", new [] { "UseMvc" ,"UseMvcWithDefaultRoute" } },
            { "UseDefaultFiles", new [] { "UseStaticFiles" } },
            { "UseResponseCompression", new [] { "UseMvc" ,"UseMvcWithDefaultRoute" } },
            { "UseCors", new [] { "UseMvc" ,"UseMvcWithDefaultRoute" } },
        };

        private DiagnosticDescriptor _descriptor = Descriptors.MiddlewareOrderWrong;
        private List<string> _order = new List<string>();

        public MiddlewareOrderAnalyzer()
        {
            SupportedDiagnostics = ImmutableArray.Create(_descriptor);
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        public override void Initialize(AnalysisContext context)
        {
            _order.Clear();

            context.RegisterSyntaxNodeAction(syntaxContext =>
            {
                var invocation = (InvocationExpressionSyntax)syntaxContext.Node;
                SymbolInfo symbolInfo = syntaxContext.SemanticModel.GetSymbolInfo(invocation, syntaxContext.CancellationToken);

                if (!(symbolInfo.Symbol is IMethodSymbol methodSymbol && methodSymbol.ReceiverType?.Name == "IApplicationBuilder"))
                {
                    return;
                }

                CheckForOrderingErrors(syntaxContext, invocation, methodSymbol);

                _order.Add(methodSymbol.Name);

            }, SyntaxKind.InvocationExpression);
        }

        private void CheckForOrderingErrors(SyntaxNodeAnalysisContext syntaxContext, InvocationExpressionSyntax invocation, IMethodSymbol methodSymbol)
        {
            if (!_rules.TryGetValue(methodSymbol.Name, out var others))
            {
                return;
            }

            foreach (string other in others)
            {
                if (!_order.Contains(other))
                    continue;

                string parent = methodSymbol.ReceiverType.Name;
                var error = _descriptor.Clone($"The call to {parent}.{methodSymbol.Name}() must be before {parent}.{other}()");

                syntaxContext.ReportDiagnostic(Diagnostic.Create(error, invocation.GetLocation()));

            }
        }
    }
}
