using System;
using System.Linq;
using ReactiveDomain;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using ReactiveDomain.Testing;
using ReactiveUI;
using Splat;
using TournamentManager.Domain;
using TournamentManager.Presentation;
using TournamentManager.Tests.Helpers;
using Xunit;

namespace TournamentManager.Tests.Presentation
{
    [Collection("Presentation")]
    public sealed class when_managing_tournaments
    {
        private readonly MockRepositorySpecification _fixture = new MockRepositorySpecification();
        private readonly MockHostScreen _hostScreen = new MockHostScreen();
        private readonly CorrelatedStreamStoreRepository _repo;

        private readonly Guid _t1Id = Guid.NewGuid();
        private readonly Guid _t2Id = Guid.NewGuid();
        private readonly DateTime _t1Start = new DateTime(2020, 06, 1);
        private readonly DateTime _t1End = new DateTime(2020, 06, 2);
        private readonly DateTime _t2Date = new DateTime(2020, 07, 1);

        public when_managing_tournaments()
        {
            Locator.CurrentMutable.RegisterConstant(_fixture.StreamStoreConnection, typeof(IStreamStoreConnection));

            _repo = new CorrelatedStreamStoreRepository(_fixture.Repository);
            // Add some tournaments
            var tourney1 = new Tournament(
                                _t1Id,
                                "Tourney 1",
                                _t1Start, 
                                _t1End, 
                                MessageBuilder.New(() => new TestCommands.Command1()));
            _repo.Save(tourney1);
            var tourney2 = new Tournament(
                                _t2Id,
                                "Tourney 2",
                                _t2Date,
                                _t2Date,
                                MessageBuilder.New(() => new TestCommands.Command1()));
            _repo.Save(tourney2);
        }

        [Fact]
        public void can_see_existing_tournaments()
        {
            using var vm = new ManageTournamentsVM(
                                _fixture.Dispatcher,
                                _hostScreen);
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => vm.Tournaments.Count == 2);
        }

        [Fact]
        public void can_see_newly_added_Tournament()
        {
            using var vm = new ManageTournamentsVM(
                                _fixture.Dispatcher,
                                _hostScreen);
            var tourney3 = new Tournament(
                                Guid.NewGuid(),
                                "Tourney 3",
                                new DateTime(2020, 08, 15),
                                new DateTime(2020, 08, 17),
                                MessageBuilder.New(() => new TestCommands.Command1()));
            _repo.Save(tourney3);
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => vm.Tournaments.Count == 3, 1500);
        }

        [Fact]
        public void can_display_updates_to_tournament_data()
        {
            using var vm = new ManageTournamentsVM(
                                _fixture.Dispatcher,
                                _hostScreen);
            // ReSharper disable once AccessToDisposedClosure
            AssertEx.IsOrBecomesTrue(() => vm.Tournaments.Count == 2);
            foreach (var tournamentItemVM in vm.Tournaments) tournamentItemVM.Activator.Activate();

            const string tourneyName = "Tournament 1";
            var startDate = new DateTime(2020, 01, 04);
            var endDate = new DateTime(2020, 01, 05);
            var t1 = _repo.GetById<Tournament>(_t1Id, MessageBuilder.New(() => new TestCommands.Command1()));
            t1.Rename(tourneyName);
            t1.Reschedule(startDate, endDate);
            _repo.Save(t1);

            var tourney = vm.Tournaments.First(x => x.Id == _t1Id);
            AssertEx.IsOrBecomesTrue(() => tourney.Name == tourneyName);
            AssertEx.IsOrBecomesTrue(() => tourney.FirstDay == startDate);
            AssertEx.IsOrBecomesTrue(() => tourney.LastDay == endDate);
        }

        [Fact]
        public void can_add_tournament()
        {
            using var vm = new ManageTournamentsVM(
                                _fixture.Dispatcher,
                                _hostScreen);
            vm.AddTournament.Execute().Subscribe();
            IRoutableViewModel currentVM = null;
            vm.HostScreen.Router.CurrentViewModel.Subscribe(x => currentVM = x);
            AssertEx.IsOrBecomesTrue(() => currentVM is NewTournamentVM);
        }
    }
}
