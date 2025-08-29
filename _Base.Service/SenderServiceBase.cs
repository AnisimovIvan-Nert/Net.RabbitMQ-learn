using Base.Service.Services;
using Microsoft.Extensions.Hosting;

namespace Base.Service;

public abstract class SenderServiceBase<TData>(
    IDataSource<TData> dataSource,
    IDelaySource delaySource)
    : BackgroundService
{
    private SenderBase<TData>? _sender;

    protected abstract ValueTask<SenderBase<TData>> CreateSenderAsync();

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);
        
        _sender = await CreateSenderAsync();
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested == false)
        {
            var delay = await delaySource.GetDealy();
            await Task.Delay(delay, cancellationToken);

            if (_sender == null)
                continue;

            var dataCollection = await dataSource.Pull();

            foreach (var data in dataCollection)
                await _sender.SendAsync(data);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_sender != null)
            await _sender.DisposeAsync();

        await base.StopAsync(cancellationToken);
    }
}