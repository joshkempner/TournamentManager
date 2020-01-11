using System;
using ReactiveDomain;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using ReactiveDomain.Testing;
using ReactiveUI;
using Splat;
using TournamentManager.Domain;
using TournamentManager.Messages;
using TournamentManager.Presentation;
using TournamentManager.Tests.Helpers;
using Xunit;

namespace TournamentManager.Tests.Presentation
{
    public sealed class when_managing_referees
    {
        private readonly MockRepositorySpecification _fixture = new MockRepositorySpecification();
        private readonly MockHostScreen _hostScreen = new MockHostScreen();
        private readonly CorrelatedStreamStoreRepository _repo;

        public when_managing_referees()
        {
            Locator.CurrentMutable.RegisterConstant(_fixture.StreamStoreConnection, typeof(IStreamStoreConnection));

            _repo = new CorrelatedStreamStoreRepository(_fixture.Repository);
            // Add some referees
            var ref1 = new Referee(
                            Guid.NewGuid(),
                            "John",
                            "Smith",
                            RefereeMsgs.Grade.Grassroots,
                            MessageBuilder.New(() => new TestCommands.Command1()));
            ref1.AddOrUpdateEmailAddress("john.smith@aol.com");
            _repo.Save(ref1);
            var ref2 = new Referee(
                            Guid.NewGuid(),
                            "Janet",
                            "Jones",
                            RefereeMsgs.Grade.Grassroots,
                            MessageBuilder.New(() => new TestCommands.Command1()));
            ref2.AddOrUpdateEmailAddress("janet.jones@aol.com");
            _repo.Save(ref2);
        }

        [Fact]
        public void can_see_existing_referees()
        {
            using var vm = new ManageRefereesVM(
                                _fixture.Dispatcher,
                                _hostScreen);
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => vm.Referees.Count == 2);
        }

        [Fact]
        public void can_see_newly_added_referee()
        {
            using var vm = new ManageRefereesVM(
                                _fixture.Dispatcher,
                                _hostScreen);
            var ref3 = new Referee(
                            Guid.NewGuid(),
                            "Robert",
                            "Smith",
                            RefereeMsgs.Grade.Intramural,
                            MessageBuilder.New(() => new TestCommands.Command1()));
            ref3.AddOrUpdateEmailAddress("bob.smith@aol.com");
            _repo.Save(ref3);
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => vm.Referees.Count == 3);
        }

        [Fact]
        public void can_add_referee()
        {
            using var vm = new ManageRefereesVM(
                                _fixture.Dispatcher,
                                _hostScreen);
            vm.AddReferee.Execute().Subscribe();
            IRoutableViewModel currentVM = null;
            vm.HostScreen.Router.CurrentViewModel.Subscribe(x => currentVM = x);
            AssertEx.IsOrBecomesTrue(() => currentVM is NewRefereeVM);
        }
    }
}
