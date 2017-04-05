using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace RuTubeAPI.Services.DataAccess
{
	/// <summary>
	/// Interface for Entity Framework DataContext
	/// </summary>
	public interface IDbContext
	{
		DbSet<T> Set<T>() where T : class;
		EntityEntry<T> Entry<T>(T entity) where T : class;
		int SaveChanges();
		void Dispose();
	}
}
