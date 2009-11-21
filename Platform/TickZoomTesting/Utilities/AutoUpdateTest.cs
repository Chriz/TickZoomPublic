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
 * Date: 5/12/2009
 * Time: 10:45 PM
 * <http://www.tickzoom.org/wiki/Licenses>.
 */
#endregion


using System;
using System.Configuration;
using System.IO;
using System.Reflection;

using NUnit.Framework;
using TickZoom.Api;

namespace TickZoom.Utilities
{
	[TestFixture]
	public class AutoUpdateTest
	{
		private static readonly Log log = Factory.Log.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private string testVersion = "0.5.15.1400";
		private string appData;
		private string dllFolder;
		[TestFixtureSetUp]
		public void Setup() {
			appData = ConfigurationSettings.AppSettings["AppDataFolder"];
			dllFolder =  appData + Path.DirectorySeparatorChar +
				@"AutoUpdate\" + testVersion + Path.DirectorySeparatorChar;
		}
		[Test]
		public void TestKeyNotFound()
		{
			AutoUpdate updater = new AutoUpdate();
			updater.UserKey = "BadKey";
			updater.CurrentVersion = testVersion;
			bool retVal = updater.DownloadFile("TickZoomEngine.dll");
			Assert.IsFalse(retVal,"did download");
			Assert.IsTrue(updater.Message.StartsWith("Your user key was not found. Please verify that you have a valid key.") );
		}
		[Test]
		public void TestBadKey()
		{
			AutoUpdate updater = new AutoUpdate();
			updater.CurrentVersion = testVersion;
			updater.UserKey = @"Bad Key";
			bool retVal = updater.DownloadFile("TickZoomEngine.dll");
			Assert.IsFalse(retVal,"did download");
			Assert.AreEqual("Your user key was not found. Please verify that you have a valid key.\n", updater.Message);
		}
		[Test]
		public void TestBadFile()
		{
			AutoUpdate updater = new AutoUpdate();
			updater.CurrentVersion = testVersion;
			bool retVal = updater.DownloadFile("TickZoomEngine.dllXXX");
			Assert.IsFalse(retVal,"did download");
			Assert.IsTrue(updater.Message.StartsWith("File TickZoomEngine"));
			Assert.IsTrue(updater.Message.EndsWith("not found for membership Gold.\n"));
		}

		[Test]
		public void TestFileListSuccess()
		{
			AutoUpdate updater = new AutoUpdate();
			updater.CurrentVersion = testVersion;
			string[] files = updater.GetFileList();
			Assert.AreEqual(2,files.Length);
			Assert.AreEqual("ProviderCommon-"+testVersion+".dll.zip 4914f31fb3c975b27e25b19083c00817",files[0]);
			Assert.AreEqual("TickZoomEngine-"+testVersion+".dll.zip 7e5fe5b44e7ab46d323368d6804f2534",files[1]);
		}
		
		[Test]
		public void TestFileListFailure()
		{
			AutoUpdate updater = new AutoUpdate();
			updater.UserKey = "";
			updater.CurrentVersion = testVersion;
			string[] files = updater.GetFileList();
			Assert.IsNull(files);
		}
		
		[Test]
		public void TestFreeEngineFailure()
		{
			AutoUpdate updater = new AutoUpdate();
			updater.UserKey = "";
			updater.CurrentVersion = testVersion;
			bool retVal = updater.DownloadFile("TickZoomEngine-"+testVersion+".dll");
			Assert.IsFalse(retVal,"did download");
		}
		
		[Test]
		public void TestFreeEngineSuccess()
		{
			AutoUpdate updater = new AutoUpdate();
//			updater.UserKey = "";
			updater.CurrentVersion = testVersion;
			bool retVal = updater.DownloadFile("TickZoomEngine-"+testVersion+".dll");
			Assert.IsTrue(retVal,"did download");
			Assert.AreEqual("", updater.Message);
			string appData = ConfigurationSettings.AppSettings["AppDataFolder"];
			string dllFile =  appData + Path.DirectorySeparatorChar +
				@"AutoUpdate\" + updater.CurrentVersion + Path.DirectorySeparatorChar +
				"TickZoomEngine-"+updater.CurrentVersion+".dll.zip";
			Assert.IsTrue(File.Exists(dllFile));
		}

		[Test]
		public void TestFreeUpdateAllFailure()
		{
			while( Directory.Exists(dllFolder)) {
				try {
					Directory.Delete(dllFolder,true);
					break;				
				} catch( IOException) {
				}
			}
			AutoUpdate updater = new AutoUpdate();
			updater.UserKey = "";
			updater.CurrentVersion = testVersion;
			bool retVal = updater.UpdateAll();
			Assert.IsFalse(retVal,"did download");
			Assert.AreEqual("Your user key was not found. Please verify that you have a valid key.\n", updater.Message);
		}
		
		[Test]
		public void TestFreeUpdateAll()
		{
			while( Directory.Exists(dllFolder)) {
				try {
					Directory.Delete(dllFolder,true);
					break;				
				} catch( IOException) {
				}
			}
			TestUpdateAll(null,"",true);
			TestUpdateAll(null,"",false);
		}
		
		[Test]
		public void TestGoldUpdateAll()
		{
			while( Directory.Exists(dllFolder)) {
				try {
					Directory.Delete(dllFolder,true);
					break;				
				} catch( IOException) {
				}
			}
			TestUpdateAll(null,"Engine",true);
			TestUpdateAll(null,"Engine",false);
		}
		
		public void TestUpdateAll(string userKey, string currentModel, bool isExpectedDownload)
		{
			AutoUpdate updater = new AutoUpdate();
			if( userKey != null) {
				updater.UserKey = userKey;
			}
			updater.CurrentVersion = testVersion;
			bool retVal = updater.UpdateAll();
			Assert.AreEqual(isExpectedDownload,retVal,"did download");
			Assert.AreEqual("", updater.Message);
			if( !isExpectedDownload) {
				return;
			}
			string dllFile = dllFolder + "TickZoomEngine-"+updater.CurrentVersion+".dll";
			Assert.IsTrue(File.Exists(dllFile));
			dllFile = dllFolder + "ProviderCommon-"+updater.CurrentVersion+".dll";
			Assert.IsTrue(File.Exists(dllFile));
		}
	}
}
