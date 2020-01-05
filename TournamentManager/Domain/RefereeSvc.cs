using System;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
using TournamentManager.Messages;

namespace TournamentManager.Domain
{
    public class RefereeSvc :
        QueuedSubscriber,
        IHandleCommand<RefereeMsgs.AddReferee>,
        IHandleCommand<RefereeMsgs.UpdateGivenName>,
        IHandleCommand<RefereeMsgs.UpdateSurname>,
        IHandleCommand<RefereeMsgs.UpdateGrade>,
        IHandleCommand<RefereeMsgs.AddOrUpdateBirthdate>,
        IHandleCommand<RefereeMsgs.AddOrUpdateAge>,
        IHandleCommand<RefereeMsgs.AddOrUpdateEmailAddress>,
        IHandleCommand<RefereeMsgs.AddOrUpdateMailingAddress>,
        IHandleCommand<RefereeMsgs.AddOrUpdateMaxAgeBracket>
    {
        private readonly ICorrelatedRepository _repository;
        public RefereeSvc(
            IBus subscriber,
            IRepository repository)
            : base(subscriber)
        {
            _repository = new CorrelatedStreamStoreRepository(repository);
            Subscribe<RefereeMsgs.AddReferee>(this);
            Subscribe<RefereeMsgs.UpdateGivenName>(this);
            Subscribe<RefereeMsgs.UpdateSurname>(this);
            Subscribe<RefereeMsgs.UpdateGrade>(this);
            Subscribe<RefereeMsgs.AddOrUpdateBirthdate>(this);
            Subscribe<RefereeMsgs.AddOrUpdateAge>(this);
            Subscribe<RefereeMsgs.AddOrUpdateEmailAddress>(this);
            Subscribe<RefereeMsgs.AddOrUpdateMailingAddress>(this);
            Subscribe<RefereeMsgs.AddOrUpdateMaxAgeBracket>(this);
        }

        public CommandResponse Handle(RefereeMsgs.AddReferee command)
        {
            if (_repository.TryGetById<Referee>(command.RefereeId, out _, command))
                throw new AggregateException("Cannot create two referees with the same ID.");
            var referee = new Referee(
                                command.RefereeId,
                                command.GivenName,
                                command.Surname,
                                command.RefereeGrade,
                                command);
            _repository.Save(referee);
            return command.Succeed();
        }

        public CommandResponse Handle(RefereeMsgs.UpdateGivenName command)
        {
            var referee = _repository.GetById<Referee>(command.RefereeId, command);
            referee.UpdateGivenName(command.GivenName);
            _repository.Save(referee);
            return command.Succeed();
        }

        public CommandResponse Handle(RefereeMsgs.UpdateSurname command)
        {
            var referee = _repository.GetById<Referee>(command.RefereeId, command);
            referee.UpdateSurname(command.Surname);
            _repository.Save(referee);
            return command.Succeed();
        }

        public CommandResponse Handle(RefereeMsgs.UpdateGrade command)
        {
            var referee = _repository.GetById<Referee>(command.RefereeId, command);
            referee.UpdateRefereeGrade(command.RefereeGrade);
            _repository.Save(referee);
            return command.Succeed();
        }

        public CommandResponse Handle(RefereeMsgs.AddOrUpdateBirthdate command)
        {
            var referee = _repository.GetById<Referee>(command.RefereeId, command);
            referee.AddOrUpdateBirthdate(command.Birthdate);
            _repository.Save(referee);
            return command.Succeed();
        }

        public CommandResponse Handle(RefereeMsgs.AddOrUpdateAge command)
        {
            var referee = _repository.GetById<Referee>(command.RefereeId, command);
            referee.AddOrUpdateAge(command.Age);
            _repository.Save(referee);
            return command.Succeed();
        }

        public CommandResponse Handle(RefereeMsgs.AddOrUpdateEmailAddress command)
        {
            var referee = _repository.GetById<Referee>(command.RefereeId, command);
            referee.AddOrUpdateEmailAddress(command.Email);
            _repository.Save(referee);
            return command.Succeed();
        }

        public CommandResponse Handle(RefereeMsgs.AddOrUpdateMailingAddress command)
        {
            var referee = _repository.GetById<Referee>(command.RefereeId, command);
            referee.AddOrUpdateMailingAddress(
                command.StreetAddress1,
                command.StreetAddress2,
                command.City,
                command.State,
                command.ZipCode);
            _repository.Save(referee);
            return command.Succeed();
        }

        public CommandResponse Handle(RefereeMsgs.AddOrUpdateMaxAgeBracket command)
        {
            var referee = _repository.GetById<Referee>(command.RefereeId, command);
            referee.AddOrUpdateMaxAgeBracket(command.MaxAgeBracket);
            _repository.Save(referee);
            return command.Succeed();
        }
    }
}
