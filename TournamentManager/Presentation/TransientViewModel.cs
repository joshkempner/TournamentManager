using System;
using System.Reactive;
using ReactiveUI;
using Splat;

namespace TournamentManager.Presentation
{
    public abstract class TransientViewModel : ReactiveObject, IRoutableViewModel, IDisposable
    {
        public ReactiveCommand<Unit, Unit>? Save { get; protected set; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }

        protected TransientViewModel(IScreen screen)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

            this.WhenAnyObservable(x => x.Save)
                .InvokeCommand(HostScreen.Router.NavigateBack);

            Cancel = HostScreen.Router.NavigateBack;

            this.WhenAnyObservable(x => x.HostScreen.Router.NavigateBack)
                .Subscribe(_ => Dispose());
        }

        public abstract string UrlPathSegment { get; }
        public IScreen HostScreen { get; }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // nothing to do here.
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
