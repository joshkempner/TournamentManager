using System;
using DynamicData;
using ReactiveDomain;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging.Bus;
using Splat;
using TournamentManager.Domain;
using TournamentManager.Helpers;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    public class TournamentScheduleRM :
        ReadModelBase,
        IHandle<TournamentMsgs.GameSlotAdded>,
        IHandle<TournamentMsgs.FieldAdded>
    {
        public TournamentScheduleRM(
            Guid tournamentId)
            : base(
                nameof(TournamentScheduleRM),
                () => Locator.Current.GetService<IStreamStoreConnection>().GetListener(nameof(TournamentScheduleRM)))
        {
            EventStream.Subscribe<TournamentMsgs.GameSlotAdded>(this);
            EventStream.Subscribe<TournamentMsgs.FieldAdded>(this);
            Start<Tournament>(tournamentId);
        }

        public readonly SourceCache<GameSlotModel, Guid> GameSlots = new SourceCache<GameSlotModel, Guid>(x => x.Id);

        public void Handle(TournamentMsgs.GameSlotAdded message)
        {
            GameSlots.AddOrUpdate(new GameSlotModel(
                                        message.GameSlotId,
                                        message.StartTime,
                                        message.EndTime));
        }

        public void Handle(TournamentMsgs.FieldAdded message)
        {
            foreach (var gameSlot in GameSlots.Items)
            {
                gameSlot.AddField(message);
            }
        }
    }
}
