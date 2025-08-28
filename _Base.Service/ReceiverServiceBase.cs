using _Base;
using Base.Service.Services;
using Microsoft.Extensions.Hosting;

namespace Base.Service;

public abstract class ReceiverServiceBase<TData>(
    IDataStore<TData> dataStore,
    IDelaySource delaySource)
    : BackgroundService
{
    private ReceiverBase<TData>? _receiver;

    protected abstract ValueTask<ReceiverBase<TData>> CreateReceiverAsync();

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);
        
        _receiver = await CreateReceiverAsync();
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested == false)
        {
            var delay = await delaySource.GetDealy();
            await Task.Delay(delay, cancellationToken);

            if (_receiver == null)
                continue;

            var handledDataCollection = _receiver.PullHandledData();

            foreach (var handledData in handledDataCollection)
                await dataStore.AddAsync(handledData);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_receiver != null)
            await _receiver.DisposeAsync();

        await base.StopAsync(cancellationToken);
    }
}