using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamScape.Data
{
	public class Magic
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int? Percentage { get; set; } //Example: "+30%"
		public int? Duration { get; set; } //Example: "Kan vijanden 3 sec verdoven"
		public int? Quantity { get; set; } //Example: "+5 HP per seconde"
	}

	//info:
	//Percentage → voor effects zoals "+30% fire damage" or "+15% dodge chance".
	//Duration → voor effects zoals "stun enemies for 3 seconds".
	//Quantity → voor effects zoals "+5 HP per second".
}
