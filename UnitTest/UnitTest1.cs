using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;

namespace UnitTest
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			var model = new TestModel();

			var json = JsonConvert.SerializeObject(model, Formatting.Indented);
			Console.WriteLine(json);
		}
	}
}
