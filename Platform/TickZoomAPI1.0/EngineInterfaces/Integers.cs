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
using System.Collections.Generic;

namespace TickZoom.Api
{
	public interface Integers : Series<int>  {
	}
	public interface Doubles : Series<double> {
	}
	public interface Longs : Series<long> {
	}
	public class WrapIntegers : Doubles {
		Integers integers;
		public WrapIntegers(Integers integers) {
			this.integers = integers;
		}
		public double this[int index] {
			get { return integers[index];	}
			set { integers[index] = (int) value;	}
		}
		public int Count {
			get { return integers.Count; } 
		}
		public int BarCount {
			get { return integers.BarCount; } 
		}
		public int CurrentBar {
			get { return integers.CurrentBar; } 
		}
		public void Add(double value)
		{
			int x = (int) value;
			integers.Add(x);
		}
		public void Clear()
		{
			integers.Clear();
		}
	}
	public class WrapPrices : Doubles {
		Prices prices;
		public WrapPrices(Prices prices) {
			this.prices = prices;
		}
		public double this[int index] {
			get { return prices[index];	}
			set { throw new InvalidOperationException("You can't modify price values when wrapped by Doubles"); }
		}
		public int Count {
			get { return prices.Count; } 
		}
		public int BarCount {
			get { return prices.BarCount; } 
		}
		public int CurrentBar {
			get { return prices.BarCount-1; } 
		}
		public void Add(double value)
		{
			throw new InvalidOperationException("You can't add price values when wrapped by Doubles");
		}
		public void Clear()
		{
			throw new InvalidOperationException("You can't clear price values when wrapped by Doubles");
		}
	}
}
