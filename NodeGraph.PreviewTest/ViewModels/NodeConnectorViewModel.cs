using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeGraph.PreviewTest.ViewModels
{
    public interface NodeConnectorViewModel
    {
        Guid Guid { get; set; }
        string Label { get; set; }
    }

    public class NodeInputViewModel : ViewModel, NodeConnectorViewModel
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

    public class NodeOutputViewModel : ViewModel, NodeConnectorViewModel
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
}
