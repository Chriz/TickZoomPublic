﻿#region Copyright
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
 * Date: 11/24/2009
 * Time: 4:07 PM
 * <http://www.tickzoom.org/wiki/Licenses>.
 */
#endregion

using System;
using System.IO;
using System.Text;
using System.Xml;

namespace TickZoom.Api
{
	/// <summary>
	/// Description of SettingsFile.
	/// </summary>
	public class ConfigFile : System.Configuration.AppSettingsReader
	{
		private XmlNode node;
		private string _cfgFile;
		
		public ConfigFile() {
			string appName = Environment.CommandLine.Trim();
			appName = appName.Replace("\"","");
			appName = appName.Split(' ')[0];
			appName = Path.GetFileNameWithoutExtension(appName);
		}
			
		public ConfigFile(string name) {
  			string appDataPath = Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData);
			appDataPath += @"/TickZoom";
			Directory.CreateDirectory(appDataPath);
			_cfgFile = appDataPath + @"/"+name+".config";
			if( !Exists) {
				CreateFile();
			}
		}
		
		public bool Exists {
			get {
				return File.Exists(_cfgFile);
			}
		}

		public string GetValue (string property)
		{
			XmlDocument doc = new XmlDocument();
			loadDoc(doc);
			string key = "//appSettings//add[@key='" + property + "']";
			string sNode = "//appSettings";
			// retrieve the selected node
			try
			{
				node =  doc.SelectSingleNode(sNode);
				if( node == null )
				{
					return null;
				}
				// Xpath selects element that contains the key
				XmlElement targetElem= (XmlElement)node.SelectSingleNode(key) ;
				if (targetElem==null)
				{
					return null;
				}
				return targetElem.GetAttribute("value");
			}
			catch
			{
				return null;
			}
		}

		public new object GetValue (string property, System.Type sType)
		{
			string retVal = GetValue(property);
			if (sType == typeof(string))
				return Convert.ToString(retVal);
			else
				if (sType == typeof(bool))
			{
				if (retVal.Equals("True") || retVal.Equals("False"))
					return Convert.ToBoolean(retVal);
				else
					return false;
			}
			else
				if (sType == typeof(int))
				return Convert.ToInt32(retVal);
			else
				if (sType == typeof(double))
				return Convert.ToDouble(retVal);
			else
				if (sType == typeof(DateTime))
				return Convert.ToDateTime(retVal);
			else
				return Convert.ToString(retVal);
		}

		public bool SetValue (string property, string val)
		{
			XmlDocument doc = new XmlDocument();
			loadDoc(doc);
			try
			{
				// retrieve the target node
				string key = "//appSettings//add[@key='" + property + "']";
				string sNode = "//appSettings";
				node =  doc.SelectSingleNode(sNode);
				if( node == null ) {
					throw new ApplicationException("Can't find appSettings configuration section in " + _cfgFile);
				}
				// Set element that contains the key
				XmlElement targetElem= (XmlElement) node.SelectSingleNode(key);
				if (targetElem!=null)
				{
					// set new value
					targetElem.SetAttribute("value", val);
				}
					// create new element with key/value pair and add it
				else
				{
					
					sNode = key.Substring(key.LastIndexOf("//")+2);
					
					XmlElement entry = doc.CreateElement(sNode.Substring(0, sNode.IndexOf("[@")).Trim());
					sNode =  sNode.Substring(sNode.IndexOf("'")+1);
					
					entry.SetAttribute("key", sNode.Substring(0, sNode.IndexOf("'")) );
					
					entry.SetAttribute("value", val);
					node.AppendChild(entry);
				}
				saveDoc(doc, this._cfgFile);
				return true;
			}
			catch
			{
				return false;
			}
		}

		private void saveDoc (XmlDocument doc, string docPath)
		{
			// save document
			// choose to ignore if web.config since it may cause server sessions interruptions
			if(  this._cfgFile.Equals("web.config") )
				return;
			else
				try
				{
					XmlTextWriter writer = new XmlTextWriter( docPath , null );
					writer.Formatting = Formatting.Indented;
					doc.WriteTo( writer );
					writer.Flush();
					writer.Close();
					return;
				}
				catch
				{}
		}

		public bool removeElement (string key)
		{
			XmlDocument doc = new XmlDocument();
			loadDoc(doc);
			try
			{
				string sNode = key.Substring(0, key.LastIndexOf("//"));
				// retrieve the appSettings node
				node =  doc.SelectSingleNode(sNode);
				if( node == null )
					return false;
				// XPath select setting "add" element that contains this key to remove
				XmlNode nd = node.SelectSingleNode(key);
				node.RemoveChild(nd);
				saveDoc(doc, this._cfgFile);
				return true;
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		private void loadDoc ( XmlDocument doc )
		{
			using( StreamReader streamReader = new StreamReader(_cfgFile)) {
				doc.Load( streamReader);
			}
		}
		
		private void CreateFile() {
			string contents = BeautifyXML(defaultContents);
	        using (StreamWriter sw = new StreamWriter(_cfgFile)) 
	        {
	            // Add some text to the file.
	            sw.Write( contents);
	        }
		}
		
		private static string BeautifyXML(string xml)
		{
			using( StringReader reader = new StringReader(xml)) {
				XmlDocument doc = new XmlDocument();
				doc.Load( reader);
			    StringBuilder sb = new StringBuilder();
			    XmlWriterSettings settings = new XmlWriterSettings();
			    settings.Indent = true;
			    settings.IndentChars = "  ";
			    settings.NewLineChars = "\r\n";
			    settings.NewLineHandling = NewLineHandling.Replace;
			    using( XmlWriter writer = XmlWriter.Create(sb, settings)) {
				    doc.Save(writer);
			    }
			    return sb.ToString();
			}
		}
		
		private string defaultContents = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <appSettings>
  </appSettings>
</configuration>";
	}
}