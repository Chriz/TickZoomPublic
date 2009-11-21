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
 * Date: 3/15/2009
 * Time: 12:52 AM
 * <http://www.tickzoom.org/wiki/Licenses>.
 */
#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

using TickZoom.Api;
using TickZoom.Common;

namespace TickZoom
{
	/// <summary>
	/// Description of PortfolioControl.
	/// </summary>
	public partial class PortfolioControl : UserControl
	{
		bool isInitialized = false;
		Log log;
		ProjectProperties projectProperties;
		ProjectDoc projectDoc;
		
		public PortfolioControl()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
		}

		void PortfolioControlLoad(object sender, EventArgs e)
		{
			this.projectDoc = (ProjectDoc) this.Parent.Parent.Parent;
			if( !DesignMode && !isInitialized) {
				log = Factory.Log.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
				projectProperties = new ProjectProperties();
				treeView.LabelEdit = true;
				TickZoom.Api.ProjectProperties loadProjectProperties = ProjectPropertiesCommon.Create(new StreamReader(@"C:\TickZoom\portfolio.xml"));
				ReloadProjectModels(loadProjectProperties);
			}
		}
		
		private void ReloadProjectModels(TickZoom.Api.ProjectProperties loadProjectProperties) {
			ModelProperties model = loadProjectProperties.Model;
            TreeNode project = new TreeNode("New Portfolio Project");
           	PropertyTable properties = new PropertyTable(projectProperties);
           	project.Tag = properties;
	        PortfolioNode node = ReloadPortfolio(model);
           	loadProjectProperties.Chart.CopyProperties(projectProperties.Chart);
           	loadProjectProperties.Starter.CopyProperties(projectProperties.Starter);
           	loadProjectProperties.Engine.CopyProperties(projectProperties.Engine);
           	properties.UpdateAfterProjectFile();
            project.Nodes.Add(node);
            this.treeView.Nodes.Add(project);
			this.treeView.ExpandAll();
			this.treeView.SelectedNode = project;
			isInitialized = true;
		}

		private PortfolioNode ReloadPortfolio(ModelProperties properties) {
			PortfolioNode portfolio = new PortfolioNode(properties.Type,properties);
			portfolio.Name = properties.Name;
			string[] keys = properties.GetModelKeys();
			for( int i=0; i<keys.Length; i++) {
				ModelProperties modelProperties = properties.GetModel(keys[i]);
				if( modelProperties.ModelType == ModelType.Indicator) {
				} else {
					// type null for performance, positionsize, exitstrategy, etc.
					if( modelProperties.Type == null)
					{
//						HandlePropertySet( model, modelProperties);
					} else {
						PortfolioNode node = ReloadPortfolio(modelProperties);
						portfolio.Nodes.Add(node);
					}
				}
			}
			return portfolio;
		}

		private void NewProject() {
            TreeNode project = new TreeNode("New Portfolio Project");
            PortfolioNode portfolio = null;
            try {
            	project.Tag = new PropertyTable(projectProperties);
	            portfolio = new PortfolioNode("PortfolioCommon");
            } catch( Exception ex) {
            	log.Debug(ex.ToString());
            }
            project.Nodes.Add(portfolio);
//	        portfolio.Add("ExampleSimpleStrategy");
            portfolio.Add("ExampleSMAStrategy");
            this.treeView.Nodes.Add(project);
			this.treeView.ExpandAll();
			this.treeView.SelectedNode = project;
			isInitialized = true;
		}
		
		void TreeViewClick(object sender, EventArgs e)
		{
			
		}
		
		void TreeViewNodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if( e.Button == MouseButtons.Right) {
				// Show a menu.
			}
			if( e.Button == MouseButtons.Left) {
				
			}
		}
		
		void TreeViewAfterSelect(object sender, TreeViewEventArgs e)
		{
			if( projectDoc.MainForm.PropertyWindow.Visible) {
				projectDoc.MainForm.PropertyWindow.SelectedObject = e.Node.Tag;
				projectDoc.MainForm.PropertyWindow.PropertyGrid.PropertySort = PropertySort.NoSort;
				projectDoc.MainForm.PropertyWindow.PropertyGrid.ExpandAllGridItems();
			}
			WriteProject();
		}
		
		public void WriteProject() {
			TreeNode node = treeView.Nodes[0];
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.IndentChars = ("    ");
			using (XmlWriter writer = XmlWriter.Create(@"C:\TickZoom\portfolio.xml", settings))
			{
				writer.WriteStartDocument();
				SerializeNode(writer, node);
				writer.WriteEndDocument();
			    writer.Flush();
			    writer.Close();
			}
		}
		
		public void SerializeNode(XmlWriter writer, TreeNode node) {
			object obj = node.Tag;
			string modelName = obj.GetType().FullName;
	    	PropertyTable properties = obj as PropertyTable;
	    	if( properties != null) {
	    		if( typeof(Indicator).IsAssignableFrom(properties.Value.GetType())) {
			    	writer.WriteStartElement("indicator");
	    		} else if( typeof(StrategySupportInterface).IsAssignableFrom(properties.Value.GetType())) {
			    	writer.WriteStartElement("strategy");
	    		} else if( typeof(ProjectProperties).IsAssignableFrom(properties.Value.GetType())) {
			    	writer.WriteStartElement("projectproperties");
	    		} else {
			    	writer.WriteStartElement("model");
	    		}
		    	writer.WriteAttributeString("name",properties.Name);
	    		writer.WriteAttributeString("type",properties.Value.GetType().Name);
	    		properties.Serialize(writer);
	    	} else {
	    		throw new ApplicationException( "Found unexpected type in tree view for xml: " + obj.GetType());
	    	}
			for( int i=0; i<node.Nodes.Count; i++) {
				SerializeNode( writer, node.Nodes[i]);
			}
			writer.WriteEndElement();
		}
		
		void TreeViewNodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{ 
			e.Node.BeginEdit();
		}
		
		void TreeViewAfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			if( e.Label != null) {
				ModelInterface model = Plugins.Instance.GetModel( e.Label);
				e.Node.Tag = model;
				projectDoc.MainForm.PropertyWindow.SelectedObject = e.Node.Tag;
			}
		}
		
		public ProjectProperties ProjectProperties {
			get { return projectProperties; }
		}
	}
}
