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
        private readonly Guid _gameSlotId = Guid.NewGuid();
        private readonly Guid _fieldId = Guid.NewGuid();
        private const TeamMsgs.AgeBracket AgeBracket = TeamMsgs.AgeBracket.U10;
        private readonly Guid _homeTeamId = Guid.NewGuid();
        private readonly Guid _awayTeamId = Guid.NewGuid();
        private readonly Guid _thirdTeamId = Guid.NewGuid();
        private readonly Guid _referee1Id = Guid.NewGuid();
        private readonly Guid _referee2Id = Guid.NewGuid();

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
            tournament.AddGameSlot(
                _gameSlotId,
                new DateTime(2020, 6, 1, 9, 0, 0),
                new DateTime(2020, 6, 1, 10, 0, 0));
            tournament.AddTeam(
                _homeTeamId,
                "Team 1",
                AgeBracket);
            tournament.AddTeam(
                _awayTeamId,
                "Team 2",
                AgeBracket);
            tournament.AddTeam(
                _thirdTeamId,
                "Team 3",
                AgeBracket);
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
                _gameSlotId,
                _fieldId,
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
                _gameSlotId,
                _fieldId,
                _homeTeamId,
                _awayTeamId);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is GameMsgs.GameAdded evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.GameId == _gameId &&
                                 evt.GameSlotId == _gameSlotId &&
                                 evt.FieldId == _fieldId &&
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
                        _gameSlotId,
                        Guid.Empty,
                        _homeTeamId,
                        _awayTeamId));
            Assert.Throws<ArgumentException>(
                () => tournament.AddGame(
                        _gameId,
                        _gameSlotId,
                        Guid.NewGuid(),
                        _homeTeamId,
                        _awayTeamId));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void cannot_add_game_to_invalid_game_slot()
        {
            var tournament = AddTournament();
            Assert.Throws<ArgumentException>(
                () => tournament.AddGame(
                        _gameId,
                        Guid.Empty,
                        _fieldId,
                        _homeTeamId,
                        _awayTeamId));
            Assert.Throws<ArgumentException>(
                () => tournament.AddGame(
                        _gameId,
                        Guid.NewGuid(),
                        _fieldId,
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
                        _gameSlotId,
                        _fieldId,
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
                        _gameSlotId,
                        _fieldId,
                        Guid.Empty,
                        _awayTeamId));
            Assert.Throws<ArgumentException>(
                () => tournament.AddGame(
                        _gameId,
                        _gameSlotId,
                        _fieldId,
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
                        _gameSlotId,
                        _fieldId,
                        _homeTeamId,
                        Guid.Empty));
            Assert.Throws<ArgumentException>(
                () => tournament.AddGame(
                        _gameId,
                        _gameSlotId,
                        _fieldId,
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
