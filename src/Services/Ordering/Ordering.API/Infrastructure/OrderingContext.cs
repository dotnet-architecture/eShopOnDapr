using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.Services.Ordering.API.Model;
using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure.EntityConfigurations;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure
{
    public class OrderingContext : DbContext
    {
        public const string DEFAULT_SCHEMA = "ordering";

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; }
        public DbSet<CardType> CardTypes { get; set; }

        public OrderingContext(DbContextOptions<OrderingContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().OwnsOne(o => o.Address);

            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CardTypeEntityTypeConfiguration());
        }

        //public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    // Dispatch Domain Events collection. 
        //    // Choices:
        //    // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
        //    // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
        //    // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
        //    // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
        //    await _mediator.DispatchDomainEventsAsync(this);

        //    // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
        //    // performed through the DbContext will be committed
        //    var result = await base.SaveChangesAsync(cancellationToken);

        //    return true;
        //}

        //public async Task<IDbContextTransaction> BeginTransactionAsync()
        //{
        //    if (_currentTransaction != null) return null;

        //    _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        //    return _currentTransaction;
        //}

        //public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        //{
        //    if (transaction == null) throw new ArgumentNullException(nameof(transaction));
        //    if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

        //    try
        //    {
        //        await SaveChangesAsync();
        //        transaction.Commit();
        //    }
        //    catch
        //    {
        //        RollbackTransaction();
        //        throw;
        //    }
        //    finally
        //    {
        //        if (_currentTransaction != null)
        //        {
        //            _currentTransaction.Dispose();
        //            _currentTransaction = null;
        //        }
        //    }
        //}

        //public void RollbackTransaction()
        //{
        //    try
        //    {
        //        _currentTransaction?.Rollback();
        //    }
        //    finally
        //    {
        //        if (_currentTransaction != null)
        //        {
        //            _currentTransaction.Dispose();
        //            _currentTransaction = null;
        //        }
        //    }
        //}
    }

    //public class OrderingContextDesignFactory : IDesignTimeDbContextFactory<OrderingContext>
    //{
    //    public OrderingContext CreateDbContext(string[] args)
    //    {
    //        var optionsBuilder = new DbContextOptionsBuilder<OrderingContext>()
    //            .UseSqlServer("Server=.;Initial Catalog=Microsoft.eShopOnContainers.Services.OrderingDb;Integrated Security=true");

    //        return new OrderingContext(optionsBuilder.Options, new NoMediator());
    //    }

    //    class NoMediator : IMediator
    //    {
    //        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default(CancellationToken)) where TNotification : INotification
    //        {
    //            return Task.CompletedTask;
    //        }

    //        public Task Publish(object notification, CancellationToken cancellationToken = default)
    //        {
    //            return Task.CompletedTask;
    //        }

    //        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default(CancellationToken))
    //        {
    //            return Task.FromResult<TResponse>(default(TResponse));
    //        }

    //    }
    //}
}
