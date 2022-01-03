using System;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
using ReactiveDomain.UI;
using ReactiveUI;
using TournamentManager.Messages;

namespace TournamentManager.Presentation
{
    public class NewFieldVM : TransientViewModel
    {
        private readonly FieldsRM _rm;

        public ReactiveCommand<Unit, Unit> Save { get; }

        public NewFieldVM(
            Guid tournamentId,
            IDispatcher bus,
            IScreen screen)
            : base(screen)
        {
            _rm = new FieldsRM(tournamentId);

            this.WhenAnyValue(x => x.FieldName)
                .Select(name => !string.IsNullOrWhiteSpace(name) && !_rm.FieldNameExists(name))
                .ToProperty(this, x => x.CanAddField, out _canAddField);

            Save = bus.BuildSendCommand(
                            this.WhenAnyValue(x => x.CanAddField),
                            () => MessageBuilder
                                    .New(() => new TournamentMsgs.AddField(
                                                    tournamentId,
                                                    Guid.NewGuid(),
                                                    FieldName)));

            this.WhenAnyObservable(x => x.Save)
                .InvokeCommand(Complete);
        }

        private bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
                _rm.Dispose();
            _disposed = true;
            base.Dispose(disposing);
        }

        public string FieldName
        {
            get => _fieldName;
            set => this.RaiseAndSetIfChanged(ref _fieldName, value);
        }
        private string _fieldName = string.Empty;

        public bool CanAddField => _canAddField.Value;
        private readonly ObservableAsPropertyHelper<bool> _canAddField;

        public override string UrlPathSegment => "Add Field";
    }
}
