using ReactiveDomain.Messaging.Bus;
using ReactiveUI;
using Splat;

namespace TournamentManager.Presentation
{
    public class MainRefereesVM : IScreen
    {
        private readonly IDispatcher _bus;

        public MainRefereesVM(IDispatcher bus)
        {
            _bus = bus;
            Locator.CurrentMutable.RegisterConstant(this, typeof(IScreen));
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
