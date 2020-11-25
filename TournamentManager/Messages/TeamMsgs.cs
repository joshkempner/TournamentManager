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

        public class CreateTeam : Command
        {
            public readonly Guid TeamId;
            public readonly string Name;
            public readonly AgeBracket AgeBracket;

            public CreateTeam(
                Guid teamId,
                string name,
                AgeBracket ageBracket)
            {
                TeamId = teamId;
                Name = name;
                AgeBracket = ageBracket;
            }
        }

        public class TeamCreated : Event
        {
            public readonly Guid TeamId;
            public readonly string Name;
            public readonly AgeBracket AgeBracket;

            public TeamCreated(
                Guid teamId,
                string name,
                AgeBracket ageBracket)
            {
                TeamId = teamId;
                Name = name;
                AgeBracket = ageBracket;
            }
        }

        public class DeleteTeam : Command
        {
            public readonly Guid TeamId;

            public DeleteTeam(Guid teamId)
            {
                TeamId = teamId;
            }
        }

        public class TeamDeleted : Event
        {
            public readonly Guid TeamId;

            public TeamDeleted(Guid teamId)
            {
                TeamId = teamId;
            }
        }

        public class RenameTeam : Command
        {
            public readonly Guid TeamId;
            public readonly string Name;

            public RenameTeam(
                Guid teamId,
                string name)
            {
                TeamId = teamId;
                Name = name;
            }
        }

        public class TeamRenamed : Event
        {
            public readonly Guid TeamId;
            public readonly string Name;

            public TeamRenamed(
                Guid teamId,
                string name)
            {
                TeamId = teamId;
                Name = name;
            }
        }

        public class UpdateAgeBracket : Command
        {
            public readonly Guid TeamId;
            public readonly AgeBracket AgeBracket;

            public UpdateAgeBracket(
                Guid teamId,
                AgeBracket ageBracket)
            {
                TeamId = teamId;
                AgeBracket = ageBracket;
            }
        }

        public class AgeBracketUpdated : Event
        {
            public readonly Guid TeamId;
            public readonly AgeBracket AgeBracket;

            public AgeBracketUpdated(
                Guid teamId,
                AgeBracket ageBracket)
            {
                TeamId = teamId;
                AgeBracket = ageBracket;
            }
        }
    }
}
