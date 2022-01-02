using System;
using System.Threading.Tasks;

namespace TournamentManager.Helpers
{
    public static class Threading
    {
        public static void RunOnUiThreadAsync(Action action)
        {
            if (System.Windows.Application.Current?.Dispatcher?.CheckAccess() ?? false)
            {
                action(); // we're on the ui thread, just go for it
                return;
            }
            Task.Run(() => RunOnUiThread(_ => action()));
        }

        public static void RunOnUiThread(Action action)
        {
            RunOnUiThread(_ => action());
        }

        public static void RunOnUiThread(Action<object?> action, object? param = null, bool fallback = true)
        {
            if (System.Windows.Application.Current?.Dispatcher?.CheckAccess() ?? false)
            {
                action(param); // we're on the ui thread, just go for it
                return;
            }
            // Execute the action on the UI thread.  Note that we sometimes call this
            // function from a unit-test, in which case there IS no UI thread.
            if (System.Windows.Application.Current != null)
                System.Windows.Application.Current.Dispatcher?.Invoke(action, param);
            else if (fallback)
                action(param);
            else
                throw new InvalidOperationException("Unable to run on UI thread!");
        }
    }
}
