using Livet;
using Livet.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeGraph.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        public ViewModelCommand AddNodeCommand
        {
            get
            {
                if (_AddNodeCommand == null)
                {
                    _AddNodeCommand = new ViewModelCommand(AddNode);
                }
                return _AddNodeCommand;
            }
        }
        ViewModelCommand _AddNodeCommand = null;

        public IEnumerable<INodeViewModel> NodeViewModels => _NodeViewModels;
        ObservableCollection<INodeViewModel> _NodeViewModels = new ObservableCollection<INodeViewModel>();

        public MainWindowViewModel()
        {
            _NodeViewModels.Add(new NodeViewModel() { Name = "Node1", Body = "Content1" });
            _NodeViewModels.Add(new NodeViewModel() { Name = "Node2", Body = "Content2" });
            _NodeViewModels.Add(new NodeViewModel() { Name = "Node3", Body = "Content3" });
        }

        void AddNode()
        {
            _NodeViewModels.Add(new NodeViewModel2() { Name = "NewNode", Body = "NewContent" });
        }
    }
}
