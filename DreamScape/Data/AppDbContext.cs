using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace DreamScape.Data
{
	internal class AppDbContext : DbContext
	{
		public DbSet<InventoryItem> InventoryItems { get; set; }
		public DbSet<Item> Items { get; set; }
		public DbSet<Magic> Magics { get; set; }
		public DbSet<Rarity> Rarities { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<Statistic> Statistics { get; set; }
		public DbSet<Trade> Trades { get; set; }
		public DbSet<TradeItem> TradeItems { get; set; }
		public DbSet<TradeStatus> TradeStatuses { get; set; }
		public DbSet<Type> Types { get; set; }
		public DbSet<User> Users { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseMySql(
				"server=localhost;port=3306;user=root;password=;database=DreamScape",
			ServerVersion.Parse("5.7.33")
			);
		}

		public static string HashPassword(string password)
		{
			using(SHA256 sha256 = SHA256.Create())
			{
				byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
				StringBuilder builder = new StringBuilder();
				foreach(byte b in bytes)
				{
					builder.Append(b.ToString("x2"));
				}
				return builder.ToString();
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			//User <-> Role (Many-to-One)
			modelBuilder.Entity<User>()
				.HasOne(u => u.Role)
				.WithMany()
				.HasForeignKey(u => u.RoleId);

			//InventoryItem (Pivot Table)
			modelBuilder.Entity<InventoryItem>()
				.HasKey(ii => new { ii.UserId, ii.ItemId });

			modelBuilder.Entity<InventoryItem>()
				.HasOne(ii => ii.User)
				.WithMany()
				.HasForeignKey(ii => ii.UserId);

			modelBuilder.Entity<InventoryItem>()
				.HasOne(ii => ii.Item)
				.WithMany()
				.HasForeignKey(ii => ii.ItemId);

			//Item <-> Type (Many-to-One)
			modelBuilder.Entity<Item>()
				.HasOne(i => i.Type)
				.WithMany()
				.HasForeignKey(i => i.TypeId);

			//Item <-> Statistics (Many-to-One)
			modelBuilder.Entity<Item>()
				.HasOne(i => i.Statistics)
				.WithMany()
				.HasForeignKey(i => i.StatisticsId);

			//Statistics <-> Rarity (Many-to-One)
			modelBuilder.Entity<Statistic>()
				.HasOne(s => s.Rarity)
				.WithMany()
				.HasForeignKey(s => s.RarityId);

			//Trade (Self-referencing relationships for Sender & Receiver)
			modelBuilder.Entity<Trade>()
				.HasOne(t => t.Sender)
				.WithMany()
				.HasForeignKey(t => t.SenderId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Trade>()
				.HasOne(t => t.Receiver)
				.WithMany()
				.HasForeignKey(t => t.ReceiverId)
				.OnDelete(DeleteBehavior.Restrict);

			//Trade <-> TradeStatus (Many-to-One)
			modelBuilder.Entity<Trade>()
				.HasOne(t => t.Status)
				.WithMany()
				.HasForeignKey(t => t.StatusId);

			//TradeItem (Pivot Table)
			modelBuilder.Entity<TradeItem>()
				.HasKey(ti => new { ti.TradeId, ti.ItemId });

			modelBuilder.Entity<TradeItem>()
				.HasOne(ti => ti.Trade)
				.WithMany()
				.HasForeignKey(ti => ti.TradeId);

			modelBuilder.Entity<TradeItem>()
				.HasOne(ti => ti.Item)
				.WithMany()
				.HasForeignKey(ti => ti.ItemId);

			//Roles:
			modelBuilder.Entity<Role>().HasData(
				new Role
				{
					Id = 1,
					Name = "Speler",
				},
				new Role
				{
					Id = 2,
					Name = "Beheerder",
				}
			);

			//Users:
			modelBuilder.Entity<User>().HasData(
				new User
				{
					Id = 1,
					Username = "ShadowSlayer",
					Password = HashPassword("Test123!"),
					Email = "shadow@example.com",
					RoleId = 1
				},
				new User
				{
					Id = 2,
					Username = "MysticMage",
					Password = HashPassword("Mage2024"),
					Email = "mystic@example.com",
					RoleId = 1
				},
				new User
				{
					Id = 3,
					Username = "DragonKnight",
					Password = HashPassword("Dragon!99"),
					Email = "dragon@example.com",
					RoleId = 1
				},
				new User
				{
					Id = 4,
					Username = "AdminMaster",
					Password = HashPassword("Admin007"),
					Email = "admin@example.com",
					RoleId = 2
				},
				new User
				{
					Id = 5,
					Username = "ThunderRogue",
					Password = HashPassword("Thund3r!!"),
					Email = "thunder@example.com",
					RoleId = 1
				}
			);

			//Types:
			modelBuilder.Entity<Type>().HasData(
				new Type
				{
					Id = 1,
					Name = "Wapen"
				},
				new Type
				{
					Id = 2,
					Name = "Accessoire"
				},
				new Type
				{
					Id = 3,
					Name = "Armor"
				}
			);

			//Rarities:
			modelBuilder.Entity<Rarity>().HasData(
				new Rarity
				{
					Id = 1,
					Name = "Normaal"
				},
				new Rarity
				{
					Id = 2,
					Name = "Zeldzaam"
				},
				new Rarity
				{
					Id = 3,
					Name = "Episch"
				},
				new Rarity
				{
					Id = 4,
					Name = "Legendarisch"
				}
			);

			//Magics:
			modelBuilder.Entity<Magic>().HasData(
				new Magic
				{
					Id = 1,
					Name = "Vuurschade",
					Percentage = 30,
					Duration = null,
					Quantity = null,
				},
				new Magic
				{
					Id = 2,
					Name = "Weerstand tegen ijsaanvallen",
					Percentage = 25,
					Duration = null,
					Quantity = null,
				},
				new Magic
				{
					Id = 3,
					Name = "Kans om aanvallen te ontwijken",
					Percentage = 15,
					Duration = null,
					Quantity = null,
				},
				new Magic
				{
					Id = 4,
					Name = "Kan vijanden verdoven",
					Percentage = null,
					Duration = 3, // Stun duration in seconds
					Quantity = null,
				},
				new Magic
				{
					Id = 5,
					Name = "Kans op kritieke schade",
					Percentage = 10,
					Duration = null,
					Quantity = null,
				},
				new Magic
				{
					Id = 6,
					Name = "Herstelt HP per seconde",
					Percentage = null,
					Duration = 1, // Stun duration in seconds
					Quantity = 5, // +5 HP per second
				},
				new Magic
				{
					Id = 7,
					Name = "Absorbeert ontvangen schade",
					Percentage = 20,
					Duration = null,
					Quantity = null,
				}
			);

			//Statistics:
			modelBuilder.Entity<Statistic>().HasData(
				new Statistic
				{
					Id = 1,
					RequiredLevel = null,
					Strength = 90,
					Speed = 60,
					Durability = 80,
					MagicId = 1, // Vuurschade
					RarityId = 4 // Legendarisch
				},
				new Statistic
				{
					Id = 2,
					RequiredLevel = null,
					Strength = 20,
					Speed = 10,
					Durability = 70,
					MagicId = 2, // Weerstand tegen ijsaanvallen
					RarityId = 3 // Episch
				},
				new Statistic
				{
					Id = 3,
					RequiredLevel = null,
					Strength = 40,
					Speed = 85,
					Durability = 50,
					MagicId = 3, // Kans om aanvallen te ontwijken
					RarityId = 2 // Zeldzaam
				},
				new Statistic
				{
					Id = 4,
					RequiredLevel = null,
					Strength = 95,
					Speed = 40,
					Durability = 90,
					MagicId = 4, // Kan vijanden verdoven
					RarityId = 4 // Legendarisch
				},
				new Statistic
				{
					Id = 5,
					RequiredLevel = null,
					Strength = 85,
					Speed = 75,
					Durability = 60,
					MagicId = 5, // Kans op kritieke schade
					RarityId = 3 // Episch
				},
				new Statistic
				{
					Id = 6,
					RequiredLevel = null,
					Strength = 10,
					Speed = 5,
					Durability = 100,
					MagicId = 6, // Herstelt HP per seconde
					RarityId = 2 // Zeldzaam
				},
				new Statistic
				{
					Id = 7,
					RequiredLevel = null,
					Strength = 75,
					Speed = 50,
					Durability = 95,
					MagicId = 7, // Absorbeert ontvangen schade
					RarityId = 4 // Legendarisch
				}
			);

			//Items:
			modelBuilder.Entity<Item>().HasData(
				new Item
				{
					Id = 101,
					Name = "Zwaard des Vuur",
					Description = "Een mythisch zwaard met een vlammende gloed.",
					TypeId = 1, // Wapen
					StatisticsId = 1
				},
				new Item
				{
					Id = 102,
					Name = "IJs Amulet",
					Description = "Een amulet dat de drager beschermt tegen kou.",
					TypeId = 2, // Accessoire
					StatisticsId = 2
				},
				new Item
				{
					Id = 103,
					Name = "Schaduw Mantel",
					Description = "Een donkere mantel die je bewegingen verbergt.",
					TypeId = 3, // Armor
					StatisticsId = 3
				},
				new Item
				{
					Id = 104,
					Name = "Hamer der Titanen",
					Description = "Een massieve hamer met de kracht van de aarde.",
					TypeId = 1, // Wapen
					StatisticsId = 4
				},
				new Item
				{
					Id = 105,
					Name = "Lichtboog",
					Description = "Een boog die pijlen van pure energie afvuurt.",
					TypeId = 1, // Wapen
					StatisticsId = 5
				},
				new Item
				{
					Id = 106,
					Name = "Helende Ring",
					Description = "Een ring die de gezondheid van de drager herstelt.",
					TypeId = 2, // Accessoire
					StatisticsId = 6
				},
				new Item
				{
					Id = 107,
					Name = "Demonen Harnas",
					Description = "Een verdoemd harnas met duistere krachten.",
					TypeId = 3, // Armor
					StatisticsId = 7
				}
			);

			//InventoryItems:
			modelBuilder.Entity<InventoryItem>().HasData(
				new InventoryItem
				{
					UserId = 1,
					ItemId = 101,
					Quantity = 1
				},
				new InventoryItem
				{
					UserId = 2,
					ItemId = 102,
					Quantity = 2
				},
				new InventoryItem
				{
					UserId = 3,
					ItemId = 102,
					Quantity = 1
				},
				new InventoryItem
				{
					UserId = 3,
					ItemId = 103,
					Quantity = 1
				}
			);

			//TradeStatuses:
			modelBuilder.Entity<TradeStatus>().HasData(
				new TradeStatus
				{
					Id = 1,
					Name = "Pending"
				},
				new TradeStatus
				{
					Id = 2,
					Name = "Accepted"
				},
				new TradeStatus
				{
					Id = 3,
					Name = "Declined"
				}
			);

			//Trades:
			modelBuilder.Entity<Trade>().HasData(
				new Trade
				{
					Id = 1,
					SenderId = 1,
					ReceiverId = 2,
					CreatedAt = DateTime.Now,
					StatusId = 1
				}
			);

			//TradeItem:
			modelBuilder.Entity<TradeItem>().HasData(
				new TradeItem
				{
					TradeId = 1,
					ItemId = 101,
					Quantity = 1,
					IsOffered = true
				},
				new TradeItem
				{
					TradeId = 1,
					ItemId = 102,
					Quantity = 2,
					IsOffered = false
				}
			);
		}
	}
}
