using ReactiveDomain.Messaging.Bus;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    public class MainTournamentVM : IScreen
    {
        private readonly IDispatcher _bus;

        public MainTournamentVM(IDispatcher bus)
        {
            _bus = bus;
        }

        public RoutingState Router { get; } = new RoutingState();

        public void NavigateToInitialView()
        {
            Router.Navigate.Execute(new ManageTournamentsVM(
                                            _bus,
                                            this));
        }
    }
}
