using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    /// <summary>
    /// Interaction logic for RefereesHost.xaml
    /// </summary>
    public partial class RefereesHost : IViewFor<RefereesHostVM>
    {
        public RefereesHost()
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
            typeof(RefereesHostVM),
            typeof(RefereesHost),
            new PropertyMetadata(default(RefereesHostVM)));

        public RefereesHostVM ViewModel
        {
            get => (RefereesHostVM)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (RefereesHostVM)value;
        }
    }
}
