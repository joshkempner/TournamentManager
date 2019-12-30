using System.Windows;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IViewFor<MainWindowVM>
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(MainWindowVM),
            typeof(MainWindow),
            new PropertyMetadata(default(MainWindowVM)));

        public MainWindowVM ViewModel
        {
            get => (MainWindowVM)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (MainWindowVM)value;
        }
    }
}
