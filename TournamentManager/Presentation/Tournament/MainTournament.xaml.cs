using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    /// <summary>
    /// Interaction logic for MainTournament.xaml
    /// </summary>
    public partial class MainTournament : IViewFor<MainTournamentVM>
    {
        public MainTournament()
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
            typeof(MainTournamentVM),
            typeof(MainTournament),
            new PropertyMetadata(default(MainTournamentVM)));

        public MainTournamentVM ViewModel
        {
            get => (MainTournamentVM)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (MainTournamentVM)value;
        }
    }
}
