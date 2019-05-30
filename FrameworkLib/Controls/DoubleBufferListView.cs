using System.Windows.Forms;

namespace FrameworkLib.Controls
{
	public sealed class DoubleBufferListView : ListView
	{
		public DoubleBufferListView()
		{
			SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
			UpdateStyles();
		}
	}
}
