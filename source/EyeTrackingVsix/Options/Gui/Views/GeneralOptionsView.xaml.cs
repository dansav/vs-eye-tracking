using System.Windows.Controls;
using EyeTrackingVsix.Options.Gui.ViewModels;

namespace EyeTrackingVsix.Options.Gui.Views
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
