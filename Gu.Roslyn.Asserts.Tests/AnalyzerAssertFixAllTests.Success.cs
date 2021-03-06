namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using Gu.Roslyn.Asserts.Tests.CodeFixes;
    using Microsoft.CodeAnalysis;
    using NUnit.Framework;

    [TestFixture]
    public partial class AnalyzerAssertFixAllTests
    {
        public class Success
        {
            [TearDown]
            public void TearDown()
            {
                AnalyzerAssert.MetadataReferences.Clear();
            }

            [Test]
            public void OneErrorCorrectFix()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode);
            }

            [TestCase("Rename to: value1", "value1")]
            [TestCase("Rename to: value2", "value2")]
            public void SingleClassOneErrorTwoFixesCorrectFix(string title, string expected)
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
                fixedCode = fixedCode.AssertReplace("value", expected);
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(code, fixedCode, title);
            }

            [Test]
            public void TwoErrorsCorrectFix()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value1;
        private readonly int ↓_value2;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value1;
        private readonly int value2;
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode);
            }

            [Test]
            public void FixAllInDocumentTwoErrorsCorrectFix()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value1;
        private readonly int ↓_value2;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value1;
        private readonly int value2;
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                AnalyzerAssert.FixAllInDocument<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode);
            }

            [Test]
            public void FixAllOneByOneTwoErrorsCorrectFix()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value1;
        private readonly int ↓_value2;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value1;
        private readonly int value2;
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                AnalyzerAssert.FixAllOneByOne<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(code, fixedCode);
            }

            [Test]
            public void SingleClassOneErrorCorrectFixExplicitTitleExpectedDiagnosticWithPositionAnalyzerSupportsTwoDiagnostics1()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        public readonly int ↓_value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        public readonly int value;
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic.Id1);
                AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, code, fixedCode);
                AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, new[] { code }, fixedCode);
                AnalyzerAssert.FixAll(new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, code, fixedCode);
                AnalyzerAssert.FixAll(new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, new[] { code }, fixedCode);
                AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, code, fixedCode, "Rename to: value");
                AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, new[] { code }, fixedCode, "Rename to: value");
                AnalyzerAssert.FixAll(new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, code, fixedCode, fixTitle: "Rename to: value");
                AnalyzerAssert.FixAll(new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, new[] { code }, fixedCode, fixTitle: "Rename to: value");
            }

            [Test]
            public void SingleClassOneErrorCorrectFixExplicitTitleExpectedDiagnosticWithPositionAnalyzerSupportsTwoDiagnostics2()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic.Id2);
                AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, code, fixedCode);
                AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, new[] { code }, fixedCode);
                AnalyzerAssert.FixAll(new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, code, fixedCode);
                AnalyzerAssert.FixAll(new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, new[] { code }, fixedCode);
                AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, code, fixedCode, "Rename to: value");
                AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic, DontUseUnderscoreCodeFixProvider>(expectedDiagnostic, new[] { code }, fixedCode, "Rename to: value");
                AnalyzerAssert.FixAll(new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, code, fixedCode, fixTitle: "Rename to: value");
                AnalyzerAssert.FixAll(new FieldNameMustNotBeginWithUnderscoreDifferentDiagnosticsForPublic(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, new[] { code }, fixedCode, fixTitle: "Rename to: value");
            }

            [Test]
            public void TwoClassesTwoErrorsTwoFixes()
            {
                var code1 = @"
namespace RoslynSandbox
{
    using System;

    public class Foo1
    {
        public event EventHandler ↓Bar;
    }
}";

                var code2 = @"
namespace RoslynSandbox
{
    using System;

    public class Foo2
    {
        public event EventHandler ↓Bar;
    }
}";

                var fixed1 = @"
namespace RoslynSandbox
{
    using System;

    public class Foo1
    {
    }
}";

                var fixed2 = @"
namespace RoslynSandbox
{
    using System;

    public class Foo2
    {
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create("CS0067");
                AnalyzerAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code1, code2 }, new[] { fixed1, fixed2 });
                AnalyzerAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code2, code1 }, new[] { fixed2, fixed1 });
            }

            [TestCase("Rename to: value1", "value1")]
            [TestCase("Rename to: value2", "value2")]
            public void TwoClassesOneErrorWhenCodeFixProviderHasManyFixes(string title, string expected)
            {
                var code1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int ↓_value;
    }
}";
                var code2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
        private readonly int value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int value;
    }
}";

                fixedCode = fixedCode.AssertReplace("value", expected);
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(new[] { code1, code2 }, new[] { fixedCode, code2 }, title);
                AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(new[] { code1, code2 }, fixedCode, title);
                AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(expectedDiagnostic, new[] { code1, code2 }, new[] { fixedCode, code2 }, title);
                AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(expectedDiagnostic, new[] { code1, code2 }, fixedCode, title);
            }

            [TestCase("Rename to: value1", "value1")]
            [TestCase("Rename to: value2", "value2")]
            public void TwoClassesOneFixCorrectFixPassOnlyFixedCode(string title, string expected)
            {
                var code1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int ↓_value;
    }
}";
                var code2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
        private readonly int value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int value;
    }
}";

                fixedCode = fixedCode.AssertReplace("value", expected);
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(new[] { code1, code2 }, fixedCode, title);
            }

            [TestCase("Rename to: value1", "value1")]
            [TestCase("Rename to: value2", "value2")]
            public void TwoClassesTwoFixesCorrectFix(string title, string expected)
            {
                var code1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int ↓_value;
    }
}";
                var code2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
        private readonly int ↓_value;
    }
}";

                var fixedCode1 = @"
namespace RoslynSandbox
{
    class Foo1
    {
        private readonly int value;
    }
}";

                var fixedCode2 = @"
namespace RoslynSandbox
{
    class Foo2
    {
        private readonly int value;
    }
}";
                fixedCode1 = fixedCode1.AssertReplace("value", expected);
                fixedCode2 = fixedCode2.AssertReplace("value", expected);
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreManyCodeFixProvider>(new[] { code1, code2 }, new[] { fixedCode1, fixedCode2 }, title);
            }

            [Test]
            public void TwoClassesDifferentProjectsCodeFixOnlyOneFix()
            {
                var code1 = @"
namespace RoslynSandbox.Core
{
    using System;

    public class Foo1
    {
        public event EventHandler ↓Bar;
    }
}";

                var code2 = @"
namespace RoslynSandbox.Client
{
    public class Foo2
    {
    }
}";

                var fixedCode = @"
namespace RoslynSandbox.Core
{
    using System;

    public class Foo1
    {
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", code1, out code1);
                AnalyzerAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code1, code2 }, new[] { fixedCode, code2 });
                AnalyzerAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code2, code1 }, new[] { code2, fixedCode });
            }

            [Test]
            public void TwoClassesDifferentProjectsCodeFixOnlyOneFixPassingOnlyFixedCode()
            {
                var code1 = @"
namespace RoslynSandbox.Core
{
    using System;

    public class Foo1
    {
        public event EventHandler ↓Bar;
    }
}";

                var code2 = @"
namespace RoslynSandbox.Client
{
    public class Foo2
    {
    }
}";

                var fixedCode = @"
namespace RoslynSandbox.Core
{
    using System;

    public class Foo1
    {
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", code1, out code1);
                AnalyzerAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code1, code2 }, fixedCode);
                AnalyzerAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code2, code1 }, fixedCode);
                AnalyzerAssert.FixAll(new RemoveUnusedFixProvider(), expectedDiagnostic, new[] { code2, code1 }, fixedCode);
            }

            [Test]
            public void TwoClassesDifferentProjectsCodeFixOnlyCorrectFix()
            {
                var code1 = @"
namespace RoslynSandbox.Core
{
    using System;

    public class FooCore
    {
        public event EventHandler ↓Bar;
    }
}";

                var code2 = @"
namespace RoslynSandbox.Client
{
    public class FooClient
    {
    }
}";

                var fixedCode = @"
namespace RoslynSandbox.Core
{
    using System;

    public class FooCore
    {
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", code1, out code1);
                AnalyzerAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code1, code2 }, fixedCode);
                AnalyzerAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code2, code1 }, fixedCode);
                AnalyzerAssert.FixAll(new RemoveUnusedFixProvider(), expectedDiagnostic, new[] { code2, code1 }, fixedCode);
            }

            [Test]
            public void TwoClassesDifferentProjectsInheritingCodeFixOnlyCorrectFix()
            {
                var code1 = @"
namespace RoslynSandbox.Core
{
    using System;

    public class Foo1
    {
        public event EventHandler ↓Bar;
    }
}";

                var code2 = @"
namespace RoslynSandbox.Client
{
    public class Foo2 : RoslynSandbox.Core.Foo1
    {
    }
}";

                var fixedCode = @"
namespace RoslynSandbox.Core
{
    using System;

    public class Foo1
    {
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", code1, out code1);
                AnalyzerAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code1, code2 }, fixedCode);
                AnalyzerAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code2, code1 }, fixedCode);
                AnalyzerAssert.FixAll(new RemoveUnusedFixProvider(), expectedDiagnostic, new[] { code2, code1 }, fixedCode);
            }

            [Test]
            public void TwoClassesDifferentProjectsInheritingCodeFixOnlyCorrectFix2()
            {
                var code1 = @"
namespace RoslynSandbox.Core
{
    public class Foo1
    {
    }
}";

                var code2 = @"
namespace RoslynSandbox.Client
{
    using System;

    public class Foo2 : RoslynSandbox.Core.Foo1
    {
        public event EventHandler ↓Bar;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox.Client
{
    using System;

    public class Foo2 : RoslynSandbox.Core.Foo1
    {
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", code2, out code2);
                AnalyzerAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code1, code2 }, fixedCode);
                AnalyzerAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, new[] { code2, code1 }, fixedCode);
                AnalyzerAssert.FixAll(new RemoveUnusedFixProvider(), expectedDiagnostic, new[] { code2, code1 }, fixedCode);
            }

            [Test]
            public void SingleClassCodeFixOnlyCorrectFix()
            {
                var code = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public event EventHandler ↓Bar;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(EventHandler).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.CreateFromCodeWithErrorsIndicated("CS0067", code, out code);
                AnalyzerAssert.FixAll<RemoveUnusedFixProvider>(expectedDiagnostic, code, fixedCode);
                AnalyzerAssert.FixAll(new RemoveUnusedFixProvider(), expectedDiagnostic, code, fixedCode);
            }

            [Test]
            public void TwoClassOneErrorCorrectFix()
            {
                var barCode = @"
namespace RoslynSandbox
{
    class Bar
    {
        private readonly int value;
    }
}";

                var testCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓_value;
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int value;
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldNameMustNotBeginWithUnderscore.DiagnosticId);
                AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(new[] { barCode, testCode }, new[] { barCode, fixedCode });
                AnalyzerAssert.FixAll<FieldNameMustNotBeginWithUnderscore, DontUseUnderscoreCodeFixProvider>(new[] { barCode, testCode }, fixedCode);
                AnalyzerAssert.FixAll(new FieldNameMustNotBeginWithUnderscore(), new DontUseUnderscoreCodeFixProvider(), new[] { barCode, testCode }, fixedCode);
                AnalyzerAssert.FixAll(new FieldNameMustNotBeginWithUnderscore(), new DontUseUnderscoreCodeFixProvider(), new[] { barCode, testCode }, new[] { barCode, fixedCode });
                AnalyzerAssert.FixAll(new FieldNameMustNotBeginWithUnderscore(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, new[] { barCode, testCode }, fixedCode);
                AnalyzerAssert.FixAll(new FieldNameMustNotBeginWithUnderscore(), new DontUseUnderscoreCodeFixProvider(), expectedDiagnostic, new[] { barCode, testCode }, new[] { barCode, fixedCode });
            }

            [Test]
            public void WhenFixIntroducesCompilerErrorsThatAreAccepted()
            {
                var code = @"
namespace RoslynSandbox
{
    ↓class Foo
    {
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        public event EventHandler SomeEvent;
    }
}";
                AnalyzerAssert.FixAll<ClassMustHaveEventAnalyzer, InsertEventFixProvider>(code, fixedCode, allowCompilationErrors: AllowCompilationErrors.Yes);
            }

            [Test]
            public void WithExpectedDiagnosticWhenOneReportsError()
            {
                var code = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int ↓wrongName;
        
        public int WrongName { get; set; }
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    class Foo
    {
        private readonly int foo;
        
        public int WrongName { get; set; }
    }
}";
                AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
                var expectedDiagnostic = ExpectedDiagnostic.Create(FieldAndPropertyMustBeNamedFooAnalyzer.FieldDiagnosticId);
                AnalyzerAssert.FixAll<FieldAndPropertyMustBeNamedFooAnalyzer, RenameToFooCodeFixProvider>(expectedDiagnostic, code, fixedCode);
                AnalyzerAssert.FixAll(new FieldAndPropertyMustBeNamedFooAnalyzer(), new RenameToFooCodeFixProvider(), expectedDiagnostic, code, fixedCode);
            }
        }
    }
}
