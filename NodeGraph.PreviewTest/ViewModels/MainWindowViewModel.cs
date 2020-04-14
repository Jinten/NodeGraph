using Livet;
using Livet.Commands;
using NodeGraph.CommandParameters;
using NodeGraph.PreviewTest.ViewModels;
using NodeGraph.Utilities;
using System;
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

        public ViewModelCommand RemoveNodesCommand => _RemoveNodesCommand.Get(RemoveNodes);
        ViewModelCommandHandler _RemoveNodesCommand = new ViewModelCommandHandler();

        public ListenerCommand<PreviewConnectCommandParameter> PreviewConnectCommand => _PreviewConnectCommand.Get(PreviewConnect);
        ViewModelCommandHandle<PreviewConnectCommandParameter> _PreviewConnectCommand = new ViewModelCommandHandle<PreviewConnectCommandParameter>();

        public ListenerCommand<ConnectCommandParameter> ConnectCommand => _ConnectCommand.Get(Connect);
        ViewModelCommandHandle<ConnectCommandParameter> _ConnectCommand = new ViewModelCommandHandle<ConnectCommandParameter>();

        public ListenerCommand<DisconnectCommandParameter> DisconnectCommand => _DisconnectCommand.Get(Disconnect);
        ViewModelCommandHandle<DisconnectCommandParameter> _DisconnectCommand = new ViewModelCommandHandle<DisconnectCommandParameter>();

        public ListenerCommand<MovedNodesCommandParameter> MovedNodesCommand => _MovedNodesCommand.Get(MovedNodes);
        ViewModelCommandHandle<MovedNodesCommandParameter> _MovedNodesCommand = new ViewModelCommandHandle<MovedNodesCommandParameter>();

        public ViewModelCommand AddTestNodeLinkCommand => _AddTestNodeLinkCommand.Get(AddTestNodeLink);
        ViewModelCommandHandler _AddTestNodeLinkCommand = new ViewModelCommandHandler();

        public ViewModelCommand MoveTestNodesCommand => _MoveTestNodesCommand.Get(MoveTestNodes);
        ViewModelCommandHandler _MoveTestNodesCommand = new ViewModelCommandHandler();

        public IEnumerable<INodeViewModel> NodeViewModels => _NodeViewModels;
        ObservableCollection<INodeViewModel> _NodeViewModels = new ObservableCollection<INodeViewModel>();

        public IEnumerable<NodeLinkViewModel> NodeLinkViewModels => _NodeLinkViewModels;
        ObservableCollection<NodeLinkViewModel> _NodeLinkViewModels = new ObservableCollection<NodeLinkViewModel>();

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

        void RemoveNodes()
        {
            var removeNodes = _NodeViewModels.Where(arg => arg.IsSelected).ToArray();
            foreach(var removeNode in removeNodes)
            {
                _NodeViewModels.Remove(removeNode);
            }
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
