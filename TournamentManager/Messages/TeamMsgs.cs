using System;
using ReactiveDomain.Messaging;

namespace TournamentManager.Messages
{
    public class TeamMsgs
    {
        public class CreateTeam : Command
        {
            public readonly Guid TeamId;
            public readonly string Name;

            public CreateTeam(
                Guid teamId,
                string name)
            {
                TeamId = teamId;
                Name = name;
            }
        }

        public class TeamCreated : Event
        {
            public readonly Guid TeamId;
            public readonly string Name;

            public TeamCreated(
                Guid teamId,
                string name)
            {
                TeamId = teamId;
                Name = name;
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
    }
}
