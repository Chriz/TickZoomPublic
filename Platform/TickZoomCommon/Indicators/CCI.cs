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
 * Date: 5/15/2009
 * Time: 10:58 PM
 * <http://www.tickzoom.org/wiki/Licenses>.
 */
#endregion

using System;
using System.Drawing;
using TickZoom.Api;

namespace TickZoom.Common
{
	/// <summary>
	/// The Commodity Channel Index (CCI) examines the variation
	/// of a price from the mean. High values indicate that prices
	/// are very high compared to the average whereas low values
	/// point out that prices are considerably lower than average.
	/// </summary>
	public class CCI : IndicatorCommon
	{
		#region Variables
		int	period		= 14;
		SMA sma;
		#endregion

		public CCI(object anyPrice, int period)
		{
			AnyInput = anyPrice;
			StartValue = 0;
			this.period = period;
		}
		
		public override void OnInitialize()
		{
			Name = "CCI";
			Drawing.Color = Color.Orange;
			Drawing.PaneType = PaneType.Secondary;
			Drawing.IsVisible = true;
			
			Formula.Line(200, Color.DarkGray, "Level 2");
			Formula.Line(100, Color.DarkGray, "Level 1");
			Formula.Line(0,   Color.DarkGray, "Zero line");
			Formula.Line(-100, Color.DarkGray, "Level -1");
			Formula.Line(-200, Color.DarkGray, "Level -2");
			sma = Formula.SMA(Input, Period);
		}
		
		public override void Update()
		{
			int currBar = Input.CurrentBar;
			double s = sma[0];
			double val = Input[0];
			
			double mean = 0;
			for (int idx = Math.Min(Input.CurrentBar, Period - 1); idx >= 0; idx--)
				mean += Math.Abs(Input[idx] - sma[0]);
			this[0] = (Input[0] - sma[0]) / (mean == 0 ? 1 : (0.015 * (mean / Math.Min(Period, Input.CurrentBar + 1))));
			double result = this[0];
		}

		public int Period {
			get { return period; }
			set { period = value; }
		}
	}
}
