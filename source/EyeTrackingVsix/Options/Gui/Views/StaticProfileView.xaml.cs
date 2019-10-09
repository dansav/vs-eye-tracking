using System.Windows;
using System.Windows.Controls;

namespace EyeTrackingVsix.Options.Gui.Views
{
    /// <summary>
    /// Interaction logic for StaticProfileView.xaml
    /// </summary>
    public partial class StaticProfileView : UserControl
    {
        public static readonly DependencyProperty ParentContextProperty = DependencyProperty.Register(
            "ParentContext", typeof(object), typeof(StaticProfileView), new PropertyMetadata(default(object)));

        public object ParentContext
        {
            get { return (object)GetValue(ParentContextProperty); }
            set { SetValue(ParentContextProperty, value); }
        }

        public StaticProfileView()
        {
            InitializeComponent();
        }
    }
}
