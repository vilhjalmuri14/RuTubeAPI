using System.Linq;

namespace RuTubeAPI.Services.DataAccess
{
	/// <summary>
	/// Interface for a Generic Repository
	/// </summary>
	/// <typeparam name="T">Model class</typeparam>
	public interface IRepository<T> where T : class
	{
		void Add(T entity);
		void Delete(T entity);
		void Update(T entity);
		IQueryable<T> All();
	}
}
