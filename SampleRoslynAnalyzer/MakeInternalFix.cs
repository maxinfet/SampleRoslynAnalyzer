namespace SampleRoslynAnalyzer
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MakeInternalFix))]
    [Shared]
    public class MakeInternalFix : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            ClassMustBeInternalAnalyzer.DiagnosticId);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                          .ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                if (syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start) is SyntaxToken token &&
                    token.IsKind(SyntaxKind.PublicKeyword))
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            "Make internal.",
                            _ => Task.FromResult(
                                context.Document.WithSyntaxRoot(
                                    syntaxRoot.ReplaceToken(
                                        token,
                                        SyntaxFactory.Token(SyntaxKind.InternalKeyword).WithTriviaFrom(token)))),
                            nameof(MakeInternalFix)),
                        diagnostic);
                }
            }
        }
    }
}