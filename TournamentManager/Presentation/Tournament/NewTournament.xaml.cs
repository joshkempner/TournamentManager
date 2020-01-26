using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    /// <summary>
    /// Interaction logic for NewTournament.xaml
    /// </summary>
    public partial class NewTournament : IViewFor<NewTournamentVM>
    {
        public NewTournament()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel, vm => vm.Name, v => v.TournamentName.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.FirstDay, v => v.FirstDay.SelectedDate)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.LastDay, v => v.LastDay.SelectedDate)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.Save, v => v.Add)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.Cancel, v => v.Cancel)
                    .DisposeWith(disposables);
            });
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(NewTournamentVM),
            typeof(NewTournament),
            new PropertyMetadata(default(NewTournamentVM)));

        public NewTournamentVM ViewModel
        {
            get => (NewTournamentVM)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (NewTournamentVM)value;
        }
    }
}
