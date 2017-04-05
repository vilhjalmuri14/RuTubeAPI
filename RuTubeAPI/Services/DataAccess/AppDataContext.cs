using Microsoft.EntityFrameworkCore;
using RuTubeAPI.Services.Models.Entities;

namespace RuTubeAPI.Services.DataAccess
{
	public class AppDataContext : DbContext, IDbContext
	{
		public AppDataContext(DbContextOptions<AppDataContext> options)
		: base(options)
		{
		}

        public DbSet<kings> kings { get; set; }

        public DbSet<User>              	Users              		{ get; set; }
		public DbSet<Video>              	Videos              	{ get; set; }
		public DbSet<FavoriteVideosByUser> 	FavoriteVideosByUsers 	{ get; set; }
		public DbSet<UserFriend>           	UserFriends 			{ get; set; }
		public DbSet<VideosByChannel>       VideosByChannels 		{ get; set; }
		public DbSet<Channel>      	 		Channels		 		{ get; set; }
	}
}