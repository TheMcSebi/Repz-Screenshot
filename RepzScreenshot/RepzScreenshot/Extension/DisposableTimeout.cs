using System;
using System.Threading;

namespace RepzScreenshot.Extension
{
    //https://stackoverflow.com/questions/21468137/async-network-operations-never-finish
    public sealed class DisposableScope : IDisposable
    {
        private readonly Action _closeScopeAction;
        public DisposableScope(Action closeScopeAction)
        {
            _closeScopeAction = closeScopeAction;
        }
        public void Dispose()
        {
            _closeScopeAction();
        }
    }

    public static class DisposableTimeout
    {
        public static IDisposable SetTimeout(this IDisposable disposable, TimeSpan timeSpan)
        {
            var cancellationTokenSource = new CancellationTokenSource(timeSpan);
            var cancellationTokenRegistration = cancellationTokenSource.Token.Register(disposable.Dispose);
            return new DisposableScope(
                () =>
                {
                    cancellationTokenRegistration.Dispose();
                    cancellationTokenSource.Dispose();
                    disposable.Dispose();
                });
        }

    }
}
