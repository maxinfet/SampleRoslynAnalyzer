namespace SampleRoslynAnalyzer.Tests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Tests
    {
        private static readonly ClassMustBeInternalAnalyzer Analyzer = new ClassMustBeInternalAnalyzer();
        private static readonly MakeInternalFix Fix = new MakeInternalFix();

        [Test]
        public static void ValidWhenExplicitInternal()
        {
            var code = @"
namespace N
{
    internal class C { }
}";

            RoslynAssert.Valid(Analyzer, code);
        }

        [Test]
        public static void ValidWhenImplicitInternal()
        {
            var code = @"
namespace N
{
    class C { }
}";

            RoslynAssert.Valid(Analyzer, code);
        }

        [Test]
        public static void FixWhenPublic()
        {
            var before = @"
namespace N
{
    ↓public class C { }
}";

            var after = @"
namespace N
{
    internal class C { }
}";

            RoslynAssert.CodeFix(Analyzer, Fix, before, after);
        }
    }
}
