using Microsoft.CodeAnalysis;

namespace WebEssentials.AspNetCore.Analyzers
{
    internal static class Descriptors
    {
        public static DiagnosticDescriptor MiddlewareOrderWrong => Generate(1000,
            "Specify at least one source files. Source files can not be null or empty.",
            "Specify at least one source files. Source files can not be null or empty.");

        public static DiagnosticDescriptor AsyncVoidPageHandler => Generate(1001,
            "A page handler should never be async void.",
            "A page handler should never be async void.");

        private static DiagnosticDescriptor Generate(int id, string title, string messageFormat, DiagnosticSeverity severity = DiagnosticSeverity.Warning)
        {
            return new DiagnosticDescriptor(
               id: $"WE{id}",
               title: title,
               messageFormat: messageFormat,
               category: "Usage",
               defaultSeverity: severity,
               isEnabledByDefault: true);
        }

        internal static DiagnosticDescriptor Clone(this DiagnosticDescriptor descriptor, string title)
        {
            return Generate(
                id: int.Parse(descriptor.Id.Substring(2)),
                title: title,
                messageFormat: title,
                severity: descriptor.DefaultSeverity
                );
        }
    }
}
