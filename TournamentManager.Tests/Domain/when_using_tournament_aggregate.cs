using System;
using ReactiveDomain.Messaging;
using ReactiveDomain.Testing;
using TournamentManager.Domain;
using TournamentManager.Messages;
using Xunit;

namespace TournamentManager.Tests.Domain
{
    public class when_using_tournament_aggregate
    {
        private readonly Guid _tournamentId = Guid.NewGuid();
        private const string TournamentName = "The Milk Cup";
        private readonly DateTime _firstDay = new DateTime(2020, 6, 1);
        private readonly DateTime _lastDay = new DateTime(2020, 6, 2);

        [Fact]
        public void can_create_tournament()
        {
            var sourceMsg = MessageBuilder.New(() => new TestCommands.Command1());
            var tournament = AddTournament(sourceMsg);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is TournamentMsgs.TournamentAdded evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.Name == TournamentName &&
                                 evt.FirstDay == _firstDay &&
                                 evt.LastDay == _lastDay));
        }

        [Fact]
        public void cannot_create_tournament_with_invalid_id()
        {
            Assert.Throws<ArgumentException>(
                () => new Tournament(
                            Guid.Empty,
                            TournamentName,
                            _firstDay,
                            _lastDay,
                            MessageBuilder.New(() => new TestCommands.Command1())));
        }

        [Fact]
        public void cannot_create_tournament_with_invalid_name()
        {
            Assert.Throws<ArgumentNullException>(
                () => new Tournament(
                            _tournamentId,
                            string.Empty,
                            _firstDay,
                            _lastDay,
                            MessageBuilder.New(() => new TestCommands.Command1())));
        }

        [Fact]
        public void can_create_single_day_tournament()
        {
            var sourceMsg = MessageBuilder.New(() => new TestCommands.Command1());
            var tournament = new Tournament(
                                    _tournamentId,
                                    TournamentName,
                                    _firstDay,
                                    _firstDay,
                                    sourceMsg);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is TournamentMsgs.TournamentAdded evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.Name == TournamentName &&
                                 evt.FirstDay == _firstDay &&
                                 evt.LastDay == _firstDay));
        }

        [Fact]
        public void cannot_create_tournament_with_invalid_dates()
        {
            Assert.Throws<ArgumentException>(
                () => new Tournament(
                            _tournamentId,
                            TournamentName,
                            _lastDay,
                            _firstDay,
                            MessageBuilder.New(() => new TestCommands.Command1())));
        }

        private Tournament AddTournament(ICorrelatedMessage sourceMsg)
        {
            return new Tournament(
                        _tournamentId,
                        TournamentName,
                        _firstDay,
                        _lastDay,
                        sourceMsg);
        }

        [Fact]
        public void can_rename_tournament()
        {
            const string newName = "The Bourbon Cup";
            var sourceMsg = MessageBuilder.New(() => new TestCommands.Command1());
            var tournament = AddTournament(sourceMsg);
            tournament.Rename(newName);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Equal(2, events.Length);
            Assert.Collection(
                events,
                e => Assert.True(e is TournamentMsgs.TournamentAdded),
                e => Assert.True(e is TournamentMsgs.TournamentRenamed evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.Name == newName));
        }

        [Fact]
        public void cannot_rename_to_empty_name()
        {
            var sourceMsg = MessageBuilder.New(() => new TestCommands.Command1());
            var tournament = AddTournament(sourceMsg);
            Assert.Throws<ArgumentNullException>(() => tournament.Rename(string.Empty));
            Assert.Throws<ArgumentException>(() => tournament.Rename(" "));

            // Make sure the "Added" event is the only one.
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Equal(1, events.Length);
        }

        [Fact]
        public void can_reschedule_tournament()
        {
            var newFirstDay = _firstDay.AddDays(1);
            var newLastDay = _lastDay.AddDays(1);
            var sourceMsg = MessageBuilder.New(() => new TestCommands.Command1());
            var tournament = AddTournament(sourceMsg);
            tournament.Reschedule(
                newFirstDay,
                newLastDay);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Equal(2, events.Length);
            Assert.Collection(
                events,
                e => Assert.True(e is TournamentMsgs.TournamentAdded),
                e => Assert.True(e is TournamentMsgs.TournamentRescheduled evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.FirstDay == newFirstDay &&
                                 evt.LastDay == newLastDay));
        }

        [Fact]
        public void can_reschedule_tournament_to_single_day()
        {
            var sourceMsg = MessageBuilder.New(() => new TestCommands.Command1());
            var tournament = AddTournament(sourceMsg);
            tournament.Reschedule(
                _firstDay,
                _firstDay);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Equal(2, events.Length);
            Assert.Collection(
                events,
                e => Assert.True(e is TournamentMsgs.TournamentAdded),
                e => Assert.True(e is TournamentMsgs.TournamentRescheduled evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.FirstDay == _firstDay &&
                                 evt.LastDay == _firstDay));
        }

        [Fact]
        public void cannot_reschedule_tournament_to_invalid_dates()
        {
            var sourceMsg = MessageBuilder.New(() => new TestCommands.Command1());
            var tournament = AddTournament(sourceMsg);
            Assert.Throws<ArgumentException>(
                () => tournament.Reschedule(
                        _lastDay,
                        _firstDay));

            // Make sure the "Added" event is the only one.
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Equal(1, events.Length);
        }

        [Fact]
        public void can_add_field()
        {
            var fieldId = Guid.NewGuid();
            const string fieldName = "Field 1";
            var sourceMsg = MessageBuilder.New(() => new TestCommands.Command1());
            var tournament = AddTournament(sourceMsg);
            tournament.AddField(
                fieldId,
                fieldName);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Equal(2, events.Length);
            Assert.Collection(
                events,
                e => Assert.True(e is TournamentMsgs.TournamentAdded),
                e => Assert.True(e is TournamentMsgs.FieldAdded evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.FieldId == fieldId &&
                                 evt.FieldName == fieldName));
        }

        [Fact]
        public void cannot_add_field_with_duplicate_id()
        {
            var fieldId = Guid.NewGuid();
            const string fieldName = "Field 1";
            var sourceMsg = MessageBuilder.New(() => new TestCommands.Command1());
            var tournament = AddTournament(sourceMsg);
            tournament.AddField(
                fieldId,
                fieldName);
            Assert.Throws<ArgumentException>(
                () => tournament.AddField(
                        fieldId,
                        fieldName));

            // Make sure we have the right number of events
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Equal(2, events.Length);
        }

        [Fact]
        public void cannot_add_field_with_invalid_id()
        {
            const string fieldName = "Field 1";
            var sourceMsg = MessageBuilder.New(() => new TestCommands.Command1());
            var tournament = AddTournament(sourceMsg);
            Assert.Throws<ArgumentException>(
                () => tournament.AddField(
                        Guid.Empty,
                        fieldName));

            // Make sure the "Added" event is the only one.
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Equal(1, events.Length);
        }

        [Fact]
        public void cannot_add_field_with_invalid_name()
        {
            var fieldId = Guid.NewGuid();
            var sourceMsg = MessageBuilder.New(() => new TestCommands.Command1());
            var tournament = AddTournament(sourceMsg);
            Assert.Throws<ArgumentNullException>(
                () => tournament.AddField(
                        fieldId,
                        string.Empty));

            // Make sure the "Added" event is the only one.
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Equal(1, events.Length);
        }

        [Fact]
        public void can_add_game_slot()
        {
            var gameSlotId = Guid.NewGuid();
            var startTime = _firstDay.AddHours(9);
            var endTime = startTime.AddHours(1);
            var sourceMsg = MessageBuilder.New(() => new TestCommands.Command1());
            var tournament = AddTournament(sourceMsg);
            tournament.AddGameSlot(
                gameSlotId,
                startTime,
                endTime);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Equal(2, events.Length);
            Assert.Collection(
                events,
                e => Assert.True(e is TournamentMsgs.TournamentAdded),
                e => Assert.True(e is TournamentMsgs.GameSlotAdded evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.StartTime == startTime &&
                                 evt.EndTime == endTime));
        }

        [Fact]
        public void cannot_add_game_slot_with_duplicate_id()
        {
            var gameSlotId = Guid.NewGuid();
            var startTime = _firstDay.AddHours(9);
            var endTime = startTime.AddHours(1);
            var sourceMsg = MessageBuilder.New(() => new TestCommands.Command1());
            var tournament = AddTournament(sourceMsg);
            tournament.AddGameSlot(
                gameSlotId,
                startTime,
                endTime);
            Assert.Throws<ArgumentException>(
                () => tournament.AddGameSlot(
                        gameSlotId,
                        startTime,
                        endTime));

            // Make sure we have the right number of events
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Equal(2, events.Length);
        }

        [Fact]
        public void cannot_add_game_slot_with_invalid_id()
        {
            var startTime = _firstDay.AddHours(9);
            var endTime = startTime.AddHours(1);
            var sourceMsg = MessageBuilder.New(() => new TestCommands.Command1());
            var tournament = AddTournament(sourceMsg);
            Assert.Throws<ArgumentException>(
                () => tournament.AddGameSlot(
                        Guid.Empty, 
                        startTime,
                        endTime));
        }

        [Fact]
        public void cannot_add_game_slot_outside_tournament_days()
        {
            var gameSlotId = Guid.NewGuid();
            var startTime = _lastDay.AddDays(1).AddHours(9);
            var endTime = startTime.AddHours(1);
            var sourceMsg = MessageBuilder.New(() => new TestCommands.Command1());
            var tournament = AddTournament(sourceMsg);
            Assert.Throws<ArgumentException>(
                () => tournament.AddGameSlot(
                        gameSlotId,
                        startTime,
                        endTime));

            // Make sure the "Added" event is the only one.
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Equal(1, events.Length);
        }

        [Fact]
        public void cannot_add_game_slot_with_start_time_after_end_time()
        {
            var gameSlotId = Guid.NewGuid();
            var startTime = _firstDay.AddHours(9);
            var endTime = startTime.AddHours(-1);
            var sourceMsg = MessageBuilder.New(() => new TestCommands.Command1());
            var tournament = AddTournament(sourceMsg);
            Assert.Throws<ArgumentException>(
                () => tournament.AddGameSlot(
                        gameSlotId,
                        startTime,
                        endTime));

            // Make sure the "Added" event is the only one.
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Equal(1, events.Length);
        }
    }
}
