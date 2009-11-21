#region Copyright
/*
 * Copyright 2008 M. Wayne Walter
 * Software: TickZoom Trading Platform
 * User: Wayne Walter
 * 
 * You can use and modify this software under the terms of the
 * TickZOOM General Public License Version 1.0 or (at your option)
 * any later version.
 * 
 * Businesses are restricted to 30 days of use.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * TickZOOM General Public License for more details.
 *
 * You should have received a copy of the TickZOOM General Public
 * License along with this program.  If not, see
 * 
 * 
 *
 * User: Wayne Walter
 * Date: 3/18/2009
 * Time: 6:52 PM
 * <http://www.tickzoom.org/wiki/Licenses>.
 */
#endregion

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using TickZoom.Api;

namespace TickZoom.Common
{
	[Serializable]
	public class SymbolInfoCommon : PropertiesBase, TickZoom.Api.SymbolInfo
	{
		private Elapsed sessionStart = new Elapsed( 8, 0, 0);
		private Elapsed sessionEnd = new Elapsed( 16, 30, 0);
		private bool simulateTicks;
		private string symbol;
		private double minimumTick;
		private double fullPointValue;
		private int level2LotSize;
		private double level2Increment;
		private int level2LotSizeMinimum;
		private ulong binaryIdentifier;
		private SymbolInfo universalSymbol;
		
		public int Level2LotSizeMinimum {
			get { return level2LotSizeMinimum; }
			set { level2LotSizeMinimum = value; }
		}
		
		public SymbolInfoCommon() {
		}
		
	    public SymbolInfoCommon Copy()
	    {
	    	SymbolInfoCommon result;
	    	
	        using (var memory = new MemoryStream())
	        {
	            var formatter = new BinaryFormatter();
	            formatter.Serialize(memory, this);
	            memory.Position = 0;
	
	            result = (SymbolInfoCommon)formatter.Deserialize(memory);
	            memory.Close();
	        }
	
	        return result;
	    }
	    
		public override string ToString()
		{
			return symbol == null ? "empty" : symbol;
		}
		
		[Obsolete("Please create your data with the IsSimulateTicks flag set to true instead of this property.",true)]
		public bool SimulateTicks {
			get { return simulateTicks; }
			set { simulateTicks = value; }
		}
		
		public Elapsed SessionEnd {
			get { return sessionEnd; }
			set { sessionEnd = value; }
		}
		
		public Elapsed SessionStart {
			get { return sessionStart; }
			set { sessionStart = value; }
		}
		
		public double MinimumTick {
			get { return minimumTick; }
			set { minimumTick = value; }
		}
		
		public double FullPointValue {
			get { return fullPointValue; }
			set { fullPointValue = value; }
		}

		public string Symbol {
			get { return symbol; }
			set { symbol = value; }
		}
		
		public int Level2LotSize {
			get { return level2LotSize; }
			set { level2LotSize = value; }
		}
		
		public double Level2Increment {
			get { return level2Increment; }
			set { level2Increment = value; }
		}
		
		public SymbolInfo UniversalSymbol {
			get { return universalSymbol; }
			set { universalSymbol = value; }
		}
		
		public ulong BinaryIdentifier {
			get { return binaryIdentifier; }
			set { binaryIdentifier = value; }
		}
		
		public override bool Equals(object obj)
		{
			return ((SymbolInfo)obj).BinaryIdentifier == binaryIdentifier;
		}

		public override int GetHashCode()
		{
			return binaryIdentifier.GetHashCode();
		}
	}
}
