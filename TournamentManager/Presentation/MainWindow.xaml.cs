using System;
using System.Reactive.Disposables;
using System.Windows;
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
                this.OneWayBind(ViewModel, vm => vm.TournamentVM, v => v.MainTournamentsView.ViewModel)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.OverlayVM, v => v.OverlayViewHost.ViewModel)
                    .DisposeWith(disposables);
            });

#pragma warning disable CS8602
            this.WhenAnyValue(x => x.ViewModel.OverlayVM)
                .Subscribe(x => Overlay.Visibility = x is null ? Visibility.Collapsed : Visibility.Visible);
#pragma warning restore CS8602
        }
    }
}
