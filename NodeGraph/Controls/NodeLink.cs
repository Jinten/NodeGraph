using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NodeGraph.Controls
{
	public class NodeLink : Shape
	{
		protected override Geometry DefiningGeometry => _TestGeometry;
		LineGeometry _TestGeometry = null;

		public NodeLink()
		{
			_TestGeometry = new LineGeometry(new Point(0, 0), new Point(100, 100));
			this.StrokeThickness = 1;
			this.Stroke = Brushes.Black;
		}
	}
}
