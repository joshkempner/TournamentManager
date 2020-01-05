using System;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using ReactiveDomain.Testing;
using TournamentManager.Messages;
using Xunit;

namespace TournamentManager.Tests.Domain
{
    public sealed class when_managing_tournament_core_through_service : with_tournament_service
    {
        [Fact]
        public void can_add_tournament()
        {
            var cmd = MessageBuilder.New(() => new TournamentMsgs.AddTournament(
                                                    TournamentId,
                                                    TournamentName,
                                                    FirstDay,
                                                    LastDay));
            Fixture.Dispatcher.Send(cmd);
            Fixture.RepositoryEvents.WaitFor<TournamentMsgs.TournamentAdded>(TimeSpan.FromMilliseconds(200));
            Fixture
                .TestQueue
                .AssertNext<TournamentMsgs.AddTournament>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture
                .RepositoryEvents
                .AssertNext<TournamentMsgs.TournamentAdded>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(TournamentId, evt.TournamentId);
            Assert.Equal(TournamentName, evt.Name);
            Assert.Equal(FirstDay, evt.FirstDay);
            Assert.Equal(LastDay, evt.LastDay);
        }

        [Fact]
        public void cannot_add_duplicate_tournament()
        {
            AddTournament();
            var cmd = MessageBuilder.New(() => new TournamentMsgs.AddTournament(
                                                       TournamentId,
                                                       TournamentName,
                                                       FirstDay,
                                                       LastDay));
            AssertEx.CommandThrows<AggregateException>(() => Fixture.Dispatcher.Send(cmd));
            Fixture
                .TestQueue
                .AssertNext<TournamentMsgs.AddTournament>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_rename_tournament()
        {
            const string newName = "The Bourbon Cup";
            AddTournament();
            var cmd = MessageBuilder.New(() => new TournamentMsgs.RenameTournament(
                                                    TournamentId,
                                                    newName));
            Fixture.Dispatcher.Send(cmd);
            Fixture.RepositoryEvents.WaitFor<TournamentMsgs.TournamentRenamed>(TimeSpan.FromMilliseconds(200));
            Fixture
                .TestQueue
                .AssertNext<TournamentMsgs.RenameTournament>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture
                .RepositoryEvents
                .AssertNext<TournamentMsgs.TournamentRenamed>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(TournamentId, evt.TournamentId);
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
            AssertEx.CommandThrows<AggregateNotFoundException>(() => Fixture.Dispatcher.Send(cmd));
            Fixture
                .TestQueue
                .AssertNext<TournamentMsgs.RenameTournament>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_reschedule_tournament()
        {
            var newFirstDay = FirstDay.AddDays(7);
            var newLastDay = LastDay.AddDays(7);
            AddTournament();
            var cmd = MessageBuilder.New(() => new TournamentMsgs.RescheduleTournament(
                                                    TournamentId,
                                                    newFirstDay,
                                                    newLastDay));
            Fixture.Dispatcher.Send(cmd);
            Fixture.RepositoryEvents.WaitFor<TournamentMsgs.TournamentRescheduled>(TimeSpan.FromMilliseconds(200));
            Fixture
                .TestQueue
                .AssertNext<TournamentMsgs.RescheduleTournament>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture
                .RepositoryEvents
                .AssertNext<TournamentMsgs.TournamentRescheduled>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(TournamentId, evt.TournamentId);
            Assert.Equal(newFirstDay, evt.FirstDay);
            Assert.Equal(newLastDay, evt.LastDay);
        }

        [Fact]
        public void cannot_reschedule_invalid_tournament()
        {
            var newFirstDay = FirstDay.AddDays(7);
            var newLastDay = LastDay.AddDays(7);
            AddTournament();
            var cmd = MessageBuilder.New(() => new TournamentMsgs.RescheduleTournament(
                                                    Guid.NewGuid(),
                                                    newFirstDay,
                                                    newLastDay));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => Fixture.Dispatcher.Send(cmd));
            Fixture
                .TestQueue
                .AssertNext<TournamentMsgs.RescheduleTournament>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_add_field()
        {
            var fieldId = Guid.NewGuid();
            const string fieldName = "Field 1";
            AddTournament();
            var cmd = MessageBuilder.New(() => new TournamentMsgs.AddField(
                                                    TournamentId,
                                                    fieldId,
                                                    fieldName));
            Fixture.Dispatcher.Send(cmd);
            Fixture.RepositoryEvents.WaitFor<TournamentMsgs.FieldAdded>(TimeSpan.FromMilliseconds(200));
            Fixture
                .TestQueue
                .AssertNext<TournamentMsgs.AddField>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture
                .RepositoryEvents
                .AssertNext<TournamentMsgs.FieldAdded>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(TournamentId, evt.TournamentId);
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
            AssertEx.CommandThrows<AggregateNotFoundException>(() => Fixture.Dispatcher.Send(cmd));
            Fixture
                .TestQueue
                .AssertNext<TournamentMsgs.AddField>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_add_game_slot()
        {
            var gameSlotId = Guid.NewGuid();
            var startTime = FirstDay.AddHours(9);
            var endTime = startTime.AddHours(1);
            AddTournament();
            var cmd = MessageBuilder.New(() => new TournamentMsgs.AddGameSlot(
                                                    TournamentId,
                                                    gameSlotId,
                                                    startTime,
                                                    endTime));
            Fixture.Dispatcher.Send(cmd);
            Fixture.RepositoryEvents.WaitFor<TournamentMsgs.GameSlotAdded>(TimeSpan.FromMilliseconds(200));
            Fixture
                .TestQueue
                .AssertNext<TournamentMsgs.AddGameSlot>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture
                .RepositoryEvents
                .AssertNext<TournamentMsgs.GameSlotAdded>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(TournamentId, evt.TournamentId);
            Assert.Equal(gameSlotId, evt.GameSlotId);
            Assert.Equal(startTime, evt.StartTime);
            Assert.Equal(endTime, evt.EndTime);
        }

        [Fact]
        public void cannot_add_game_slot_to_invalid_tournament()
        {
            var gameSlotId = Guid.NewGuid();
            var startTime = FirstDay.AddHours(9);
            var endTime = startTime.AddHours(1);
            AddTournament();
            var cmd = MessageBuilder.New(() => new TournamentMsgs.AddGameSlot(
                                                    Guid.NewGuid(),
                                                    gameSlotId,
                                                    startTime,
                                                    endTime));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => Fixture.Dispatcher.Send(cmd));
            Fixture
                .TestQueue
                .AssertNext<TournamentMsgs.AddGameSlot>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_add_referee_to_tournament()
        {
            var refereeId = Guid.NewGuid();
            AddTournament();
            var cmd = MessageBuilder.New(() => new TournamentMsgs.AddRefereeToTournament(
                                                    TournamentId,
                                                    refereeId));
            Fixture.Dispatcher.Send(cmd);
            Fixture.RepositoryEvents.WaitFor<TournamentMsgs.RefereeAddedToTournament>(TimeSpan.FromMilliseconds(200));
            Fixture
                .TestQueue
                .AssertNext<TournamentMsgs.AddRefereeToTournament>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture
                .RepositoryEvents
                .AssertNext<TournamentMsgs.RefereeAddedToTournament>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(TournamentId, evt.TournamentId);
            Assert.Equal(refereeId, evt.RefereeId);
        }

        [Fact]
        public void cannot_add_referee_to_invalid_tournament()
        {
            var refereeId = Guid.NewGuid();
            AddTournament();
            var cmd = MessageBuilder.New(() => new TournamentMsgs.AddRefereeToTournament(
                                                    Guid.NewGuid(),
                                                    refereeId));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => Fixture.Dispatcher.Send(cmd));
            Fixture
                .TestQueue
                .AssertNext<TournamentMsgs.AddRefereeToTournament>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }
    }
}
