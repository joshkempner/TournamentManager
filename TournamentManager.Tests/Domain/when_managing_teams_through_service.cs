using System;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using ReactiveDomain.Testing;
using TournamentManager.Domain;
using TournamentManager.Messages;
using Xunit;

namespace TournamentManager.Tests.Domain
{
    public sealed class when_managing_teams_through_service : with_tournament_service
    {
        private readonly Guid _teamId = Guid.NewGuid();
        private const string TeamName = "Springfield United";
        private const TeamMsgs.AgeBracket AgeBracket = TeamMsgs.AgeBracket.U14;

        public when_managing_teams_through_service()
        {
            AddTournament();
        }

        private void AddTeam()
        {
            var repo = new CorrelatedStreamStoreRepository(Fixture.Repository);
            var tournament = repo.GetById<Tournament>(TournamentId, MessageBuilder.New(() => new TestCommands.Command1()));
            tournament.AddTeam(
                _teamId,
                TeamName,
                AgeBracket);
            repo.Save(tournament);
            Fixture.RepositoryEvents.WaitFor<TeamMsgs.TeamAdded>(TimeSpan.FromMilliseconds(200));
            Fixture.ClearQueues();
        }

        [Fact]
        public void can_add_team()
        {
            var cmd = MessageBuilder.New(() => new TeamMsgs.AddTeam(
                                                    TournamentId,
                                                    _teamId,
                                                    TeamName,
                                                    AgeBracket));
            Fixture.Dispatcher.Send(cmd);
            Fixture.RepositoryEvents.WaitFor<TeamMsgs.TeamAdded>(TimeSpan.FromMilliseconds(200));
            Fixture
                .TestQueue
                .AssertNext<TeamMsgs.AddTeam>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture
                .RepositoryEvents
                .AssertNext<TeamMsgs.TeamAdded>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(TournamentId, evt.TournamentId);
            Assert.Equal(_teamId, evt.TeamId);
            Assert.Equal(TeamName, evt.Name);
            Assert.Equal(AgeBracket, evt.AgeBracket);
        }

        [Fact]
        public void cannot_add_team_to_invalid_tournament()
        {
            var cmd = MessageBuilder.New(() => new TeamMsgs.AddTeam(
                                                    Guid.NewGuid(),
                                                    _teamId,
                                                    TeamName,
                                                    AgeBracket));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => Fixture.Dispatcher.Send(cmd));
            Fixture
                .TestQueue
                .AssertNext<TeamMsgs.AddTeam>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_remove_team()
        {
            AddTeam();
            var cmd = MessageBuilder.New(() => new TeamMsgs.RemoveTeam(
                                                    TournamentId,
                                                    _teamId));
            Fixture.Dispatcher.Send(cmd);
            Fixture.RepositoryEvents.WaitFor<TeamMsgs.TeamRemoved>(TimeSpan.FromMilliseconds(200));
            Fixture
                .TestQueue
                .AssertNext<TeamMsgs.RemoveTeam>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture
                .RepositoryEvents
                .AssertNext<TeamMsgs.TeamRemoved>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(TournamentId, evt.TournamentId);
            Assert.Equal(_teamId, evt.TeamId);
        }

        [Fact]
        public void cannot_remove_team_from_invalid_tournament()
        {
            AddTeam();
            var cmd = MessageBuilder.New(() => new TeamMsgs.RemoveTeam(
                                                    Guid.NewGuid(),
                                                    _teamId));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => Fixture.Dispatcher.Send(cmd));
            Fixture
                .TestQueue
                .AssertNext<TeamMsgs.RemoveTeam>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_rename_team()
        {
            const string newName = "Springfield Divided";
            AddTeam();
            var cmd = MessageBuilder.New(() => new TeamMsgs.RenameTeam(
                                                    TournamentId,
                                                    _teamId,
                                                    newName));
            Fixture.Dispatcher.Send(cmd);
            Fixture.RepositoryEvents.WaitFor<TeamMsgs.TeamRenamed>(TimeSpan.FromMilliseconds(200));
            Fixture
                .TestQueue
                .AssertNext<TeamMsgs.RenameTeam>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture
                .RepositoryEvents
                .AssertNext<TeamMsgs.TeamRenamed>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(TournamentId, evt.TournamentId);
            Assert.Equal(_teamId, evt.TeamId);
            Assert.Equal(newName, evt.Name);
        }

        [Fact]
        public void cannot_rename_team_from_invalid_tournament()
        {
            const string newName = "Springfield Divided";
            AddTeam();
            var cmd = MessageBuilder.New(() => new TeamMsgs.RenameTeam(
                                                    Guid.NewGuid(),
                                                    _teamId,
                                                    newName));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => Fixture.Dispatcher.Send(cmd));
            Fixture
                .TestQueue
                .AssertNext<TeamMsgs.RenameTeam>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_update_team_age_bracket()
        {
            const TeamMsgs.AgeBracket newBracket = TeamMsgs.AgeBracket.U16;
            AddTeam();
            var cmd = MessageBuilder.New(() => new TeamMsgs.UpdateAgeBracket(
                                                    TournamentId,
                                                    _teamId,
                                                    newBracket));
            Fixture.Dispatcher.Send(cmd);
            Fixture.RepositoryEvents.WaitFor<TeamMsgs.AgeBracketUpdated>(TimeSpan.FromMilliseconds(200));
            Fixture
                .TestQueue
                .AssertNext<TeamMsgs.UpdateAgeBracket>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture
                .RepositoryEvents
                .AssertNext<TeamMsgs.AgeBracketUpdated>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(TournamentId, evt.TournamentId);
            Assert.Equal(_teamId, evt.TeamId);
            Assert.Equal(newBracket, evt.AgeBracket);
        }

        [Fact]
        public void cannot_update_team_age_bracket_from_invalid_tournament()
        {
            const TeamMsgs.AgeBracket newBracket = TeamMsgs.AgeBracket.U16;
            AddTeam();
            var cmd = MessageBuilder.New(() => new TeamMsgs.UpdateAgeBracket(
                                                    Guid.NewGuid(),
                                                    _teamId,
                                                    newBracket));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => Fixture.Dispatcher.Send(cmd));
            Fixture
                .TestQueue
                .AssertNext<TeamMsgs.UpdateAgeBracket>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }
    }
}
