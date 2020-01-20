using System.Windows;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    /// <summary>
    /// Interaction logic for ManageTournaments.xaml
    /// </summary>
    public partial class ManageTournaments : IViewFor<ManageTournamentsVM>
    {
        public ManageTournaments()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(ManageTournamentsVM),
            typeof(ManageTournaments),
            new PropertyMetadata(default(ManageTournamentsVM)));

        public ManageTournamentsVM ViewModel
        {
            get => (ManageTournamentsVM)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (ManageTournamentsVM)value;
        }
    }
}
