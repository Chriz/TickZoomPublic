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
	public class SRShortDayLow : StrategyCommon
	{
		double entryLevel = 0;
		double profitTarget = 1000;
		IndicatorCommon dayHigh = new IndicatorCommon();
		IndicatorCommon dayLow = new IndicatorCommon();
		
		public SRShortDayLow()
		{
			IntervalDefault = Intervals.Minute1;
		}
		
		public override void OnInitialize() {
			AddIndicator(dayHigh);
			AddIndicator(dayLow);
		}
		
		public override bool OnIntervalClose(Interval timeFrame) {
			if( timeFrame.Equals(Intervals.Day1)) {
				// Exit at the end of the day.
				Exit.GoFlat();
			}
			if( timeFrame.Equals(IntervalDefault)) {
				dayHigh[0] = Days.High[1];
				dayLow[0] = Days.Low[1];
				// Do we have a setup?
				if( Next.Position.HasPosition ) {
					if( Formula.CrossesUnder( Bars.Typical, Days.Low[1])) {
						Enter.SellMarket();
						entryLevel = Bars.Close[0];
					}
				} else {
					Exit.GoFlat();
				}
				// Look for profit target!
				if( Position.HasPosition && Formula.CrossesUnder( Minutes.Low, entryLevel - profitTarget)) {
					Exit.GoFlat();
				}
			}
			return true;
		}
		
		public double ProfitTarget {
			get { return profitTarget; }
			set { profitTarget = value; }
		}
	}
}
