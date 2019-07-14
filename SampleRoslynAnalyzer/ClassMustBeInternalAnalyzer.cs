namespace SampleRoslynAnalyzer
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ClassMustBeInternalAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ClassMustBeSealed";

        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "This analyzer reports warnings if a class is public.",
            messageFormat: "The class {0} should be internal.",
            category: "Weird.Warning",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(c => HandleDeclaration(c), SyntaxKind.ClassDeclaration);
        }

        private static void HandleDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ClassDeclarationSyntax classDeclaration &&
                TryFindPublicKeyword(classDeclaration.Modifiers, out var publicKeyword))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, publicKeyword.GetLocation()));
            }

            bool TryFindPublicKeyword(SyntaxTokenList modifiers, out SyntaxToken token)
            {
                foreach (var modifier in modifiers)
                {
                    if (modifier.IsKind(SyntaxKind.PublicKeyword))
                    {
                        token = modifier;
                        return true;
                    }
                }

                token = default;
                return false;
            }
        }
    }
}
