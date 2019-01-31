using System;
using System.IO;
using Xunit;
using YouSubtle;

namespace XUnitTestProject1
{
	public class UnitTest1
	{
		[Fact]
		public void Test1()
		{
			try
			{
				var foundFiles =  new DirectoryInfo(@"D:\CsScriptExecuter\ConsoleApp1\bin\Debug\netcoreapp2.1")
				.GetDescendentFiles("System.Runtime.dll");
			}
			catch(Exception)
			{
				Assert.True(false);
			}
			
				
		}
	}
}
