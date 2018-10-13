using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WindowsFormsApp
{
	public class Data : INotifyPropertyChanged
	{
		private int index;
		private int latency;
		private string description;

		public int Index
		{
			get => index;
			set
			{
				if (index != value)
				{
					index = value;
					NotifyPropertyChanged();
				}
			}
		}

		public int Latency
		{
			get => latency;
			set
			{
				if (latency != value)
				{
					latency = value;
					NotifyPropertyChanged();
				}
			}
		}

		public string Description
		{
			get => description;
			set
			{
				if (description != value)
				{
					description = value;
					NotifyPropertyChanged();
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = @"")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public Data()
		{
			index = 0;
			latency = 0;
			description = string.Empty;
		}
	}
}
