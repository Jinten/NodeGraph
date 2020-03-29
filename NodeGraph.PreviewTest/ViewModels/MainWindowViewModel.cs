using Livet;
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
        public IEnumerable<INodeViewModel> NodeViewModels => _NodeViewModels;
        ObservableCollection<NodeViewModel> _NodeViewModels = new ObservableCollection<NodeViewModel>();

        public MainWindowViewModel()
        {
            _NodeViewModels.Add(new NodeViewModel());
        }
    }
}
