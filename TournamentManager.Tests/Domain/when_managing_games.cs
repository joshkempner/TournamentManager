using System;
using ReactiveDomain;
using ReactiveDomain.Messaging;
using ReactiveDomain.Testing;
using TournamentManager.Domain;
using TournamentManager.Messages;
using Xunit;

namespace TournamentManager.Tests.Domain
{
    public sealed class when_managing_games
    {
        private readonly Guid _tournamentId = Guid.NewGuid();
        private readonly Guid _gameId = Guid.NewGuid();
        private readonly Guid _fieldId = Guid.NewGuid();
        private readonly Guid _homeTeamId = Guid.NewGuid();
        private readonly Guid _awayTeamId = Guid.NewGuid();
        private readonly Guid _thirdTeamId = Guid.NewGuid();
        private readonly Guid _referee1Id = Guid.NewGuid();
        private readonly Guid _referee2Id = Guid.NewGuid();
        private const TournamentMsgs.AgeBracket AgeBracket = TournamentMsgs.AgeBracket.U14;
        private readonly DateTime _startTime = new DateTime(2020, 6, 1, 9, 0, 0);
        private readonly DateTime _endTime = new DateTime(2020, 6, 1, 10, 0, 0);

        private Tournament AddTournament()
        {
            var tournament = new Tournament(
                                    _tournamentId,
                                    "The Milk Cup",
                                    new DateTime(2020, 6, 1),
                                    new DateTime(2020, 6, 1),
                                    MessageBuilder.New(() => new TestCommands.Command1()));
            tournament.AddField(
                _fieldId,
                "Field 1");
            tournament.AddTeamToTournament(_homeTeamId, AgeBracket);
            tournament.AddTeamToTournament(_awayTeamId, AgeBracket);
            tournament.AddTeamToTournament(_thirdTeamId, AgeBracket);
            tournament.AddReferee(_referee1Id);
            tournament.AddReferee(_referee2Id);
            // Take events and reset the Source so we can continue to use the aggregate as "pre-hydrated"
            tournament.TakeEvents();
            ((ICorrelatedEventSource)tournament).Source = MessageBuilder.New(() => new TestCommands.Command1());
            return tournament;
        }

        private Tournament AddTournamentWithGame()
        {
            var tournament = AddTournament();
            tournament.AddGame(
                _gameId,
                _fieldId,
                _startTime,
                _endTime,
                _homeTeamId,
                _awayTeamId);
            // Take events and reset the Source so we can continue to use the aggregate as "pre-hydrated"
            tournament.TakeEvents();
            ((ICorrelatedEventSource)tournament).Source = MessageBuilder.New(() => new TestCommands.Command1());
            return tournament;
        }

        [Fact]
        public void can_add_game()
        {
            var tournament = AddTournament();
            tournament.AddGame(
                _gameId,
                _fieldId,
                _startTime,
                _endTime,
                _homeTeamId,
                _awayTeamId);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is GameMsgs.GameAdded evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.GameId == _gameId &&
                                 evt.FieldId == _fieldId &&
                                 evt.StartTime == _startTime &&
                                 evt.EndTime == _endTime &&
                                 evt.HomeTeamId == _homeTeamId &&
                                 evt.AwayTeamId == _awayTeamId));
        }

        [Fact]
        public void cannot_add_game_to_invalid_field()
        {
            var tournament = AddTournament();
            Assert.Throws<ArgumentException>(
                () => tournament.AddGame(
                        _gameId,
                        Guid.Empty,
                        _startTime,
                        _endTime,
                        _homeTeamId,
                        _awayTeamId));
            Assert.Throws<ArgumentException>(
                () => tournament.AddGame(
                        _gameId,
                        Guid.NewGuid(),
                        _startTime,
                        _endTime,
                        _homeTeamId,
                        _awayTeamId));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void cannot_add_game_with_invalid_id()
        {
            var tournament = AddTournament();
            Assert.Throws<ArgumentException>(
                () => tournament.AddGame(
                        Guid.Empty,
                        _fieldId,
                        _startTime,
                        _endTime,
                        _homeTeamId,
                        _awayTeamId));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void cannot_add_game_with_duplicate_id()
        {
            var tournament = AddTournament();
            tournament.AddGame(
                _gameId,
                _fieldId,
                _startTime,
                _endTime,
                _homeTeamId,
                _awayTeamId);
            Assert.Throws<ArgumentException>(
                () => tournament.AddGame(
                        _gameId,
                        _fieldId,
                        _startTime,
                        _endTime,
                        _homeTeamId,
                        _awayTeamId));
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.IsType<GameMsgs.GameAdded>(e));
        }

        [Fact]
        public void cannot_add_game_with_same_start_and_end_times()
        {
            var tournament = AddTournament();
            Assert.Throws<ArgumentException>(
                () => tournament.AddGame(
                        Guid.Empty,
                        _fieldId,
                        _startTime,
                        _startTime,
                        _homeTeamId,
                        _awayTeamId));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void cannot_add_game_with_start_time_after_end_time()
        {
            var tournament = AddTournament();
            Assert.Throws<ArgumentException>(
                () => tournament.AddGame(
                        Guid.Empty,
                        _fieldId,
                        _endTime,
                        _startTime,
                        _homeTeamId,
                        _awayTeamId));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void cannot_add_game_outside_tournament_dates()
        {
            var tournament = AddTournament();
            Assert.Throws<ArgumentException>(
                () => tournament.AddGame(
                        Guid.Empty,
                        _fieldId,
                        _startTime - TimeSpan.FromDays(1),
                        _endTime - TimeSpan.FromDays(1),
                        _homeTeamId,
                        _awayTeamId));
            Assert.Throws<ArgumentException>(
                () => tournament.AddGame(
                        Guid.Empty,
                        _fieldId,
                        _startTime + TimeSpan.FromDays(1),
                        _endTime + TimeSpan.FromDays(1),
                        _homeTeamId,
                        _awayTeamId));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void cannot_add_game_with_invalid_home_team()
        {
            var tournament = AddTournament();
            Assert.Throws<ArgumentException>(
                () => tournament.AddGame(
                        _gameId,
                        _fieldId,
                        _startTime,
                        _endTime,
                        Guid.Empty,
                        _awayTeamId));
            Assert.Throws<ArgumentException>(
                () => tournament.AddGame(
                        _gameId,
                        _fieldId,
                        _startTime,
                        _endTime,
                        Guid.NewGuid(),
                        _awayTeamId));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void cannot_add_game_with_invalid_away_team()
        {
            var tournament = AddTournament();
            Assert.Throws<ArgumentException>(
                () => tournament.AddGame(
                        _gameId,
                        _fieldId,
                        _startTime,
                        _endTime,
                        _homeTeamId,
                        Guid.Empty));
            Assert.Throws<ArgumentException>(
                () => tournament.AddGame(
                        _gameId,
                        _fieldId,
                        _startTime,
                        _endTime,
                        _homeTeamId,
                        Guid.NewGuid()));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void can_cancel_game()
        {
            var tournament = AddTournamentWithGame();
            tournament.CancelGame(_gameId);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is GameMsgs.GameCancelled evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.GameId == _gameId));
        }

        [Fact]
        public void cannot_cancel_game_with_empty_id()
        {
            var tournament = AddTournamentWithGame();
            Assert.Throws<ArgumentException>(() => tournament.CancelGame(Guid.Empty));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void cancelling_nonexistent_game_succeeds_implicitly()
        {
            var tournament = AddTournamentWithGame();
            tournament.CancelGame(Guid.NewGuid());
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void can_reschedule_game()
        {
            var newStart = _startTime + TimeSpan.FromHours(1);
            var newEnd = _endTime + TimeSpan.FromHours(1);
            var tournament = AddTournamentWithGame();
            tournament.RescheduleGame(
                _gameId,
                newStart,
                newEnd);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is GameMsgs.GameRescheduled evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.GameId == _gameId &&
                                 evt.StartTime == newStart &&
                                 evt.EndTime == newEnd));
        }

        [Fact]
        public void reschedule_game_is_idempotent()
        {
            var newStart = _startTime + TimeSpan.FromHours(1);
            var newEnd = _endTime + TimeSpan.FromHours(1);
            var tournament = AddTournamentWithGame();
            // rescheduling to initial times should not raise a new event.
            tournament.RescheduleGame(
                _gameId,
                _startTime,
                _endTime);
            // rescheduling to new times SHOULD raise a new event.
            tournament.RescheduleGame(
                _gameId,
                newStart,
                newEnd);
            // rescheduling identically should not raise a new event.
            tournament.RescheduleGame(
                _gameId,
                newStart,
                newEnd);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is GameMsgs.GameRescheduled evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.GameId == _gameId &&
                                 evt.StartTime == newStart &&
                                 evt.EndTime == newEnd));
        }

        [Fact]
        public void cannot_reschedule_game_with_invalid_game_id()
        {
            var newStart = _startTime + TimeSpan.FromHours(1);
            var newEnd = _endTime + TimeSpan.FromHours(1);
            var tournament = AddTournamentWithGame();
            Assert.Throws<ArgumentException>(
                () => tournament.RescheduleGame(
                        Guid.NewGuid(),
                        newStart,
                        newEnd));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void cannot_reschedule_game_with_empty_game_id()
        {
            var newStart = _startTime + TimeSpan.FromHours(1);
            var newEnd = _endTime + TimeSpan.FromHours(1);
            var tournament = AddTournamentWithGame();
            Assert.Throws<ArgumentException>(
                () => tournament.RescheduleGame(
                        Guid.Empty,
                        newStart,
                        newEnd));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void cannot_reschedule_game_with_same_start_and_end_times()
        {
            var newTime = _startTime + TimeSpan.FromHours(1);
            var tournament = AddTournamentWithGame();
            Assert.Throws<ArgumentException>(
                () => tournament.RescheduleGame(
                        Guid.NewGuid(),
                        newTime,
                        newTime));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void cannot_reschedule_game_with_start_time_after_end_time()
        {
            var newStart = _startTime + TimeSpan.FromHours(1);
            var newEnd = _endTime + TimeSpan.FromHours(1);
            var tournament = AddTournamentWithGame();
            Assert.Throws<ArgumentException>(
                () => tournament.RescheduleGame(
                        Guid.NewGuid(),
                        newEnd,
                        newStart));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void cannot_reschedule_game_outside_tournament_dates()
        {
            var tournament = AddTournamentWithGame();
            Assert.Throws<ArgumentException>(
                () => tournament.RescheduleGame(
                        Guid.NewGuid(),
                        _startTime - TimeSpan.FromDays(1),
                        _endTime - TimeSpan.FromDays(1)));
            Assert.Throws<ArgumentException>(
                () => tournament.RescheduleGame(
                        Guid.NewGuid(),
                        _startTime + TimeSpan.FromDays(1),
                        _endTime + TimeSpan.FromDays(1)));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void can_update_home_team()
        {
            var tournament = AddTournamentWithGame();
            tournament.UpdateHomeTeam(
                _gameId,
                _thirdTeamId);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is GameMsgs.HomeTeamUpdated evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.GameId == _gameId &&
                                 evt.HomeTeamId == _thirdTeamId));
        }

        [Fact]
        public void cannot_update_home_team_for_nonexistent_game()
        {
            var tournament = AddTournamentWithGame();
            Assert.Throws<ArgumentException>(
                () => tournament.UpdateHomeTeam(
                        Guid.Empty, 
                        _thirdTeamId));
            Assert.Throws<ArgumentException>(
                () => tournament.UpdateHomeTeam(
                        Guid.NewGuid(),
                        _thirdTeamId));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void cannot_update_home_team_with_invalid_team_id()
        {
            var tournament = AddTournamentWithGame();
            Assert.Throws<ArgumentException>(
                () => tournament.UpdateHomeTeam(
                        _gameId,
                        Guid.Empty));
            Assert.Throws<ArgumentException>(
                () => tournament.UpdateHomeTeam(
                        _gameId,
                        Guid.NewGuid()));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void can_update_away_team()
        {
            var tournament = AddTournamentWithGame();
            tournament.UpdateAwayTeam(
                _gameId,
                _thirdTeamId);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is GameMsgs.AwayTeamUpdated evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.GameId == _gameId &&
                                 evt.AwayTeamId == _thirdTeamId));
        }

        [Fact]
        public void cannot_update_away_team_for_nonexistent_game()
        {
            var tournament = AddTournamentWithGame();
            Assert.Throws<ArgumentException>(
                () => tournament.UpdateAwayTeam(
                        Guid.Empty,
                        _thirdTeamId));
            Assert.Throws<ArgumentException>(
                () => tournament.UpdateAwayTeam(
                        Guid.NewGuid(),
                        _thirdTeamId));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void cannot_update_away_team_with_invalid_team_id()
        {
            var tournament = AddTournamentWithGame();
            Assert.Throws<ArgumentException>(
                () => tournament.UpdateAwayTeam(
                        _gameId,
                        Guid.Empty));
            Assert.Throws<ArgumentException>(
                () => tournament.UpdateAwayTeam(
                        _gameId,
                        Guid.NewGuid()));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void can_assign_referee_to_game()
        {
            var tournament = AddTournamentWithGame();
            tournament.AssignRefereeToGame(
                _gameId,
                _referee1Id);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is GameMsgs.RefereeAssigned evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.GameId == _gameId &&
                                 evt.RefereeId == _referee1Id));
        }

        [Fact]
        public void can_change_referee_without_removing_existing_one()
        {
            var tournament = AddTournamentWithGame();
            tournament.AssignRefereeToGame(
                _gameId,
                _referee1Id);
            tournament.AssignRefereeToGame(
                _gameId,
                _referee2Id);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is GameMsgs.RefereeAssigned evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.GameId == _gameId &&
                                 evt.RefereeId == _referee1Id),
                e => Assert.True(e is GameMsgs.RefereeAssigned evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.GameId == _gameId &&
                                 evt.RefereeId == _referee2Id));
        }

        [Fact]
        public void cannot_assign_referee_to_nonexistent_game()
        {
            var tournament = AddTournamentWithGame();
            Assert.Throws<ArgumentException>(
                () => tournament.AssignRefereeToGame(
                        Guid.Empty, 
                        _referee1Id));
            Assert.Throws<ArgumentException>(
                () => tournament.AssignRefereeToGame(
                        Guid.NewGuid(),
                        _referee1Id));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void cannot_assign_referee_with_invalid_id()
        {
            var tournament = AddTournamentWithGame();
            Assert.Throws<ArgumentException>(
                () => tournament.AssignRefereeToGame(
                        _gameId,
                        Guid.Empty));
            Assert.Throws<ArgumentException>(
                () => tournament.AssignRefereeToGame(
                        _gameId,
                        Guid.NewGuid()));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void can_remove_assigned_referee_from_game()
        {
            var tournament = AddTournamentWithGame();
            tournament.AssignRefereeToGame(
                _gameId,
                _referee1Id);
            tournament.TakeEvents();
            ((ICorrelatedEventSource)tournament).Source = MessageBuilder.New(() => new TestCommands.Command1());
            tournament.RemoveRefereeFromGame(_gameId);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is GameMsgs.RefereeRemoved evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.GameId == _gameId));
        }

        [Fact]
        public void remove_referee_from_unassigned_game_succeeds_implicitly()
        {
            var tournament = AddTournamentWithGame();
            tournament.RemoveRefereeFromGame(_gameId);
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void cannot_remove_referee_from_nonexistent_game()
        {
            var tournament = AddTournamentWithGame();
            Assert.Throws<ArgumentException>(() => tournament.RemoveRefereeFromGame(Guid.Empty));
            Assert.Throws<ArgumentException>(() => tournament.RemoveRefereeFromGame(Guid.NewGuid()));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void can_confirm_referee_assignment()
        {
            var tournament = AddTournamentWithGame();
            tournament.AssignRefereeToGame(
                _gameId,
                _referee1Id);
            tournament.TakeEvents();
            ((ICorrelatedEventSource)tournament).Source = MessageBuilder.New(() => new TestCommands.Command1());
            tournament.ConfirmRefereeForGame(_gameId);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is GameMsgs.RefereeConfirmed evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.GameId == _gameId));
        }

        [Fact]
        public void cannot_confirm_referee_for_unassigned_game()
        {
            var tournament = AddTournamentWithGame();
            Assert.Throws<ArgumentException>(() => tournament.ConfirmRefereeForGame(_gameId));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void cannot_confirm_referee_assignment_for_nonexistent_game()
        {
            var tournament = AddTournamentWithGame();
            Assert.Throws<ArgumentException>(() => tournament.ConfirmRefereeForGame(Guid.Empty));
            Assert.Throws<ArgumentException>(() => tournament.ConfirmRefereeForGame(Guid.NewGuid()));
            Assert.False(tournament.HasRecordedEvents);
        }
    }
}
