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

#region Copyright
/*
 * Software: TickZoom Trading Platform
 * Copyright 2008 M. Wayne Walter
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.

 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

using TickZoom.Api;
using TickZoom.TickUtil;

namespace TickZoom.Common
{
	/// <summary>
	/// Description of Test.
	/// </summary>
	public class HistoricalStarter : StarterCommon
	{
		Log log = Factory.Log.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		TickEngine engine;
		protected RunMode runMode = RunMode.Historical;
   		
		public HistoricalStarter() 
		{
		} 
		
		public HistoricalStarter(bool releaseEngineCache) : base(releaseEngineCache)
		{
		} 
		
		bool CancelPending {
			get { if( BackgroundWorker != null) {
					return BackgroundWorker.CancellationPending;
				} else {
					return false;
				}
			}
		}

		public override void Run(ModelInterface model)
		{
			engine = Factory.Engine.TickEngine;
			ProjectProperties.Engine.CopyProperties(engine);
			// Chaining of models.
			engine.Model = model;
			engine.ChartProperties = ProjectProperties.Chart;
			engine.SymbolInfo = ProjectProperties.Starter.SymbolInfo;
			
			engine.IntervalDefault = ProjectProperties.Starter.IntervalDefault;
//			engine.BreakAtBar = ProjectProperties.Engine.BreakAtBar;
			engine.EnableTickFilter = ProjectProperties.Engine.EnableTickFilter;
			
			engine.Providers = SetupTickQueues(false,false);
			engine.BackgroundWorker = BackgroundWorker;
			engine.RunMode = runMode;
			engine.StartCount = StartCount;
			engine.EndCount = EndCount;
			engine.StartTime = ProjectProperties.Starter.StartTime;
			engine.EndTime = ProjectProperties.Starter.EndTime;
	
			if(CancelPending) return;
			
	    	engine.TickReplaySpeed = ProjectProperties.Engine.TickReplaySpeed;
	    	engine.BarReplaySpeed = ProjectProperties.Engine.BarReplaySpeed;
	    	engine.ShowChartCallback = ShowChartCallback;
			engine.CreateChartCallback = CreateChartCallback;
			
			engine.Run();

			if(CancelPending) return;
			
			if( engine.TickCount > 0 &&
			    ProjectProperties.Engine.BarReplaySpeed == 0 &&
			    ProjectProperties.Engine.TickReplaySpeed == 0 &&
			    ShowChartCallback!=null) {
				ShowChartCallback();
			}
		}
		
		public override void Wait() {
			// finishes during Run()
		}
	}
}
