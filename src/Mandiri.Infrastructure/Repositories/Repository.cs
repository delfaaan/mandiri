using Mandiri.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Mandiri.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Mandiri.Infrastructure.Repositories
{
	public class Repository<T> : IRepository<T> where T : class
	{
		private readonly AppDbContext _context;
		private IDbContextTransaction? _currentTransaction;

		public Repository(AppDbContext context)
		{
			_context = context;
		}

		public async Task BeginTransactionAsync()
		{
			_currentTransaction ??= await _context.Database.BeginTransactionAsync();
		}

		public async Task CommitTransactionAsync()
		{
			if (_currentTransaction != null)
			{
				await _currentTransaction.CommitAsync();
				await _currentTransaction.DisposeAsync();
				_currentTransaction = null;
			}
		}

		public async Task RollbackTransactionAsync()
		{
			if (_currentTransaction != null)
			{
				await _currentTransaction.RollbackAsync();
				await _currentTransaction.DisposeAsync();
				_currentTransaction = null;
			}
		}

		public async Task SaveAsync()
		{
			await _context.SaveChangesAsync();
		}

		public async Task<T?> GetByIdAsync(int id)
		{
			return await _context.Set<T>().FindAsync(id);
		}

		public async Task<IEnumerable<T>> GetAllAsync()
		{
			return await _context.Set<T>().ToListAsync();
		}

		public async Task AddAsync(T entity)
		{
			await _context.Set<T>().AddAsync(entity);
		}

		public async Task UpdateAsync(T entity)
		{
			_context.Set<T>().Update(entity);
		}

		public async Task DeleteAsync(T entity)
		{
			_context.Set<T>().Remove(entity);
		}
	}
}