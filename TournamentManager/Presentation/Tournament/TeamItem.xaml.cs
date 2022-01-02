using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    /// <summary>
    /// Interaction logic for TeamItem.xaml
    /// </summary>
    public partial class TeamItem : IViewFor<TeamItemVM>
    {
        public TeamItem()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.Name, v => v.TeamName.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Name, v => v.TeamNameEdit.Text)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.AgeBracket, v => v.AgeBracket.SelectedItem)
                    .DisposeWith(disposables);
                // TODO: bind division

                ViewModel?
                    .ConfirmRemove
                    .RegisterHandler(
                        interaction =>
                        {
                            var remove = MessageBox.Show(
                                            "Are you sure you want to remove this team from the tournament?",
                                            "Remove Team?",
                                            MessageBoxButton.YesNo,
                                            MessageBoxImage.Warning);
                            interaction.SetOutput(remove == MessageBoxResult.Yes);
                        })
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.RemoveTeam, v => v.RemoveTeam)
                    .DisposeWith(disposables);
            });
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(TeamItemVM),
            typeof(TeamItem),
            new PropertyMetadata(default(TeamItemVM)));

        public TeamItemVM? ViewModel
        {
            get => (TeamItemVM)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = value as TeamItemVM;
        }
    }
}
