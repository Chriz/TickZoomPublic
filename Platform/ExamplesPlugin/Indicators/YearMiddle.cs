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
using TickZoom.Api;
using System.Collections.Generic;
using System.Drawing;
using TickZoom.Common;

namespace TickZoom
{
	/// <summary>
	/// Description of EMA.
	/// </summary>
	public class YearMiddle : IndicatorCommon
	{
		public YearMiddle() {
			IntervalDefault = Intervals.Day1;
			Drawing.Color = Color.Green;
			Drawing.PaneType = PaneType.Primary;
		}
		
		public override bool OnIntervalClose() {
			if( Years.Count > 1) {
//				this[0] = (Math.Min(Formula.Years.Low[1],Formula.Years.Low[0]) +
//				                    Math.Max(Formula.Years.High[1],Formula.Years.High[0]))/2;
//				this[0] = (Formula.Years.Low[1] + Formula.Years.High[0])/2;
//				this[0] = (Formula.Years.Low[0] + Formula.Years.High[1])/2;
//				this[0] = (Formula.Highest(Formula.Months.High,12)+Formula.Lowest(Formula.Months.Low,12))/2;
				this[0] = (Years.Low[1] + Years.High[1])/2;
			}
			return true;
		}
		
	}
}
