using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EyeTrackingVsix.Options.Gui
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
