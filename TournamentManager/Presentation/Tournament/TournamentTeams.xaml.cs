using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    /// <summary>
    /// Interaction logic for TournamentTeams.xaml
    /// </summary>
    public partial class TournamentTeams : IViewFor<TournamentTeamsVM>
    {
        public TournamentTeams()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.Teams, v => v.Teams.ItemsSource)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.NewTeamName, v => v.TeamName.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.NewTeamAgeBracket, v => v.AgeBracket.SelectedItem)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.AddTeam, v => v.AddTeam)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.Cancel, v => v.Done)
                    .DisposeWith(disposables);
            });
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(TournamentTeamsVM),
            typeof(TournamentTeams),
            new PropertyMetadata(default(TournamentTeamsVM)));

        public TournamentTeamsVM? ViewModel
        {
            get => (TournamentTeamsVM)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = value as TournamentTeamsVM;
        }

    }
}
