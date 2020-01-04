using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.UI;
using ReactiveUI;
using Splat;
using TournamentManager.Helpers;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    public class ContactInfoVM : ReactiveObject, IRoutableViewModel, IDisposable
    {
        private readonly ContactInfoRM _rm;

        public ReactiveCommand<Unit, Unit> Save { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }

        public ContactInfoVM(
            Guid refereeId,
            string fullName,
            IDispatcher bus,
            IScreen screen)
        {
            FullName = fullName;
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
            _rm = new ContactInfoRM(refereeId);

            _rm.EmailAddress
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                    LastSavedEmailAddress = x;
                    EmailAddress = x;
                });

            _rm.StreetAddress1
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                    LastSavedStreetAddress1 = x;
                    StreetAddress1 = x;
                });

            _rm.StreetAddress2
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                    LastSavedStreetAddress2 = x;
                    StreetAddress2 = x;
                });

            _rm.City
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                    LastSavedCity = x;
                    City = x;
                });

            _rm.StateAbbreviation
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                    LastSavedStateAbbreviation = x;
                    StateAbbreviation = x;
                    SelectedStateName = StringUtilities.States.FirstOrDefault(s => s.Value == x).Key;
                });

            _rm.ZipCode
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                    LastSavedZipCode = x;
                    ZipCode = x;
                });

            this.WhenAnyValue(x => x.SelectedStateName)
                .Select(x => string.IsNullOrEmpty(x) ? string.Empty : StringUtilities.States[x])
                .Subscribe(x => StateAbbreviation = x);

            this.WhenAnyValue(x => x.ZipCode)
                .Select(StringUtilities.IsValidUSZipCode)
                .ToProperty(this, x => x.IsZipCodeValid, out _isZipCodeValid);

            this.WhenAnyValue(
                    x => x.StreetAddress1,
                    x => x.City,
                    x => x.StateAbbreviation,
                    x => x.ZipCode,
                    x => x.IsZipCodeValid,
                    (a1, c, s, z, zv) => !string.IsNullOrWhiteSpace(a1) && !string.IsNullOrWhiteSpace(c) && !string.IsNullOrWhiteSpace(s) && zv ||
                                         string.IsNullOrEmpty(a1) && string.IsNullOrEmpty(c) && string.IsNullOrEmpty(s) && string.IsNullOrEmpty(z))
                .ToProperty(this, x => x.IsAddressValid, out _isAddressValid);

            this.WhenAnyValue(
                    x => x.EmailAddress,
                    x => x.IsAddressValid,
                    (e, a) => (string.IsNullOrEmpty(e) || StringUtilities.IsValidEmailAddress(e)) && a)
                .ToProperty(this, x => x.CanSave, out _canSave);

            Save = CommandBuilder.FromAction(
                    this.WhenAnyValue(x => x.CanSave),
                    () =>
                    {
                        if (StreetAddress1 != LastSavedStreetAddress1 || StreetAddress2 != LastSavedStreetAddress2 ||
                            City != LastSavedCity || StateAbbreviation != LastSavedStateAbbreviation || ZipCode != LastSavedZipCode)
                        {
                            bus.Send(MessageBuilder.New(
                                        () => new RefereeMsgs.AddOrUpdateMailingAddress(
                                                    refereeId,
                                                    StreetAddress1,
                                                    StreetAddress2,
                                                    City,
                                                    StateAbbreviation,
                                                    ZipCode)));
                        }
                        if (EmailAddress != LastSavedEmailAddress)
                        {
                            bus.Send(MessageBuilder.New(
                                        () => new RefereeMsgs.AddOrUpdateEmailAddress(
                                                    refereeId,
                                                    EmailAddress)));
                        }
                    });

            this.WhenAnyObservable(x => x.Save)
                .InvokeCommand(HostScreen.Router.NavigateBack);

            Cancel = HostScreen.Router.NavigateBack;

            this.WhenAnyObservable(x => x.HostScreen.Router.NavigateBack)
                .Subscribe(_ => Dispose());
        }

        public void Dispose()
        {
            _rm.Dispose();
        }

        public IEnumerable<string> StateNames => StringUtilities.States.Keys;

        public string FullName { get; }

        private string LastSavedEmailAddress { get; set; } = string.Empty;

        public string EmailAddress
        {
            get => _emailAddress;
            set => this.RaiseAndSetIfChanged(ref _emailAddress, value);
        }
        private string _emailAddress = string.Empty;

        private string LastSavedStreetAddress1 { get; set; } = string.Empty;

        public string StreetAddress1
        {
            get => _streetAddress1;
            set => this.RaiseAndSetIfChanged(ref _streetAddress1, value);
        }
        private string _streetAddress1 = string.Empty;

        private string LastSavedStreetAddress2 { get; set; } = string.Empty;

        public string StreetAddress2
        {
            get => _streetAddress2;
            set => this.RaiseAndSetIfChanged(ref _streetAddress2, value);
        }
        private string _streetAddress2 = string.Empty;

        private string LastSavedCity { get; set; } = string.Empty;

        public string City
        {
            get => _city;
            set => this.RaiseAndSetIfChanged(ref _city, value);
        }
        private string _city = string.Empty;

        private string LastSavedStateAbbreviation { get; set; } = string.Empty;

        public string StateAbbreviation
        {
            get => _stateAbbreviation;
            set => this.RaiseAndSetIfChanged(ref _stateAbbreviation, value);
        }
        private string _stateAbbreviation = string.Empty;

        public string? SelectedStateName
        {
            get => _selectedStateName;
            set => this.RaiseAndSetIfChanged(ref _selectedStateName, value);
        }
        private string? _selectedStateName;

        private string LastSavedZipCode { get; set; } = string.Empty;

        public string ZipCode
        {
            get => _zipCode;
            set => this.RaiseAndSetIfChanged(ref _zipCode, value);
        }
        private string _zipCode = string.Empty;

        public bool IsZipCodeValid => _isZipCodeValid.Value;
        private readonly ObservableAsPropertyHelper<bool> _isZipCodeValid;

        public bool IsAddressValid => _isAddressValid.Value;
        private readonly ObservableAsPropertyHelper<bool> _isAddressValid;

        public bool CanSave => _canSave.Value;
        private readonly ObservableAsPropertyHelper<bool> _canSave;

        public string UrlPathSegment => "Contact Info";
        public IScreen HostScreen { get; }
    }
}
