using System.Windows;

namespace Reorder_item_WPF
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region  Constructors

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
        }

        #endregion
    }
}