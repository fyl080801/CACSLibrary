using System;
using System.Collections;
using System.Windows;

namespace CACSLibrary.Silverlight.Maps
{
	public interface IMapVirtualSource
	{
		void Request(double minZoom, double maxZoom, Point lowerLeft, Point upperRight, Action<ICollection> callback);
	}
}
