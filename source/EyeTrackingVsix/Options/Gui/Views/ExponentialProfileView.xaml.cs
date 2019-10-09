using System.Windows;
using System.Windows.Controls;

namespace EyeTrackingVsix.Options.Gui.Views
{
    /// <summary>
    /// Interaction logic for ExponentialProfileView.xaml
    /// </summary>
    public partial class ExponentialProfileView : UserControl
    {
        public static readonly DependencyProperty ParentContextProperty = DependencyProperty.Register(
            "ParentContext", typeof(object), typeof(ExponentialProfileView), new PropertyMetadata(default(object)));

        public object ParentContext
        {
            get { return (object)GetValue(ParentContextProperty); }
            set { SetValue(ParentContextProperty, value); }
        }

        public ExponentialProfileView()
        {
            InitializeComponent();
        }
    }
}
