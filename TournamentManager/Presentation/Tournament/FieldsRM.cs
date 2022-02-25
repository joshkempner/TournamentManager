using System;
using System.Collections.Generic;
using ReactiveDomain;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging.Bus;
using Splat;
using TournamentManager.Helpers;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    public class FieldsRM :
        ReadModelBase,
        IHandle<TournamentMsgs.FieldAdded>
    {
        public FieldsRM(Guid tournamentId)
            : base(
                nameof(FieldsRM),
                () => Locator.Current.GetService<IStreamStoreConnection>()!.GetListener(nameof(FieldsRM)))
        {
            EventStream.Subscribe<TournamentMsgs.FieldAdded>(this);
        }

        public bool FieldNameExists(string name)
        {
            return _fieldNames.Contains(name);
        }

        private readonly List<string> _fieldNames = new List<string>();

        public void Handle(TournamentMsgs.FieldAdded message)
        {
            _fieldNames.Add(message.FieldName);
        }
    }
}