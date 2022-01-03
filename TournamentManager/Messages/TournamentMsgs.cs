using System;
using ReactiveDomain.Messaging;

namespace TournamentManager.Messages
{
    public class TournamentMsgs
    {
        public class AddTournament : Command
        {
            public readonly Guid TournamentId;
            public readonly string Name;
            public readonly DateTime FirstDay;
            public readonly DateTime LastDay;

            public AddTournament(
                Guid tournamentId,
                string name,
                DateTime firstDay,
                DateTime lastDay)
            {
                TournamentId = tournamentId;
                Name = name;
                FirstDay = firstDay;
                LastDay = lastDay;
            }
        }

        public class TournamentAdded : Event
        {
            public readonly Guid TournamentId;
            public readonly string Name;
            public readonly DateTime FirstDay;
            public readonly DateTime LastDay;

            public TournamentAdded(
                Guid tournamentId,
                string name,
                DateTime firstDay,
                DateTime lastDay)
            {
                TournamentId = tournamentId;
                Name = name;
                FirstDay = firstDay;
                LastDay = lastDay;
            }
        }

        public class RenameTournament : Command
        {
            public readonly Guid TournamentId;
            public readonly string Name;

            public RenameTournament(
                Guid tournamentId,
                string name)
            {
                TournamentId = tournamentId;
                Name = name;
            }
        }

        public class TournamentRenamed : Event
        {
            public readonly Guid TournamentId;
            public readonly string Name;

            public TournamentRenamed(
                Guid tournamentId,
                string name)
            {
                TournamentId = tournamentId;
                Name = name;
            }
        }

        public class RescheduleTournament : Command
        {
            public readonly Guid TournamentId;
            public readonly DateTime FirstDay;
            public readonly DateTime LastDay;
            
            public RescheduleTournament(
                Guid tournamentId,
                DateTime firstDay,
                DateTime lastDay)
            {
                TournamentId = tournamentId;
                FirstDay = firstDay;
                LastDay = lastDay;
            }
        }

        public class TournamentRescheduled : Event
        {
            public readonly Guid TournamentId;
            public readonly DateTime FirstDay;
            public readonly DateTime LastDay;

            public TournamentRescheduled(
                Guid tournamentId,
                DateTime firstDay,
                DateTime lastDay)
            {
                TournamentId = tournamentId;
                FirstDay = firstDay;
                LastDay = lastDay;
            }
        }

        public class AddField : Command
        {
            public readonly Guid TournamentId;
            public readonly Guid FieldId;
            public readonly string FieldName;

            public AddField(
                Guid tournamentId,
                Guid fieldId,
                string fieldName)
            {
                TournamentId = tournamentId;
                FieldId = fieldId;
                FieldName = fieldName;
            }
        }

        public class FieldAdded : Event
        {
            public readonly Guid TournamentId;
            public readonly Guid FieldId;
            public readonly string FieldName;

            public FieldAdded(
                Guid tournamentId,
                Guid fieldId,
                string fieldName)
            {
                TournamentId = tournamentId;
                FieldId = fieldId;
                FieldName = fieldName;
            }
        }

        public class AddRefereeToTournament : Command
        {
            public readonly Guid TournamentId;
            public readonly Guid RefereeId;

            public AddRefereeToTournament(
                Guid tournamentId,
                Guid refereeId)
            {
                TournamentId = tournamentId;
                RefereeId = refereeId;
            }
        }

        public class RefereeAddedToTournament : Event
        {
            public readonly Guid TournamentId;
            public readonly Guid RefereeId;

            public RefereeAddedToTournament(
                Guid tournamentId,
                Guid refereeId)
            {
                TournamentId = tournamentId;
                RefereeId = refereeId;
            }
        }

        public enum AgeBracket
        {
            U8,
            U10,
            U12,
            U14,
            U16,
            U18,
            Adult
        }

        public class AddTeamToTournament : Command
        {
            public readonly Guid TournamentId;
            public readonly Guid TeamId;
            public readonly AgeBracket AgeBracket;

            public AddTeamToTournament(
                Guid tournamentId,
                Guid teamId,
                AgeBracket ageBracket)
            {
                TournamentId = tournamentId;
                TeamId = teamId;
                AgeBracket = ageBracket;
            }
        }

        public class TeamAddedToTournament : Event
        {
            public readonly Guid TournamentId;
            public readonly Guid TeamId;
            public readonly AgeBracket AgeBracket;

            public TeamAddedToTournament(
                Guid tournamentId,
                Guid teamId,
                AgeBracket ageBracket)
            {
                TournamentId = tournamentId;
                TeamId = teamId;
                AgeBracket = ageBracket;
            }
        }

        public class RemoveTeamFromTournament : Command
        {
            public readonly Guid TournamentId;
            public readonly Guid TeamId;

            public RemoveTeamFromTournament(
                Guid tournamentId,
                Guid teamId)
            {
                TournamentId = tournamentId;
                TeamId = teamId;
            }
        }

        public class TeamRemovedFromTournament : Event
        {
            public readonly Guid TournamentId;
            public readonly Guid TeamId;

            public TeamRemovedFromTournament(
                Guid tournamentId,
                Guid teamId)
            {
                TournamentId = tournamentId;
                TeamId = teamId;
            }
        }

        public class ChangeTeamAgeBracket : Command
        {
            public readonly Guid TournamentId;
            public readonly Guid TeamId;
            public readonly AgeBracket AgeBracket;

            public ChangeTeamAgeBracket(
                Guid tournamentId,
                Guid teamId,
                AgeBracket ageBracket)
            {
                TournamentId = tournamentId;
                TeamId = teamId;
                AgeBracket = ageBracket;
            }
        }

        public class TeamAgeBracketChanged : Event
        {
            public readonly Guid TournamentId;
            public readonly Guid TeamId;
            public readonly AgeBracket AgeBracket;

            public TeamAgeBracketChanged(
                Guid tournamentId,
                Guid teamId,
                AgeBracket ageBracket)
            {
                TournamentId = tournamentId;
                TeamId = teamId;
                AgeBracket = ageBracket;
            }
        }
    }
}
