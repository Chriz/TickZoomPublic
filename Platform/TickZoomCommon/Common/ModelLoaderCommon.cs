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
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;

using TickZoom.Api;

namespace TickZoom.Common
{
	/// <summary>
	/// Description of Starter.
	/// </summary>
	public abstract class ModelLoaderCommon : ModelLoader
	{
		Log log = Factory.Log.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		IList<StrategyCommon> models = new List<StrategyCommon>();
		List<ModelProperty> variables = new List<ModelProperty>();
		List<OptimizeRange> rules = new List<OptimizeRange>();
		bool isVisibleInGUI = true;
		bool quietMode = false;
		ModelInterface topModel;
		Stream optimizeOutput;
		
		public ModelLoaderCommon()
		{ 
			name = this.GetType().Name;
		}
		
		public virtual void OnInitialize(ProjectProperties model) {
		}
		
		public abstract void OnLoad(ProjectProperties model);
		
		[Obsolete("Fallen into disuse.",true)]
		public IList<ModelInterface> Models {
			get { return null; }
		}

		public void OnClear() {
			models.Clear();
		}
		
		[Obsolete("Please use CreateStrategy() instead.",true)]
		public ModelInterface CreateModel( string type, string name) {
			return CreateStrategy(type,name);
		}
		 
		public StrategyCommon CreateStrategy( string type, string name) {
			ModelInterface model;
			for( int i = 0; i< models.Count; i++) {
				if( models[i].Name == name) {
					if( models[i].GetType().Name != type) {
						throw new ApplicationException("Model already exists with name " + name + " with different type");
					}
					return models[i] as StrategyCommon;
				}
			}
			model = CreateStrategy(type);
			model.Name = name;
			return model as StrategyCommon;
		}
		
		[Obsolete("Please use CreateStrategy() instead.",true)]
		public ModelInterface CreateModel( string type) {
			return CreateStrategy( type);
		}
		
		public StrategyCommon CreateStrategy( string type) {
			ModelInterface model;
			for( int i = 0; i< models.Count; i++) {
				if( models[i].Name == type) {
					if( models[i].GetType().Name != type) {
						throw new ApplicationException("Model already exists with name " + name + " with different type");
					}
					return models[i];
				}
			}
			try { 
				model = Plugins.Instance.GetModel(type);
			} catch( Exception ex) {
				log.Error( "Please make sure " + type + " exists and has a default constructor. ", ex);
				return null;
			}
			models.Add( model as StrategyCommon);
			return model as StrategyCommon;
		}

		public StrategyCommon GetStrategy( string name) {
			return (StrategyCommon) GetModelInternal( name);
		}
		
		[Obsolete("Please use GetStrategy() instead",true)]
		public ModelInterface GetModel( string name) {
			return GetModelInternal(name);
		}
		
		private ModelInterface GetModelInternal( string name) {
			ModelInterface model;
			for( int i = 0; i< models.Count; i++) {
				if( models[i].Name == name) {
					return models[i];
				}
			}
			model = CreateStrategy( name);
			if( model != null) {
				return model;
			}
			throw new ApplicationException( "Strategy name '"+name+"' was not found in GetStrategy()");
		}
		
		public void AddVariable(string name, double start, double end, double increment, bool isActive) {
			if( isActive) {
				ModelProperty property = Factory.Starter.ModelProperty(name,start.ToString(),start,end,increment,isActive);
				AddVariable(property);
			}
		}
		
		public void AddDependency( string current, string previous) {
			StrategyCommon previousStrategy = null;
			for( int i = 0; i< models.Count; i++) {
				if( models[i].Name == previous) {
					previousStrategy = models[i];
				}
			}
			if( previousStrategy == null) {
				previousStrategy = CreateStrategy( previous);
			}
			if( previousStrategy == null) {
				throw new ApplicationException( "Parent Strategy '" + previous + "' was not found for AddDependency()");
			}
			AddDependency( current, previousStrategy);
		}
		
		public void AddDependency( string current, StrategyCommon previousStrategy) {
			StrategyCommon currentStrategy = null;
			for( int i = 0; i< models.Count; i++) {
				if( models[i].Name == current) {
					currentStrategy = models[i];
				}
			}
			if( currentStrategy == null) {
				currentStrategy = CreateStrategy( current);
			}
			if( currentStrategy == null) {
				throw new ApplicationException( "Child Strategy '" + current + "' was not found for AddDependency()");
			}
			AddDependency(currentStrategy,previousStrategy);
		}
		
		public void AddDependency( StrategyCommon currentStrategy, StrategyCommon previousStrategy) {
			currentStrategy.Chain.Dependencies.Add(previousStrategy.Chain.Root);
		}
		                          
		public void Chain( string current, string previous) {
			ModelInterface currentStrategy = null;
			ModelInterface previousStrategy = null;
			for( int i = 0; i< models.Count; i++) {
				if( models[i].Name == current) {
					currentStrategy = models[i];
				}
				if( models[i].Name == previous) {
					previousStrategy = models[i];
				}
			}
			if( currentStrategy == null) {
				throw new ApplicationException( "Child Strategy '" + current + "' was not found for AddDependency()");
			}
			if( previousStrategy == null) {
				throw new ApplicationException( "Parent Strategy '" + previous + "' was not found for AddDependency()");
			}
			currentStrategy.Chain.Dependencies.Add(previousStrategy.Chain);
		}
		
		public void AddVariable( ModelProperty property) {
			variables.Add(property);
		}

		public List<ModelProperty> Variables {
			get { return variables; }
		}
		
		protected string name;
		public string Name {
			get { return category + ": " + name; } 
		}
		
		protected string category;
		public string Category {
			get { return category; }
		}
		
		public bool IsVisibleInGUI {
			get { return isVisibleInGUI; }
			set { isVisibleInGUI = value; }
		}
		
		public bool QuietMode {
			get { return quietMode; }
			set { quietMode = value; }
		}
		
		public ModelInterface TopModel {
			get { return topModel; }
			set { topModel = value; }
		}
		
		public Stream OptimizeOutput {
			get { return optimizeOutput; }
			set { optimizeOutput = value; }
		}
	}
}
