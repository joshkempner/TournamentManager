using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    /// <summary>
    /// Interaction logic for NewGame.xaml
    /// </summary>
    public partial class NewGame : IViewFor<NewGameVM>
    {
        public NewGame()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.StartTime, v => v.StartTime.SelectedItem)
                    .DisposeWith(d);
                this.Bind(ViewModel, vm => vm.EndTime, v => v.EndTime.SelectedItem)
                    .DisposeWith(d);
                this.Bind(ViewModel, vm => vm.HomeTeam, v => v.HomeTeam.SelectedItem)
                    .DisposeWith(d);
                this.Bind(ViewModel, vm => vm.AwayTeam, v => v.AwayTeam.SelectedItem)
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.AddGame, v => v.AddGame)
                    .DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.Cancel, v => v.Cancel)
                    .DisposeWith(d);
            });
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(NewGameVM),
            typeof(NewGame),
            new PropertyMetadata(default(NewGameVM)));

        public NewGameVM? ViewModel
        {
            get => (NewGameVM)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = value as NewGameVM;
        }
    }
}
