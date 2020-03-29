using Livet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeGraph.ViewModels
{
    public class NodeInputViewModel : ViewModel
    {
        public string Label
        {
            get => _Label;
            set => RaisePropertyChangedIfSet(ref _Label, value);
        }
        string _Label = string.Empty;

        public NodeInputViewModel(string label)
        {
            Label = label;
        }
    }

    public class NodeOutputViewModel : ViewModel
    {
        public string Label
        {
            get => _Label;
            set => RaisePropertyChangedIfSet(ref _Label, value);
        }
        string _Label = string.Empty;

        public NodeOutputViewModel(string label)
        {
            Label = label;
        }
    }

    public class NodeViewModel : ViewModel, INodeViewModel
    {
        public IEnumerable<NodeInputViewModel> Inputs => _Inputs;
        ObservableCollection<NodeInputViewModel> _Inputs = new ObservableCollection<NodeInputViewModel>();

        public IEnumerable<NodeOutputViewModel> Outputs => _Outputs;
        ObservableCollection<NodeOutputViewModel> _Outputs = new ObservableCollection<NodeOutputViewModel>();

        public NodeViewModel()
        {
            _Inputs.Add(new NodeInputViewModel("Input1"));
            _Inputs.Add(new NodeInputViewModel("Input2"));

            _Outputs.Add(new NodeOutputViewModel("Output"));
        }
    }
}
