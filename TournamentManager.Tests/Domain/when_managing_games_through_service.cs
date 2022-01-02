using System;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using ReactiveDomain.Testing;
using TournamentManager.Domain;
using TournamentManager.Messages;
using Xunit;

namespace TournamentManager.Tests.Domain
{
    public sealed class when_managing_games_through_service : with_tournament_service
    {
        private readonly Guid _gameId = Guid.NewGuid();
        private readonly Guid _gameSlotId = Guid.NewGuid();
        private readonly Guid _fieldId = Guid.NewGuid();
        private readonly Guid _homeTeamId = Guid.NewGuid();
        private readonly Guid _awayTeamId = Guid.NewGuid();
        private readonly Guid _thirdTeamId = Guid.NewGuid();
        private readonly Guid _referee1Id = Guid.NewGuid();
        private readonly Guid _referee2Id = Guid.NewGuid();
        private const TournamentMsgs.AgeBracket AgeBracket = TournamentMsgs.AgeBracket.U14;

        public when_managing_games_through_service()
        {
            AddTournament();
            AddTournamentDetails();
        }

        private void AddTournamentDetails()
        {
            var repo = new CorrelatedStreamStoreRepository(Fixture.Repository);
            var tournament = repo.GetById<Tournament>(TournamentId, MessageBuilder.New(() => new TestCommands.Command1()));
            tournament.AddField(
                _fieldId,
                "Field 1");
            tournament.AddGameSlot(
                _gameSlotId,
                new DateTime(2020, 6, 1, 9, 0, 0),
                new DateTime(2020, 6, 1, 10, 0, 0));
            tournament.AddTeamToTournament(_homeTeamId, AgeBracket);
            tournament.AddTeamToTournament(_awayTeamId, AgeBracket);
            tournament.AddTeamToTournament(_thirdTeamId, AgeBracket);
            tournament.AddReferee(_referee1Id);
            tournament.AddReferee(_referee2Id);
            repo.Save(tournament);
            Fixture.RepositoryEvents.WaitForMultiple<TournamentMsgs.RefereeAddedToTournament>(2, TimeSpan.FromMilliseconds(200));
            Fixture.ClearQueues();
        }

        private void AddGame()
        {
            var repo = new CorrelatedStreamStoreRepository(Fixture.Repository);
            var tournament = repo.GetById<Tournament>(TournamentId, MessageBuilder.New(() => new TestCommands.Command1()));
            tournament.AddGame(
                _gameId,
                _gameSlotId,
                _fieldId,
                _homeTeamId,
                _awayTeamId);
            repo.Save(tournament);
            Fixture.RepositoryEvents.WaitFor<GameMsgs.GameAdded>(TimeSpan.FromMilliseconds(200));
            Fixture.ClearQueues();
        }

        private void AddGameWithReferee()
        {
            var repo = new CorrelatedStreamStoreRepository(Fixture.Repository);
            var tournament = repo.GetById<Tournament>(TournamentId, MessageBuilder.New(() => new TestCommands.Command1()));
            tournament.AddGame(
                _gameId,
                _gameSlotId,
                _fieldId,
                _homeTeamId,
                _awayTeamId);
            tournament.AssignRefereeToGame(
                _gameId,
                _referee1Id);
            repo.Save(tournament);
            Fixture.RepositoryEvents.WaitFor<GameMsgs.RefereeAssigned>(TimeSpan.FromMilliseconds(200));
            Fixture.ClearQueues();
        }

        [Fact]
        public void can_add_game()
        {
            var cmd = MessageBuilder.New(() => new GameMsgs.AddGame(
                                                    TournamentId,
                                                    _gameId,
                                                    _gameSlotId,
                                                    _fieldId,
                                                    _homeTeamId,
                                                    _awayTeamId));
            Fixture.Dispatcher.Send(cmd);
            Fixture.RepositoryEvents.WaitFor<GameMsgs.GameAdded>(TimeSpan.FromMilliseconds(200));
            Fixture
                .TestQueue
                .AssertNext<GameMsgs.AddGame>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture
                .RepositoryEvents
                .AssertNext<GameMsgs.GameAdded>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(TournamentId, evt.TournamentId);
            Assert.Equal(_gameId, evt.GameId);
            Assert.Equal(_gameSlotId, evt.GameSlotId);
            Assert.Equal(_fieldId, evt.FieldId);
            Assert.Equal(_homeTeamId, evt.HomeTeamId);
            Assert.Equal(_awayTeamId, evt.AwayTeamId);
        }

        [Fact]
        public void cannot_add_game_to_invalid_tournament()
        {
            var cmd = MessageBuilder.New(() => new GameMsgs.AddGame(
                                                    Guid.NewGuid(),
                                                    _gameId,
                                                    _gameSlotId,
                                                    _fieldId,
                                                    _homeTeamId,
                                                    _awayTeamId));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => Fixture.Dispatcher.Send(cmd));
            Fixture
                .TestQueue
                .AssertNext<GameMsgs.AddGame>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_cancel_game()
        {
            AddGame();
            var cmd = MessageBuilder.New(() => new GameMsgs.CancelGame(
                                                    TournamentId,
                                                    _gameId));
            Fixture.Dispatcher.Send(cmd);
            Fixture.RepositoryEvents.WaitFor<GameMsgs.GameCancelled>(TimeSpan.FromMilliseconds(200));
            Fixture
                .TestQueue
                .AssertNext<GameMsgs.CancelGame>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture
                .RepositoryEvents
                .AssertNext<GameMsgs.GameCancelled>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(TournamentId, evt.TournamentId);
            Assert.Equal(_gameId, evt.GameId);
        }

        [Fact]
        public void cannot_cancel_game_in_invalid_tournament()
        {
            var cmd = MessageBuilder.New(() => new GameMsgs.CancelGame(
                                                    Guid.NewGuid(),
                                                    _gameId));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => Fixture.Dispatcher.Send(cmd));
            Fixture
                .TestQueue
                .AssertNext<GameMsgs.CancelGame>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_update_home_team()
        {
            AddGame();
            var cmd = MessageBuilder.New(() => new GameMsgs.UpdateHomeTeam(
                                                    TournamentId,
                                                    _gameId,
                                                    _thirdTeamId));
            Fixture.Dispatcher.Send(cmd);
            Fixture.RepositoryEvents.WaitFor<GameMsgs.HomeTeamUpdated>(TimeSpan.FromMilliseconds(200));
            Fixture
                .TestQueue
                .AssertNext<GameMsgs.UpdateHomeTeam>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture
                .RepositoryEvents
                .AssertNext<GameMsgs.HomeTeamUpdated>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(TournamentId, evt.TournamentId);
            Assert.Equal(_gameId, evt.GameId);
            Assert.Equal(_thirdTeamId, evt.HomeTeamId);
        }

        [Fact]
        public void cannot_update_home_team_in_invalid_tournament()
        {
            var cmd = MessageBuilder.New(() => new GameMsgs.UpdateHomeTeam(
                                                    Guid.NewGuid(),
                                                    _gameId,
                                                    _thirdTeamId));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => Fixture.Dispatcher.Send(cmd));
            Fixture
                .TestQueue
                .AssertNext<GameMsgs.UpdateHomeTeam>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_update_away_team()
        {
            AddGame();
            var cmd = MessageBuilder.New(() => new GameMsgs.UpdateAwayTeam(
                                                    TournamentId,
                                                    _gameId,
                                                    _thirdTeamId));
            Fixture.Dispatcher.Send(cmd);
            Fixture.RepositoryEvents.WaitFor<GameMsgs.AwayTeamUpdated>(TimeSpan.FromMilliseconds(200));
            Fixture
                .TestQueue
                .AssertNext<GameMsgs.UpdateAwayTeam>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture
                .RepositoryEvents
                .AssertNext<GameMsgs.AwayTeamUpdated>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(TournamentId, evt.TournamentId);
            Assert.Equal(_gameId, evt.GameId);
            Assert.Equal(_thirdTeamId, evt.AwayTeamId);
        }

        [Fact]
        public void cannot_update_away_team_in_invalid_tournament()
        {
            var cmd = MessageBuilder.New(() => new GameMsgs.UpdateAwayTeam(
                                                    Guid.NewGuid(),
                                                    _gameId,
                                                    _thirdTeamId));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => Fixture.Dispatcher.Send(cmd));
            Fixture
                .TestQueue
                .AssertNext<GameMsgs.UpdateAwayTeam>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_assign_referee_to_game()
        {
            AddGame();
            var cmd = MessageBuilder.New(() => new GameMsgs.AssignReferee(
                                                    TournamentId,
                                                    _gameId,
                                                    _referee1Id));
            Fixture.Dispatcher.Send(cmd);
            Fixture.RepositoryEvents.WaitFor<GameMsgs.RefereeAssigned>(TimeSpan.FromMilliseconds(200));
            Fixture
                .TestQueue
                .AssertNext<GameMsgs.AssignReferee>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture
                .RepositoryEvents
                .AssertNext<GameMsgs.RefereeAssigned>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(TournamentId, evt.TournamentId);
            Assert.Equal(_gameId, evt.GameId);
            Assert.Equal(_referee1Id, evt.RefereeId);
        }

        [Fact]
        public void cannot_assign_referee_to_game_in_invalid_tournament()
        {
            var cmd = MessageBuilder.New(() => new GameMsgs.AssignReferee(
                                                    Guid.NewGuid(),
                                                    _gameId,
                                                    _referee1Id));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => Fixture.Dispatcher.Send(cmd));
            Fixture
                .TestQueue
                .AssertNext<GameMsgs.AssignReferee>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_confirm_referee_assignment()
        {
            AddGameWithReferee();
            var cmd = MessageBuilder.New(() => new GameMsgs.ConfirmReferee(
                                                    TournamentId,
                                                    _gameId));
            Fixture.Dispatcher.Send(cmd);
            Fixture.RepositoryEvents.WaitFor<GameMsgs.RefereeConfirmed>(TimeSpan.FromMilliseconds(200));
            Fixture
                .TestQueue
                .AssertNext<GameMsgs.ConfirmReferee>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture
                .RepositoryEvents
                .AssertNext<GameMsgs.RefereeConfirmed>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(TournamentId, evt.TournamentId);
            Assert.Equal(_gameId, evt.GameId);
        }

        [Fact]
        public void cannot_confirm_referee_assignment_in_invalid_tournament()
        {
            var cmd = MessageBuilder.New(() => new GameMsgs.ConfirmReferee(
                                                    Guid.NewGuid(),
                                                    _gameId));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => Fixture.Dispatcher.Send(cmd));
            Fixture
                .TestQueue
                .AssertNext<GameMsgs.ConfirmReferee>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_remove_referee_from_game()
        {
            AddGameWithReferee();
            var cmd = MessageBuilder.New(() => new GameMsgs.RemoveReferee(
                                                    TournamentId,
                                                    _gameId));
            Fixture.Dispatcher.Send(cmd);
            Fixture.RepositoryEvents.WaitFor<GameMsgs.RefereeRemoved>(TimeSpan.FromMilliseconds(200));
            Fixture
                .TestQueue
                .AssertNext<GameMsgs.RemoveReferee>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture
                .RepositoryEvents
                .AssertNext<GameMsgs.RefereeRemoved>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(TournamentId, evt.TournamentId);
            Assert.Equal(_gameId, evt.GameId);
        }

        [Fact]
        public void cannot_remove_referee_from_game_in_invalid_tournament()
        {
            var cmd = MessageBuilder.New(() => new GameMsgs.RemoveReferee(
                                                    Guid.NewGuid(),
                                                    _gameId));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => Fixture.Dispatcher.Send(cmd));
            Fixture
                .TestQueue
                .AssertNext<GameMsgs.RemoveReferee>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }
    }
}
