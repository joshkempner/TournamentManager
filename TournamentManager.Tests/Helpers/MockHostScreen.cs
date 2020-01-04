using ReactiveUI;

namespace TournamentManager.Tests.Helpers
{
    public class MockHostScreen : IScreen
    {
        public RoutingState Router { get; } = new RoutingState();

        public MockHostScreen()
        {
            Home = new MockViewModel(this);
            Router.Navigate.Execute(Home);
        }

        public IRoutableViewModel Home { get; }
    }
}
