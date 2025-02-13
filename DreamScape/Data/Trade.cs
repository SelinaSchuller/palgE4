﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamScape.Data
{
	internal class Trade
	{
		public int Id { get; set; }
		public DateTime CreatedAt { get; set; }
		public int SenderId { get; set; }
		public User Sender { get; set; }
		public int ReceiverId { get; set; }
		public User Receiver { get; set; }
		public int StatusId { get; set; }
		public TradeStatus Status { get; set; }
	}
}
