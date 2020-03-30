using NodeGraph.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NodeGraph.Controls
{
    public class NodeGraph : MultiSelector
    {
        public Canvas Canvas { get; private set; } = null;

        ControlTemplate NodeTemplate => _NodeTemplate.Get("__NodeTemplate__");
        ResourceInstance<ControlTemplate> _NodeTemplate = new ResourceInstance<ControlTemplate>();

        Style NodeBaseStyle => _NodeBaseStyle.Get("__NodeBaseStyle__");
        ResourceInstance<Style> _NodeBaseStyle = new ResourceInstance<Style>();

        bool _IsNodeSelected = false;
        bool _IsStartDragging = false;

        NodeLink _DraggingNodeLink = null;
        List<Node> _DraggingNodes = new List<Node>();
        List<NodeConnectorContent> _DraggingConnectors = new List<NodeConnectorContent>();
        Point _DragStartPoint = new Point();

        NodeLink _RemoveNodeLink = null;

        List<object> _DelayToBindVMs = new List<object>();

        static NodeGraph()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeGraph), new FrameworkPropertyMetadata(typeof(NodeGraph)));
        }

        public Point GetDragNodePosition(MouseEventArgs e)
        {
            return e.GetPosition(Canvas);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Canvas = GetTemplateChild("__NodeGraphCanvas__") as Canvas;

            if (_DelayToBindVMs.Count > 0)
            {
                AddNodesToCanvas(_DelayToBindVMs.OfType<object>());
                _DelayToBindVMs.Clear();
            }
        }

        protected override void OnItemContainerStyleChanged(Style oldItemContainerStyle, Style newItemContainerStyle)
        {
            base.OnItemContainerStyleChanged(oldItemContainerStyle, newItemContainerStyle);
            ItemContainerStyle.BasedOn = NodeBaseStyle;
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            if (Canvas == null)
            {
                _DelayToBindVMs.AddRange(newValue.OfType<object>());
            }
            else
            {
                AddNodesToCanvas(newValue.OfType<object>());
            }

            if (oldValue != null && oldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= NodeCollectionChanged;
            }
            if (newValue != null && newValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += NodeCollectionChanged;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_DraggingNodes.Count > 0)
            {
                if (_IsStartDragging == false)
                {
                    if (_DraggingNodes[0].IsSelected)
                    {
                        foreach (var node in Canvas.Children.OfType<Node>())
                        {
                            if (node != _DraggingNodes[0] && node.IsSelected)
                            {
                                node.CaptureDragStartPosition();
                                _DraggingNodes.Add(node);
                            }
                        }
                    }
                    else
                    {
                        _DraggingNodes[0].IsSelected = true; // select first drag node.
                        foreach (var node in Canvas.Children.OfType<Node>())
                        {
                            if (node != _DraggingNodes[0])
                            {
                                node.IsSelected = false;
                            }
                        }
                    }
                    _IsStartDragging = true;
                }

                var current = e.GetPosition(Canvas);
                var diff = new Point(current.X - _DragStartPoint.X, current.Y - _DragStartPoint.Y);

                foreach (var node in _DraggingNodes)
                {
                    double x = node.DragStartPosition.X + diff.X;
                    double y = node.DragStartPosition.Y + diff.Y;
                    node.UpdatePosition(Canvas, x, y);
                }
            }
            if (_DraggingNodeLink != null)
            {
                var pos = e.GetPosition(Canvas);
                _DraggingNodeLink.EndPoint = new Point(pos.X - 1, pos.Y - 1); // minus -1 for not detect mouse up on node link control.
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            _IsStartDragging = false;

            if (_IsNodeSelected == false)
            {
                foreach (var node in Canvas.Children.OfType<Node>())
                {
                    node.IsSelected = false;
                }
            }
            _IsNodeSelected = false;
            _DraggingNodes.Clear();

            var element = e.OriginalSource as FrameworkElement;

            // clicked connect or not.
            if (_DraggingNodeLink != null && _DraggingConnectors.Count > 0)
            {
                // only be able to connect input to output or output to input.
                // it will reject except above condition.
                if (element != null && element.Tag is NodeConnectorContent connector && connector.CanConnectTo(_DraggingConnectors[0]))
                {
                    var transformer = element.TransformToVisual(Canvas);
                    var posOnCanvas = transformer.Transform(new Point(element.ActualWidth * 0.5, element.ActualHeight * 0.5));

                    switch (connector)
                    {
                        case NodeInputContent inputContent:
                            _DraggingNodeLink.Connect(inputContent);
                            break;
                        case NodeOutputContent outputContent:
                            _DraggingNodeLink.Connect(outputContent);
                            break;
                        default:
                            throw new InvalidCastException();
                    }

                    _DraggingNodeLink.EndPoint = new Point(posOnCanvas.X, posOnCanvas.Y);

                    connector.Connect(_DraggingNodeLink);
                    _DraggingConnectors.ForEach(arg => arg.Connect(_DraggingNodeLink));

                    // add node link.
                    _DraggingNodeLink.MouseDown += NodeLink_MouseDown;

                }
                else
                {
                    Canvas.Children.Remove(_DraggingNodeLink);
                    _DraggingNodeLink.Dispose();
                }

                _DraggingNodeLink = null;
                _DraggingConnectors.Clear();
            }

            if (_RemoveNodeLink != null && _RemoveNodeLink != element)
            {
                Canvas.Children.Remove(_RemoveNodeLink);

                _RemoveNodeLink.MouseDown -= NodeLink_MouseDown;
                _RemoveNodeLink.Disconnect();
                _RemoveNodeLink.Dispose();
                _RemoveNodeLink = null;
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            _IsStartDragging = false;
            _DraggingNodes.Clear();
            _DraggingConnectors.Clear();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            _IsStartDragging = false;
            _DraggingNodes.Clear();
            _DraggingConnectors.Clear();
        }

        void NodeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems?.Count > 0)
            {
                RemoveNodesFromCanvas(e.OldItems.OfType<object>());
            }
            if (e.NewItems?.Count > 0)
            {
                AddNodesToCanvas(e.NewItems.OfType<object>());
            }
        }

        void RemoveNodesFromCanvas(IEnumerable<object> removeVMs)
        {
            var removeElements = new List<Node>();
            var children = Canvas.Children.OfType<Node>();

            foreach (var removeVM in removeVMs)
            {
                var removeElement = children.First(arg => arg.DataContext == removeVM);
                removeElements.Add(removeElement);
            }

            foreach (var removeElement in removeElements)
            {
                Canvas.Children.Remove(removeElement);

                removeElement.MouseUp -= Node_MouseUp;
                removeElement.MouseDown -= Node_MouseDown;
                removeElement.Dispose();
            }
        }

        void AddNodesToCanvas(IEnumerable<object> addVMs)
        {
            foreach (var vm in addVMs)
            {
                var node = new Node(this)
                {
                    DataContext = vm,
                    Template = NodeTemplate,
                    Style = ItemContainerStyle
                };

                node.MouseDown += Node_MouseDown;
                node.MouseUp += Node_MouseUp;

                Canvas.Children.Add(node);
            }
        }

        void NodeLink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _RemoveNodeLink = sender as NodeLink;
        }

        void Node_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _IsNodeSelected = true;

            if (_IsStartDragging)
            {
                return;
            }

            if ((Keyboard.GetKeyStates(Key.LeftCtrl) & KeyStates.Down) != 0)
            {
                var node = sender as Node;
                node.IsSelected = !node.IsSelected;
            }
            else
            {
                foreach (var node in Canvas.Children.OfType<Node>())
                {
                    if (node != sender)
                    {
                        node.IsSelected = false;
                    }
                    else
                    {
                        node.IsSelected = true;
                    }
                }
            }
        }

        void Node_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = e.OriginalSource as FrameworkElement;

            if (element != null && element.Tag is NodeConnectorContent connector)
            {
                // clicked on connector
                var transformer = element.TransformToVisual(Canvas);
                var posOnCanvas = transformer.Transform(new Point(element.ActualWidth * 0.5, element.ActualHeight * 0.5));

                switch (connector)
                {
                    case NodeOutputContent outputContent:
                        _DraggingNodeLink = new NodeLink(posOnCanvas.X, posOnCanvas.Y, outputContent);
                        break;
                    case NodeInputContent inputContent:
                        _DraggingNodeLink = new NodeLink(posOnCanvas.X, posOnCanvas.Y, inputContent);
                        break;
                    default:
                        throw new InvalidCastException();
                }

                _DraggingConnectors.Add(connector);
                Canvas.Children.Add(_DraggingNodeLink);
            }
            else
            {
                // clicked on Node
                var pos = e.GetPosition(Canvas);

                var firstNode = e.Source as Node;
                firstNode.CaptureDragStartPosition();

                _DraggingNodes.Add(firstNode);

                _DragStartPoint = e.GetPosition(Canvas);
            }
        }
    }
}
