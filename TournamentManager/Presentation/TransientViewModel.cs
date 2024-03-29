﻿using System;
using System.Reactive;
using ReactiveUI;

namespace TournamentManager.Presentation
{
    public abstract class TransientViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel, IDisposable
    {
        protected ReactiveCommand<Unit, IRoutableViewModel?> Complete { get; }
        public ReactiveCommand<Unit, IRoutableViewModel?> Cancel { get; }

        protected TransientViewModel(IScreen screen)
        {
            HostScreen = screen;

            Complete = HostScreen.Router.NavigateBack;
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

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
