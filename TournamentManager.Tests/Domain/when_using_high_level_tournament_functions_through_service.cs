using System;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using ReactiveDomain.Testing;
using TournamentManager.Domain;
using TournamentManager.Messages;
using Xunit;

namespace TournamentManager.Tests.Domain
{
    public class when_using_high_level_tournament_functions_through_service
    {
        private readonly MockRepositorySpecification _fixture = new MockRepositorySpecification();
        private readonly TournamentSvc _tournamentSvc;

        private readonly Guid _tournamentId = Guid.NewGuid();
        private const string TournamentName = "The Milk Cup";
        private readonly DateTime _firstDay = new DateTime(2020, 6, 1);
        private readonly DateTime _lastDay = new DateTime(2020, 6, 2);

        public when_using_high_level_tournament_functions_through_service()
        {
            _tournamentSvc = new TournamentSvc(_fixture.Dispatcher, _fixture.Repository);
        }

        [Fact]
        public void can_add_tournament()
        {
            var cmd = MessageBuilder.New(() => new TournamentMsgs.AddTournament(
                                                    _tournamentId,
                                                    TournamentName,
                                                    _firstDay,
                                                    _lastDay));
            _fixture.Dispatcher.Send(cmd);
            _fixture.RepositoryEvents.WaitFor<TournamentMsgs.TournamentAdded>(TimeSpan.FromMilliseconds(200));
            _fixture
                .TestQueue
                .AssertNext<TournamentMsgs.AddTournament>(cmd.CorrelationId)
                .AssertEmpty();
            _fixture
                .RepositoryEvents
                .AssertNext<TournamentMsgs.TournamentAdded>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(_tournamentId, evt.TournamentId);
            Assert.Equal(TournamentName, evt.Name);
            Assert.Equal(_firstDay, evt.FirstDay);
            Assert.Equal(_lastDay, evt.LastDay);
        }

        private void AddTournament()
        {
            var tournament = new Tournament(
                                    _tournamentId,
                                    TournamentName,
                                    _firstDay,
                                    _lastDay,
                                    MessageBuilder.New(() => new TestCommands.Command1()));
            var repo = new CorrelatedStreamStoreRepository(_fixture.Repository);
            repo.Save(tournament);
            _fixture.RepositoryEvents.WaitFor<TournamentMsgs.TournamentAdded>(TimeSpan.FromMilliseconds(200));
            _fixture.ClearQueues();
        }

        [Fact]
        public void cannot_add_duplicate_tournament()
        {
            AddTournament();
            var cmd = MessageBuilder.New(() => new TournamentMsgs.AddTournament(
                                                       _tournamentId,
                                                       TournamentName,
                                                       _firstDay,
                                                       _lastDay));
            AssertEx.CommandThrows<AggregateException>(() => _fixture.Dispatcher.Send(cmd));
            _fixture
                .TestQueue
                .AssertNext<TournamentMsgs.AddTournament>(cmd.CorrelationId)
                .AssertEmpty();
            _fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_rename_tournament()
        {
            const string newName = "The Bourbon Cup";
            AddTournament();
            var cmd = MessageBuilder.New(() => new TournamentMsgs.RenameTournament(
                                                    _tournamentId,
                                                    newName));
            _fixture.Dispatcher.Send(cmd);
            _fixture.RepositoryEvents.WaitFor<TournamentMsgs.TournamentRenamed>(TimeSpan.FromMilliseconds(200));
            _fixture
                .TestQueue
                .AssertNext<TournamentMsgs.RenameTournament>(cmd.CorrelationId)
                .AssertEmpty();
            _fixture
                .RepositoryEvents
                .AssertNext<TournamentMsgs.TournamentRenamed>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(_tournamentId, evt.TournamentId);
            Assert.Equal(newName, evt.Name);
        }

        [Fact]
        public void cannot_rename_invalid_tournament()
        {
            const string newName = "The Bourbon Cup";
            AddTournament();
            var cmd = MessageBuilder.New(() => new TournamentMsgs.RenameTournament(
                                                    Guid.NewGuid(),
                                                    newName));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => _fixture.Dispatcher.Send(cmd));
            _fixture
                .TestQueue
                .AssertNext<TournamentMsgs.RenameTournament>(cmd.CorrelationId)
                .AssertEmpty();
            _fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_reschedule_tournament()
        {
            var newFirstDay = _firstDay.AddDays(7);
            var newLastDay = _lastDay.AddDays(7);
            AddTournament();
            var cmd = MessageBuilder.New(() => new TournamentMsgs.RescheduleTournament(
                                                    _tournamentId,
                                                    newFirstDay,
                                                    newLastDay));
            _fixture.Dispatcher.Send(cmd);
            _fixture.RepositoryEvents.WaitFor<TournamentMsgs.TournamentRescheduled>(TimeSpan.FromMilliseconds(200));
            _fixture
                .TestQueue
                .AssertNext<TournamentMsgs.RescheduleTournament>(cmd.CorrelationId)
                .AssertEmpty();
            _fixture
                .RepositoryEvents
                .AssertNext<TournamentMsgs.TournamentRescheduled>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(_tournamentId, evt.TournamentId);
            Assert.Equal(newFirstDay, evt.FirstDay);
            Assert.Equal(newLastDay, evt.LastDay);
        }

        [Fact]
        public void cannot_reschedule_invalid_tournament()
        {
            var newFirstDay = _firstDay.AddDays(7);
            var newLastDay = _lastDay.AddDays(7);
            AddTournament();
            var cmd = MessageBuilder.New(() => new TournamentMsgs.RescheduleTournament(
                                                    Guid.NewGuid(),
                                                    newFirstDay,
                                                    newLastDay));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => _fixture.Dispatcher.Send(cmd));
            _fixture
                .TestQueue
                .AssertNext<TournamentMsgs.RescheduleTournament>(cmd.CorrelationId)
                .AssertEmpty();
            _fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_add_field()
        {
            var fieldId = Guid.NewGuid();
            const string fieldName = "Field 1";
            AddTournament();
            var cmd = MessageBuilder.New(() => new TournamentMsgs.AddField(
                                                    _tournamentId,
                                                    fieldId,
                                                    fieldName));
            _fixture.Dispatcher.Send(cmd);
            _fixture.RepositoryEvents.WaitFor<TournamentMsgs.FieldAdded>(TimeSpan.FromMilliseconds(200));
            _fixture
                .TestQueue
                .AssertNext<TournamentMsgs.AddField>(cmd.CorrelationId)
                .AssertEmpty();
            _fixture
                .RepositoryEvents
                .AssertNext<TournamentMsgs.FieldAdded>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(_tournamentId, evt.TournamentId);
            Assert.Equal(fieldId, evt.FieldId);
            Assert.Equal(fieldName, evt.FieldName);
        }

        [Fact]
        public void cannot_add_field_to_invalid_tournament()
        {
            var fieldId = Guid.NewGuid();
            const string fieldName = "Field 1";
            AddTournament();
            var cmd = MessageBuilder.New(() => new TournamentMsgs.AddField(
                                                    Guid.NewGuid(),
                                                    fieldId,
                                                    fieldName));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => _fixture.Dispatcher.Send(cmd));
            _fixture
                .TestQueue
                .AssertNext<TournamentMsgs.AddField>(cmd.CorrelationId)
                .AssertEmpty();
            _fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_add_game_slot()
        {
            var gameSlotId = Guid.NewGuid();
            var startTime = _firstDay.AddHours(9);
            var endTime = startTime.AddHours(1);
            AddTournament();
            var cmd = MessageBuilder.New(() => new TournamentMsgs.AddGameSlot(
                                                    _tournamentId,
                                                    gameSlotId,
                                                    startTime,
                                                    endTime));
            _fixture.Dispatcher.Send(cmd);
            _fixture.RepositoryEvents.WaitFor<TournamentMsgs.GameSlotAdded>(TimeSpan.FromMilliseconds(200));
            _fixture
                .TestQueue
                .AssertNext<TournamentMsgs.AddGameSlot>(cmd.CorrelationId)
                .AssertEmpty();
            _fixture
                .RepositoryEvents
                .AssertNext<TournamentMsgs.GameSlotAdded>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(_tournamentId, evt.TournamentId);
            Assert.Equal(gameSlotId, evt.GameSlotId);
            Assert.Equal(startTime, evt.StartTime);
            Assert.Equal(endTime, evt.EndTime);
        }

        [Fact]
        public void cannot_add_game_slot_to_invalid_tournament()
        {
            var gameSlotId = Guid.NewGuid();
            var startTime = _firstDay.AddHours(9);
            var endTime = startTime.AddHours(1);
            AddTournament();
            var cmd = MessageBuilder.New(() => new TournamentMsgs.AddGameSlot(
                                                    Guid.NewGuid(),
                                                    gameSlotId,
                                                    startTime,
                                                    endTime));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => _fixture.Dispatcher.Send(cmd));
            _fixture
                .TestQueue
                .AssertNext<TournamentMsgs.AddGameSlot>(cmd.CorrelationId)
                .AssertEmpty();
            _fixture.RepositoryEvents.AssertEmpty();
        }
    }
}
