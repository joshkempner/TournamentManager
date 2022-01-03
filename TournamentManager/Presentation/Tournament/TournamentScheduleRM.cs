using System;
using DynamicData;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging.Bus;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    public class TournamentScheduleRM :
        ReadModelBase,
        IHandle<TournamentMsgs.FieldAdded>
    {
        public TournamentScheduleRM(
            Guid tournamentId)
            : base(
                nameof(TournamentScheduleRM),
                () => Bootstrap.GetListener(nameof(TournamentScheduleRM)))
        {
            EventStream.Subscribe<TournamentMsgs.FieldAdded>(this);
            Start<Domain.Tournament>(tournamentId);
        }

        public readonly SourceCache<FieldModel, Guid> Fields = new SourceCache<FieldModel, Guid>(x => x.FieldId);

        public void Handle(TournamentMsgs.FieldAdded message)
        {
            Fields.AddOrUpdate(new FieldModel(message.FieldId, message.FieldName));
        }
    }
}
