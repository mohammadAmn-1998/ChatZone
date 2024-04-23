using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatZone.Domain.Entities.Chats;
using ChatZone.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace ChatZone.Domain.Context
{
	public class ChatDbContext : DbContext
	{

		public ChatDbContext(DbContextOptions<ChatDbContext> options): base(options)
		{
			
		}

		public DbSet<User> Users { get; set; }

		public DbSet<UserGroup> UserGroups { get; set; }

		public DbSet<Chat> Chats { get; set; }

		public DbSet<ChatGroup> ChatGroups { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
			base.OnModelCreating(modelBuilder);


			modelBuilder.Entity<Chat>().HasQueryFilter(e => !e.IsDeleted);
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<ChatGroup>().HasQueryFilter(e => !e.IsDeleted);
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<UserGroup>().HasQueryFilter(e => !e.IsDeleted);
			base.OnModelCreating(modelBuilder);


			foreach (var relationShip in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
				relationShip.DeleteBehavior = DeleteBehavior.Restrict;

			base.OnModelCreating(modelBuilder);

		}
	}
}
