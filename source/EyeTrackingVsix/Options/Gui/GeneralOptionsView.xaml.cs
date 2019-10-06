using System.Windows.Controls;

namespace EyeTrackingVsix.Options.Gui
{
    /// <summary>
    /// Interaction logic for GeneralOptionsView.xaml
    /// </summary>
    public partial class GeneralOptionsView : UserControl
    {
        internal GeneralOptionsView(GeneralOptionsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
