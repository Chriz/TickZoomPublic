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
using NUnit.Framework;
using TickZoom.Common;

#if TESTING
namespace TickZoom.TradingFramework
{
	[TestFixture]
	public class EquityStatsTest : TradeStatsTest
	{
		private EquityStats equityStats;
		public override void Constructor(TransactionPairs trades)
		{
			equityStats = new EquityStats(trades,trades,trades,trades);
			baseStats = tradeStats = equityStats.Daily;
		}
		[Test]
		public void Daily()
		{
			Assert.AreEqual(186,equityStats.Daily.Count,"Daily Count");
			Assert.AreEqual(-0.0300,Math.Round(equityStats.Daily.Average,2),"Daily Average");
		}
		[Test]
		public void Weekly()
		{
			Assert.AreEqual(186,equityStats.Weekly.Count,"Weekly Count");
			Assert.AreEqual(-0.0300,Math.Round(equityStats.Weekly.Average,2),"Weekly Average");
		}
		[Test]
		public void Monthly()
		{
			Assert.AreEqual(186,equityStats.Weekly.Count,"Monthly Count");
			Assert.AreEqual(-0.0300,Math.Round(equityStats.Weekly.Average,2),"Monthly Average");
		}
		
	}
}
#endif
