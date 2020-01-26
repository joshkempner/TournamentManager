using System;
using System.Threading;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.Testing;
using TournamentManager.Messages;
using TournamentManager.Presentation;
using TournamentManager.Tests.Helpers;
using Xunit;

namespace TournamentManager.Tests.Presentation
{
    public sealed class when_adding_a_tournament :
        IDisposable,
        IHandleCommand<TournamentMsgs.AddTournament>
    {
        private readonly MockRepositorySpecification _fixture = new MockRepositorySpecification();
        private readonly MockHostScreen _mockHostScreen = new MockHostScreen();

        public when_adding_a_tournament()
        {
            // ReSharper disable once RedundantTypeArgumentsOfMethod
            _fixture.Dispatcher.Subscribe<TournamentMsgs.AddTournament>(this);
        }

        [Fact]
        public void can_add_tournament()
        {
            using var vm = new NewTournamentVM(_fixture.Dispatcher, _mockHostScreen)
            {
                Name = "Test",
                FirstDay = DateTime.Today,
                LastDay = DateTime.Today.AddDays(1)
            };
            // ReSharper disable AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => vm.CanAddTournament);
            vm.Save.Execute().Subscribe();

            _fixture.TestQueue.WaitFor<TournamentMsgs.AddTournament>(TimeSpan.FromMilliseconds(100));
            _fixture
                .TestQueue
                .AssertNext<TournamentMsgs.AddTournament>(cmd => cmd.TournamentId != Guid.Empty &&
                                                                 cmd.Name == vm.Name &&
                                                                 cmd.FirstDay == vm.FirstDay &&
                                                                 cmd.LastDay == vm.LastDay)
                .AssertEmpty();
            _fixture.RepositoryEvents.AssertEmpty();
            // ReSharper restore AccessToDisposedClosure
        }

        [Fact]
        public void can_add_single_day_tournament()
        {
            using var vm = new NewTournamentVM(_fixture.Dispatcher, _mockHostScreen)
            {
                Name = "Test",
                FirstDay = DateTime.Today,
                LastDay = DateTime.Today
            };
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => vm.CanAddTournament);
        }

        [Fact]
        public void cannot_set_last_day_before_first_day()
        {
            using var vm = new NewTournamentVM(_fixture.Dispatcher, _mockHostScreen)
            {
                Name = "Test",
                FirstDay = DateTime.Today,
            };
            // ReSharper disable AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => vm.LastDay == DateTime.Today);
            vm.LastDay = DateTime.Today.AddDays(-1);
            AssertEx.IsOrBecomesTrue(() => vm.LastDay == DateTime.Today);

            vm.FirstDay = DateTime.Today.AddMonths(1);
            AssertEx.IsOrBecomesTrue(() => vm.LastDay == vm.FirstDay);
            // ReSharper restore AccessToDisposedClosure
        }

        [Fact]
        public void cannot_add_tournament_with_empty_name()
        {
            using var vm = new NewTournamentVM(_fixture.Dispatcher, _mockHostScreen);
            // ReSharper disable once AccessToDisposedClosure
            Assert.False(SpinWait.SpinUntil(
                            () => vm.CanAddTournament,
                            TimeSpan.FromMilliseconds(500)));
        }

        public void Dispose()
        {
            // ReSharper disable once RedundantTypeArgumentsOfMethod
            _fixture.Dispatcher.Unsubscribe<TournamentMsgs.AddTournament>(this);
        }

        public CommandResponse Handle(TournamentMsgs.AddTournament command)
        {
            return command.Succeed();
        }
    }
}
