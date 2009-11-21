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
 * Date: 10/5/2009
 * Time: 12:43 PM
 * <http://www.tickzoom.org/wiki/Licenses>.
 */
#endregion


using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

using NUnit.Framework;
using TickZoom;

namespace MiscTest
{
	[TestFixture]
	public class GUITest
	{
		Form1 form;
		
		[SetUp]
		public void Setup() {
			Process[] processes = Process.GetProcessesByName("TickZoomProviderMock");
    		foreach( Process proc in processes) {
    			proc.Kill();
    		}
			form = new Form1();
			form.Show();
			WaitForEngine();
		}

		[TearDown]
		public void TearDown() {
			form.Close();
		}
		
		private void WaitComplete(int seconds) {
			WaitComplete(seconds,null);
		}
		private void WaitComplete(int seconds, Func<bool> onCompleteCallback) {
			long end = Environment.TickCount + (seconds * 1000);
			long current;
			while( (current = Environment.TickCount) < end) {
				Application.DoEvents();
				form.Catch();
				if( onCompleteCallback != null && onCompleteCallback()) {
					break;
				}
				Thread.Sleep(1);
			}
		}
		
		[Test]
		public void TestStartRun()
		{
			form.TxtSymbol.Text = "USD/JPY";
			form.DefaultBox.Text = "1";
			form.DefaultCombo.Text = "Hour";
			form.btnStartRun_Click(null,null);
			WaitComplete(30, () => { return !form.ProcessWorker.IsBusy; } );
			Assert.IsFalse(form.ProcessWorker.IsBusy,"ProcessWorker.Busy");
			Assert.AreEqual(form.PortfolioDocs.Count,1,"Charts");
		}
		
		public void WaitForEngine() {
			while( !form.IsEngineLoaded) {
				Thread.Sleep(1);
				Application.DoEvents();
			}
		}
		
		[Test]
		public void TestRealTimeNoHistorical()
		{
			form.TxtSymbol.Text = "IBM,/ESZ9";
			form.DefaultBox.Text = "10";
			form.DefaultCombo.Text = "Tick";
			form.RealTimeButtonClick(null,null);
			WaitComplete(30, () => { return form.PortfolioDocs.Count == 2; } );
			Assert.AreEqual(2,form.PortfolioDocs.Count,"Charts");
			form.btnStop_Click(null,null);
			WaitComplete(30, () => { return !form.ProcessWorker.IsBusy; } );
			Assert.IsFalse(form.ProcessWorker.IsBusy,"ProcessWorker.Busy");
		}
		
		[Test]
		public void TestRealTime()
		{
			form.TxtSymbol.Text = "MSFT";
			form.DefaultBox.Text = "10";
			form.DefaultCombo.Text = "Tick";
			form.RealTimeButtonClick(null,null);
			WaitComplete(30, () => { return form.PortfolioDocs.Count == 1; } );
			Assert.AreEqual(1,form.PortfolioDocs.Count,"Charts");
			form.btnStop_Click(null,null);
			
			WaitComplete(30, () => { return !form.ProcessWorker.IsBusy; } );
			Assert.IsFalse(form.ProcessWorker.IsBusy,"ProcessWorker.Busy");
			
			Assert.Greater(form.LogOutput.Lines.Length,2,"number of log lines");
		}
	}
}
