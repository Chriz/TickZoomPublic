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
using System.ComponentModel;
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
	public class RealTimeStarter : HistoricalStarter
	{
		Log log = Factory.Log.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public override Provider[] SetupTickQueues(bool quietMode, bool singleLoad)
		{
			return base.SetupDataProviders();
		}
		
		public override void Run(ModelInterface model)
		{
			ServiceConnection service = Factory.Provider.ProviderService();
			service.OnStart();
			runMode = RunMode.RealTime;
			base.Run(model);
			
			service.OnStop();
		}
	}
}
