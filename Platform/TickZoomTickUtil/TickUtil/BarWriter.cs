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
 * Date: 3/8/2009
 * Time: 10:13 AM
 * <http://www.tickzoom.org/wiki/Licenses>.
 */
#endregion

using System;
using TickZoom.Api;

namespace TickZoom.TickUtil
{
	/// <summary>
	/// Description of BarImport.
	/// </summary>
	public class BarWriter : TickWriter
	{
		TickImpl openTick = new TickImpl();
		TickImpl highTick = new TickImpl();
		TickImpl lowTick = new TickImpl();
		TickImpl closeTick = new TickImpl();
		TimeStamp timeStamp = new TimeStamp();
		
		public BarWriter(bool eraseFileToStart) : base( eraseFileToStart) {
			
		}
		
		public void AddBar(double time, double open, double high, double low, double close, int volume, int openInterest) {
			timeStamp.Internal = time;
			closeTick.init( timeStamp, TradeSide.None, 0, volume, close, close);
			timeStamp.AddMilliseconds(-1);
			highTick.init( timeStamp, TradeSide.None, 0, 0, high, high);
			timeStamp.AddMilliseconds(-1);
			lowTick.init( timeStamp, TradeSide.None, 0, 0, low, low);
			timeStamp.AddMilliseconds(-1);
			openTick.init( timeStamp, TradeSide.None, 0, 0, open, open);
			Add(openTick);
			Add(lowTick);
			Add(highTick);
			Add(closeTick);
		}
	}
}
