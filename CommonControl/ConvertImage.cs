using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

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

		public static Bitmap GetGrayImage(Bitmap image)
		{
			var result = image.Clone() as Bitmap;

			for (var i = 0; i < image.Width; ++i)
			{
				for (var j = 0; j < image.Height; ++j)
				{
					var c = result.GetPixel(i, j);

					var ret = (int)(c.R * 0.299 + c.G * 0.587 + c.B * 0.114);

					result.SetPixel(i, j, Color.FromArgb(ret, ret, ret));
				}
			}

			return result;
		}

		public static Bitmap GetBinaryzationImage(Bitmap image)
		{
			var result = image.Clone() as Bitmap;

			var tempList = new List<int>();
			for (var i = 0; i < result.Width; i++)
			{
				for (var j = 0; j < result.Height; j++)
				{
					if (result.GetPixel(i, j).A != 0)
					{
						tempList.Add(result.GetPixel(i, j).R);
						tempList.Add(result.GetPixel(i, j).G);
						tempList.Add(result.GetPixel(i, j).B);
					}
				}
			}

			var average = tempList.Average();


			for (var i = 0; i < result.Width; i++)
			{
				for (var j = 0; j < result.Height; j++)
				{
					var color = result.GetPixel(i, j);
					if (color.A != 0)
					{
						double v = (color.R + color.G + color.B);
						if (v / 3 > average)
						{
							result.SetPixel(i, j, Color.White);
						}
						else
						{
							result.SetPixel(i, j, Color.Black);
						}
					}
				}
			}

			return result;
		}

		public static Image ToImage(Icon icon)
		{
			return Image.FromHbitmap(icon.ToBitmap().GetHbitmap());
		}

		public static Image ToImage(string filepath)
		{
			return Image.FromFile(filepath);
		}

		public static Bitmap ToBitmap(Image img)
		{
			return new Bitmap(img);

		}

		public static Icon ToIcon(Bitmap map)
		{
			return Icon.FromHandle(map.GetHicon());
		}
	}
}
