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
        private readonly Guid _tournamentId = Guid.NewGuid();
        private readonly Guid _teamId = Guid.NewGuid();
        private const string TeamName = "Springfield United";
        private const TeamMsgs.AgeBracket AgeBracket = TeamMsgs.AgeBracket.U14;

        private Tournament AddTournament()
        {
            var tournament = new Tournament(
                                    _tournamentId,
                                    "The Milk Cup",
                                    new DateTime(2020, 6, 1),
                                    new DateTime(2020, 6, 1),
                                    MessageBuilder.New(() => new TestCommands.Command1()));
            // Take events and reset the Source so we can continue to use the aggregate as "pre-hydrated"
            tournament.TakeEvents();
            ((ICorrelatedEventSource)tournament).Source = MessageBuilder.New(() => new TestCommands.Command1());
            return tournament;
        }

        private Tournament AddTournamentWithTeam()
        {
            var tournament = AddTournament();
            tournament.AddTeam(
                _teamId,
                TeamName,
                AgeBracket);
            // Take events and reset the Source so we can continue to use the aggregate as "pre-hydrated"
            tournament.TakeEvents();
            ((ICorrelatedEventSource)tournament).Source = MessageBuilder.New(() => new TestCommands.Command1());
            return tournament;
        }

        [Fact]
        public void can_add_team()
        {
            var tournament = AddTournament();
            tournament.AddTeam(
                _teamId,
                TeamName,
                AgeBracket);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is TeamMsgs.TeamAdded evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.TeamId == _teamId &&
                                 evt.Name == TeamName &&
                                 evt.AgeBracket == AgeBracket));
        }

        [Fact]
        public void cannot_add_team_with_invalid_id()
        {
            var tournament = AddTournament();
            Assert.Throws<ArgumentException>(
                () => tournament.AddTeam(
                        Guid.Empty,
                        TeamName,
                        AgeBracket));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void cannot_add_team_with_duplicate_id()
        {
            var tournament = AddTournamentWithTeam();
            Assert.Throws<ArgumentException>(
                () => tournament.AddTeam(
                        _teamId,
                        TeamName,
                        AgeBracket));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void cannot_add_team_with_invalid_name()
        {
            var tournament = AddTournament();
            Assert.Throws<ArgumentNullException>(
                () => tournament.AddTeam(
                        _teamId,
                        string.Empty,
                        AgeBracket));
            Assert.Throws<ArgumentException>(
                () => tournament.AddTeam(
                        _teamId,
                        " ",
                        AgeBracket));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void can_remove_team()
        {
            var tournament = AddTournamentWithTeam();
            tournament.RemoveTeam(_teamId);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is TeamMsgs.TeamRemoved evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.TeamId == _teamId));
        }

        [Fact]
        public void removing_nonexistent_team_succeeds_implicitly()
        {
            var tournament = AddTournament();
            tournament.RemoveTeam(_teamId);
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void cannot_remove_team_with_empty_id()
        {
            var tournament = AddTournamentWithTeam();
            Assert.Throws<ArgumentException>(() => tournament.RemoveTeam(Guid.Empty));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void can_rename_team()
        {
            const string newName = "Springfield Divided";
            var tournament = AddTournamentWithTeam();
            tournament.RenameTeam(
                _teamId,
                newName);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is TeamMsgs.TeamRenamed evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.TeamId == _teamId &&
                                 evt.Name == newName));
        }

        [Fact]
        public void cannot_rename_team_with_invalid_team_id()
        {
            const string newName = "Springfield Divided";
            var tournament = AddTournamentWithTeam();
            Assert.Throws<ArgumentException>(
                () => tournament.RenameTeam(
                    Guid.NewGuid(),
                    newName));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void cannot_rename_team_with_invalid_name()
        {
            var tournament = AddTournamentWithTeam();
            Assert.Throws<ArgumentNullException>(
                () => tournament.RenameTeam(
                        _teamId,
                        string.Empty));
            Assert.Throws<ArgumentException>(
                () => tournament.RenameTeam(
                        _teamId,
                        " "));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void can_change_age_bracket()
        {
            var ageBracket = TeamMsgs.AgeBracket.U16;
            var tournament = AddTournamentWithTeam();
            tournament.UpdateAgeBracket(
                _teamId,
                ageBracket);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is TeamMsgs.AgeBracketUpdated evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.TeamId == _teamId &&
                                 evt.AgeBracket == ageBracket));
        }

        [Fact]
        public void cannot_change_age_bracket_with_invalid_team_id()
        {
            var ageBracket = TeamMsgs.AgeBracket.U16;
            var tournament = AddTournamentWithTeam();
            Assert.Throws<ArgumentException>(
                () => tournament.UpdateAgeBracket(
                        Guid.NewGuid(),
                        ageBracket));
            Assert.False(tournament.HasRecordedEvents);
        }
    }
}
