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
using TickZoom.Api;
using TickZoom.Common;

//using mscoree;






// for domain enum                            // for domain enum. Add the following as a COM reference - C:\WINDOWS\Microsoft.NET\Framework\vXXXXXX\mscoree.tlb

#if TESTING
namespace TickZoom.TradingFramework
{
	[TestFixture]
	public class ExitStrategyMiscTest : MarshalByRefObject
	{
		private static readonly Log log = Factory.Log.GetLogger(typeof(ExitStrategyTest));
		private static readonly bool debug = log.IsDebugEnabled;
		private static readonly bool trace = log.IsTraceEnabled;
		ExitStrategyMock exitStrategy;
		
	    	[TestFixtureSetUp]
	    	public virtual void Init() {
	    		TimeStamp.SetToUtcTimeZone();
			log.Notice("Setup ExitStrategyTest");
	    	}
	    	
	    	[TestFixtureTearDown]
	    	public void Dispose() {
	    		TimeStamp.ResetUtcOffset();
	    	}
		
		[Test]
		public void Constructor()
		{
			StrategyCommon logic = new StrategyCommon();
			exitStrategy = new ExitStrategyMock(logic);
			Assert.IsNotNull(exitStrategy,"ExitSupport constructor");
			logic.ExitStrategy = exitStrategy;
			if( trace) log.Trace(exitStrategy.Chain.ToString());
			Assert.AreSame(logic.PositionSize,exitStrategy.Chain.Previous.Model,"Strategy property");
		}
		
		[Test]
		public void Variables()
		{
			StrategyCommon logic = new StrategyCommon();
			ExitStrategyCommon strategy = new ExitStrategyCommon(logic);
			Assert.AreEqual(false,strategy.ControlStrategy,"ControlStrategySignal");
			Assert.AreEqual(0,strategy.Position.Signal,"Signal");
			Assert.AreEqual(0,strategy.StopLoss,"Stop");
			Assert.AreEqual(0,strategy.TargetProfit,"Target");
		}
		
		[Test]
		public void DataSeriesSetup()
		{
	    		StrategyCommon logic = new StrategyCommon();
			ExitStrategyCommon exit = logic.ExitStrategy;
			
			Starter starter = new HistoricalStarter();
			starter.EndCount = 1;
			starter.ProjectProperties.Starter.Symbols = "USD_JPY_YEARS";
			starter.ProjectProperties.Starter.IntervalDefault = Intervals.Hour1;
			starter.DataFolder = "TestData";
			starter.Run(logic);
			
			Assert.AreSame(logic.Hours,exit.Hours,"Exit Signal before entry");
			Assert.AreSame(logic.Ticks,exit.Ticks,"Exit Signal before entry");
			Assert.AreEqual(1,exit.Hours.Count,"Number of hour bars ");
			Assert.AreEqual(1,exit.Ticks.Count,"Number of tick bars ");
		}
		
	}
}
#endif
