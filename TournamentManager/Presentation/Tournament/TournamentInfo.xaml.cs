using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    /// <summary>
    /// Interaction logic for TournamentInfo.xaml
    /// </summary>
    public partial class TournamentInfo : IViewFor<TournamentInfoVM>
    {
        public TournamentInfo()
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

                this.BindCommand(ViewModel, vm => vm.Save, v => v.Save)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.Cancel, v => v.Cancel)
                    .DisposeWith(disposables);
            });
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(TournamentInfoVM),
            typeof(TournamentInfo),
            new PropertyMetadata(default(TournamentInfoVM)));

        public TournamentInfoVM? ViewModel
        {
            get => (TournamentInfoVM)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = value as TournamentInfoVM;
        }
    }
}
