using Livet;
using System;

namespace NodeGraph.PreviewTest.NET6.ViewModels
{
    public class NodeLinkViewModel : ViewModel
    {
        public Guid Guid
        {
            get => _Guid;
            set => RaisePropertyChangedIfSet(ref _Guid, value);
        }
        Guid _Guid = Guid.NewGuid();

        public Guid InputConnectorGuid
        {
            get => _InputConnectorGuid;
            set => RaisePropertyChangedIfSet(ref _InputConnectorGuid, value);
        }
        Guid _InputConnectorGuid = Guid.NewGuid();

        public Guid OutputConnectorGuid
        {
            get => _OutputConnectorGuid;
            set => RaisePropertyChangedIfSet(ref _OutputConnectorGuid, value);
        }
        Guid _OutputConnectorGuid = Guid.NewGuid();

        public Guid InputConnectorNodeGuid
        {
            get => _InputNodeGuid;
            set => RaisePropertyChangedIfSet(ref _InputNodeGuid, value);
        }
        Guid _InputNodeGuid = Guid.NewGuid();

        public Guid OutputConnectorNodeGuid
        {
            get => _OutputNodeGuid;
            set => RaisePropertyChangedIfSet(ref _OutputNodeGuid, value);
        }
        Guid _OutputNodeGuid = Guid.NewGuid();

        public bool IsLocked
        {
            get => _IsLocked;
            set => RaisePropertyChangedIfSet(ref _IsLocked, value);
        }
        bool _IsLocked = false;

        public bool IsSelected
        {
            get => _IsSelected;
            set => RaisePropertyChangedIfSet(ref _IsSelected, value);
        }
        bool _IsSelected = false;
    }
}
