using System;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.UI;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    public class ContactInfoRM :
        ReadModelBase,
        IHandle<RefereeMsgs.EmailAddressChanged>,
        IHandle<RefereeMsgs.MailingAddressChanged>
    {
        public ContactInfoRM(Guid refereeId)
            : base(
                nameof(ContactInfoRM),
                () => Bootstrap.GetListener(nameof(ContactInfoRM)))
        {
            EventStream.Subscribe<RefereeMsgs.EmailAddressChanged>(this);
            EventStream.Subscribe<RefereeMsgs.MailingAddressChanged>(this);
            Start<Domain.Referee>(refereeId, blockUntilLive: true);
        }

        public IObservable<string> EmailAddress => _emailAddress;
        private readonly ReadModelProperty<string> _emailAddress = new ReadModelProperty<string>(string.Empty);

        public IObservable<string> StreetAddress1 => _streetAddress1;
        private readonly ReadModelProperty<string> _streetAddress1 = new ReadModelProperty<string>(string.Empty);

        public IObservable<string> StreetAddress2 => _streetAddress2;
        private readonly ReadModelProperty<string> _streetAddress2 = new ReadModelProperty<string>(string.Empty);

        public IObservable<string> City => _city;
        private readonly ReadModelProperty<string> _city = new ReadModelProperty<string>(string.Empty);

        public IObservable<string> StateAbbreviation => _stateAbbreviation;
        private readonly ReadModelProperty<string> _stateAbbreviation = new ReadModelProperty<string>(string.Empty);

        public IObservable<string> ZipCode => _zipCode;
        private readonly ReadModelProperty<string> _zipCode = new ReadModelProperty<string>(string.Empty);

        public void Handle(RefereeMsgs.EmailAddressChanged message)
        {
            _emailAddress.Update(message.Email);
        }

        public void Handle(RefereeMsgs.MailingAddressChanged message)
        {
            _streetAddress1.Update(message.StreetAddress1);
            _streetAddress2.Update(message.StreetAddress2);
            _city.Update(message.City);
            _stateAbbreviation.Update(message.State);
            _zipCode.Update(message.ZipCode);
        }
    }
}
