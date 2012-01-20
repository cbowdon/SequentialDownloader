using System;
using System.Collections.Generic;

namespace ImageScraperLib
{
	/// <summary>
	/// Date type.
	/// Indexed class with each date type and format as a property.
	/// </summary>
	public class DateType
	{
		#region Properties
		public static string Iso { get { return "yyyyMMdd"; } }

		public static string Uk { get { return "ddMMyyyy"; } }

		public static string Us { get { return "MMddyyyy"; } }

		public static string IsoShort { get { return "yyMMdd"; } }

		public static string UkShort { get { return "ddMMyy"; } }

		public static string UsShort { get { return "MMddyy"; } }
		
		public static string NotRecognized { get { return ""; } }
		
		public int Length { get { return 6; }}
		#endregion
		
		#region Constructor
		public DateType ()
		{
		}
		#endregion
		
		#region Indexer and GetEnumerator
		/// <summary>
		/// Gets the <see cref="ImageScraperLib.DateType"/> at the specified index.
		/// I essentially wanted something between an enum and a dictionary
		/// </summary>
		/// <param name='index'>
		/// Index.
		/// </param>
		public string this [int index] {
			get {
				switch (index) {
				case 0:
					return Iso;
				case 1:
					return Uk;
				case 2:
					return Us;
				case 3:
					return IsoShort;
				case 4:
					return UkShort;
				case 5:
					return UsShort;
				default:
					return NotRecognized;
				}
			}
		}
		#endregion
	}
}

