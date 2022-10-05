using NodeGraph.NET6.Extensions;
using NodeGraph.NET6.Utilities;
using NodeGraph.NET6.Operation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace NodeGraph.NET6.Controls
{
    public enum GroupIntersectType
    {
        CursorPoint,
        BoundingBox,
    }

    public enum RangeSelectionMode
    {
        Contain,
        Intersect,
    }

    public class NodeGraph : MultiSelector
    {
        public Canvas Canvas { get; private set; } = null;

        public Key MoveWithKey
        {
            get => (Key)GetValue(MoveWithKeyProperty);
            set => SetValue(MoveWithKeyProperty, value);
        }
        public static readonly DependencyProperty MoveWithKeyProperty =
            DependencyProperty.Register(nameof(MoveWithKey), typeof(Key), typeof(NodeGraph), new FrameworkPropertyMetadata(Key.None));

        public MouseButton MoveWithMouse
        {
            get => (MouseButton)GetValue(MoveWithMouseProperty);
            set => SetValue(MoveWithMouseProperty, value);
        }
        public static readonly DependencyProperty MoveWithMouseProperty =
            DependencyProperty.Register(nameof(MoveWithMouse), typeof(MouseButton), typeof(NodeGraph), new FrameworkPropertyMetadata(MouseButton.Middle));

        public Key ScaleWithKey
        {
            get => (Key)GetValue(ScaleWithKeyProperty);
            set => SetValue(ScaleWithKeyProperty, value);
        }
        public static readonly DependencyProperty ScaleWithKeyProperty =
            DependencyProperty.Register(nameof(ScaleWithKey), typeof(Key), typeof(NodeGraph), new FrameworkPropertyMetadata(Key.None));

        public double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register(nameof(Scale), typeof(double), typeof(NodeGraph), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double MinScale
        {
            get => (double)GetValue(MinScaleProperty);
            set => SetValue(MinScaleProperty, value);
        }
        public static readonly DependencyProperty MinScaleProperty =
            DependencyProperty.Register(nameof(MinScale), typeof(double), typeof(NodeGraph), new FrameworkPropertyMetadata(0.1));

        public double ScaleRate
        {
            get => (double)GetValue(ScaleRateProperty);
            set => SetValue(ScaleRateProperty, value);
        }
        public static readonly DependencyProperty ScaleRateProperty =
            DependencyProperty.Register(nameof(ScaleRate), typeof(double), typeof(NodeGraph), new FrameworkPropertyMetadata(0.1));

        public Point Offset
        {
            get => (Point)GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
        }
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register(nameof(Offset), typeof(Point), typeof(NodeGraph), new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OffsetPropertyChanged));

        public ICommand PreviewConnectLinkCommand
        {
            get => (ICommand)GetValue(PreviewConnectLinkCommandProperty);
            set => SetValue(PreviewConnectLinkCommandProperty, value);
        }
        public static readonly DependencyProperty PreviewConnectLinkCommandProperty =
            DependencyProperty.Register(nameof(PreviewConnectLinkCommand), typeof(ICommand), typeof(NodeGraph), new FrameworkPropertyMetadata(null));

        public ICommand ConnectedLinkCommand
        {
            get => (ICommand)GetValue(ConnectedLinkCommandProperty);
            set => SetValue(ConnectedLinkCommandProperty, value);
        }
        public static readonly DependencyProperty ConnectedLinkCommandProperty =
            DependencyProperty.Register(nameof(ConnectedLinkCommand), typeof(ICommand), typeof(NodeGraph), new FrameworkPropertyMetadata(null));

        public ICommand DisconnectedLinkCommand
        {
            get => (ICommand)GetValue(DisconnectedLinkCommandProperty);
            set => SetValue(DisconnectedLinkCommandProperty, value);
        }
        public static readonly DependencyProperty DisconnectedLinkCommandProperty =
            DependencyProperty.Register(nameof(DisconnectedLinkCommand), typeof(ICommand), typeof(NodeGraph), new FrameworkPropertyMetadata(null));

        public ICommand BeginMoveNodesCommand
        {
            get => (ICommand)GetValue(BeginMoveNodesCommandProperty);
            set => SetValue(BeginMoveNodesCommandProperty, value);
        }
        public static readonly DependencyProperty BeginMoveNodesCommandProperty =
            DependencyProperty.Register(nameof(BeginMoveNodesCommand), typeof(ICommand), typeof(NodeGraph), new FrameworkPropertyMetadata(null));

        public ICommand EndMoveNodesCommand
        {
            get => (ICommand)GetValue(EndMoveNodesCommandProperty);
            set => SetValue(EndMoveNodesCommandProperty, value);
        }
        public static readonly DependencyProperty EndMoveNodesCommandProperty =
            DependencyProperty.Register(nameof(EndMoveNodesCommand), typeof(ICommand), typeof(NodeGraph), new FrameworkPropertyMetadata(null));

        public Style NodeLinkStyle
        {
            get => (Style)GetValue(NodeLinkStyleProperty);
            set => SetValue(NodeLinkStyleProperty, value);
        }
        public static readonly DependencyProperty NodeLinkStyleProperty =
            DependencyProperty.Register(nameof(NodeLinkStyle), typeof(Style), typeof(NodeGraph), new FrameworkPropertyMetadata(null, NodeLinkStylePropertyChanged));

        public Style GroupNodeStyle
        {
            get => (Style)GetValue(GroupNodeStyleProperty);
            set => SetValue(GroupNodeStyleProperty, value);
        }
        public static readonly DependencyProperty GroupNodeStyleProperty =
            DependencyProperty.Register(nameof(GroupNodeStyle), typeof(Style), typeof(NodeGraph), new FrameworkPropertyMetadata(null));

        public IEnumerable NodeLinks
        {
            get => (IEnumerable)GetValue(NodeLinksProperty);
            set => SetValue(NodeLinksProperty, value);
        }
        public static readonly DependencyProperty NodeLinksProperty =
            DependencyProperty.Register(nameof(NodeLinks), typeof(IEnumerable), typeof(NodeGraph), new FrameworkPropertyMetadata(null, NodeLinksPropertyChanged));

        public IEnumerable GroupNodes
        {
            get => (IEnumerable)GetValue(GroupNodesProperty);
            set => SetValue(GroupNodesProperty, value);
        }
        public static readonly DependencyProperty GroupNodesProperty =
            DependencyProperty.Register(nameof(GroupNodes), typeof(IEnumerable), typeof(NodeGraph), new FrameworkPropertyMetadata(null, GroupNodesPropertyChanged));

        public GroupIntersectType GroupIntersectType
        {
            get => (GroupIntersectType)GetValue(GroupIntersectTypeProperty);
            set => SetValue(GroupIntersectTypeProperty, value);
        }
        public static readonly DependencyProperty GroupIntersectTypeProperty =
            DependencyProperty.Register(nameof(GroupIntersectType), typeof(GroupIntersectType), typeof(NodeGraph), new FrameworkPropertyMetadata(GroupIntersectType.BoundingBox));

        public bool AllowToOverrideConnection
        {
            get => (bool)GetValue(AllowToOverrideConnectionProperty);
            set => SetValue(AllowToOverrideConnectionProperty, value);
        }
        public static readonly DependencyProperty AllowToOverrideConnectionProperty =
            DependencyProperty.Register(nameof(AllowToOverrideConnection), typeof(bool), typeof(NodeGraph), new FrameworkPropertyMetadata(false, AllowToOverrideConnectionPropertyChanged));

        public RangeSelectionMode RangeSelectionMdoe
        {
            get => (RangeSelectionMode)GetValue(RangeSelectionMdoeProperty);
            set => SetValue(RangeSelectionMdoeProperty, value);
        }
        public static readonly DependencyProperty RangeSelectionMdoeProperty =
            DependencyProperty.Register(nameof(RangeSelectionMdoe), typeof(RangeSelectionMode), typeof(NodeGraph), new FrameworkPropertyMetadata(RangeSelectionMode.Contain));

        public ICommand PreviewDropOnCanvasCommand
        {
            get => (ICommand)GetValue(PreviewDropOnCanvasCommandProperty);
            set => SetValue(PreviewDropOnCanvasCommandProperty, value);
        }
        public static readonly DependencyProperty PreviewDropOnCanvasCommandProperty =
            DependencyProperty.Register(nameof(PreviewDropOnCanvasCommand), typeof(ICommand), typeof(NodeGraph), new FrameworkPropertyMetadata(null));

        public ICommand MouseMoveOnCanvasCommand
        {
            get => (ICommand)GetValue(MouseMoveOnCanvasCommandProperty);
            set => SetValue(MouseMoveOnCanvasCommandProperty, value);
        }
        public static readonly DependencyProperty MouseMoveOnCanvasCommandProperty =
            DependencyProperty.Register(nameof(MouseMoveOnCanvasCommand), typeof(ICommand), typeof(NodeGraph), new FrameworkPropertyMetadata(null));


        ControlTemplate NodeTemplate => _NodeTemplate.Get("__NodeTemplate__");
        ResourceInstance<ControlTemplate> _NodeTemplate = new ResourceInstance<ControlTemplate>();

        ControlTemplate GroupNodeTemplate => _GroupNodeTemplate.Get("__GroupNodeTemplate__");
        ResourceInstance<ControlTemplate> _GroupNodeTemplate = new ResourceInstance<ControlTemplate>();

        Style NodeLinkAnimationStyle => _NodeLinkAnimationStyle.Get("__NodeLinkAnimationStyle__");
        ResourceInstance<Style> _NodeLinkAnimationStyle = new ResourceInstance<Style>();

        bool _IsStartDraggingNode = false;
        bool _PressedKeyToMove = false;
        bool _PressedMouseToMove = false;
        bool _PressedMouseToSelect = false;
        bool _PressedRightBotton = false;
        bool _IsRangeSelecting = false;
        bool _IsSelectionChanging = false;

        const double DEADLENGTH_FOR_DRAGGING_MOVE = 4.0;

        class DraggingNodeLinkParam
        {
            public NodeLink NodeLink { get; } = null;
            public NodeConnectorContent StartConnector { get; } = null;

            public DraggingNodeLinkParam(NodeLink nodeLink, NodeConnectorContent connector)
            {
                NodeLink = nodeLink;
                StartConnector = connector;
            }
        }

        NodeLink _ReconnectGhostNodeLink = null;
        GroupNode _DraggingToResizeGroupNode = null;
        DraggingNodeLinkParam _DraggingNodeLinkParam = null;

        Point _DragStartPointToMoveNode = new Point();
        Point _DragStartPointToMoveOffset = new Point();
        Point _CaptureOffset = new Point();

        Point _DragStartPointToSelect = new Point();

        readonly List<NodeBase> _DraggingNodes = new List<NodeBase>();
        readonly HashSet<NodeConnectorContent> _PreviewedConnectors = new HashSet<NodeConnectorContent>();

        readonly List<object> _DelayToBindNodeVMs = new List<object>();
        readonly List<object> _DelayToBindNodeLinkVMs = new List<object>();
        readonly List<object> _DelayToBindGroupNodeVMs = new List<object>();

        readonly RangeSelector _RangeSelector = new RangeSelector();

        static void OffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var nodeGraph = (NodeGraph)d;

            // need to calculate node position before calculate link absolute position.
            foreach (var obj in nodeGraph.Canvas.Children.OfType<DefaultNode>())
            {
                obj.UpdateOffset(nodeGraph.Offset);
            }

            foreach (var obj in nodeGraph.Canvas.Children.OfType<NodeLink>())
            {
                obj.UpdateOffset(nodeGraph.Offset);
            }

            foreach (var obj in nodeGraph.Canvas.Children.OfType<GroupNode>())
            {
                obj.UpdateOffset(nodeGraph.Offset);
            }
        }

        static void NodeLinkStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                var nodeGraph = (NodeGraph)d;
                var nodeLinkStyle = (Style)e.NewValue;

                // it will throw exception what cannot find name of element in this style scope if specified BasedOn that using RemoveStoryboard with BeginStoryboardName
                // because override style doesn't know Name of BeginStoryboardName.
                // so cannot find to remove element, so need to RegisterName that name and should be remove element here.
                foreach (var trigger in nodeGraph.NodeLinkAnimationStyle.Triggers)
                {
                    foreach (var beginStoryboard in trigger.EnterActions.OfType<BeginStoryboard>())
                    {
                        nodeLinkStyle.RegisterName(beginStoryboard.Name, beginStoryboard);
                    }
                }
                nodeLinkStyle.Seal();
            }
        }

        static void NodeLinksPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var nodeGraph = d as NodeGraph;

            var oldValue = e.OldValue as IEnumerable;
            var newValue = e.NewValue as IEnumerable;
            CollectionPropertyChanged<NodeLink>(
                nodeGraph,
                oldValue,
                newValue,
                nodeGraph.NodeLinkCollectionChanged,
                nodeGraph._DelayToBindNodeLinkVMs,
                nodeGraph.AddNodeLinksToCanvas);
        }

        static void GroupNodesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var nodeGraph = d as NodeGraph;

            var oldValue = e.OldValue as IEnumerable;
            var newValue = e.NewValue as IEnumerable;
            CollectionPropertyChanged<GroupNode>(
                nodeGraph,
                oldValue,
                newValue,
                nodeGraph.GroupNodeCollectionChanged,
                nodeGraph._DelayToBindGroupNodeVMs,
                nodeGraph.AddGroupNodesToCanvas);
        }

        static void CollectionPropertyChanged<T>(
            NodeGraph nodeGraph,
            IEnumerable oldValue,
            IEnumerable newValue,
            NotifyCollectionChangedEventHandler collectionChanged,
            List<object> delayToBindVMs,
            Action<object[]> addElementToCanvas) where T : UIElement, ICanvasObject
        {
            if (oldValue != null && oldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= collectionChanged;
            }
            if (newValue != null && newValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += collectionChanged;
            }

            // below process is node collection changed.
            if (nodeGraph.Canvas == null)
            {
                if (newValue != null && newValue is IEnumerable enumerable)
                {
                    delayToBindVMs.AddRange(enumerable.OfType<object>());
                }
                return;
            }

            // remove old node links
            var removeElements = nodeGraph.Canvas.Children.OfType<T>().ToArray();
            foreach (var removeElement in removeElements)
            {
                removeElement.Dispose();
                nodeGraph.Canvas.Children.Remove(removeElement);
            }

            // add new node links
            if (newValue != null)
            {
                var newEnumerable = newValue as IEnumerable;
                addElementToCanvas(newEnumerable.OfType<object>().ToArray());
            }
        }

        static void AllowToOverrideConnectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NodeInputContent.AllowToOverrideConnection = (bool)e.NewValue;
        }

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

            if (_DelayToBindNodeVMs.Count > 0)
            {
                AddNodesToCanvas(_DelayToBindNodeVMs.OfType<object>().ToArray());
                _DelayToBindNodeVMs.Clear();
            }

            if (_DelayToBindNodeLinkVMs.Count > 0)
            {
                AddNodeLinksToCanvas(_DelayToBindNodeLinkVMs.OfType<object>().ToArray());
                _DelayToBindNodeLinkVMs.Clear();
            }

            if (_DelayToBindGroupNodeVMs.Count > 0)
            {
                AddGroupNodesToCanvas(_DelayToBindGroupNodeVMs.OfType<object>().ToArray());
                _DelayToBindGroupNodeVMs.Clear();
            }
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            CollectionPropertyChanged<DefaultNode>(
                this,
                oldValue,
                newValue,
                NodeCollectionChanged,
                _DelayToBindNodeVMs,
                AddNodesToCanvas);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (MoveWithKey == Key.None || e.Key == MoveWithKey)
            {
                _PressedKeyToMove = true;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            _PressedKeyToMove = false;
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            if (ScaleWithKey == Key.None || (Keyboard.GetKeyStates(ScaleWithKey) & KeyStates.Down) != 0)
            {
                var s = Scale + (e.Delta > 0 ? +ScaleRate : -ScaleRate);
                Scale = Math.Max(MinScale, s);
            }
        }

        protected override void OnPreviewDrop(DragEventArgs e)
        {
            base.OnPreviewDrop(e);

            var posOnCanvas = e.GetPosition(Canvas);

            if (PreviewDropOnCanvasCommand != null)
            {
                var offsetPosOnCanvasX = posOnCanvas.X - Offset.X;
                var offsetPosOnCanvasY = posOnCanvas.Y - Offset.Y;
                var canvasMouseEventArgs = new CanvasMouseEventArgs(new Point(offsetPosOnCanvasX, offsetPosOnCanvasY));

                if (PreviewDropOnCanvasCommand.CanExecute(canvasMouseEventArgs))
                {
                    PreviewDropOnCanvasCommand.Execute(canvasMouseEventArgs);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var posOnCanvas = e.GetPosition(Canvas);

            if (MouseMoveOnCanvasCommand != null)
            {
                var offsetPosOnCanvasX = posOnCanvas.X - Offset.X;
                var offsetPosOnCanvasY = posOnCanvas.Y - Offset.Y;
                var canvasMouseEventArgs = new CanvasMouseEventArgs(new Point(offsetPosOnCanvasX, offsetPosOnCanvasY));

                if (MouseMoveOnCanvasCommand.CanExecute(canvasMouseEventArgs))
                {
                    MouseMoveOnCanvasCommand.Execute(canvasMouseEventArgs);
                }
            }

            if (_DraggingNodes.Count > 0)
            {
                if (_IsStartDraggingNode == false)
                {
                    if (_DraggingNodes[0].IsSelected)
                    {
                        // If dragging node is already selected, selected nodes move with dragging.
                        // In this case, these were selected by Ctrl+Click each nodes.
                        foreach (var node in Canvas.Children.OfType<NodeBase>().Where(arg => arg != _DraggingNodes[0] && arg.IsSelected))
                        {
                            node.CaptureDragStartPosition();
                            _DraggingNodes.Add(node);
                        }

                        var args = new BeginMoveNodesOperationEventArgs(_DraggingNodes.Select(arg => arg.Guid).ToArray());
                        BeginMoveNodesCommand?.Execute(args);
                    }
                    else
                    {
                        // If it is not selected dragging node, need to select first node and unselect already selected the nodes.
                        BeginSelectionChanging();

                        _DraggingNodes[0].IsSelected = true; // select first drag node.
                        UpdateSelectedItems(_DraggingNodes[0]);

                        foreach (var node in Canvas.Children.OfType<NodeBase>())
                        {
                            if (node != _DraggingNodes[0])
                            {
                                node.IsSelected = false;
                                UpdateSelectedItems(node);
                            }
                        }
                        EndSelectionChanging();
                    }

                    var draggingGroupNodes = _DraggingNodes.OfType<GroupNode>().ToArray();
                    var movingNodeTargets = Canvas.Children.OfType<NodeBase>().Where(arg => draggingGroupNodes.Contains(arg) == false).ToArray();
                    foreach (var target in movingNodeTargets)
                    {
                        var target_bb = target.GetBoundingBox();
                        foreach (var groupNodeTarget in draggingGroupNodes)
                        {
                            if (groupNodeTarget.IsInsideCompletely(target_bb))
                            {
                                target.CaptureDragStartPosition();
                                _DraggingNodes.Add(target);
                            }
                        }
                    }

                    _IsStartDraggingNode = true;
                }

                var current = e.GetPosition(Canvas);
                var diff = new Point(current.X - _DragStartPointToMoveNode.X, current.Y - _DragStartPointToMoveNode.Y);

                foreach (var node in _DraggingNodes)
                {
                    double x = node.DragStartPosition.X + diff.X;
                    double y = node.DragStartPosition.Y + diff.Y;
                    node.UpdatePosition(x, y);
                }

                {   // change group node innter color if node inside.
                    var groupNodes = Canvas.Children.OfType<GroupNode>().ToArray();
                    var draggingGroupNodes = _DraggingNodes.OfType<GroupNode>().ToArray();
                    var boundingBoxes = _DraggingNodes.Select(arg => arg.GetBoundingBox()).ToArray();
                    foreach (var groupNode in groupNodes)
                    {
                        if (draggingGroupNodes.Contains(groupNode))
                        {
                            // no detect if it is same dragging group node to the other group node.
                            continue;
                        }

                        bool isInsideNode = false;
                        switch (GroupIntersectType)
                        {
                            case GroupIntersectType.CursorPoint:
                                {
                                    isInsideNode = groupNode.IsInsideCompletely(current.Sub(Offset));
                                }
                                break;
                            case GroupIntersectType.BoundingBox:
                                {
                                    var groupBoundingBox = groupNode.GetInnerBoundingBox();
                                    isInsideNode = boundingBoxes.Any(arg => groupBoundingBox.IntersectsWith(arg));
                                }
                                break;
                            default:
                                throw new InvalidProgramException();
                        }
                        groupNode.ChangeInnerColor(isInsideNode);
                    }
                }
            }
            else if (_PressedMouseToMove && (MoveWithKey == Key.None || _PressedKeyToMove))
            {
                var delta = new Vector(posOnCanvas.X - _DragStartPointToMoveOffset.X, posOnCanvas.Y - _DragStartPointToMoveOffset.Y);
                if (delta.Length > DEADLENGTH_FOR_DRAGGING_MOVE)
                {
                    var x = _CaptureOffset.X + delta.X;
                    var y = _CaptureOffset.Y + delta.Y;
                    Offset = new Point(x, y);

                    Cursor = Cursors.ScrollAll;
                }
            }
            else if (_DraggingNodeLinkParam != null)
            {
                HitTestToPreviewConnector(posOnCanvas, _DraggingNodeLinkParam);

                _DraggingNodeLinkParam.NodeLink.UpdateEdgePoint(posOnCanvas.X, posOnCanvas.Y);
            }
            else if (_PressedMouseToSelect)
            {
                StartRangeSelecting();

                _RangeSelector.RangeRect = new Rect(_DragStartPointToSelect, posOnCanvas);

                BeginSelectionChanging();

                bool anyIntersects = false;
                Rect selectingRangeRect = new Rect(_DragStartPointToSelect.Sub(Offset), posOnCanvas.Sub(Offset));

                foreach (var selectableObject in Canvas.Children.OfType<ISelectableObject>())
                {
                    switch (RangeSelectionMdoe)
                    {
                        case RangeSelectionMode.Contain:
                            selectableObject.IsSelected = selectableObject.Contains(selectingRangeRect);
                            break;
                        case RangeSelectionMode.Intersect:
                            selectableObject.IsSelected = selectableObject.IntersectsWith(selectingRangeRect);
                            break;
                    }

                    anyIntersects |= selectableObject.IsSelected;

                    UpdateSelectedItems(selectableObject);
                }
                _RangeSelector.IsIntersects = anyIntersects;

                EndSelectionChanging();
            }
            else if (_DraggingToResizeGroupNode != null && e.LeftButton == MouseButtonState.Pressed)
            {
                _DraggingToResizeGroupNode.Resize(posOnCanvas.Sub(Offset));
            }

            e.Handled = true;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            _PressedRightBotton = e.RightButton == MouseButtonState.Pressed;

            base.OnMouseDown(e);

            var posOnCanvas = e.GetPosition(Canvas);

            if (e.LeftButton == MouseButtonState.Pressed && (MoveWithKey == Key.None || Keyboard.GetKeyStates(MoveWithKey) != KeyStates.Down))
            {
                // start to select by range rect.
                _PressedMouseToSelect = true;
                _RangeSelector.Reset(posOnCanvas);

                _DragStartPointToSelect = posOnCanvas;
            }

            // it is able to offset only in state of not operating anything.
            if (_DraggingNodes.Count == 0 && _DraggingNodeLinkParam == null && _PressedMouseToSelect == false)
            {
                UpdateMouseMoveState(e);
            }

            if (_PressedMouseToMove)
            {
                _CaptureOffset = Offset;
                _DragStartPointToMoveOffset = posOnCanvas;
            }

            e.Handled = true;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            _PressedRightBotton = false;

            // dragged to offset node graph?
            e.Handled = _PressedMouseToMove ? Offset != _CaptureOffset : false;

            // clear _PressedMouseToMove flag at process.
            UpdateMouseMoveState(e);

            Cursor = Cursors.Arrow;

            _DraggingToResizeGroupNode?.ReleaseToResizeDragging();
            _DraggingToResizeGroupNode = null;

            // not related to select node with left mouse button click.
            if (_PressedMouseToSelect == false || e.LeftButton != MouseButtonState.Released)
            {
                NodeLinkDragged(e.OriginalSource as FrameworkElement);
                return;
            }

            _PressedMouseToSelect = false;

            if (Canvas.Children.OfType<ISelectableObject>().Any(arg => arg.IsSelected) && _IsRangeSelecting == false)
            {
                // click empty area to unselect nodes.
                BeginSelectionChanging();
                foreach (var selectableObject in Canvas.Children.OfType<ISelectableObject>())
                {
                    selectableObject.IsSelected = false;
                    UpdateSelectedItems(selectableObject);
                }
                EndSelectionChanging();
            }

            ClearRangeSelecting();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            Cursor = Cursors.Arrow;

            _PressedMouseToSelect = false;
            _PressedMouseToMove = false;
            _PressedRightBotton = false;
            _IsStartDraggingNode = false;
            _DraggingNodeLinkParam = null;
            _DraggingNodes.Clear();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            Cursor = Cursors.Arrow;

            _PressedMouseToSelect = false;
            _PressedMouseToMove = false;
            _PressedRightBotton = false;
            _IsStartDraggingNode = false;
            if (_DraggingNodeLinkParam != null)
            {
                DisconnectNodeLink(_DraggingNodeLinkParam.NodeLink);
                if (_ReconnectGhostNodeLink != null)
                {
                    _ReconnectGhostNodeLink.Dispose();
                    Canvas.Children.Remove(_ReconnectGhostNodeLink);
                    _ReconnectGhostNodeLink = null;
                }
            }
            _DraggingNodeLinkParam = null;
            _DraggingNodes.Clear();

            ClearRangeSelecting();

            InvalidateVisual();
        }

        void UpdateMouseMoveState(MouseButtonEventArgs e)
        {
            switch (MoveWithMouse)
            {
                case MouseButton.Left:
                    if (MoveWithKey == Key.None)
                    {
                        throw new InvalidProgramException("Not allow combination that no set MoveWithKey(Key.None) and left click.");
                    }

                    if (Keyboard.GetKeyStates(MoveWithKey) == KeyStates.Down)
                    {
                        // offset center focus.
                        _PressedMouseToMove = e.LeftButton == MouseButtonState.Pressed;
                    }
                    _PressedMouseToMove = e.LeftButton == MouseButtonState.Pressed;
                    break;
                case MouseButton.Middle:
                    _PressedMouseToMove = e.MiddleButton == MouseButtonState.Pressed;
                    break;
                case MouseButton.Right:
                    _PressedMouseToMove = e.RightButton == MouseButtonState.Pressed;
                    break;
            }
        }

        void DisconnectNodeLink(NodeLink nodeLink)
        {
            if (nodeLink.Input != null && nodeLink.Output != null)
            {
                var inputNode = nodeLink.Input.Node;
                var outputNode = nodeLink.Output.Node;
                var args = new DisconnectedLinkOperationEventArgs(nodeLink.Guid, nodeLink.OutputConnectorGuid, outputNode.Guid, nodeLink.InputConnectorGuid, inputNode.Guid);
                DisconnectedLinkCommand?.Execute(args);
            }

            nodeLink.Dispose();
            Canvas.Children.Remove(nodeLink);
        }

        void ClearPreviewedConnect()
        {
            foreach (var connector in _PreviewedConnectors)
            {
                connector.CanConnect = true;
            }
            _PreviewedConnectors.Clear();
        }

        void HitTestToPreviewConnector(Point pos, DraggingNodeLinkParam param)
        {
            foreach (var connector in _PreviewedConnectors)
            {
                connector.CanConnect = true;
            }
            _PreviewedConnectors.Clear();

            Cursor = Cursors.Cross;

            VisualTreeHelper.HitTest(Canvas, null, new HitTestResultCallback(arg =>
            {
                var element = arg.VisualHit as FrameworkElement;
                if (element != null && element.Tag is NodeConnectorContent toConnector)
                {
                    if (param.StartConnector != toConnector && param.NodeLink.ToEndConnector != toConnector)
                    {
                        PreviewConnect(param.StartConnector, toConnector);
                        _PreviewedConnectors.Add(toConnector);

                        return HitTestResultBehavior.Stop;
                    }
                }
                return HitTestResultBehavior.Continue;
            }), new PointHitTestParameters(pos));
        }

        void NodeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged<DefaultNode>(e.Action, e.OldItems, e.NewItems, RemoveNodesFromCanvas, RemoveNodesFromCanvas, AddNodesToCanvas);
        }

        void GroupNodeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged<GroupNode>(e.Action, e.OldItems, e.NewItems, RemoveGroupNodesFromCanvas, RemoveGroupNodesFromCanvas, AddGroupNodesToCanvas);
        }

        void NodeLinkCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged<NodeLink>(e.Action, e.OldItems, e.NewItems, RemoveNodeLinksFromCanvas, RemoveNodeLinksFromCanvas, AddNodeLinksToCanvas);
        }

        void CollectionChanged<T>(
            NotifyCollectionChangedAction action,
            IList oldItems,
            IList newItems,
            Action<object[]> removeItemWithDataContext,
            Action<T[]> removeItemDirectly,
            Action<object[]> addItemWithDataContext) where T : UIElement, ICanvasObject
        {
            switch (action)
            {
                case NotifyCollectionChangedAction.Reset:
                    if (Canvas != null)
                    {
                        removeItemDirectly(Canvas.Children.OfType<T>().ToArray());
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    // ignore.
                    break;
                default:
                    if (oldItems?.Count > 0)
                    {
                        removeItemWithDataContext(oldItems.OfType<object>().ToArray());
                    }
                    if (newItems?.Count > 0)
                    {
                        addItemWithDataContext(newItems.OfType<object>().ToArray());
                    }
                    break;
            }
        }

        void RemoveNodeLinksFromCanvas(object[] removeVMs)
        {
            var removeNodeLinks = new List<NodeLink>();
            var children = Canvas.Children.OfType<NodeLink>().ToArray();

            foreach (var removeVM in removeVMs)
            {
                var removeNodeLink = children.First(arg => arg.DataContext == removeVM);
                removeNodeLinks.Add(removeNodeLink);
            }

            RemoveNodeLinksFromCanvas(removeNodeLinks.ToArray());
        }

        void RemoveNodeLinksFromCanvas(NodeLink[] removeNodeLinks)
        {
            foreach (var removeNodeLink in removeNodeLinks)
            {
                // add node link.
                removeNodeLink.MouseDown -= NodeLink_MouseDown;
                removeNodeLink.MouseUp -= NodeLink_MouseUp;
                removeNodeLink.Dispose();

                Canvas.Children.Remove(removeNodeLink);
            }
        }

        void AddNodeLinksToCanvas(object[] addVMs)
        {
            if (Canvas == null)
            {
                _DelayToBindNodeLinkVMs.AddRange(addVMs);
                return;
            }

            foreach (var vm in addVMs)
            {
                var nodeLink = new NodeLink(Canvas, Scale, Offset)
                {
                    DataContext = vm,
                };

                nodeLink.Style = NodeLinkStyle ?? NodeLinkAnimationStyle;

                nodeLink.Validate();

                var nodeLinks = Canvas.Children.OfType<NodeLink>().ToArray();
                if (nodeLinks.Any(arg => arg.Guid == nodeLink.Guid))
                {
                    throw new InvalidOperationException($"Already exists adding node link. Guid = {nodeLink.Guid}");
                }

                var nodes = Canvas.Children.OfType<DefaultNode>().ToArray();
                var nodeInput = FindConnectorContentInNodes<NodeInputContent>(nodes, nodeLink.InputConnectorGuid);
                var nodeOutput = FindConnectorContentInNodes<NodeOutputContent>(nodes, nodeLink.OutputConnectorGuid);

                nodeInput.Connect(nodeLink);
                nodeOutput.Connect(nodeLink);

                nodeLink.Connect(nodeInput, nodeOutput);
                nodeLink.MouseDown += NodeLink_MouseDown;
                nodeLink.MouseUp += NodeLink_MouseUp;

                Canvas.Children.Add(nodeLink);
            }
        }

        void RemoveNodesFromCanvas(object[] removeVMs)
        {
            var removeNodes = new List<DefaultNode>();
            var children = Canvas.Children.OfType<DefaultNode>().ToArray();

            foreach (var removeVM in removeVMs)
            {
                var removeElement = children.First(arg => arg.DataContext == removeVM);
                removeNodes.Add(removeElement);
            }

            RemoveNodesFromCanvas(removeNodes.ToArray());
        }

        void RemoveNodesFromCanvas(DefaultNode[] removeNodes)
        {
            foreach (var removeNode in removeNodes)
            {
                UnsubscribeNodeEvent(removeNode);

                removeNode.Dispose();

                Canvas.Children.Remove(removeNode);
            }
        }

        void AddNodesToCanvas(object[] addVMs)
        {
            if (Canvas == null)
            {
                _DelayToBindNodeVMs.AddRange(addVMs);
                return;
            }

            foreach (var vm in addVMs)
            {
                var node = new DefaultNode(Canvas, Offset, Scale);
                node.DataContext = vm;
                node.Template = NodeTemplate;
                node.ApplyTemplate();

                node.Style = GetNodeStyle(vm, node);
                node.Initialize();

                SubscribeNodeEvent(node);

                Canvas.Children.Add(node);

                // recalculate header size, position.
                node.UpdateLayout();
            }
        }

        void RemoveGroupNodesFromCanvas(object[] removeVMs)
        {
            var removeGroupNodes = new List<GroupNode>();
            var children = Canvas.Children.OfType<GroupNode>().ToArray();

            foreach (var removeVM in removeVMs)
            {
                var removeElement = children.First(arg => arg.DataContext == removeVM);
                removeGroupNodes.Add(removeElement);
            }

            RemoveGroupNodesFromCanvas(removeGroupNodes.ToArray());
        }

        void RemoveGroupNodesFromCanvas(GroupNode[] removeGroupNodes)
        {
            foreach (var removeGroupNode in removeGroupNodes)
            {
                UnsubscribeNodeEvent(removeGroupNode);

                removeGroupNode.Dispose();

                Canvas.Children.Remove(removeGroupNode);
            }
        }

        void AddGroupNodesToCanvas(object[] addVMs)
        {
            if (Canvas == null)
            {
                _DelayToBindGroupNodeVMs.AddRange(addVMs);
                return;
            }

            foreach (var vm in addVMs)
            {
                var groupNode = new GroupNode(Canvas, Offset, Scale);
                groupNode.DataContext = vm;
                groupNode.Template = GroupNodeTemplate;
                groupNode.ApplyTemplate();

                groupNode.Style = GroupNodeStyle;
                groupNode.Initialize();

                SubscribeNodeEvent(groupNode);

                Canvas.Children.Add(groupNode);

                // recalculate header size, position, inner size and inner position.
                groupNode.UpdateLayout();
            }
        }

        T FindConnectorContentInNodes<T>(DefaultNode[] nodes, Guid guid) where T : NodeConnectorContent
        {
            foreach (var node in nodes)
            {
                var connectorContent = node.FindNodeConnectorContent(guid);
                if (connectorContent != null)
                {
                    return (T)connectorContent;
                }
            }

            return null;
        }

        Style GetNodeStyle(object dataContext, DependencyObject element)
        {
            if (ItemContainerStyleSelector != null)
            {
                return ItemContainerStyleSelector.SelectStyle(dataContext, element);
            }

            return ItemContainerStyle;
        }

        void PreviewConnect(NodeConnectorContent start, NodeConnectorContent toEnd)
        {
            if (start.CanConnectTo(toEnd) && toEnd.CanConnectTo(start))
            {
                var args = new PreviewConnectLinkOperationEventArgs(start.Node.Guid, start.Guid, toEnd.Node.Guid, toEnd.Guid);
                PreviewConnectLinkCommand?.Execute(args);
                toEnd.CanConnect = args.CanConnect;
            }
            else
            {
                // connecting to same connect in node connector.
                toEnd.CanConnect = false;
            }

            Cursor = toEnd.CanConnect ? Cursors.Cross : Cursors.No;
        }

        void SubscribeNodeEvent(NodeBase node)
        {
            node.MouseDown += Node_MouseDown;
            node.MouseUp += Node_MouseUp;
            node.BeginSelectionChanged += Node_BeginSelectionChanged;
            node.EndSelectionChanged += Node_EndSelectionChanged;
        }

        void UnsubscribeNodeEvent(NodeBase node)
        {
            node.EndSelectionChanged -= Node_EndSelectionChanged;
            node.BeginSelectionChanged -= Node_BeginSelectionChanged;
            node.MouseUp -= Node_MouseUp;
            node.MouseDown -= Node_MouseDown;
        }

        void NodeLink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((Keyboard.GetKeyStates(Key.LeftCtrl) & KeyStates.Down) != 0)
            {
                // it will select as multiple.
                return;
            }

            _PressedRightBotton = e.RightButton == MouseButtonState.Pressed;

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                e.Handled = true;
                return;
            }

            var nodeLink = (NodeLink)sender;
            if (nodeLink.IsLocked)
            {
                e.Handled = true;
                return;
            }
            _DraggingNodeLinkParam = new DraggingNodeLinkParam(nodeLink, nodeLink.StartConnector);

            _ReconnectGhostNodeLink = nodeLink.CreateGhost();
            _ReconnectGhostNodeLink.Style = NodeLinkStyle ?? NodeLinkAnimationStyle;

            Canvas.Children.Add(_ReconnectGhostNodeLink);

            Cursor = Cursors.Cross;

            nodeLink.ReleaseEndPoint();

            e.Handled = true;
        }

        void NodeLink_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if ((Keyboard.GetKeyStates(Key.LeftCtrl) & KeyStates.Down) == 0 || _IsStartDraggingNode || _IsRangeSelecting)
            {
                // nothing process here if not key down ctrl or other conditions.
                return;
            }

            UpdateNodeSelectionItem((NodeLink)sender);
        }

        void Node_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _PressedMouseToSelect = false;

            if (_PressedRightBotton)
            {
                _PressedRightBotton = false;
                return;
            }

            Cursor = Cursors.Arrow;

            _DraggingToResizeGroupNode?.ReleaseToResizeDragging();
            _DraggingToResizeGroupNode = null;

            if (_IsStartDraggingNode || _IsRangeSelecting)
            {
                _IsStartDraggingNode = false;

                ClearRangeSelecting();

                // mouse cursor must be on the node when dragged nodes.
                NodesDragged(e);

                // clear intersects with group node color.
                foreach (var groupNode in Canvas.Children.OfType<GroupNode>())
                {
                    groupNode.ChangeInnerColor(false);
                }

                e.Handled = true;
                return;
            }

            _DraggingNodes.Clear();

            NodeLinkDragged((FrameworkElement)e.OriginalSource);

            UpdateNodeSelectionItem((NodeBase)sender);

            e.Handled = true;
        }

        void Node_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _PressedRightBotton = e.RightButton == MouseButtonState.Pressed;

            if (sender is GroupNode groupNode)
            {
                groupNode.CaptureToResizeDragging();

                if (groupNode.IsDraggingToResize)
                {
                    _DraggingToResizeGroupNode = groupNode;
                    e.Handled = true;

                    return;
                }
            }

            var originalSender = e.OriginalSource as FrameworkElement;
            if (originalSender?.Tag is NodeConnectorContent connector)
            {
                // clicked on the connector
                var posOnCanvas = connector.GetContentPosition(Canvas);

                var nodeLink = new NodeLink(connector, posOnCanvas.X, posOnCanvas.Y, Canvas, Scale, Offset)
                {
                    DataContext = null,
                };

                nodeLink.Style = NodeLinkStyle ?? NodeLinkAnimationStyle;

                _DraggingNodeLinkParam = new DraggingNodeLinkParam(nodeLink, connector);
                Canvas.Children.Add(_DraggingNodeLinkParam.NodeLink);

                Cursor = Cursors.Cross;
            }
            else if (IsOffsetMoveWithMouse(e) == false)
            {
                // clicked on the Node
                StartToMoveDraggingNode(e.GetPosition(Canvas), (NodeBase)e.Source);
            }

            e.Handled = true;
        }

        void Node_BeginSelectionChanged(object sender, EventArgs e)
        {
            BeginSelectionChanging();
        }

        void Node_EndSelectionChanged(object sender, EventArgs e)
        {
            UpdateSelectedItems((NodeBase)sender);

            EndSelectionChanging();
        }

        void BeginSelectionChanging()
        {
            if (_IsSelectionChanging == false)
            {
                _IsSelectionChanging = true;
                BeginUpdateSelectedItems();
            }
        }

        void EndSelectionChanging()
        {
            if (_IsSelectionChanging)
            {
                EndUpdateSelectedItems();
                _IsSelectionChanging = false;
            }
        }

        void UpdateNodeSelectionItem<T>(T item) where T : ISelectableObject
        {
            BeginSelectionChanging();

            if ((Keyboard.GetKeyStates(Key.LeftCtrl) & KeyStates.Down) != 0)
            {
                item.IsSelected = !item.IsSelected;
                UpdateSelectedItems(item);
            }
            else
            {
                // exclusive selection.
                var selectableContents = Canvas.Children.OfType<ISelectableObject>();

                foreach (var selectableContent in selectableContents)
                {
                    selectableContent.IsSelected = false;
                }

                item.IsSelected = true;
                UpdateSelectedItems(item);
            }

            EndSelectionChanging();
        }

        void StartToMoveDraggingNode(Point pos, NodeBase node)
        {
            node.CaptureDragStartPosition();

            _DraggingNodes.Add(node);

            var args = new BeginMoveNodesOperationEventArgs(_DraggingNodes.Select(arg => arg.Guid).ToArray());
            BeginMoveNodesCommand?.Execute(args);

            _DragStartPointToMoveNode = pos;

            Cursor = Cursors.SizeAll;
        }

        bool IsOffsetMoveWithMouse(MouseButtonEventArgs e)
        {
            switch (MoveWithMouse)
            {
                case MouseButton.Left:
                    return e.LeftButton == MouseButtonState.Pressed;
                case MouseButton.Middle:
                    return e.MiddleButton == MouseButtonState.Pressed;
                case MouseButton.Right:
                    return e.RightButton == MouseButtonState.Pressed;
                default:
                    throw new InvalidCastException();
            }
        }

        void UpdateSelectedItems<T>(T item) where T : ISelectableObject
        {
            if (SelectedItems.Contains(item.DataContext))
            {
                if (item.IsSelected == false)
                {
                    SelectedItems.Remove(item.DataContext);
                }
            }
            else
            {
                if (item.IsSelected)
                {
                    SelectedItems.Add(item.DataContext);
                }
            }
        }

        void NodesDragged(MouseButtonEventArgs e)
        {
            if (_DraggingNodes.Count == 0)
            {
                return;
            }
            var args = new EndMoveNodesOperationEventArgs(_DraggingNodes.Select(arg => arg.Guid).ToArray());
            EndMoveNodesCommand?.Execute(args);

            // expand the group node area size if node within group.
            var groupNodes = Canvas.Children.OfType<GroupNode>().Where(arg => _DraggingNodes.Contains(arg) == false).ToArray();

            var isInsideAtLeastOneNode = false;
            switch (GroupIntersectType)
            {
                case GroupIntersectType.CursorPoint:
                    {
                        var cursor_bb = new Rect(e.GetPosition(Canvas).Sub(Offset), new Size(1, 1));
                        isInsideAtLeastOneNode = groupNodes.Any(arg => cursor_bb.IntersectsWith(arg.GetBoundingBox()));
                        break;
                    }
                case GroupIntersectType.BoundingBox:
                    {
                        var groupBoundingBoxes = groupNodes.Select(arg => arg.GetInnerBoundingBox()).ToArray();
                        isInsideAtLeastOneNode = _DraggingNodes.Any(arg => groupBoundingBoxes.Any(bb => bb.IntersectsWith(arg.GetBoundingBox())));
                        break;
                    }
            }

            if (isInsideAtLeastOneNode)
            {
                var min_x = _DraggingNodes.Min(arg => arg.Position.X);
                var min_y = _DraggingNodes.Min(arg => arg.Position.Y);
                var max_x = _DraggingNodes.Max(arg => arg.ActualWidth + arg.Position.X);
                var max_y = _DraggingNodes.Max(arg => arg.ActualHeight + arg.Position.Y);
                var rect = new Rect(min_x, min_y, max_x - min_x, max_y - min_y);

                // collect target of expand groups.
                var targetGroupNodes = groupNodes.Where(arg => rect.IntersectsWith(arg.GetInnerBoundingBox())).ToArray();
                foreach (var targetGroupNode in targetGroupNodes)
                {
                    targetGroupNode.ExpandSize(rect);
                }
            }
            _DraggingNodes.Clear();
        }
        void StartRangeSelecting()
        {
            if (_IsRangeSelecting)
            {
                return;
            }

            Canvas.Children.Add(_RangeSelector);

            _IsRangeSelecting = true;

            // links dont need to trigger link animation.
            // cannot control link animation disable when range selecting and IsSelected condition.
            // because it must action with condtion as IsMouseOver=truea and IsSelected=false.(Probably only first time)
            // So disable IsHitTestVisible when range selecting. it is better performance.
            foreach (var link in Canvas.Children.OfType<NodeLink>())
            {
                link.IsHitTestVisible = false;
            }

            Cursor = Cursors.SizeAll;
        }

        void ClearRangeSelecting()
        {
            if (_IsRangeSelecting == false)
            {
                return;
            }

            Canvas.Children.Remove(_RangeSelector);
            _IsRangeSelecting = false;

            // recover to be able to action link animation from range selecting.
            foreach (var link in Canvas.Children.OfType<NodeLink>())
            {
                link.IsHitTestVisible = true;
            }
        }

        void NodeLinkDragged(FrameworkElement element)
        {
            ClearPreviewedConnect();

            // clicked connect or not.
            if (_DraggingNodeLinkParam == null)
            {
                return;
            }

            bool connected = false;

            // only be able to connect input to output or output to input.
            // it will reject except above condition.
            if (element != null && element.Tag is NodeConnectorContent toEndConnector)
            {
                var nodeLink = _DraggingNodeLinkParam.NodeLink;
                var startConnector = _DraggingNodeLinkParam.StartConnector;

                if (nodeLink.ToEndConnector == toEndConnector)
                {
                    // tried to reconnect but reconnecting same with before connector.
                    _DraggingNodeLinkParam = null;
                    nodeLink.RestoreEndPoint();

                    connected = true;
                }
                else if (NodeConnectorContent.CanConnectEachOther(startConnector, toEndConnector))
                {
                    NodeInputContent input = null;
                    NodeOutputContent output = null;

                    switch (toEndConnector)
                    {
                        case NodeInputContent inputConnector:
                            {
                                input = inputConnector;
                                output = (NodeOutputContent)_DraggingNodeLinkParam.StartConnector;

                                if (AllowToOverrideConnection && input.ConnectedCount > 0)
                                {
                                    // it has to disconnect previous connected node link.
                                    var nodeLinks = Canvas.Children.OfType<NodeLink>().ToArray();
                                    var disconnectNodeLink = nodeLinks.First(arg => arg.InputConnectorGuid == input.Guid);

                                    // but cannot disconnect if node link is locked.
                                    if (disconnectNodeLink.IsLocked)
                                    {
                                        ClearPreviewingNodeLink();
                                        ClearConnectingNodeLinkState();
                                        return;
                                    }
                                    DisconnectNodeLink(disconnectNodeLink);
                                }
                                break;
                            }
                        case NodeOutputContent outputConnector:
                            {
                                output = outputConnector;
                                input = (NodeInputContent)_DraggingNodeLinkParam.StartConnector;
                                break;
                            }
                        default:
                            throw new InvalidCastException();
                    }

                    ClearPreviewingNodeLink();

                    var args = new ConnectedLinkOperationEventArgs(output.Guid, output.Node.Guid, input.Guid, input.Node.Guid);
                    ConnectedLinkCommand?.Execute(args);

                    connected = true;
                }
            }

            if (connected == false)
            {
                DisconnectNodeLink(_DraggingNodeLinkParam.NodeLink);
            }

            ClearConnectingNodeLinkState();
        }

        void ClearPreviewingNodeLink()
        {
            // reconnect or not.
            if (_DraggingNodeLinkParam.NodeLink.IsConnecting)
            {
                // reconnecting node link was already connected, so need to dispatch disconnect command.
                DisconnectNodeLink(_DraggingNodeLinkParam.NodeLink);
            }
            else
            {
                // preview node link. (new connecting)
                _DraggingNodeLinkParam.NodeLink.Dispose();
                Canvas.Children.Remove(_DraggingNodeLinkParam.NodeLink);
            }
        }

        void ClearConnectingNodeLinkState()
        {
            if (_ReconnectGhostNodeLink != null)
            {
                _ReconnectGhostNodeLink.Dispose();
                Canvas.Children.Remove(_ReconnectGhostNodeLink);
                _ReconnectGhostNodeLink = null;
            }

            _DraggingNodeLinkParam = null;
        }
    }
}
