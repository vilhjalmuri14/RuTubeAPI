using System;
using Microsoft.EntityFrameworkCore;

namespace RuTubeAPI.Services.DataAccess
{
	/// <summary>
	/// Interface for Unit Of Work pattern
	/// </summary>
	public interface IUnitOfWork : IDisposable
	{
		IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
		void Save();
	}
}
