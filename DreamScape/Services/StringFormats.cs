using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamScape.Services
{
    public class StringFormats
    {

		public static string CapitalizeFirstLetter(string input)
		{
			if(string.IsNullOrWhiteSpace(input))
				return input;

			return char.ToUpper(input[0]) + input.Substring(1);
		}

	}
}
