using ReactiveDomain.Messaging.Bus;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    public class TournamentsHostVM : IScreen
    {
        private readonly IDispatcher _bus;

        public TournamentsHostVM(IDispatcher bus)
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
