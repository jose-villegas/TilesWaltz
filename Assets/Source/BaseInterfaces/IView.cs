using System.ComponentModel;

namespace TilesWalk.BaseInterfaces
{
	public interface IView
	{
		void PropertyChanged(object sender, PropertyChangedEventArgs e);
	}
}

