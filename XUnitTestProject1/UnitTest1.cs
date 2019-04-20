using System;
using System.IO;
using Xunit;
using YouSubtle;
using YouSubtle.IO;

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
	}
}
