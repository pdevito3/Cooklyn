namespace Cooklyn.IntegrationTests;

using Cooklyn.Server.Databases;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

public class TestingServiceScope
{
    private readonly IServiceScope _scope;

    public TestingServiceScope()
    {
        _scope = TestFixture.BaseScopeFactory.CreateScope();
    }

    public TScopedService GetService<TScopedService>() where TScopedService : notnull
    {
        return _scope.ServiceProvider.GetRequiredService<TScopedService>();
    }

    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        var mediator = _scope.ServiceProvider.GetRequiredService<ISender>();
        return await mediator.Send(request);
    }

    public async Task SendAsync(IRequest request)
    {
        var mediator = _scope.ServiceProvider.GetRequiredService<ISender>();
        await mediator.Send(request);
    }

    public async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues)
        where TEntity : class
    {
        var context = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await context.FindAsync<TEntity>(keyValues);
    }

    public async Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        var context = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Add(entity);
        await context.SaveChangesAsync();
    }

    public async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
    {
        return await action(_scope.ServiceProvider);
    }

    public Task<T> ExecuteDbContextAsync<T>(Func<AppDbContext, Task<T>> action)
        => ExecuteScopeAsync(sp => action(sp.GetRequiredService<AppDbContext>()));

    public async Task<int> InsertAsync<T>(params T[] entities) where T : class
    {
        return await ExecuteDbContextAsync(async db =>
        {
            foreach (var entity in entities)
            {
                db.Set<T>().Add(entity);
            }
            return await db.SaveChangesAsync();
        });
    }
}
