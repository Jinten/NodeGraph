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
        bool IsEnable { get; set; }
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

        public bool IsEnable
        {
            get => _IsEnable;
            set => RaisePropertyChangedIfSet(ref _IsEnable, value);
        }
        bool _IsEnable = true;

        public bool AllowToConnectMultiple
        {
            get => _AllowToConnectMultiple;
            set => RaisePropertyChangedIfSet(ref _AllowToConnectMultiple, value);
        }
        bool _AllowToConnectMultiple = false;

        public NodeInputViewModel(string label, bool allowToConnectMultiple)
        {
            Label = label;
            AllowToConnectMultiple = allowToConnectMultiple;
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

        public bool IsEnable
        {
            get => _IsEnable;
            set => RaisePropertyChangedIfSet(ref _IsEnable, value);
        }
        bool _IsEnable = true;

        public NodeOutputViewModel(string label)
        {
            Label = label;
        }
    }
}
