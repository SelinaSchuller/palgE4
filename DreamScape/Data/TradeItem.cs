using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamScape.Data
{
	internal class TradeItem
	{
		public int TradeId { get; set; }
		public Trade Trade { get; set; }
		public int ItemId { get; set; }
		public Item Item { get; set; }
		public int Quantity { get; set; }
		public bool IsOffered { get; set; }
	}
}
