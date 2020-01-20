using ReactiveDomain.Messaging.Bus;

namespace TournamentManager.Presentation
{
    public class MainWindowVM
    {
        public MainWindowVM(IDispatcher bus)
        {
            RefereesVM = new MainRefereesVM(bus);
            TournamentVM = new MainTournamentVM(bus);
        }

        public MainRefereesVM RefereesVM { get; }

        public MainTournamentVM TournamentVM { get; }
    }
}
