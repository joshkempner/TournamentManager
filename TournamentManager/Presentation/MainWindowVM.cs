using ReactiveDomain.Messaging.Bus;
using ReactiveUI;
using Splat;

namespace TournamentManager.Presentation
{
    public class MainWindowVM
    {
        public MainWindowVM(IDispatcher bus)
        {
            RefereesVM = new MainRefereesVM(bus);
        }

        public MainRefereesVM RefereesVM { get; }
    }
}
