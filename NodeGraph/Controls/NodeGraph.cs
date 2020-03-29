using System;
using System.Collections;
using System.Collections.Generic;
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
    /// <summary>
    /// このカスタム コントロールを XAML ファイルで使用するには、手順 1a または 1b の後、手順 2 に従います。
    ///
    /// 手順 1a) 現在のプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:NodeGraph.Controls"
    ///
    ///
    /// 手順 1b) 異なるプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:NodeGraph.Controls;assembly=NodeGraph.Controls"
    ///
    /// また、XAML ファイルのあるプロジェクトからこのプロジェクトへのプロジェクト参照を追加し、
    /// リビルドして、コンパイル エラーを防ぐ必要があります:
    ///
    ///     ソリューション エクスプローラーで対象のプロジェクトを右クリックし、
    ///     [参照の追加] の [プロジェクト] を選択してから、このプロジェクトを参照し、選択します。
    ///
    ///
    /// 手順 2)
    /// コントロールを XAML ファイルで使用します。
    ///
    ///     <MyNamespace:NodeGraph/>
    ///
    /// </summary>
    public class NodeGraph : MultiSelector
    {
        public Canvas Canvas { get; private set; } = null;

        ControlTemplate NodeTemplate
        {
            get
            {
                if (_NodeTemplate == null)
                {
                    _NodeTemplate = Application.Current.TryFindResource("__NodeTemplate__") as ControlTemplate;
                }
                return _NodeTemplate;
            }
        }
        ControlTemplate _NodeTemplate = null;

        Style NodeBaseStyle
        {
            get
            {
                if (_NodeBaseStyle == null)
                {
                    _NodeBaseStyle = Application.Current.TryFindResource("__NodeBaseStyle__") as Style;
                }
                return _NodeBaseStyle;
            }
        }
        Style _NodeBaseStyle = null;

        bool _IsNodeSelected = false;
        bool _IsStartDragging = false;

        NodeLink _DraggingNodeLink = null;
        List<Node> _DraggingNodes = new List<Node>();
        List<NodeOutputContent> _DraggingOutputs = new List<NodeOutputContent>();
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
            if (_DraggingNodeLink != null)
            {
                if (element != null && element.Tag is NodeInputContent inputContent)
                {
                    var transformer = element.TransformToVisual(Canvas);
                    var posOnCanvas = transformer.Transform(new Point(element.ActualWidth * 0.5, element.ActualHeight * 0.5));

                    _DraggingNodeLink.Connect(inputContent);
                    _DraggingNodeLink.EndPoint = new Point(posOnCanvas.X, posOnCanvas.Y);

                    inputContent.Connect(_DraggingNodeLink);
                    _DraggingOutputs.ForEach(arg => arg.Connect(_DraggingNodeLink));

                    _DraggingNodeLink.MouseDown += NodeLink_MouseDown;
                }
                else
                {
                    Canvas.Children.Remove(_DraggingNodeLink);
                    _DraggingNodeLink.Dispose();
                }

                _DraggingNodeLink = null;
                _DraggingOutputs.Clear();
            }

            if (_RemoveNodeLink != null && _RemoveNodeLink != element)
            {
                Canvas.Children.Remove(_RemoveNodeLink);

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
            _DraggingOutputs.Clear();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            _IsStartDragging = false;
            _DraggingNodes.Clear();
            _DraggingOutputs.Clear();
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

            // clicked connect or not.
            if (element != null && element.Tag is NodeOutputContent outputContent)
            {
                var transformer = element.TransformToVisual(Canvas);
                var posOnCanvas = transformer.Transform(new Point(element.ActualWidth * 0.5, element.ActualHeight * 0.5));
                _DraggingNodeLink = new NodeLink(posOnCanvas.X, posOnCanvas.Y, outputContent);

                _DraggingOutputs.Add(outputContent);
                Canvas.Children.Add(_DraggingNodeLink);
                return;
            }

            var pos = e.GetPosition(Canvas);

            var firstNode = e.Source as Node;
            firstNode.CaptureDragStartPosition();

            _DraggingNodes.Add(firstNode);

            _DragStartPoint = e.GetPosition(Canvas);
        }
    }
}
