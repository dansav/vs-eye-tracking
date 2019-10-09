using System.Windows;
using System.Windows.Controls;

namespace EyeTrackingVsix.Options.Gui.Views
{
    /// <summary>
    /// Interaction logic for LinearProfileView.xaml
    /// </summary>
    public partial class LinearProfileView : UserControl
    {
        public static readonly DependencyProperty ParentContextProperty = DependencyProperty.Register(
            "ParentContext", typeof(object), typeof(LinearProfileView), new PropertyMetadata(default(object)));

        public object ParentContext
        {
            get { return (object)GetValue(ParentContextProperty); }
            set { SetValue(ParentContextProperty, value); }
        }

        public LinearProfileView()
        {
            InitializeComponent();
        }
    }
}
