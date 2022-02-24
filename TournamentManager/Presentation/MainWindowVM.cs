using System;
using ReactiveDomain.Messaging.Bus;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    public class MainWindowVM : ReactiveObject
    {
        public MainWindowVM(IDispatcher bus)
        {
            RefereesVM = new RefereesHostVM(bus);
            TournamentVM = new TournamentsHostVM(bus);

#pragma warning disable CS8602
            this.WhenAnyObservable(x => x.OverlayVM.Done)
                .Subscribe(_ =>
                {
                    OverlayVM.Dispose();
                    OverlayVM = null;
                });
#pragma warning restore CS8602
        }

        public RefereesHostVM RefereesVM { get; }

        public TournamentsHostVM TournamentVM { get; }

        public OverlayViewModel? OverlayVM
        {
            get => _overlayVM;
            set => this.RaiseAndSetIfChanged(ref _overlayVM, value);
        }
        private OverlayViewModel? _overlayVM;
    }
}
