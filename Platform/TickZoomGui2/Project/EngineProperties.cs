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
 * Date: 3/19/2009
 * Time: 12:02 AM
 * <http://www.tickzoom.org/wiki/Licenses>.
 */
#endregion

using System;
using System.ComponentModel;
using System.Drawing.Design;

using TickZoom.Api;

namespace TickZoom
{
	/// <summary>
	/// Description of EngineProperties.
	/// </summary>
	public class EngineProperties
	{
		public EngineProperties()
		{
			try {
				intervalDefault = Factory.Engine.DefineInterval(BarUnit.Default,0);
			} catch {
				
			}
		}
		
		bool enableTickFilter = false;
		
		[DefaultValue(false)]
		public bool EnableTickFilter {
			get { return enableTickFilter; }
			set { enableTickFilter = value; }
		}
		
		Interval intervalDefault;
		
		[Editor(typeof(IntervalPropertyEditor),typeof(UITypeEditor))]
		[TypeConverter(typeof(IntervalTypeConverter))]
		public Interval IntervalDefault {
			get { return intervalDefault; }
			set { intervalDefault = value; }
		}
		
		int breakAtBar = 0;
		
		[DefaultValue(0)]
		public int BreakAtBar {
			get { return breakAtBar; }
			set { breakAtBar = value; }
		}
		
		int maxBarsBack = 0;
		
		[DefaultValue(0)]
		public int MaxBarsBack {
			get { return maxBarsBack; }
			set { maxBarsBack = value; }
		}
		
		int maxTicksBack = 0;
		
		[DefaultValue(0)]
		public int MaxTicksBack {
			get { return maxTicksBack; }
			set { maxTicksBack = value; }
		}
		
		int tickReplaySpeed = 0;
		
		[DefaultValue(0)]
		public int TickReplaySpeed {
			get { return tickReplaySpeed; }
			set { tickReplaySpeed = value; }
		}

		int barReplaySpeed = 0;
		
		[DefaultValue(0)]
		public int BarReplaySpeed {
			get { return barReplaySpeed; }
			set { barReplaySpeed = value; }
		}
		
		public override string ToString()
		{
			return "";
		}
		
	}
}
