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
using System.Reflection;

namespace TickZoom.Api
{
	/// <summary>
	/// Description of ModelLoaderManager.
	/// </summary>
	public class Plugins
	{
		List<Type> modelLoaders;
		List<Type> models;
		int errorCount = 0;
		Log log = Factory.Log.GetLogger(typeof(Plugins));
		[ThreadStatic]
		static Plugins plugins;
		
		public string PluginFolder;
		
		/// <summary>
		/// Loads all plugins each time you get an instance
		/// so that plugins can be installed without restarting
		/// TickZoom.
		/// </summary>
		public static Plugins Instance {
			get { if( plugins==null) {
					plugins = new Plugins();
				}
				return plugins;
			}
		}
		
		private Plugins()
		{
			string appData = ConfigurationManager.AppSettings.Get("AppDataFolder");
			if( appData == null) {
				throw new ApplicationException("AppDataFolder was not set in app.config.");
			}
			PluginFolder = appData + @"\Plugins";
       		Directory.CreateDirectory( PluginFolder);
       		LoadAssemblies( PluginFolder);
		}
		
		public ModelLoader GetLoader( string name) {
			for( int i=0; i<modelLoaders.Count; i++) {
				Type type = modelLoaders[i];
				ModelLoader loader = (ModelLoader)Activator.CreateInstance(type);
				if( loader.Name.Equals(name)) {
					return loader;
				}
			}
			throw new Exception("ModelLoader '"+name+"' not found.");
		}
		
		public ModelInterface GetModel( string name) {
			for( int i=0; i<models.Count; i++) {
				if( models[i].Name.Equals(name)) {
					return (ModelInterface)Activator.CreateInstance(models[i]);
				}
			}
			throw new Exception("Model '"+name+"' not found.");
		}
		
		private void LoadAssemblies(String path)
		{
			errorCount = 0;
		    modelLoaders = new List<Type>();
		    models = new List<Type>();
		
		    // This loads plugins from the plugin folder
		    List<string> files = new List<string>();
		    files.AddRange( Directory.GetFiles(path, "*plugin*.dll", SearchOption.AllDirectories));
	        // This loads plugins from the installation folder
	        // so all the common models and modelloaders get loaded.
	        files.AddRange( Directory.GetFiles(System.Environment.CurrentDirectory, "*plugin*.dll", SearchOption.AllDirectories));
	        files.AddRange( Directory.GetFiles(System.Environment.CurrentDirectory, "*test*.dll", SearchOption.AllDirectories));
	        
	        
		    foreach (String filename in files)
		    {
		        log.Info("Loading " + filename);
		        Type t2 = typeof(object);
		        try
		        {
		            Assembly assembly = Assembly.LoadFrom(filename);
		
		            foreach (Type t in assembly.GetTypes())
		            {
		            	t2 = t;
		            	if (t.IsClass && !t.IsAbstract) {
		            		if( t.GetInterface("ModelLoader") != null)
			                {
		            			try {
		            				ModelLoader loader = (ModelLoader)Activator.CreateInstance(t);
			                		modelLoaders.Add(t);
		            			} catch( MissingMethodException) {
		            				errorCount++;
						            log.Notice("ModelLoader '" + t.Name + "' in '" + filename + "' failed to load due to missing default constructor" );
		            			}
			                }
		            		if( t.GetInterface("ModelInterface") != null)
			                {
		            			// Exit and Enter Common aren't directly accessible.
		            			// They provide support for custom strategies.
		            			try {
			                		models.Add(t);
		            			} catch( MissingMethodException) {
		            				errorCount++;
						            log.Notice("Model '" + t.Name + "' in '" + filename + "' failed to load due to missing default constructor" );
		            			}
			                }
		            	}
		            }
		        } catch (ReflectionTypeLoadException ex) {
		        	log.Warn("Plugin load failed for '" + t2.Name + "' in '"+ filename +"' with loader exceptions:");
		        	for( int i=0; i<ex.LoaderExceptions.Length; i++) {
		        		Exception lex = ex.LoaderExceptions[i];
		        		log.Warn( lex.ToString());
		        	}
		        } catch (Exception err) {
		            log.Warn("Plugin load failed for '" + t2.Name + "' in '"+ filename +"': " + err.ToString());
		        }
		    }
		    if( modelLoaders.Count == 0) {
		    	log.Warn("Zero plugins found in " + PluginFolder );
		    }
		}
		
		public int ErrorCount {
			get { return errorCount; }
		}
		
		public List<ModelLoader> GetLoaders() {
			List<ModelLoader> loaders = new List<ModelLoader>();
			for( int i=0; i<modelLoaders.Count; i++) {
				loaders.Add((ModelLoader)Activator.CreateInstance(modelLoaders[i]));
			}
			return loaders;
		}
		
		public List<Type> Models {
			get { return models; }
		}
		
	}
}
