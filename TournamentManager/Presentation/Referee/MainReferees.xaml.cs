using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    /// <summary>
    /// Interaction logic for MainReferees.xaml
    /// </summary>
    public partial class MainReferees : IViewFor<MainRefereesVM>
    {
        public MainReferees()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // Bind the view model router to the RoutedViewHost.Router property.
                this.OneWayBind(ViewModel, vm => vm.Router, v => v.RoutedViewHost.Router)
                    .DisposeWith(disposables);
            });

            Loaded += (sender, args) => ViewModel?.NavigateToInitialView();
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(MainRefereesVM),
            typeof(MainReferees),
            new PropertyMetadata(default(MainRefereesVM)));

        public MainRefereesVM ViewModel
        {
            get => (MainRefereesVM)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (MainRefereesVM)value;
        }
    }
}
