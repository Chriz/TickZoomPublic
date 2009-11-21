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
using TickZoom.Api;
using TickZoom.Common;

namespace TickZoom
{
	/// <summary>
	/// Description of Starter.
	/// </summary>
	public class ExampleDualSymbolLoader : ModelLoaderCommon
	{
		public ExampleDualSymbolLoader() {
			/// <summary>
			/// IMPORTANT: You can personalize the name of each model loader.
			/// </summary>
			category = "Example";
			name = "Dual Symbol";
		}
		
		public override void OnInitialize(ProjectProperties properties) {
			if( properties.Starter.SymbolInfo.Length < 2) {
				throw new ApplicationException( "This loader requires at least 2 symbols.");
			}
		}
		
		public override void OnLoad(ProjectProperties properties) {
			ModelInterface fullTicks = CreateStrategy("ExampleOrderStrategy","FullTicksData");
			fullTicks.SymbolDefault = properties.Starter.SymbolInfo[0].Symbol;
			ModelInterface fourTicks = CreateStrategy("ExampleOrderStrategy","FourTicksData");
			fourTicks.SymbolDefault = properties.Starter.SymbolInfo[1].Symbol;
			AddDependency("PortfolioCommon","FullTicksData");
			AddDependency("PortfolioCommon","FourTicksData");
			StrategyCommon strategy = GetStrategy("PortfolioCommon");
			strategy.Performance.GraphTrades = false;
			TopModel = strategy;
		}
	}
}
