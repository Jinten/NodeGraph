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
        public Guid Guid
        {
            get => _Guid;
            set => RaisePropertyChangedIfSet(ref _Guid, value);
        }
        Guid _Guid = Guid.NewGuid();

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
        public Guid Guid
        {
            get => _Guid;
            set => RaisePropertyChangedIfSet(ref _Guid, value);
        }
        Guid _Guid = Guid.NewGuid();

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
        public Guid Guid
        {
            get => _Guid;
            set => RaisePropertyChangedIfSet(ref _Guid, value);
        }
        Guid _Guid = Guid.NewGuid();

        public string Name
        {
            get => _Name;
            set => RaisePropertyChangedIfSet(ref _Name, value);
        }
        string _Name = string.Empty;

        public string Body
        {
            get => _Body;
            set => RaisePropertyChangedIfSet(ref _Body, value);
        }
        string _Body = string.Empty;

        public IEnumerable<NodeInputViewModel> Inputs => _Inputs;
        ObservableCollection<NodeInputViewModel> _Inputs = new ObservableCollection<NodeInputViewModel>();

        public IEnumerable<NodeOutputViewModel> Outputs => _Outputs;
        ObservableCollection<NodeOutputViewModel> _Outputs = new ObservableCollection<NodeOutputViewModel>();

        public NodeViewModel()
        {
            for (int i = 0; i < 2; ++i)
            {
                _Inputs.Add(new NodeInputViewModel($"Input{i}"));
            }


            _Outputs.Add(new NodeOutputViewModel("Output"));
        }
    }

    public class NodeViewModel2 : ViewModel, INodeViewModel
    {
        public string Name
        {
            get => _Name;
            set => RaisePropertyChangedIfSet(ref _Name, value);
        }
        string _Name = string.Empty;

        public string Body
        {
            get => _Body;
            set => RaisePropertyChangedIfSet(ref _Body, value);
        }
        string _Body = string.Empty;

        public IEnumerable<NodeInputViewModel> Inputs => _Inputs;
        ObservableCollection<NodeInputViewModel> _Inputs = new ObservableCollection<NodeInputViewModel>();

        public IEnumerable<NodeOutputViewModel> Outputs => _Outputs;
        ObservableCollection<NodeOutputViewModel> _Outputs = new ObservableCollection<NodeOutputViewModel>();

        public NodeViewModel2()
        {
            for (int i = 0; i < 5; ++i)
            {
                _Inputs.Add(new NodeInputViewModel($"Input{i}"));
            }


            for(int i=0; i<2;++i)
            {
                _Outputs.Add(new NodeOutputViewModel($"Output{i}"));
            }
        }
    }
}
