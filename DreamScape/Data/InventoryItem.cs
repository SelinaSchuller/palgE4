using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamScape.Data
{
	public class InventoryItem
	{
		public int UserId { get; set; }
		public User User { get; set; }
		public int ItemId { get; set; }
		public Item Item { get; set; }
		public int Quantity { get; set; }
	}
}
