using Livet;
using Livet.Commands;
using NodeGraph.NET7.Operation;
using NodeGraph.PreviewTest.NET7.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace NodeGraph.PreviewTest.NET7.ViewModels
{
    public enum GroupIntersectType
    {
        CursorPointVMDefine,
        BoundingBoxVMDefine,
    }

    public enum RangeSelectionMode
    {
        ContainVMDefine,
        IntersectVMDefine,
    }

    public class MainWindowViewModel : ViewModel
    {
        public double Scale
        {
            get => _Scale;
            set => RaisePropertyChangedIfSet(ref _Scale, value);
        }
        double _Scale = 1.0f;

        public ViewModelCommand AddNodeCommand => _AddNodeCommand.Get(AddNode);
        ViewModelCommandHandler _AddNodeCommand = new ViewModelCommandHandler();

        public ViewModelCommand AddGroupNodeCommand => _AddGroupNodeCommand.Get(AddGroupNode);
        ViewModelCommandHandler _AddGroupNodeCommand = new ViewModelCommandHandler();

        public ViewModelCommand RemoveNodesCommand => _RemoveNodesCommand.Get(RemoveNodes);
        ViewModelCommandHandler _RemoveNodesCommand = new ViewModelCommandHandler();

        public ListenerCommand<PreviewConnectLinkOperationEventArgs> PreviewConnectLinkCommand => _PreviewConnectLinkCommand.Get(PreviewConnect);
        ViewModelCommandHandler<PreviewConnectLinkOperationEventArgs> _PreviewConnectLinkCommand = new ViewModelCommandHandler<PreviewConnectLinkOperationEventArgs>();

        public ListenerCommand<ConnectedLinkOperationEventArgs> ConnectedLinkCommand => _ConnectedLinkCommand.Get(Connected);
        ViewModelCommandHandler<ConnectedLinkOperationEventArgs> _ConnectedLinkCommand = new ViewModelCommandHandler<ConnectedLinkOperationEventArgs>();

        public ListenerCommand<DisconnectedLinkOperationEventArgs> DisconnectedLinkCommand => _DisconnectedLinkCommand.Get(Disconnected);
        ViewModelCommandHandler<DisconnectedLinkOperationEventArgs> _DisconnectedLinkCommand = new ViewModelCommandHandler<DisconnectedLinkOperationEventArgs>();

        public ListenerCommand<EndMoveNodesOperationEventArgs> EndMoveNodesCommand => _EndMoveNodesCommand.Get(NodesMoved);
        ViewModelCommandHandler<EndMoveNodesOperationEventArgs> _EndMoveNodesCommand = new ViewModelCommandHandler<EndMoveNodesOperationEventArgs>();

        public ListenerCommand<IList> SelectionChangedCommand => _SelectionChangedCommand.Get(SelectionChanged);
        ViewModelCommandHandler<IList> _SelectionChangedCommand = new ViewModelCommandHandler<IList>();

        public ViewModelCommand AddTestNodeLinkCommand => _AddTestNodeLinkCommand.Get(AddTestNodeLink);
        ViewModelCommandHandler _AddTestNodeLinkCommand = new ViewModelCommandHandler();

        public ViewModelCommand MoveTestNodesCommand => _MoveTestNodesCommand.Get(MoveTestNodes);
        ViewModelCommandHandler _MoveTestNodesCommand = new ViewModelCommandHandler();

        public ViewModelCommand ClearNodesCommand => _ClearNodesCommand.Get(ClearNodes);
        ViewModelCommandHandler _ClearNodesCommand = new ViewModelCommandHandler();

        public ViewModelCommand ClearNodeLinksCommand => _ClearNodeLinksCommand.Get(ClearNodeLinks);
        ViewModelCommandHandler _ClearNodeLinksCommand = new ViewModelCommandHandler();

        public ViewModelCommand MoveGroupNodeCommand => _MoveGroupNodeCommand.Get(MoveGroupNode);
        ViewModelCommandHandler _MoveGroupNodeCommand = new ViewModelCommandHandler();

        public ViewModelCommand ChangeGroupInnerSizeCommand => _ChangeGroupInnerSizeCommand.Get(ChangeGroupInnerSize);
        ViewModelCommandHandler _ChangeGroupInnerSizeCommand = new ViewModelCommandHandler();

        public ViewModelCommand ChangeGroupInnerPositionCommand => _ChangeGroupInnerPositionCommand.Get(ChangeGroupInnerPosition);
        ViewModelCommandHandler _ChangeGroupInnerPositionCommand = new ViewModelCommandHandler();

        public ViewModelCommand ResetScaleCommand => _ResetScaleCommand.Get(ResetScale);
        ViewModelCommandHandler _ResetScaleCommand = new ViewModelCommandHandler();

        public IEnumerable<DefaultNodeViewModel> NodeViewModels => _NodeViewModels;
        ObservableCollection<DefaultNodeViewModel> _NodeViewModels = new ObservableCollection<DefaultNodeViewModel>();

        public IEnumerable<NodeLinkViewModel> NodeLinkViewModels => _NodeLinkViewModels;
        ObservableCollection<NodeLinkViewModel> _NodeLinkViewModels = new ObservableCollection<NodeLinkViewModel>();

        public IEnumerable<GroupNodeViewModel> GroupNodeViewModels => _GroupNodeViewModels;
        ObservableCollection<GroupNodeViewModel> _GroupNodeViewModels = new ObservableCollection<GroupNodeViewModel>();

        public GroupIntersectType[] GroupIntersectTypes { get; } = Enum.GetValues(typeof(GroupIntersectType)).OfType<GroupIntersectType>().ToArray();
        public RangeSelectionMode[] RangeSelectionModes { get; } = Enum.GetValues(typeof(RangeSelectionMode)).OfType<RangeSelectionMode>().ToArray();

        public GroupIntersectType SelectedGroupIntersectType
        {
            get => _SelectedGroupIntersectType;
            set => RaisePropertyChangedIfSet(ref _SelectedGroupIntersectType, value);
        }
        GroupIntersectType _SelectedGroupIntersectType;

        public RangeSelectionMode SelectedRangeSelectionMode
        {
            get => _SelectedRangeSelectionMode;
            set => RaisePropertyChangedIfSet(ref _SelectedRangeSelectionMode, value);
        }
        RangeSelectionMode _SelectedRangeSelectionMode = RangeSelectionMode.ContainVMDefine;

        public bool IsLockedAllNodeLinks
        {
            get => _IsLockedAllNodeLinks;
            set => UpdateIsLockedAllNodeLinksProperty(value);
        }
        bool _IsLockedAllNodeLinks = false;

        public bool IsEnableAllNodeConnectors
        {
            get => _IsEnableAllNodeConnectors;
            set => UpdateIsEnableAllNodeConnectorsProperty(value);
        }
        bool _IsEnableAllNodeConnectors = true;

        public bool AllowToOverrideConnection
        {
            get => _AllowToOverrideConnection;
            set => RaisePropertyChangedIfSet(ref _AllowToOverrideConnection, value);
        }
        bool _AllowToOverrideConnection = true;

        public bool ClipToBounds
        {
            get => _ClipToBounds;
            set => RaisePropertyChangedIfSet(ref _ClipToBounds, value);
        }
        bool _ClipToBounds = true;

        public MainWindowViewModel()
        {
            _GroupNodeViewModels.Add(new GroupNodeViewModel() { Name = "Group1" });
            _NodeViewModels.Add(new Test1DefaultNodeViewModel() { Name = "Node1", Body = "Content1", Position = new Point(0, 100) });
            _NodeViewModels.Add(new Test1DefaultNodeViewModel() { Name = "Node2", Body = "Content2", Position = new Point(100, 200) });
            _NodeViewModels.Add(new Test1DefaultNodeViewModel() { Name = "Node3", Body = "Content3", Position = new Point(200, 300) });
            _NodeViewModels.Add(new Test3DefaultNodeViewModel() { Name = "Node4", Body = "OutputsOnlyNode", Position = new Point(500, 100) });
            _NodeViewModels.Add(new Test4DefaultNodeViewModel() { Name = "Node5", Body = "InputsOnlyNode", Position = new Point(600, 200) });
        }

        void AddNode()
        {
            _NodeViewModels.Add(new Test2DefaultNodeViewModel() { Name = "NewNode", Body = "NewContent" });
        }

        void AddGroupNode()
        {
            _GroupNodeViewModels.Add(new GroupNodeViewModel() { Name = "NewGroupNode" });
        }

        void RemoveNodes()
        {
            var removeNodes = _NodeViewModels.Where(arg => arg.IsSelected).ToArray();
            foreach (var removeNode in removeNodes)
            {
                _NodeViewModels.Remove(removeNode);

                var removeNodeLink = NodeLinkViewModels.FirstOrDefault(arg => arg.InputConnectorNodeGuid == removeNode.Guid || arg.OutputConnectorNodeGuid == removeNode.Guid);
                _NodeLinkViewModels.Remove(removeNodeLink);
            }
        }

        void ClearNodes()
        {
            _NodeLinkViewModels.Clear();
            _NodeViewModels.Clear();
        }

        void ClearNodeLinks()
        {
            _NodeLinkViewModels.Clear();
        }

        void MoveGroupNode()
        {
            _GroupNodeViewModels[0].InterlockPosition = new Point(0, 0);
        }

        void ChangeGroupInnerSize()
        {
            _GroupNodeViewModels[0].InnerWidth = 300;
            _GroupNodeViewModels[0].InnerHeight = 300;
        }

        void ChangeGroupInnerPosition()
        {
            _GroupNodeViewModels[0].InnerPosition = new Point(0, 0);
        }

        void ResetScale()
        {
            Scale = 1.0f;
        }

        void UpdateIsLockedAllNodeLinksProperty(bool value)
        {
            _IsLockedAllNodeLinks = !_IsLockedAllNodeLinks;

            foreach (var nodeLink in _NodeLinkViewModels)
            {
                nodeLink.IsLocked = _IsLockedAllNodeLinks;
            }

            RaisePropertyChanged(nameof(IsLockedAllNodeLinks));
        }

        void UpdateIsEnableAllNodeConnectorsProperty(bool value)
        {
            _IsEnableAllNodeConnectors = !_IsEnableAllNodeConnectors;

            foreach (var node in _NodeViewModels)
            {
                foreach (var input in node.Inputs)
                {
                    input.IsEnable = _IsEnableAllNodeConnectors;
                }
                foreach (var output in node.Outputs)
                {
                    output.IsEnable = _IsEnableAllNodeConnectors;
                }
            }

            RaisePropertyChanged(nameof(IsEnableAllNodeConnectors));
        }

        void PreviewConnect(PreviewConnectLinkOperationEventArgs args)
        {
            var inputNode = NodeViewModels.First(arg => arg.Guid == args.ConnectToEndNodeGuid);
            var inputConnector = inputNode.FindConnector(args.ConnectToEndConnectorGuid);
            args.CanConnect = inputConnector.Label == "Limited Input" == false;
        }

        void Connected(ConnectedLinkOperationEventArgs param)
        {
            var nodeLink = new NodeLinkViewModel()
            {
                OutputConnectorGuid = param.OutputConnectorGuid,
                OutputConnectorNodeGuid = param.OutputConnectorNodeGuid,
                InputConnectorGuid = param.InputConnectorGuid,
                InputConnectorNodeGuid = param.InputConnectorNodeGuid,
                IsLocked = IsLockedAllNodeLinks,
            };
            _NodeLinkViewModels.Add(nodeLink);
        }

        void Disconnected(DisconnectedLinkOperationEventArgs param)
        {
            var nodeLink = _NodeLinkViewModels.First(arg => arg.Guid == param.NodeLinkGuid);
            _NodeLinkViewModels.Remove(nodeLink);
        }

        void NodesMoved(EndMoveNodesOperationEventArgs param)
        {

        }

        void SelectionChanged(IList list)
        {

        }

        void AddTestNodeLink()
        {
            if (_NodeViewModels.Count < 2)
            {
                return;
            }
            var nodeLink = new NodeLinkViewModel();
            nodeLink.OutputConnectorGuid = _NodeViewModels[0].Outputs.ElementAt(0).Guid;
            nodeLink.InputConnectorGuid = _NodeViewModels[1].Inputs.ElementAt(0).Guid;
            _NodeLinkViewModels.Add(nodeLink);
        }

        void MoveTestNodes()
        {
            if (_NodeLinkViewModels.Count > 0)
            {
                _NodeViewModels[0].Position = new Point(0, 0);
            }
        }
    }
}
