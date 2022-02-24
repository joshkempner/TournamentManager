using System;
using System.Reactive;
using ReactiveDomain.UI;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    public abstract class OverlayViewModel : ReactiveObject, IActivatableViewModel, IDisposable
    {
        public ReactiveCommand<Unit, Unit> Done { get; }

        protected OverlayViewModel()
        {
            Done = CommandBuilder.FromAction(() => { /* Nothing to do here. */ });
        }

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

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
