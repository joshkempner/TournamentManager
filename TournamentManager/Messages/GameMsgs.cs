using System;
using ReactiveDomain.Messaging;

namespace TournamentManager.Messages
{
    public class GameMsgs
    {
        public class AddGame : Command
        {
            public readonly Guid TournamentId;
            public readonly Guid GameId;
            public readonly Guid FieldId;
            public readonly uint TournamentDay;
            public readonly DateTime StartTime;
            public readonly DateTime EndTime;

            public AddGame(
                Guid tournamentId,
                Guid gameId,
                Guid fieldId,
                uint tournamentDay,
                DateTime startTime,
                DateTime endTime)
            {
                TournamentId = tournamentId;
                GameId = gameId;
                FieldId = fieldId;
                TournamentDay = tournamentDay;
                StartTime = startTime;
                EndTime = endTime;
            }
        }

        public class GameAdded : Event
        {
            public readonly Guid TournamentId;
            public readonly Guid GameId;
            public readonly Guid FieldId;
            public readonly uint TournamentDay;
            public readonly DateTime StartTime;
            public readonly DateTime EndTime;

            public GameAdded(
                Guid tournamentId,
                Guid gameId,
                Guid fieldId,
                uint tournamentDay,
                DateTime startTime,
                DateTime endTime)
            {
                TournamentId = tournamentId;
                GameId = gameId;
                FieldId = fieldId;
                TournamentDay = tournamentDay;
                StartTime = startTime;
                EndTime = endTime;
            }
        }

        public class CancelGame : Command
        {
            public readonly Guid TournamentId;
            public readonly Guid GameId;

            public CancelGame(
                Guid tournamentId,
                Guid gameId)
            {
                TournamentId = tournamentId;
                GameId = gameId;
            }
        }

        public class GameCancelled : Event
        {
            public readonly Guid TournamentId;
            public readonly Guid GameId;

            public GameCancelled(
                Guid tournamentId,
                Guid gameId)
            {
                TournamentId = tournamentId;
                GameId = gameId;
            }
        }

        public class RescheduleGame : Command
        {
            public readonly Guid TournamentId;
            public readonly Guid GameId;
            public readonly uint TournamentDay;
            public readonly DateTime StartTime;
            public readonly DateTime EndTime;

            public RescheduleGame(
                Guid tournamentId,
                Guid gameId,
                uint tournamentDay,
                DateTime startTime,
                DateTime endTime)
            {
                TournamentId = tournamentId;
                GameId = gameId;
                TournamentDay = tournamentDay;
                StartTime = startTime;
                EndTime = endTime;
            }
        }

        public class GameRescheduled : Event
        {
            public readonly Guid TournamentId;
            public readonly Guid GameId;
            public readonly uint TournamentDay;
            public readonly DateTime StartTime;
            public readonly DateTime EndTime;

            public GameRescheduled(
                Guid tournamentId,
                Guid gameId,
                uint tournamentDay,
                DateTime startTime,
                DateTime endTime)
            {
                TournamentId = tournamentId;
                GameId = gameId;
                TournamentDay = tournamentDay;
                StartTime = startTime;
                EndTime = endTime;
            }
        }

        public class UpdateHomeTeam : Command
        {
            public readonly Guid TournamentId;
            public readonly Guid GameId;
            public readonly Guid HomeTeamId;

            public UpdateHomeTeam(
                Guid tournamentId,
                Guid gameId,
                Guid homeTeamId)
            {
                TournamentId = tournamentId;
                GameId = gameId;
                HomeTeamId = homeTeamId;
            }
        }

        public class HomeTeamUpdated : Event
        {
            public readonly Guid TournamentId;
            public readonly Guid GameId;
            public readonly Guid HomeTeamId;

            public HomeTeamUpdated(
                Guid tournamentId,
                Guid gameId,
                Guid homeTeamId)
            {
                TournamentId = tournamentId;
                GameId = gameId;
                HomeTeamId = homeTeamId;
            }
        }

        public class UpdateAwayTeam : Command
        {
            public readonly Guid TournamentId;
            public readonly Guid GameId;
            public readonly Guid AwayTeamId;

            public UpdateAwayTeam(
                Guid tournamentId,
                Guid gameId,
                Guid awayTeamId)
            {
                TournamentId = tournamentId;
                GameId = gameId;
                AwayTeamId = awayTeamId;
            }
        }

        public class AwayTeamUpdated : Event
        {
            public readonly Guid TournamentId;
            public readonly Guid GameId;
            public readonly Guid AwayTeamId;

            public AwayTeamUpdated(
                Guid tournamentId,
                Guid gameId,
                Guid awayTeamId)
            {
                TournamentId = tournamentId;
                GameId = gameId;
                AwayTeamId = awayTeamId;
            }
        }

        public class AssignReferee : Command
        {
            public readonly Guid TournamentId;
            public readonly Guid GameId;
            public readonly Guid RefereeId;

            public AssignReferee(
                Guid tournamentId,
                Guid gameId,
                Guid refereeId)
            {
                TournamentId = tournamentId;
                GameId = gameId;
                RefereeId = refereeId;
            }
        }

        public class RefereeAssigned : Event
        {
            public readonly Guid TournamentId;
            public readonly Guid GameId;
            public readonly Guid RefereeId;

            public RefereeAssigned(
                Guid tournamentId,
                Guid gameId,
                Guid refereeId)
            {
                TournamentId = tournamentId;
                GameId = gameId;
                RefereeId = refereeId;
            }
        }

        public class ConfirmReferee : Command
        {
            public readonly Guid TournamentId;
            public readonly Guid GameId;

            public ConfirmReferee(
                Guid tournamentId,
                Guid gameId)
            {
                TournamentId = tournamentId;
                GameId = gameId;
            }
        }

        public class RefereeConfirmed : Event
        {
            public readonly Guid TournamentId;
            public readonly Guid GameId;

            public RefereeConfirmed(
                Guid tournamentId,
                Guid gameId)
            {
                TournamentId = tournamentId;
                GameId = gameId;
            }
        }

        public class RemoveReferee : Command
        {
            public readonly Guid TournamentId;
            public readonly Guid GameId;

            public RemoveReferee(
                Guid tournamentId,
                Guid gameId)
            {
                TournamentId = tournamentId;
                GameId = gameId;
            }
        }

        public class RefereeRemoved : Event
        {
            public readonly Guid TournamentId;
            public readonly Guid GameId;

            public RefereeRemoved(
                Guid tournamentId,
                Guid gameId)
            {
                TournamentId = tournamentId;
                GameId = gameId;
            }
        }
    }
}
