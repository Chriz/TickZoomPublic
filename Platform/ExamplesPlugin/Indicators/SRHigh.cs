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
using TickZoom.Api;
using TickZoom.Common;

namespace TickZoom
{
	/// <summary>
	/// Description of SMA.
	/// </summary>
	public class SRHigh : IndicatorCommon
	{
		double minimumMove;

		Integers pivots;
		
		public SRHigh() : this( 300)
		{
		}
		
		public SRHigh(double minMove)
		{
			pivots = Integers();
			this.minimumMove = minMove;
		}
		
		public override void OnInitialize() {

		}
		
		double lowestLow = int.MaxValue;
		double highestHigh = int.MinValue;
		public override bool OnIntervalClose() {
			if( Count == 1) {
				highestHigh = Bars.High[0];
				lowestLow = Bars.Low[0];
			} else {
				// this goes higher while..
				highestHigh = Math.Max( highestHigh, Bars.High[0]);
				
				// lowest goes lower
				lowestLow = Math.Min(Bars.Low[0],lowestLow);
				
				// when the high rises high enough it becomes a new potential pivot.
				if( Bars.High[0] > Formula.Highest(Bars.High,3,1) &&
				    Bars.High[0] - lowestLow > minimumMove) {
					highestHigh = Bars.High[0];
					lowestLow = Bars.Low[0];
				}
				
				// when low drops below highest far enough, it's a new pivot
				if( this[0] != highestHigh && highestHigh - Bars.Low[0] > minimumMove) {
					this[0] = highestHigh;
					pivots.Add((int) this[0]);
				}
				
			}
			return true;
		}
		
		public Integers Pivots {
			get { return pivots; }
		}
		
		public double MinimumMove {
			get { return minimumMove; }
			set { minimumMove = value; }
		}
	}
}
