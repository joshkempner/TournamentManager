using System;
using ReactiveDomain;
using ReactiveDomain.Messaging;
using ReactiveDomain.Testing;
using TournamentManager.Domain;
using TournamentManager.Messages;
using Xunit;

namespace TournamentManager.Tests.Domain
{
    public sealed class when_managing_teams
    {
        private readonly Guid _teamId = Guid.NewGuid();
        private const string TeamName = "Springfield United";

        private Team AddTeam()
        {
            var team = new Team(
                            _teamId,
                            TeamName,
                            MessageBuilder.New(() => new TestCommands.Command1()));
            // Take events and reset the Source so we can continue to use the aggregate as "pre-hydrated"
            team.TakeEvents();
            ((ICorrelatedEventSource)team).Source = MessageBuilder.New(() => new TestCommands.Command1());
            return team;
        }

        private Team AddAndDeleteTeam()
        {
            var team = AddTeam();
            team.DeleteTeam();
            team.TakeEvents();
            ((ICorrelatedEventSource)team).Source = MessageBuilder.New(() => new TestCommands.Command1());
            return team;
        }

        [Fact]
        public void can_create_team()
        {
            var team = new Team(
                            _teamId,
                            TeamName,
                            MessageBuilder.New(() => new TestCommands.Command1()));
            Assert.True(team.HasRecordedEvents);
            var events = team.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is TeamMsgs.TeamCreated evt &&
                                 evt.TeamId == _teamId &&
                                 evt.Name == TeamName));
        }

        [Fact]
        public void cannot_create_team_with_invalid_id()
        {
            Assert.Throws<ArgumentException>(
                () => new Team(
                            Guid.Empty,
                            TeamName,
                            MessageBuilder.New(() => new TestCommands.Command1())));
        }

        [Fact]
        public void cannot_create_team_with_invalid_name()
        {
            Assert.Throws<ArgumentNullException>(
                () => new Team(
                            _teamId,
                            string.Empty,
                            MessageBuilder.New(() => new TestCommands.Command1())));
            Assert.Throws<ArgumentException>(
                () => new Team(
                            _teamId,
                            " ",
                            MessageBuilder.New(() => new TestCommands.Command1())));
        }

        [Fact]
        public void can_delete_team()
        {
            var team = AddTeam();
            team.DeleteTeam();
            Assert.True(team.HasRecordedEvents);
            var events = team.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is TeamMsgs.TeamDeleted evt &&
                                 evt.TeamId == _teamId));
        }

        [Fact]
        public void delete_team_is_idempotent()
        {
            var team = AddAndDeleteTeam();
            team.DeleteTeam();
            Assert.False(team.HasRecordedEvents);
        }

        [Fact]
        public void can_rename_team()
        {
            const string newName = "Springfield Divided";
            var team = AddTeam();
            team.RenameTeam(newName);
            Assert.True(team.HasRecordedEvents);
            var events = team.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is TeamMsgs.TeamRenamed evt &&
                                 evt.TeamId == _teamId &&
                                 evt.Name == newName));
        }

        [Fact]
        public void cannot_rename_team_with_invalid_name()
        {
            var team = AddTeam();
            Assert.Throws<ArgumentNullException>(
                () => team.RenameTeam(string.Empty));
            Assert.Throws<ArgumentException>(
                () => team.RenameTeam(" "));
            Assert.False(team.HasRecordedEvents);
        }

        [Fact]
        public void cannot_rename_deleted_team()
        {
            const string newName = "Springfield Divided";
            var team = AddAndDeleteTeam();
            Assert.Throws<InvalidOperationException>(() => team.RenameTeam(newName));
        }
    }
}
