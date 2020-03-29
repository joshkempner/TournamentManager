using System;
using DynamicData;
using ReactiveUI;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    public class GameSlotModel : ReactiveObject
    {
        public Guid Id { get; }
        public DateTime StartTime { get; }
        public DateTime EndTime { get; }
        public SourceCache<GameModel, Guid> Games { get; } = new SourceCache<GameModel, Guid>(x => x.FieldId);

        public GameSlotModel(
            Guid id,
            DateTime startTime,
            DateTime endTime)
        {
            Id = id;
            StartTime = startTime;
            EndTime = endTime;
        }

        internal void AddField(TournamentMsgs.FieldAdded message)
        {
            Games.AddOrUpdate(new GameModel(message.FieldId, message.FieldName));
        }
    }
}
