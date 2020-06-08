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
    public interface INodeViewModel
    {
        Guid Guid { get; set; }
        Point Position { get; set; }
        bool IsSelected { get; set; }
    }

    public class GroupNodeViewModel : ViewModel, INodeViewModel
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

        public Point Position
        {
            get => _Position;
            set => RaisePropertyChangedIfSet(ref _Position, value, nameof(Comment));
        }
        Point _Position = new Point(0, 0);

        public Point InterlockPosition
        {
            get => _InterlockPosition;
            set => RaisePropertyChangedIfSet(ref _InterlockPosition, value);
        }
        Point _InterlockPosition = new Point(0, 0);

        public Point InnerPosition
        {
            get => _InnerPosition;
            set => RaisePropertyChangedIfSet(ref _InnerPosition, value, nameof(Comment));
        }
        Point _InnerPosition = new Point(0, 0);

        public double InnerWidth
        {
            get => _InnerWidth;
            set => RaisePropertyChangedIfSet(ref _InnerWidth, value, nameof(Comment));
        }
        double _InnerWidth = 100;

        public double InnerHeight
        {
            get => _InnerHeight;
            set => RaisePropertyChangedIfSet(ref _InnerHeight, value, nameof(Comment));
        }
        double _InnerHeight = 100;

        public string Comment
        {
            get => $"InnerWidth = {InnerWidth}, InnerHeight = {InnerHeight},\n Position = {Position}, InnerPosition = {InnerPosition}";
        }

        public bool IsSelected
        {
            get => _IsSelected;
            set => RaisePropertyChangedIfSet(ref _IsSelected, value);
        }
        bool _IsSelected = false;

        public ICommand SizeChangedCommand => _SizeChangedCommand.Get(SizeChanged);
        ViewModelCommandHandler<Size> _SizeChangedCommand = new ViewModelCommandHandler<Size>();

        void SizeChanged(Size newSize)
        {

        }
    }

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

        public abstract IEnumerable<NodeConnectorViewModel> Inputs { get; }
        public abstract IEnumerable<NodeConnectorViewModel> Outputs { get; }

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

        public override IEnumerable<NodeConnectorViewModel> Inputs => _Inputs;
        ObservableCollection<NodeInputViewModel> _Inputs = new ObservableCollection<NodeInputViewModel>();

        public override IEnumerable<NodeConnectorViewModel> Outputs => _Outputs;
        ObservableCollection<NodeOutputViewModel> _Outputs = new ObservableCollection<NodeOutputViewModel>();

        public NodeViewModel1()
        {
            for (int i = 0; i < 4; ++i)
            {
                if (i % 2 == 0)
                {
                    var label = $"Input{i}";
                    if(i > 1)
                    {
                        label += " Allow to connect multiple";
                    }
                    _Inputs.Add(new NodeInputViewModel(label, i > 1));
                }
                else
                {
                    _Inputs.Add(new NodeInputViewModel($"Limited Input", false));
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

        public override IEnumerable<NodeConnectorViewModel> Inputs => _Inputs;
        ObservableCollection<NodeInputViewModel> _Inputs = new ObservableCollection<NodeInputViewModel>();

        public override IEnumerable<NodeConnectorViewModel> Outputs => _Outputs;
        ObservableCollection<NodeOutputViewModel> _Outputs = new ObservableCollection<NodeOutputViewModel>();

        public NodeViewModel2()
        {
            for (int i = 0; i < 5; ++i)
            {
                var label = $"Input{i}";
                if (i > 2)
                {
                    label += " Allow to connect multiple";
                }
                _Inputs.Add(new NodeInputViewModel(label, i > 2));
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
