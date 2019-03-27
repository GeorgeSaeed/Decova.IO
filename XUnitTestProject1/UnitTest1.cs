using System;
using System.IO;
using Xunit;
using YouSubtle;
using YouSubtle.IO;

namespace XUnitTestProject1
{
	public class UnitTest1
	{
		//[Fact]
		//public void Test1()
		//{
		//	try
		//	{
		//		var foundFiles =  new DirectoryInfo(@"D:\CsScriptExecuter\ConsoleApp1\bin\Debug\netcoreapp2.1")
		//		.GetDescendentFiles("System.Runtime.dll");
		//	}
		//	catch(Exception)
		//	{
		//		Assert.True(false);
		//	}
			
				
		//}
		[Fact]
		public void PathComposer_EnsureRelativeTo_NonrelatedPathAndReference()
		{
			string path = @"D:\Test1\Test2";
			string referenceDir = @"C:\Windows";

			void faulty()
			{
				PathComposer.EnsureRelativeTo(referenceDir, path);
			};

			Assert.Throws<InvalidOperationException>(()=>faulty());
		}

		[Fact]
		public void PathComposer_EnsureAbsolute_NonrelatedPathAndReference()
		{
			string path = @"D:\Test1\Test2";
			string referenceDir = @"C:\Windows";

			void faulty()
			{
				PathComposer.EnsureAbsolutePath(referenceDir, path);
			};

			Assert.Throws<InvalidOperationException>(() => faulty());
		}

		[Fact]
		public void PathComposer_EnsureAbsolute_ValidAbsPathAndReference()
		{
			string path = @"D:\Test1\Test2";
			string referenceDir = @"D:\";

			string result = PathComposer.EnsureAbsolutePath(referenceDir, path);

			Assert.True(result == path);
		}

		[Fact]
		public void PathComposer_EnsureRelativeTo_ValidAbsPathAndReference()
		{
			string path = @"D:\Test1\Test2";
			string referenceDir = @"D:\";

			string result = PathComposer.EnsureRelativeTo(referenceDir, path);

			Assert.True(result == "Test1\\Test2");
		}

	}
}
