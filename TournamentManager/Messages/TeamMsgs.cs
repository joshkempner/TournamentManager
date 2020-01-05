using System;
using ReactiveDomain.Messaging;

namespace TournamentManager.Messages
{
    public class TeamMsgs
    {
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

        public class AddTeam : Command
        {
            public readonly Guid TournamentId;
            public readonly Guid TeamId;
            public readonly string Name;
            public readonly AgeBracket AgeBracket;

            public AddTeam(
                Guid tournamentId,
                Guid teamId,
                string name,
                AgeBracket ageBracket)
            {
                TournamentId = tournamentId;
                TeamId = teamId;
                Name = name;
                AgeBracket = ageBracket;
            }
        }

        public class TeamAdded : Event
        {
            public readonly Guid TournamentId;
            public readonly Guid TeamId;
            public readonly string Name;
            public readonly AgeBracket AgeBracket;

            public TeamAdded(
                Guid tournamentId,
                Guid teamId,
                string name,
                AgeBracket ageBracket)
            {
                TournamentId = tournamentId;
                TeamId = teamId;
                Name = name;
                AgeBracket = ageBracket;
            }
        }

        public class RemoveTeam : Command
        {
            public readonly Guid TournamentId;
            public readonly Guid TeamId;

            public RemoveTeam(
                Guid tournamentId,
                Guid teamId)
            {
                TournamentId = tournamentId;
                TeamId = teamId;
            }
        }

        public class TeamRemoved : Event
        {
            public readonly Guid TournamentId;
            public readonly Guid TeamId;

            public TeamRemoved(
                Guid tournamentId,
                Guid teamId)
            {
                TournamentId = tournamentId;
                TeamId = teamId;
            }
        }

        public class RenameTeam : Command
        {
            public readonly Guid TournamentId;
            public readonly Guid TeamId;
            public readonly string Name;

            public RenameTeam(
                Guid tournamentId,
                Guid teamId,
                string name)
            {
                TournamentId = tournamentId;
                TeamId = teamId;
                Name = name;
            }
        }

        public class TeamRenamed : Event
        {
            public readonly Guid TournamentId;
            public readonly Guid TeamId;
            public readonly string Name;

            public TeamRenamed(
                Guid tournamentId,
                Guid teamId,
                string name)
            {
                TournamentId = tournamentId;
                TeamId = teamId;
                Name = name;
            }
        }

        public class UpdateAgeBracket : Command
        {
            public readonly Guid TournamentId;
            public readonly Guid TeamId;
            public readonly AgeBracket AgeBracket;

            public UpdateAgeBracket(
                Guid tournamentId,
                Guid teamId,
                AgeBracket ageBracket)
            {
                TournamentId = tournamentId;
                TeamId = teamId;
                AgeBracket = ageBracket;
            }
        }

        public class AgeBracketUpdated : Event
        {
            public readonly Guid TournamentId;
            public readonly Guid TeamId;
            public readonly AgeBracket AgeBracket;

            public AgeBracketUpdated(
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
