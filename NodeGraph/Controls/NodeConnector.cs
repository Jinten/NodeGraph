using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace NodeGraph.Controls
{
	public enum ConnectorLayoutType
	{
		Top,
		Center,
		Bottom,
	}	

	public abstract class NodeConnectorContent : ContentControl
	{
        /// <summary>
        /// connecting node links.
        /// </summary>
        protected IEnumerable<NodeLink> NodeLinks => _NodeLinks;
        List<NodeLink> _NodeLinks = new List<NodeLink>();

        public void Connect(NodeLink nodeLink)
        {
            _NodeLinks.Add(nodeLink);
        }

        public void Disconnect(NodeLink nodeLink)
        {
            _NodeLinks.Remove(nodeLink);
        }

        public abstract void UpdatePosition(Canvas canvas);
        public abstract bool CanConnectTo(NodeConnectorContent connector);
    }

	public abstract class NodeConnector<T> : MultiSelector where T : NodeConnectorContent, new()
	{
		public ConnectorLayoutType ConnectorLayout
		{
			get => (ConnectorLayoutType)GetValue(ConnectorLayoutProperty);
			set => SetValue(ConnectorLayoutProperty, value);
		}
		public static readonly DependencyProperty ConnectorLayoutProperty = DependencyProperty.Register(
			nameof(ConnectorLayout),
			typeof(ConnectorLayoutType),
			typeof(NodeConnector<T>),
			new FrameworkPropertyMetadata(ConnectorLayoutType.Top, ConnectorLayoutPropertyChanged));

		public double ConnectorMargin
		{
			get => (double)GetValue(ConnectorMarginProperty);
			set => SetValue(ConnectorMarginProperty, value);
		}
		public static readonly DependencyProperty ConnectorMarginProperty = DependencyProperty.Register(
			nameof(ConnectorMargin),
			typeof(double),
			typeof(NodeConnector<T>),
			new FrameworkPropertyMetadata(2.0, ConnectorMarginPropertyChanged));

		/// <summary>
		/// must implement connector canvas name.
		/// </summary>
		protected abstract string ConnectorCanvasName { get; }

		/// <summary>
		/// must implement connector content template.
		/// </summary>
		protected abstract ControlTemplate NodeConnectorContentTemplate { get; }

		Canvas _Canvas = null;
		bool _IsStyleInitialized = false;
		List<object> _DelayToBindVMs = new List<object>();


		static void ConnectorMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as NodeConnector<T>).UpdateConnectorsLayout();
		}

		static void ConnectorLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as NodeConnector<T>).UpdateConnectorsLayout();
		}

        public void UpdatePosition(Canvas canvas)
        {
            foreach(var connector in _Canvas.Children.OfType<NodeConnectorContent>())
            {
                connector.UpdatePosition(canvas);
            }
        }

        public void UpdateConnectorsLayout()
		{
			switch(ConnectorLayout)
			{
				case ConnectorLayoutType.Top:
					ReplaceConnectElements(0, FontSize);
					break;
				case ConnectorLayoutType.Center:
					{
						var halfSize = _Canvas.Children.Count * FontSize * 0.5;
						ReplaceConnectElements(ActualHeight * 0.5 - halfSize, FontSize);
						break;
					}
				case ConnectorLayoutType.Bottom:
					{
						var totalSize = _Canvas.Children.Count * FontSize;
						ReplaceConnectElements(ActualHeight - totalSize, FontSize);
						break;
					}
			}

			var inputs = _Canvas.Children.OfType<NodeConnectorContent>();

			Width = inputs.Max(arg => arg.ActualWidth);
			Height = inputs.Sum(arg => arg.ActualHeight) + inputs.Count() * ConnectorMargin;
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_Canvas = GetTemplateChild(ConnectorCanvasName) as Canvas;
		}		

		protected override void OnItemContainerStyleChanged(Style oldItemContainerStyle, Style newItemContainerStyle)
		{
			base.OnItemContainerStyleChanged(oldItemContainerStyle, newItemContainerStyle);

			_IsStyleInitialized = true;

			if(_DelayToBindVMs.Count > 0 && _Canvas != null)
			{
				AddConnectorsToCanvas(_DelayToBindVMs.OfType<object>());

				UpdateConnectorsLayout();

				_DelayToBindVMs.Clear();
			}
		}

		protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
		{
			base.OnItemsSourceChanged(oldValue, newValue);
			if(_Canvas == null || _IsStyleInitialized == false)
			{
				_DelayToBindVMs.AddRange(newValue.OfType<object>());
			}
			else
			{
				RemoveConnectorFromCanvas(oldValue.OfType<object>());
				AddConnectorsToCanvas(newValue.OfType<object>());

				UpdateConnectorsLayout();
			}
		}

		void RemoveConnectorFromCanvas(IEnumerable<object> removeVMs)
		{
			var removeElements = new List<UIElement>();
			var children = _Canvas.Children.OfType<FrameworkElement>();

			foreach(var removeVM in removeVMs)
			{
				var removeElement = children.First(arg => arg.DataContext == removeVM);
				removeElements.Add(removeElement);
			}

			removeElements.ForEach(arg => _Canvas.Children.Remove(arg));
		}

		void AddConnectorsToCanvas(IEnumerable<object> addVMs)
		{
			foreach(var vm in addVMs)
			{
				var connector = new T()
				{
					DataContext = vm,
					Template = NodeConnectorContentTemplate,
					Style = ItemContainerStyle
				};

				connector.SizeChanged += ConnectorSizeChanged;
				_Canvas.Children.Add(connector);
			}
		}

		void ConnectorSizeChanged(object sender, SizeChangedEventArgs e)
		{
			UpdateConnectorsLayout();
		}

		void ReplaceConnectElements(double offset, double margin)
		{
			int index = 0;
			foreach(var element in _Canvas.Children.OfType<FrameworkElement>())
			{
				element.Margin = new Thickness(0, offset + margin * index * ConnectorMargin, 0, 0);
				++index;
			}
		}
	}
}
