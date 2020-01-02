using ReactiveDomain.Messaging.Bus;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    public class MainWindowVM : IScreen
    {
        private readonly IDispatcher _bus;

        public MainWindowVM(IDispatcher bus)
        {
            _bus = bus;
        }

        public RoutingState Router { get; } = new RoutingState();

        public void NavigateToInitialView()
        {
            Router.Navigate.Execute(new ManageRefereesVM(
                                            _bus,
                                            this));
        }
    }
}
