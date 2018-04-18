// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable PossibleNullReferenceException
namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.IO;
    using System.Reflection;
    using NUnit.Framework;

    public partial class CodeFactoryTests
    {
        public class FindFiles
        {
            private static readonly FileInfo ExecutingAssemblyDll = new FileInfo(new Uri(Assembly.GetExecutingAssembly().CodeBase, UriKind.Absolute).LocalPath);

            [Test]
            public void TryFindProjectFileInParentDirectory()
            {
                var directory = ExecutingAssemblyDll.Directory;
                var projectFileName = Path.GetFileNameWithoutExtension(ExecutingAssemblyDll.FullName) + ".csproj";
                Assert.AreEqual(true, CodeFactory.TryFindFileInParentDirectory(directory, projectFileName, out var projectFile));
                Assert.AreEqual(projectFileName, projectFile.Name);
            }

            [Test]
            public void TryFindSolutionFileInParentDirectory()
            {
                var directory = ExecutingAssemblyDll.Directory;
                Assert.AreEqual(true, CodeFactory.TryFindFileInParentDirectory(directory, "Gu.Roslyn.Asserts.sln", out var projectFile));
                Assert.AreEqual("Gu.Roslyn.Asserts.sln", projectFile.Name);
            }

            [Test]
            public void TryFindProjectFileFromDll()
            {
                Assert.AreEqual(true, CodeFactory.TryFindProjectFile(ExecutingAssemblyDll, out var projectFile));
                Assert.AreEqual(Path.GetFileNameWithoutExtension(ExecutingAssemblyDll.FullName) + ".csproj", projectFile.Name);
            }

            [TestCase("Gu.Roslyn.Asserts.Tests.csproj")]
            [TestCase("WpfApp1.csproj")]
            public void TryFindProjectFileFromName(string name)
            {
                Assert.AreEqual(true, CodeFactory.TryFindProjectFile(name, out var projectFile));
                Assert.AreEqual(name, projectFile.Name);
                projectFile = CodeFactory.FindProjectFile(name);
                Assert.AreEqual(name, projectFile.Name);
            }
        }
    }
}
