using System.Reactive.Disposables;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.RefereesVM, v => v.MainRefereesView.ViewModel)
                    .DisposeWith(disposables);
            });
        }
    }
}
