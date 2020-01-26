using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    /// <summary>
    /// Interaction logic for TournamentItem.xaml
    /// </summary>
    public partial class TournamentItem : IViewFor<TournamentItemVM>
    {
        public TournamentItem()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.Name, v => v.TournamentName.Text)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.TournamentDates, v => v.TournamentDates.Text)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.EditTournament, v => v.EditTournament)
                    .DisposeWith(disposables);
            });
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(TournamentItemVM),
            typeof(TournamentItem),
            new PropertyMetadata(default(TournamentItemVM)));

        public TournamentItemVM ViewModel
        {
            get => (TournamentItemVM) GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TournamentItemVM) value;
        }
    }
}
