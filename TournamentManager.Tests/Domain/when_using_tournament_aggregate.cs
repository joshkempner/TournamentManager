using System;
using ReactiveDomain;
using ReactiveDomain.Messaging;
using ReactiveDomain.Testing;
using TournamentManager.Domain;
using TournamentManager.Messages;
using Xunit;

namespace TournamentManager.Tests.Domain
{
    public sealed class when_using_tournament_aggregate
    {
        private readonly Guid _tournamentId = Guid.NewGuid();
        private const string TournamentName = "The Milk Cup";
        private readonly DateTime _firstDay = new DateTime(2020, 6, 1);
        private readonly DateTime _lastDay = new DateTime(2020, 6, 2);

        [Fact]
        public void can_create_tournament()
        {
            var tournament = new Tournament(
                                    _tournamentId,
                                    TournamentName,
                                    _firstDay,
                                    _lastDay,
                                    MessageBuilder.New(() => new TestCommands.Command1()));
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
            Assert.Throws<ArgumentException>(
                () => new Tournament(
                            _tournamentId,
                            " ",
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

        private Tournament AddTournament()
        {
            var tournament = new Tournament(
                                    _tournamentId,
                                    TournamentName,
                                    _firstDay,
                                    _lastDay,
                                    MessageBuilder.New(() => new TestCommands.Command1()));
            // Take events and reset the Source so we can continue to use the aggregate as "pre-hydrated"
            tournament.TakeEvents();
            ((ICorrelatedEventSource)tournament).Source = MessageBuilder.New(() => new TestCommands.Command1());
            return tournament;
        }

        [Fact]
        public void can_rename_tournament()
        {
            const string newName = "The Bourbon Cup";
            var tournament = AddTournament();
            tournament.Rename(newName);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is TournamentMsgs.TournamentRenamed evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.Name == newName));
        }

        [Fact]
        public void cannot_rename_to_empty_name()
        {
            var tournament = AddTournament();
            Assert.Throws<ArgumentNullException>(() => tournament.Rename(string.Empty));
            Assert.Throws<ArgumentException>(() => tournament.Rename(" "));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void can_reschedule_tournament()
        {
            var newFirstDay = _firstDay.AddDays(1);
            var newLastDay = _lastDay.AddDays(1);
            var tournament = AddTournament();
            tournament.Reschedule(
                newFirstDay,
                newLastDay);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is TournamentMsgs.TournamentRescheduled evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.FirstDay == newFirstDay &&
                                 evt.LastDay == newLastDay));
        }

        [Fact]
        public void can_reschedule_tournament_to_single_day()
        {
            var tournament = AddTournament();
            tournament.Reschedule(
                _firstDay,
                _firstDay);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is TournamentMsgs.TournamentRescheduled evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.FirstDay == _firstDay &&
                                 evt.LastDay == _firstDay));
        }

        [Fact]
        public void cannot_reschedule_tournament_to_invalid_dates()
        {
            var tournament = AddTournament();
            Assert.Throws<ArgumentException>(
                () => tournament.Reschedule(
                        _lastDay,
                        _firstDay));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void can_add_field()
        {
            var fieldId = Guid.NewGuid();
            const string fieldName = "Field 1";
            var tournament = AddTournament();
            tournament.AddField(
                fieldId,
                fieldName);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
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
            var tournament = AddTournament();
            tournament.AddField(
                fieldId,
                fieldName);
            Assert.Throws<ArgumentException>(
                () => tournament.AddField(
                        fieldId,
                        fieldName));

            // Make sure we have only one event from the expected "FieldAdded"
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Equal(1, events.Length);
        }

        [Fact]
        public void cannot_add_field_with_invalid_id()
        {
            const string fieldName = "Field 1";
            var tournament = AddTournament();
            Assert.Throws<ArgumentException>(
                () => tournament.AddField(
                        Guid.Empty,
                        fieldName));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void cannot_add_field_with_invalid_name()
        {
            var fieldId = Guid.NewGuid();
            var tournament = AddTournament();
            Assert.Throws<ArgumentNullException>(
                () => tournament.AddField(
                        fieldId,
                        string.Empty));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void can_add_referee_to_tournament()
        {
            var refereeId = Guid.NewGuid();
            var tournament = AddTournament();
            tournament.AddReferee(refereeId);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is TournamentMsgs.RefereeAddedToTournament evt &&
                                 evt.TournamentId == _tournamentId &&
                                 evt.RefereeId == refereeId));
        }

        [Fact]
        public void cannot_add_referee_with_empty_id_to_tournament()
        {
            var tournament = AddTournament();
            Assert.Throws<ArgumentException>(() => tournament.AddReferee(Guid.Empty));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void cannot_add_duplicate_referee_to_tournament()
        {
            var refereeId = Guid.NewGuid();
            var tournament = AddTournament();
            tournament.AddReferee(refereeId);
            tournament.TakeEvents();
            ((ICorrelatedEventSource)tournament).Source = MessageBuilder.New(() => new TestCommands.Command1());
            Assert.Throws<ArgumentException>(() => tournament.AddReferee(refereeId));
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void can_add_team_to_tournament()
        {
            var teamId = Guid.NewGuid();
            var tournament = AddTournament();
            const TournamentMsgs.AgeBracket ageBracket = TournamentMsgs.AgeBracket.U14;
            tournament.AddTeamToTournament(teamId, ageBracket);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is TournamentMsgs.TeamAddedToTournament evt &&
                                 evt.TournamentId == tournament.Id &&
                                 evt.TeamId == teamId &&
                                 evt.AgeBracket == ageBracket));
        }

        [Fact]
        public void adding_team_is_idempotent()
        {
            var teamId = Guid.NewGuid();
            var tournament = AddTournament();
            const TournamentMsgs.AgeBracket ageBracket = TournamentMsgs.AgeBracket.U14;
            tournament.AddTeamToTournament(teamId, ageBracket);
            tournament.AddTeamToTournament(teamId, ageBracket); // Add the same team a second time
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is TournamentMsgs.TeamAddedToTournament evt &&
                                 evt.TournamentId == tournament.Id &&
                                 evt.TeamId == teamId &&
                                 evt.AgeBracket == ageBracket));
        }

        [Fact]
        public void cannot_add_team_with_empty_id()
        {
            var tournament = AddTournament();
            Assert.Throws<ArgumentException>(() => tournament.AddTeamToTournament(Guid.Empty, TournamentMsgs.AgeBracket.U14));
            Assert.False(tournament.HasRecordedEvents);
        }

        private Tournament AddTournamentWithTeam(Guid teamId)
        {
            var tournament = new Tournament(
                                    _tournamentId,
                                    TournamentName,
                                    _firstDay,
                                    _lastDay,
                                    MessageBuilder.New(() => new TestCommands.Command1()));
            tournament.AddTeamToTournament(teamId, TournamentMsgs.AgeBracket.U14);
            // Take events and reset the Source so we can continue to use the aggregate as "pre-hydrated"
            tournament.TakeEvents();
            ((ICorrelatedEventSource)tournament).Source = MessageBuilder.New(() => new TestCommands.Command1());
            return tournament;
        }

        [Fact]
        public void can_remove_team_from_tournament()
        {
            var teamId = Guid.NewGuid();
            var tournament = AddTournamentWithTeam(teamId);
            tournament.RemoveTeamFromTournament(teamId);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is TournamentMsgs.TeamRemovedFromTournament evt &&
                                 evt.TournamentId == tournament.Id &&
                                 evt.TeamId == teamId));
        }

        [Fact]
        public void removing_team_is_idempotent()
        {
            var teamId = Guid.NewGuid();
            var tournament = AddTournamentWithTeam(teamId);
            tournament.RemoveTeamFromTournament(teamId);
            tournament.RemoveTeamFromTournament(teamId); // Remove the same team a second time
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is TournamentMsgs.TeamRemovedFromTournament evt &&
                                 evt.TournamentId == tournament.Id &&
                                 evt.TeamId == teamId));
        }

        [Fact]
        public void removing_nonexistent_team_succeeds_implicitly()
        {
            var tournament = AddTournament();
            tournament.RemoveTeamFromTournament(Guid.NewGuid());
            tournament.RemoveTeamFromTournament(Guid.Empty);
            Assert.False(tournament.HasRecordedEvents);
        }

        [Fact]
        public void can_change_team_age_bracket()
        {
            var teamId = Guid.NewGuid();
            var tournament = AddTournamentWithTeam(teamId);
            const TournamentMsgs.AgeBracket newAgeBracket = TournamentMsgs.AgeBracket.U16;
            tournament.ChangeTeamAgeBracket(teamId, newAgeBracket);
            Assert.True(tournament.HasRecordedEvents);
            var events = tournament.TakeEvents();
            Assert.Collection(
                events,
                e => Assert.True(e is TournamentMsgs.TeamAgeBracketChanged evt &&
                                 evt.TournamentId == tournament.Id &&
                                 evt.TeamId == teamId &&
                                 evt.AgeBracket == newAgeBracket));
        }

        [Fact]
        public void cannot_change_age_bracket_of_team_with_invalid_id()
        {
            var teamId = Guid.NewGuid();
            var tournament = AddTournamentWithTeam(teamId);
            const TournamentMsgs.AgeBracket newAgeBracket = TournamentMsgs.AgeBracket.U16;
            Assert.Throws<ArgumentException>(() => tournament.ChangeTeamAgeBracket(Guid.Empty, newAgeBracket));
        }

        [Fact]
        public void cannot_change_age_bracket_of_team_not_in_tournament()
        {
            var teamId = Guid.NewGuid();
            var tournament = AddTournamentWithTeam(teamId);
            const TournamentMsgs.AgeBracket newAgeBracket = TournamentMsgs.AgeBracket.U16;
            Assert.Throws<Exception>(() => tournament.ChangeTeamAgeBracket(Guid.NewGuid(), newAgeBracket));
        }
    }
}
