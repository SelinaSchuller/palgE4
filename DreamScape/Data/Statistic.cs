using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamScape.Data
{
	public class Statistic
	{
		public int Id { get; set; }
		public int? RequiredLevel { get; set; } //Voor nu nullable
		public int Strength { get; set; }
		public int Speed { get; set; }
		public int Durability { get; set; }
		public int MagicId { get; set; }
		public Magic Magic { get; set; }
		public int RarityId { get; set; }
		public Rarity Rarity { get; set; }
	}
}
