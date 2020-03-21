using System;
using System.IO;
using Xunit;
using Decova.IO;
using Decova;

namespace XUnitTestProject1
{
	public class UnitTest1
	{
		[Fact]
		public void PathPob_EnsureRelativeTo_NonrelatedPathAndReference()
		{
			string path = @"D:\Test1\Test2";
			string referenceDir = @"C:\Windows";

			void faulty()
			{
				PathTechie.EnsureRelativeTo(referenceDir, path);
			};

			Assert.Throws<InvalidOperationException>(()=>faulty());
		}

		[Fact]
		public void PathPob_EnsureAbsolute_NonrelatedPathAndReference()
		{
			string path = @"D:\Test1\Test2";
			string referenceDir = @"C:\Windows";

			void faulty()
			{
				PathTechie.EnsureAbsolutePath(referenceDir, path);
			};

			Assert.Throws<InvalidOperationException>(() => faulty());
		}

		[Fact]
		public void PathPob_EnsureAbsolute_ValidAbsPathAndReference()
		{
			string path = @"D:\Test1\Test2";
			string referenceDir = @"D:\";

			string result = PathTechie.EnsureAbsolutePath(referenceDir, path);

			Assert.True(result == path);
		}

		[Fact]
		public void PathPob_EnsureRelativeTo_ValidAbsPathAndReference()
		{
			string path = @"D:\Test1\Test2";
			string referenceDir = @"D:\";

			string result = PathTechie.EnsureRelativeTo(referenceDir, path);

			Assert.True(result == "Test1\\Test2");
		}

        [Fact]
        public void Smoke()
        {
            new FileInfo("D:\\testx\\testy\\test.txt").EnsureDirectory();
            new DirectoryInfo(@"D:\1\2\3\4\5").Ensure();
        }
        
        [Fact]
        public void ExtractZippedFiles()
        {
            var zip = new Zip(@"D:\WorkflowDesigner.FullVersion\WorkflowClientApp2\wwwroot\WorkflowDesignerStaticContents\StaticContents.zip");

            zip.ExtractFiles(@"D:\WorkflowDesigner.FullVersion\WorkflowClientApp2\wwwroot\WorkflowDesignerStaticContents",
                             false);
        }

        [Fact]
        public void ReplaceEntry()
        {
            var zip = new Zip(@"G:\_MyProjects\YouSubtle.Iris.Foundation\YouSubtle.Iris.Abstraction\bin\Debug\YouSubtle.Iris.Abstraction.1.0.5.nupkg");
            zip.AddFile("lib/netstandard2.0/YouSubtle.Iris.Abstraction.dll",
                        ///*Take non-protected*/@"G:\_MyProjects\YouSubtle.Iris.Foundation\YouSubtle.Iris.Abstraction\bin\Debug\netstandard2.0\YouSubtle.Iris.Abstraction.dll",
                        /*Take protected*/@"G:\_MyProjects\YouSubtle.Iris.Foundation\YouSubtle.Iris.Abstraction\bin\Debug\netstandard2.0\YouSubtle.Iris.Abstraction_Secure\YouSubtle.Iris.Abstraction.dll",
                        true,
                        out Exception exception);
        }

        [Fact]
        public void GetDescendentFiles()
        {
            var dir = new DirectoryInfo(@"G:\_MyProjects\YouSubtle.JetSuit\YouSubtle.JetSuit\bin");
            var files = dir.GetDescendantFiles(f => f.Name.ToLower() == "YouSubtle.JetSuit.CommandPad.sln".ToLower());
        }
	}
}
