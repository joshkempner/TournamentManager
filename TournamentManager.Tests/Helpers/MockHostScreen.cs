using ReactiveUI;

namespace TournamentManager.Tests.Helpers
{
    public class MockHostScreen : IScreen
    {
        public RoutingState Router { get; } = new RoutingState();
    }
}
