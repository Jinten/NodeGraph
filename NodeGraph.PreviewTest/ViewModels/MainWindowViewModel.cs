using Livet;
using Livet.Commands;
using NodeGraph.CommandParameters;
using NodeGraph.PreviewTest.ViewModels;
using NodeGraph.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NodeGraph.PreviewTest.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        public ViewModelCommand AddNodeCommand => _AddNodeCommand.Get(AddNode);
        ViewModelCommandHandler _AddNodeCommand = new ViewModelCommandHandler();

        public ViewModelCommand AddGroupNodeCommand => _AddGroupNodeCommand.Get(AddGroupNode);
        ViewModelCommandHandler _AddGroupNodeCommand = new ViewModelCommandHandler();

        public ViewModelCommand RemoveNodesCommand => _RemoveNodesCommand.Get(RemoveNodes);
        ViewModelCommandHandler _RemoveNodesCommand = new ViewModelCommandHandler();

        public ListenerCommand<PreviewConnectCommandParameter> PreviewConnectCommand => _PreviewConnectCommand.Get(PreviewConnect);
        ViewModelCommandHandler<PreviewConnectCommandParameter> _PreviewConnectCommand = new ViewModelCommandHandler<PreviewConnectCommandParameter>();

        public ListenerCommand<ConnectCommandParameter> ConnectCommand => _ConnectCommand.Get(Connect);
        ViewModelCommandHandler<ConnectCommandParameter> _ConnectCommand = new ViewModelCommandHandler<ConnectCommandParameter>();

        public ListenerCommand<DisconnectCommandParameter> DisconnectCommand => _DisconnectCommand.Get(Disconnect);
        ViewModelCommandHandler<DisconnectCommandParameter> _DisconnectCommand = new ViewModelCommandHandler<DisconnectCommandParameter>();

        public ListenerCommand<MovedNodesCommandParameter> MovedNodesCommand => _MovedNodesCommand.Get(MovedNodes);
        ViewModelCommandHandler<MovedNodesCommandParameter> _MovedNodesCommand = new ViewModelCommandHandler<MovedNodesCommandParameter>();

        public ListenerCommand<IList> SelectionChangedCommand => _SelectionChangedCommand.Get(SelectionChanged);
        ViewModelCommandHandler<IList> _SelectionChangedCommand = new ViewModelCommandHandler<IList>();

        public ViewModelCommand AddTestNodeLinkCommand => _AddTestNodeLinkCommand.Get(AddTestNodeLink);
        ViewModelCommandHandler _AddTestNodeLinkCommand = new ViewModelCommandHandler();

        public ViewModelCommand MoveTestNodesCommand => _MoveTestNodesCommand.Get(MoveTestNodes);
        ViewModelCommandHandler _MoveTestNodesCommand = new ViewModelCommandHandler();

        public ViewModelCommand ClearTestNodesCommand => _ClearTestNodesCommand.Get(ClearTestNodes);
        ViewModelCommandHandler _ClearTestNodesCommand = new ViewModelCommandHandler();

        public IEnumerable<NodeBaseViewModel> NodeViewModels => _NodeViewModels;
        ObservableCollection<NodeBaseViewModel> _NodeViewModels = new ObservableCollection<NodeBaseViewModel>();

        public IEnumerable<NodeLinkViewModel> NodeLinkViewModels => _NodeLinkViewModels;
        ObservableCollection<NodeLinkViewModel> _NodeLinkViewModels = new ObservableCollection<NodeLinkViewModel>();

        public IEnumerable<GroupNodeViewModel> GroupNodeViewModels => _GroupNodeViewModels;
        ObservableCollection<GroupNodeViewModel> _GroupNodeViewModels = new ObservableCollection<GroupNodeViewModel>();

        public bool IsEnableAllNodeLinks
        {
            get => _IsEnableAllNodeLinks;
            set => UpdateIsEnableAllNodeLinksProperty(value);
        }
        bool _IsEnableAllNodeLinks = true;

        public bool IsEnableAllNodeConnectors
        {
            get => _IsEnableAllNodeConnectors;
            set => UpdateIsEnableAllNodeConnectorsProperty(value);
        }
        bool _IsEnableAllNodeConnectors = true;


        public MainWindowViewModel()
        {
            _NodeViewModels.Add(new NodeViewModel1() { Name = "Node1", Body = "Content1" });
            _NodeViewModels.Add(new NodeViewModel1() { Name = "Node2", Body = "Content2" });
            //_NodeViewModels.Add(new NodeViewModel() { Name = "Node3", Body = "Content3" });
        }

        void AddNode()
        {
            _NodeViewModels.Add(new NodeViewModel2() { Name = "NewNode", Body = "NewContent" });
        }

        void AddGroupNode()
        {
            _GroupNodeViewModels.Add(new GroupNodeViewModel() { Name = "NewGroupNode" });
        }

        void RemoveNodes()
        {
            var removeNodes = _NodeViewModels.Where(arg => arg.IsSelected).ToArray();
            foreach(var removeNode in removeNodes)
            {
                _NodeViewModels.Remove(removeNode);
            }
        }

        void ClearTestNodes()
        {
            _NodeViewModels.Clear();
        }

        void UpdateIsEnableAllNodeLinksProperty(bool value)
        {
            _IsEnableAllNodeLinks = !_IsEnableAllNodeLinks;

            foreach (var nodeLink in _NodeLinkViewModels)
            {
                nodeLink.IsEnable = _IsEnableAllNodeLinks;
            }

            RaisePropertyChanged(nameof(IsEnableAllNodeLinks));
        }

        void UpdateIsEnableAllNodeConnectorsProperty(bool value)
        {
            _IsEnableAllNodeConnectors = !_IsEnableAllNodeConnectors;

            foreach (var node in _NodeViewModels)
            {
                foreach(var input in node.Inputs)
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

        void PreviewConnect(PreviewConnectCommandParameter param)
        {
            var inputNode = NodeViewModels.First(arg => arg.Guid == param.ConnectToEndNodeGuid);
            var inputConnector = inputNode.FindConnector(param.ConnectToEndConnectorGuid);
            param.CanConnect = inputConnector.Label == "Limited Input" == false;
        }

        void Connect(ConnectCommandParameter param)
        {
            var nodeLink = new NodeLinkViewModel()
            {
                InputGuid = param.InputConnectorGuid,
                InputNodeGuid = param.InputNodeGuid,
                OutputGuid = param.OutputConnectorGuid,
                OutputNodeGuid = param.OutputNodeGuid,
            };
            _NodeLinkViewModels.Add(nodeLink);
        }

        void Disconnect(DisconnectCommandParameter param)
        {
            var nodeLink = _NodeLinkViewModels.First(arg => arg.Guid == param.NodeLinkGuid);
            _NodeLinkViewModels.Remove(nodeLink);
        }

        void MovedNodes(MovedNodesCommandParameter param)
        {

        }

        void SelectionChanged(IList list)
        {

        }

        void AddTestNodeLink()
        {
            var nodeLink = new NodeLinkViewModel();
            nodeLink.OutputGuid = (_NodeViewModels[0] as NodeViewModel1).Outputs.ElementAt(0).Guid;
            nodeLink.InputGuid = (_NodeViewModels[1] as NodeViewModel1).Inputs.ElementAt(0).Guid;
            _NodeLinkViewModels.Add(nodeLink);
        }

        void MoveTestNodes()
        {
            _NodeViewModels[0].Position = new Point(0, 0);
        }
    }
}
