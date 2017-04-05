using System;
using System.Collections.Generic;
using System.Linq;
using RuTubeAPI.Services.DataAccess;

namespace RuTubeAPI.Tests.MockObjects
{
	public class MockRepository<T> : IRepository<T> where T : class
	{
		#region Member variables
		private List<T> _context;
		#endregion

		public MockRepository(List<T> ctx)
		{
			_context = ctx;
		}

		public List<T> SetData(List<T> d)
		{
			var oldData = _context;
			_context = d;
			return oldData;
		}

		public virtual void Add(T entity)
		{
			_context.Add(entity);
		}

		public virtual void Delete(T entity)
		{
			_context.Remove(entity);
		}

		public virtual void Update(T entity)
		{
			var entry = _context.Where(s => s == entity).SingleOrDefault();
			entry = entity;
		}

		public virtual IQueryable<T> All()
		{
			return _context.AsQueryable();
		}
	}
}