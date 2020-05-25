using Livet;
using NodeGraph.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NodeGraph.PreviewTest.ViewModels
{
    public abstract class NodeBaseViewModel : ViewModel, INodeViewModel
    {
        public double Width
        {
            get => _Width;
            set => RaisePropertyChangedIfSet(ref _Width, value);
        }
        double _Width = 0;

        public double Height
        {
            get => _Height;
            set => RaisePropertyChangedIfSet(ref _Height, value);
        }
        double _Height = 0;

        public Guid Guid
        {
            get => _Guid;
            set => RaisePropertyChangedIfSet(ref _Guid, value);
        }
        Guid _Guid = Guid.NewGuid();

        public Point Position
        {
            get => _Position;
            set => RaisePropertyChangedIfSet(ref _Position, value);
        }
        Point _Position = new Point(0, 0);

        public bool IsSelected
        {
            get => _IsSelected;
            set => RaisePropertyChangedIfSet(ref _IsSelected, value);
        }
        bool _IsSelected = false;

        public ICommand SizeChangedCommand => _SizeChangedCommand.Get(SizeChanged);
        ViewModelCommandHandler<Size> _SizeChangedCommand = new ViewModelCommandHandler<Size>();

        public abstract NodeConnectorViewModel FindConnector(Guid guid);

        void SizeChanged(Size newSize)
        {
            Width = newSize.Width;
            Height = newSize.Height;
        }
    }

    public class NodeViewModel1 : NodeBaseViewModel
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

        public NodeViewModel1()
        {
            for (int i = 0; i < 3; ++i)
            {
                if(i % 2 == 0)
                {
                    _Inputs.Add(new NodeInputViewModel($"Input{i}"));
                }
                else
                {
                    _Inputs.Add(new NodeInputViewModel($"Limited Input"));
                }
            }

            for (int i = 0; i < 5; ++i)
            {
                _Outputs.Add(new NodeOutputViewModel($"Output{i}"));
            }
        }

        public override NodeConnectorViewModel FindConnector(Guid guid)
        {
            var input = Inputs.FirstOrDefault(arg => arg.Guid == guid);
            if (input != null)
            {
                return input;
            }

            var output = Outputs.FirstOrDefault(arg => arg.Guid == guid);
            return output;
        }
    }

    public class NodeViewModel2 : NodeBaseViewModel
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

            for (int i = 0; i < 2; ++i)
            {
                _Outputs.Add(new NodeOutputViewModel($"Output{i}"));
            }
        }

        public override NodeConnectorViewModel FindConnector(Guid guid)
        {
            var input = Inputs.FirstOrDefault(arg => arg.Guid == guid);
            if (input != null)
            {
                return input;
            }

            var output = Outputs.FirstOrDefault(arg => arg.Guid == guid);
            return output;
        }
    }
}
