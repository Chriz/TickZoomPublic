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
using TickZoom.Common;

//using mscoree;






// for domain enum                            // for domain enum. Add the following as a COM reference - C:\WINDOWS\Microsoft.NET\Framework\vXXXXXX\mscoree.tlb

#if TESTING
namespace TickZoom.TradingFramework
{
	public class RandomMock : RandomCommon {
	Log log = Factory.Log.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		public double LongStop = 0;
		public double ShortStop = 0;
		public override bool OnProcessTick(Tick tick)
		{
			Exit.EnableWrongSideOrders = true;
	//			log.WriteFile( Position.Signal + ": " + tick.ToString());
			if( Position.IsShort) {
				Exit.BuyStop( LongStop);
			}
			if( Position.IsLong) {
				Exit.SellStop( ShortStop);
			}
			base.OnProcessTick(tick);
			return true;
		}
	}
}
#endif
