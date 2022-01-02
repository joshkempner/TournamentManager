using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    /// <summary>
    /// Interaction logic for ManageReferees.xaml
    /// </summary>
    public partial class ManageReferees : IViewFor<ManageRefereesVM>
    {
        public ManageReferees()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
                {
                    this.OneWayBind(ViewModel, vm => vm.Referees, v => v.Referees.ItemsSource)
                        .DisposeWith(disposables);
                    this.BindCommand(ViewModel, vm => vm.AddReferee, v => v.AddReferee)
                        .DisposeWith(disposables);
                });
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(ManageRefereesVM),
            typeof(ManageReferees),
            new PropertyMetadata(default(ManageRefereesVM)));

        public ManageRefereesVM? ViewModel
        {
            get => (ManageRefereesVM)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = value as ManageRefereesVM;
        }
    }
}
