using System.Collections.Generic;
using System.Drawing;

namespace UnitTest
{
	public class TestModel
	{
		public int Integer;
		public long Integer64;
		public string String;
		public bool Boolean;
		public decimal Decimal;
		public Color Color;
		public List<int> Integers;

		public TestModel()
		{
			Integer = int.MaxValue;
			Integer64 = long.MaxValue;
			String = string.Empty;
			Boolean = true;
			Decimal = decimal.MaxValue;
			Color = Color.Red;
			Integers = new List<int> { int.MinValue, int.MaxValue, default };
		}
	}
}
