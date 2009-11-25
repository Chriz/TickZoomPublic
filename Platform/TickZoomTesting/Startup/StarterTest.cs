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
using System.Configuration;
using System.IO;
using System.Threading;

using NUnit.Framework;
using TickZoom.Api;
using TickZoom.Common;

#if REALTIME

#endif


namespace TickZoom.StarterTest
{
	[TestFixture]
	public class StarterTest
	{
	    string storageFolder;
		Log log = Factory.Log.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
	    public delegate void ShowChartDelegate(ChartControl chart);
		List<ChartThread> chartThreads = new List<ChartThread>();
		
	    public StarterTest() {
    		storageFolder = Factory.Settings["AppDataFolder"];
   			if( storageFolder == null) {
       			throw new ApplicationException( "Must set AppDataFolder property in app.config");
   			}
	    }
		[SetUp]
		public void TestSetup() {
		}
		
		[TearDown]
		public void TearDown() {
		}
		
		[Test]
		public void TestHistorical()
		{
			Starter starter = new HistoricalStarter();
			starter.ProjectProperties.Starter.StartTime = (TimeStamp) new DateTime(2005,1,1);
    		starter.ProjectProperties.Starter.EndTime = (TimeStamp) new DateTime(2006,2,1);
    		starter.DataFolder = "TestData";
    		starter.ProjectProperties.Starter.Symbols = "USD_JPY";
			Interval intervalDefault = Intervals.Hour1;
			starter.ProjectProperties.Starter.IntervalDefault = intervalDefault;
			
			// No charting setup for these tests. Running without charts.
			
			ModelLoader loader = new OptimizeLoader();
    		starter.Run(loader);
    		PortfolioCommon strategy = loader.TopModel as PortfolioCommon;
    		Assert.AreEqual(33,strategy.Performance.ComboTrades.Count);
    		
		}
		
		[Test]
		public void TestDesign()
		{
			int start = Environment.TickCount;
			Starter starter = new DesignStarter();
			ModelInterface model = new TestSimpleStrategy();
    		starter.Run(model);
    		starter.Wait();
    		int elapsed = Environment.TickCount - start;
    		Assert.Less( elapsed, 4000);
    		// Verify the chart data.
    		Assert.IsNotNull( model.Data);
    		Assert.IsNotNull( model.Bars);
		}
		
		[Test]
		public void TestOptimize()
		{
			Starter starter = new OptimizeStarter();
    		starter.ProjectProperties.Starter.StartTime = (TimeStamp) new DateTime(2005,1,1);
    		starter.ProjectProperties.Starter.EndTime = (TimeStamp) new DateTime(2006,2,1);
			starter.ProjectProperties.Starter.IntervalDefault = Intervals.Hour1;
    		starter.DataFolder = "TestData";
    		starter.ProjectProperties.Starter.Symbols = "USD_JPY";
    		starter.Run(new OptimizeLoader());
    		Assert.IsTrue(FileCompare(storageFolder+@"\Statistics\optimizeResults.csv",@"..\..\Platform\TickZoomTesting\Startup\optimizeResults.csv"));
		}
		
		[Test]
		public void TestOptimizeBadVariable()
		{
			Thread.Sleep(2000); // Delay for file lock to get released.
			Starter starter = new OptimizeStarter();
    		starter.ProjectProperties.Starter.StartTime = (TimeStamp) new DateTime(2005,1,1);
    		starter.ProjectProperties.Starter.EndTime = (TimeStamp) new DateTime(2006,2,1);
			starter.ProjectProperties.Starter.IntervalDefault = Intervals.Hour1;
    		starter.DataFolder = "TestData";
    		starter.ProjectProperties.Starter.Symbols = "USD_JPY";
    		starter.Run(new OptimizeLoaderBad());
    		Assert.IsFalse(File.Exists(storageFolder+@"\Statistics\optimizeResults.csv"));
		}
		
		[Test]
		public void TestRealTime()
		{
#if REALTIME
			ProviderProxy provider = new ProviderProxy();

			Starter starter = new RealTimeStarter();
    		starter.ProjectProperties.Starter.StartTime = (TimeStamp) new DateTime(2004,1,1);
    		starter.ProjectProperties.Starter.EndTime = (TimeStamp) new DateTime(2004,2,1);
			starter.ProjectProperties.Starter.IntervalDefault = Intervals.Hour1;
			starter.AddDataFeed(provider);
			// No charting for these tests.
    		starter.DataFolder = "TestData";
    		starter.ProjectProperties.Starter.Symbols = "USD_JPY";
	   		starter.Run(new OptimizeLoader());	
#endif	   		
		}
		
		[Test]
		public void TestGenetic()
		{
			Starter starter = new GeneticStarter();
    		starter.ProjectProperties.Starter.StartTime = (TimeStamp) new DateTime(2005,1,1);
    		starter.ProjectProperties.Starter.EndTime = (TimeStamp) new DateTime(2006,2,1);
			starter.ProjectProperties.Starter.IntervalDefault = Intervals.Hour1;
     		starter.DataFolder = "TestData";
     		starter.ProjectProperties.Starter.Symbols = "USD_JPY";
    		starter.Run(new OptimizeLoader());
		}
	    
		public class OptimizeLoader : ModelLoaderCommon {
			public OptimizeLoader() {
				IsVisibleInGUI = false;
			}
			
			public override void OnInitialize(ProjectProperties properties) {
				AddVariable("ExampleSimpleStrategy.ExitStrategy.StopLoss",0.01,1.00,0.10,true);
				AddVariable("ExampleSimpleStrategy.ExitStrategy.TargetProfit",0.01,1.00,0.10,false);
			}		
			
			public override void OnLoad(ProjectProperties projectProperties)
			{
				ModelInterface model = CreateStrategy("ExampleSimpleStrategy");
				StrategyCommon strategy = model as StrategyCommon;
		    	AddDependency( "PortfolioCommon", "ExampleSimpleStrategy");
		    	TopModel = GetStrategy( "PortfolioCommon");
			}
		}
		
		public class OptimizeLoaderBad : ModelLoaderCommon {
			public OptimizeLoaderBad() {
				IsVisibleInGUI = false;
			}	
			
			public override void OnInitialize(ProjectProperties properties) {
				AddVariable("ExampleSimpleStrategy.xxxx.StopLoss",0.01,1.00,0.10,true);
				AddVariable("ExampleSimpleStrategy.xxxx.TargetProfit",0.01,1.00,0.10,true);
			}		
			
			public override void OnLoad(ProjectProperties projectProperties)
			{
				ModelInterface model = CreateStrategy("ExampleSimpleStrategy");
				StrategyCommon strategy = model as StrategyCommon;
		    	AddDependency( "PortfolioCommon", "ExampleSimpleStrategy");
		    	TopModel = GetStrategy( "PortfolioCommon");
			}
			
		}
		
		// This method accepts two strings the represent two files to 
		// compare. A return value of 0 indicates that the contents of the files
		// are the same. A return value of any other value indicates that the 
		// files are not the same.
		private bool FileCompare(string file1, string file2)
		{
		     int file1byte;
		     int file2byte;
		     FileStream fs1;
		     FileStream fs2;
		
		     // Determine if the same file was referenced two times.
		     if (file1 == file2)
		     {
		          // Return true to indicate that the files are the same.
		          return true;
		     }
		               
		     // Open the two files.
		     fs1 = new FileStream(file1, FileMode.Open);
		     fs2 = new FileStream(file2, FileMode.Open);
		          
		     // Check the file sizes. If they are not the same, the files 
		        // are not the same.
		     if (fs1.Length != fs2.Length)
		     {
		          // Close the file
		          fs1.Close();
		          fs2.Close();
		
		          // Return false to indicate files are different
		          return false;
		     }
		
		     // Read and compare a byte from each file until either a
		     // non-matching set of bytes is found or until the end of
		     // file1 is reached.
		     do 
		     {
		          // Read one byte from each file.
		          file1byte = fs1.ReadByte();
		          file2byte = fs2.ReadByte();
		     }
		     while ((file1byte == file2byte) && (file1byte != -1));
		     
		     // Close the files.
		     fs1.Close();
		     fs2.Close();
		
		     // Return the success of the comparison. "file1byte" is 
		     // equal to "file2byte" at this point only if the files are 
		        // the same.
		     return ((file1byte - file2byte) == 0);
		}
	}
}
