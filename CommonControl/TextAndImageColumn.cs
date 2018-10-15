using System.Drawing;
using System.Windows.Forms;

namespace Common
{
	public class TextAndImageColumn : DataGridViewTextBoxColumn
	{
		private Image imageValue;
		private Size imageSize;

		public TextAndImageColumn()
		{
			CellTemplate = new TextAndImageCell();
		}

		public sealed override DataGridViewCell CellTemplate
		{
			get => base.CellTemplate;
			set => base.CellTemplate = value;
		}

		public override object Clone()
		{
			var c = base.Clone() as TextAndImageColumn;
			c.imageValue = imageValue;
			c.imageSize = imageSize;
			return c;
		}

		public Image Image
		{
			get => imageValue;
			set
			{
				if (Image != value)
				{
					imageValue = value;
					imageSize = value.Size;

					if (InheritedStyle != null)
					{
						var inheritedPadding = InheritedStyle.Padding;
						DefaultCellStyle.Padding = new Padding(imageSize.Width,
					inheritedPadding.Top, inheritedPadding.Right,
					inheritedPadding.Bottom);
					}
				}
			}
		}
		private TextAndImageCell TextAndImageCellTemplate => CellTemplate as TextAndImageCell;

		internal Size ImageSize => imageSize;
	}

	public class TextAndImageCell : DataGridViewTextBoxCell
	{
		private Image imageValue;
		private Size imageSize;

		public override object Clone()
		{
			var c = base.Clone() as TextAndImageCell;
			c.imageValue = imageValue;
			c.imageSize = imageSize;
			return c;
		}

		public Image Image
		{
			get
			{
				if (OwningColumn == null ||
		   OwningTextAndImageColumn == null)
				{

					return imageValue;
				}
				else if (imageValue != null)
				{
					return imageValue;
				}
				else
				{
					return OwningTextAndImageColumn.Image;
				}
			}
			set
			{
				if (imageValue != value)
				{
					imageValue = value;
					imageSize = value.Size;

					var inheritedPadding = InheritedStyle.Padding;
					Style.Padding = new Padding(imageSize.Width,
				   inheritedPadding.Top, inheritedPadding.Right,
				   inheritedPadding.Bottom);
				}
			}
		}
		protected override void Paint(Graphics graphics, Rectangle clipBounds,
	   Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState,
	   object value, object formattedValue, string errorText,
	   DataGridViewCellStyle cellStyle,
	   DataGridViewAdvancedBorderStyle advancedBorderStyle,
	   DataGridViewPaintParts paintParts)
		{
			// Paint the base content
			base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState,
			  value, formattedValue, errorText, cellStyle,
			  advancedBorderStyle, paintParts);

			if (Image != null)
			{
				// Draw the image clipped to the cell.
				var container =
			   graphics.BeginContainer();

				graphics.SetClip(cellBounds);
				graphics.DrawImageUnscaled(Image, cellBounds.Location);

				graphics.EndContainer(container);
			}
		}

		private TextAndImageColumn OwningTextAndImageColumn => OwningColumn as TextAndImageColumn;
	}
}