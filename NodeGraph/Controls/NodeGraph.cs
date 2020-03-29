using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NodeGraph.Controls
{
    /// <summary>
    /// このカスタム コントロールを XAML ファイルで使用するには、手順 1a または 1b の後、手順 2 に従います。
    ///
    /// 手順 1a) 現在のプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:NodeGraph.Controls"
    ///
    ///
    /// 手順 1b) 異なるプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:NodeGraph.Controls;assembly=NodeGraph.Controls"
    ///
    /// また、XAML ファイルのあるプロジェクトからこのプロジェクトへのプロジェクト参照を追加し、
    /// リビルドして、コンパイル エラーを防ぐ必要があります:
    ///
    ///     ソリューション エクスプローラーで対象のプロジェクトを右クリックし、
    ///     [参照の追加] の [プロジェクト] を選択してから、このプロジェクトを参照し、選択します。
    ///
    ///
    /// 手順 2)
    /// コントロールを XAML ファイルで使用します。
    ///
    ///     <MyNamespace:NodeGraph/>
    ///
    /// </summary>
    public class NodeGraph : MultiSelector
    {
        static NodeGraph()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeGraph), new FrameworkPropertyMetadata(typeof(NodeGraph)));
        }

        ControlTemplate NodeTemplate
        {
            get
            {
                if (_NodeTemplate == null)
                {
                    _NodeTemplate = Application.Current.TryFindResource("__NodeTemplate__") as ControlTemplate;
                }
                return _NodeTemplate;
            }
        }
        ControlTemplate _NodeTemplate = null;

        Canvas _Canvas = null;
        List<object> _DelayToBindVMs = new List<object>();

        public Point GetDragNodePosition(MouseEventArgs e)
        {
            return e.GetPosition(_Canvas);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _Canvas = GetTemplateChild("NodeGraphCanvas") as Canvas;

            if (_DelayToBindVMs.Count > 0)
            {
                AddNodesToCanvas(_DelayToBindVMs.OfType<object>());
                _DelayToBindVMs.Clear();
            }
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            if (_Canvas == null)
            {
                _DelayToBindVMs.AddRange(newValue.OfType<object>());
            }
            else
            {
                AddNodesToCanvas(newValue.OfType<object>());
            }
        }

        void AddNodesToCanvas(IEnumerable<object> addVMs)
        {
            foreach (var vm in addVMs)
            {
                var node = new Node(this) { DataContext = vm, Template = NodeTemplate, Style = ItemContainerStyle };
                _Canvas.Children.Add(node);
            }
        }
    }
}
