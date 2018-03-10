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
            Encoding enc = (Encoding)Encoding.GetEncoding("utf-8").Clone();
            enc.EncoderFallback = new EncoderReplacementFallback("");
            char[] chars = new char[1];
            byte[] bytes = new byte[16];

            StringBuilder sw = new StringBuilder();
            for (int i = 40; i <= 255; i++)
            {
                chars[0] = (char)i;
                int count = enc.GetBytes(chars, 0, 1, bytes, 0);

                if (count != 0)
                {
                    sw.Append(chars[0]);
                    sw.Append(',');
                }
            }

            var Result = sw.ToString();
        }
    }
}
