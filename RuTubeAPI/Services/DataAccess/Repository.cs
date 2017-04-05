using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace RuTubeAPI.Services.DataAccess
{
	/// <summary>
	/// Class that implements Generic Repository pattern
	/// </summary>
	/// <typeparam name="T">Model class</typeparam>
	public class Repository<T> : IRepository<T> where T : class
	{
		private readonly IDbContext _context;
		private readonly DbSet<T> _dbset;

		/// <summary>
		/// Constructor that sets the DataContext and gets the DbSet
		/// </summary>
		/// <param name="context"></param>
		public Repository(IDbContext context)
		{
			_context = context;
			_dbset = context.Set<T>();
		}

		public virtual void Add(T entity)
		{
			_dbset.Add(entity);
		}

		public virtual void Delete(T entity)
		{
			var entry = _context.Entry(entity);
			entry.State = EntityState.Deleted;
		}

		public virtual void Update(T entity)
		{
			var entry = _context.Entry(entity);
			_dbset.Attach(entity);
			entry.State = EntityState.Modified;
		}

		public virtual IQueryable<T> All()
		{
			IQueryable<T> query = _dbset;
			return query;
		}
	}
}
