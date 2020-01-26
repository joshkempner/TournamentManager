using ReactiveDomain.Messaging.Bus;

namespace TournamentManager.Presentation
{
    public class MainWindowVM
    {
        public MainWindowVM(IDispatcher bus)
        {
            RefereesVM = new RefereesHostVM(bus);
            TournamentVM = new TournamentsHostVM(bus);
        }

        public RefereesHostVM RefereesVM { get; }

        public TournamentsHostVM TournamentVM { get; }
    }
}
