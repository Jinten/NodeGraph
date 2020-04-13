using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeGraph.PreviewTest.ViewModels
{
    public class NodeLinkViewModel : ViewModel
    {
        public Guid Guid
        {
            get => _Guid;
            set => RaisePropertyChangedIfSet(ref _Guid, value);
        }
        Guid _Guid = Guid.NewGuid();

        public Guid InputGuid
        {
            get => _InputGuid;
            set => RaisePropertyChangedIfSet(ref _InputGuid, value);
        }
        Guid _InputGuid = Guid.NewGuid();

        public Guid OutputGuid
        {
            get => _OutputGuid;
            set => RaisePropertyChangedIfSet(ref _OutputGuid, value);
        }
        Guid _OutputGuid = Guid.NewGuid();

        public Guid InputNodeGuid
        {
            get => _InputNodeGuid;
            set => RaisePropertyChangedIfSet(ref _InputNodeGuid, value);
        }
        Guid _InputNodeGuid = Guid.NewGuid();

        public Guid OutputNodeGuid
        {
            get => _OutputNodeGuid;
            set => RaisePropertyChangedIfSet(ref _OutputNodeGuid, value);
        }
        Guid _OutputNodeGuid = Guid.NewGuid();
    }
}
