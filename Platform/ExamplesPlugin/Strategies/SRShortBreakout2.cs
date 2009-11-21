#region Copyright
/*
 * Software: TickZoom Trading Platform
 * Copyright 2009 M. Wayne Walter
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, see <http://www.tickzoom.org/wiki/Licenses>
 * or write to Free Software Foundation, Inc., 51 Franklin Street,
 * Fifth Floor, Boston, MA  02110-1301, USA.
 * 
 */
#endregion

using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using TickZoom.Api;
using TickZoom.Common;

namespace TickZoom
{
	/// <summary>
	/// This strategy checks once per day to setup a trade when
	/// the typical daily price crosses over last week's high.
	/// 
	/// It checks every minute to trigger a long when the minute low
	/// crosses below last weeks low.  That gives a better entry for a long.
	/// 
	/// It exits when the daily typical price crosses below last week's low.
	/// 
	/// And it has a stop loss.
	/// 
	/// 	/// </summary>
	public class SRShortBreakout2 : SRBreakoutSupport
	{
		public SRShortBreakout2()
		{
			ExitStrategy.ControlStrategy = true;
			IntervalDefault = Intervals.Day1;
			MinimumMove = 370;
			ExitStrategy.StopLoss = 300;
			ExitStrategy.TrailStop = 2500;
		}
		
		public override bool OnIntervalClose(Interval timeFrame) {
			
			if( timeFrame.Equals(Intervals.Minute1)) {
				
				if( Formula.CrossesUnder(Minutes.High,(int) Support[0])) {
					Enter.SellMarket();
				}
				
				if( Position.IsShort ) {
					if( Minutes.High[0] > Resistance[1]) {
						Exit.GoFlat();
					}
				}
			}
			return true;
		}
		
	}
}
