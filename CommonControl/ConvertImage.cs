using System.Drawing;
using System.Drawing.Drawing2D;

namespace CommonControl
{
	public static class ConvertImage
	{
		public static Image ResizeImage(Image imgToResize, Size size)
		{
			//获取图片宽度
			var sourceWidth = imgToResize.Width;
			//获取图片高度
			var sourceHeight = imgToResize.Height;

			//计算宽度的缩放比例
			var nPercentW = size.Width / (float)sourceWidth;
			//计算高度的缩放比例
			var nPercentH = size.Height / (float)sourceHeight;

			var nPercent = nPercentH < nPercentW ? nPercentH : nPercentW;
			//期望的宽度
			var destWidth = (int)(sourceWidth * nPercent);
			//期望的高度
			var destHeight = (int)(sourceHeight * nPercent);

			var b = new Bitmap(destWidth, destHeight);
			var g = Graphics.FromImage(b);
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			//绘制图像
			g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
			g.Dispose();
			return b;
		}
	}
}
