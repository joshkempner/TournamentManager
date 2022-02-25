using ReactiveDomain;
using ReactiveDomain.Testing;
using Splat;
using TournamentManager.Tests.Helpers;
using Xunit;

namespace TournamentManager.Tests.Presentation
{
    [Collection("Presentation")]
    public abstract class with_vm_fixtures
    {
        protected readonly MockRepositorySpecification Fixture = new MockRepositorySpecification();
        protected readonly MockHostScreen Screen = new MockHostScreen();
        protected readonly RefereeTestHelper RefereeTestHelper;

        protected with_vm_fixtures()
        {
            RefereeTestHelper = new RefereeTestHelper(Fixture);
            Locator.CurrentMutable.RegisterConstant(Fixture.StreamStoreConnection, typeof(IStreamStoreConnection));
        }
    }
}
