using System.Windows;
using System.Windows.Controls;

namespace EyeTrackingVsix.Options.Gui.Views
{
    /// <summary>
    /// Interaction logic for DynamicProfileView.xaml
    /// </summary>
    public partial class DynamicProfileView : UserControl
    {
        public static readonly DependencyProperty ParentContextProperty = DependencyProperty.Register(
            "ParentContext", typeof(object), typeof(DynamicProfileView), new PropertyMetadata(default(object)));

        public object ParentContext
        {
            get { return (object)GetValue(ParentContextProperty); }
            set { SetValue(ParentContextProperty, value); }
        }

        public DynamicProfileView()
        {
            InitializeComponent();
        }
    }
}
