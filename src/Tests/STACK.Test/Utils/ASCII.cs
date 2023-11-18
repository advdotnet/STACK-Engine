using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace STACK.Test
{
	[TestClass]
	public class ASCII
	{
		[TestMethod]
		public void ASCIITest()
		{
			var enc = (Encoding)Encoding.GetEncoding("utf-8").Clone();
			enc.EncoderFallback = new EncoderReplacementFallback("");
			var chars = new char[1];
			var bytes = new byte[16];

			var sw = new StringBuilder();
			for (var i = 40; i <= 255; i++)
			{
				chars[0] = (char)i;
				var count = enc.GetBytes(chars, 0, 1, bytes, 0);

				if (count != 0)
				{
					sw.Append(chars[0]);
					sw.Append(',');
				}
			}

			var result = sw.ToString();
			System.Console.WriteLine(result);
		}
	}
}
