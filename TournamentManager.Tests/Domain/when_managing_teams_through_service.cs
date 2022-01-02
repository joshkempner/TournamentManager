using System;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using ReactiveDomain.Testing;
using TournamentManager.Messages;
using Xunit;

namespace TournamentManager.Tests.Domain
{
    public sealed class when_managing_teams_through_service : with_team_service
    {
        [Fact]
        public void can_add_team()
        {
            var cmd = MessageBuilder.New(() => new TeamMsgs.CreateTeam(
                                                    TeamId,
                                                    TeamName));
            Fixture.Dispatcher.Send(cmd);
            Fixture.RepositoryEvents.WaitFor<TeamMsgs.TeamCreated>(TimeSpan.FromMilliseconds(200));
            Fixture
                .TestQueue
                .AssertNext<TeamMsgs.CreateTeam>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture
                .RepositoryEvents
                .AssertNext<TeamMsgs.TeamCreated>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(TeamId, evt.TeamId);
            Assert.Equal(TeamName, evt.Name);
        }

        [Fact]
        public void cannot_create_team_with_duplicate_id()
        {
            AddTeam();
            var cmd = MessageBuilder.New(() => new TeamMsgs.CreateTeam(
                                                    TeamId,
                                                    TeamName));
            AssertEx.CommandThrows<Exception>(() => Fixture.Dispatcher.Send(cmd));
            Fixture
                .TestQueue
                .AssertNext<TeamMsgs.CreateTeam>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_delete_team()
        {
            AddTeam();
            var cmd = MessageBuilder.New(() => new TeamMsgs.DeleteTeam(TeamId));
            Fixture.Dispatcher.Send(cmd);
            Fixture.RepositoryEvents.WaitFor<TeamMsgs.TeamDeleted>(TimeSpan.FromMilliseconds(200));
            Fixture
                .TestQueue
                .AssertNext<TeamMsgs.DeleteTeam>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture
                .RepositoryEvents
                .AssertNext<TeamMsgs.TeamDeleted>(cmd.CorrelationId, out var evt)
                .AssertEmpty();
            Assert.Equal(TeamId, evt.TeamId);
        }

        [Fact]
        public void deleting_nonexistent_team_succeeds_implicitly()
        {
            var cmd = MessageBuilder.New(() => new TeamMsgs.DeleteTeam(TeamId));
            Fixture.Dispatcher.Send(cmd);
            Fixture
                .TestQueue
                .AssertNext<TeamMsgs.DeleteTeam>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }

        [Fact]
        public void can_rename_team()
        {
            const string newName = "Springfield Divided";
            AddTeam();
            var cmd = MessageBuilder.New(() => new TeamMsgs.RenameTeam(
                                                    TeamId,
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
            Assert.Equal(TeamId, evt.TeamId);
            Assert.Equal(newName, evt.Name);
        }

        [Fact]
        public void cannot_rename_nonexistent_team()
        {
            const string newName = "Springfield Divided";
            var cmd = MessageBuilder.New(() => new TeamMsgs.RenameTeam(
                                                    TeamId,
                                                    newName));
            AssertEx.CommandThrows<AggregateNotFoundException>(() => Fixture.Dispatcher.Send(cmd));
            Fixture
                .TestQueue
                .AssertNext<TeamMsgs.RenameTeam>(cmd.CorrelationId)
                .AssertEmpty();
            Fixture.RepositoryEvents.AssertEmpty();
        }
    }
}
