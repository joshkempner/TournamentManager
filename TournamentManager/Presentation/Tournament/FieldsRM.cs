using System;
using System.Collections.Generic;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging.Bus;
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
                () => Bootstrap.GetListener(nameof(FieldsRM)))
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