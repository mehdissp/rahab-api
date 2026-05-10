using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces
{
  public  interface IUnitOfWork
    {
        Task SaveChanges(CancellationToken cancellationToken);

        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
         Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    }
}
