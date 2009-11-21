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
using System.Diagnostics;
using System.Drawing;
using System.Reflection;

using TickZoom.Api;

namespace TickZoom.Common
{
	/// <summary>
	/// Description of Formula.
	/// </summary>
	public partial class ModelCommon : ModelInterface
	{
		string name;
		Chain chain;
		protected bool isStrategy = false;
		protected bool isIndicator = false;
		Interval intervalDefault = Intervals.Default;
		string symbolDefault = "Default";
		Drawing drawing;

		List<Interval> updateIntervals = new List<Interval>();
		Data data;
		Chart chart;
		Context context;
		Formula formula;
		private static readonly Log log = Factory.Log.GetLogger(typeof(ModelCommon));
		private static readonly bool debug = log.IsDebugEnabled;
		private static readonly bool trace = log.IsTraceEnabled;		ModelProperties properties;
		bool isOptimizeMode = false;
		
		public ModelCommon()
		{
			name = GetType().Name;
			fullName = name;
			
			drawing = new DrawingCommon(this);
			formula = new Formula(this);
			
			if( trace) log.Trace(GetType().Name+".new");
			chain = Factory.Engine.Chain(this);
		}
		
		public void RequestUpdate( Interval interval) {
			updateIntervals.Add(interval);
		}
		
		[Browsable(false)]
		public IList<Interval> UpdateIntervals {
			get { return updateIntervals; }
		}
		
		public virtual bool OnWriteReport(string folder) {
			return false;
		}

		[Browsable(false)]
		public virtual Interval IntervalDefault {
			get { return intervalDefault; }
			set { intervalDefault = value; }
		}
		
		public void AddIndicator( IndicatorCommon indicator) {
			if( chain.Dependencies.Contains(indicator.Chain)) {
				throw new ApplicationException( "Indicator " + indicator.Name + " already added.");
			}
			if( this is StrategySupport) {
				StrategySupport support = this as StrategySupport;
				indicator.Performance = support.Strategy.Performance;
			} else if( this is StrategyCommon) {
				StrategyCommon strategy = this as StrategyCommon;
				indicator.Performance = strategy.Performance;
			} else if( this is IndicatorCommon) {
				IndicatorCommon thisIndicator = this as IndicatorCommon;
				indicator.Performance = thisIndicator.Performance;
			} else {
				throw new ApplicationException("Sorry, indicators can only be added to objects derived from " +
				                               typeof(StrategyCommon).Name + ", " +
				                               typeof(StrategySupport).Name + ", or " +
				                               typeof(IndicatorCommon).Name + ".");
			}
			// Apply Properties from project.xml, if any.
			if( properties != null) {
				string[] keys = properties.GetModelKeys();
				for( int i=0; i<keys.Length; i++) {
					ModelProperties indicatorProperties = properties.GetModel(keys[i]);
					if( indicator.name.Equals(indicatorProperties.Name) &&
					    indicatorProperties.ModelType == ModelType.Indicator) {
						indicator.OnProperties(indicatorProperties);
						break;
					}
				}
			}
			chain.Dependencies.Add(indicator.Chain);
		}

		[Browsable(false)]
		public Chart Chart {
			get { return chart; }
			set { chart = value; }
		}
		
		public override string ToString()
		{
			return name;
		}
		
		#region Convenience methods to access bar data
		[Browsable(false)]
		public Data Data {
			get { return data; }
			set { data = value; }
		}
		
		Bars years = null;
		[Browsable(false)]
		public Bars Years {
			get {
				if( years == null) years = data.Get(Intervals.Year1);
				return years;
			}
		}
		
		Bars months = null;
		[Browsable(false)]
		public Bars Months {
			get {
				if( months == null) months = data.Get(Intervals.Month1);
				return months;
			}
		}
		
		Bars weeks = null;
		[Browsable(false)]
		public Bars Weeks {
			get {
				if( weeks == null) weeks = data.Get(Intervals.Week1);
				return weeks;
			}
		}
		
		Bars days = null;
		[Browsable(false)]
		public Bars Days {
			get {
				if( days == null) days = data.Get(Intervals.Day1);
				return days;
			}
		}
		
		Bars hours = null;
		[Browsable(false)]
		public Bars Hours {
			get {
				if( hours == null) hours = data.Get(Intervals.Hour1);
				return hours;
			}
		}
		
		Bars minutes = null;
		[Browsable(false)]
		public Bars Minutes {
			get {
				if( minutes == null) minutes = data.Get(Intervals.Minute1);
				return minutes;
			}
		}
		
		Bars sessions = null;
		[Browsable(false)]
		public Bars Sessions {
			get {
				if( sessions == null) sessions = data.Get(Intervals.Session1);
				return sessions;
			}
		}
		
		Bars range5 = null;
		[Browsable(false)]
		public Bars Range5 {
			get {
				if( range5 == null) range5 = data.Get(Intervals.Session1);
				return range5;
			}
		}
		
		Ticks ticks = null;
		[Browsable(false)]
		public Ticks Ticks {
			get { 
				if( ticks == null) ticks = data.Ticks;
				return ticks; }
		}
	
		Bars bars = null; 
		[Browsable(false)]
		public Bars Bars {
			get { return bars; }
			set { bars = value; }
		}
		#endregion

		[Browsable(false)]
		public Context Context {
			get { return context; }
			set { context = value; }
		}
		
		public void OnProperties(ModelProperties properties)
		{
			this.properties = properties;
   			if( trace) log.Trace(GetType().Name+".OnProperties() - NotImplemented");
			string[] propertyKeys = properties.GetPropertyKeys();
			for( int i=0; i<propertyKeys.Length; i++) {
				HandleProperty(propertyKeys[i],properties.GetProperty(propertyKeys[i]).Value);
			}
		}
		
		private void HandleProperty( string name, string str) {
			PropertyInfo property = this.GetType().GetProperty(name);
			Type propertyType = property.PropertyType;
			object value = Converters.Convert(propertyType,str);
			property.SetValue(this,value,null);
//			log.WriteFile("Property " + property.Name + " = " + value);
		}		
		
		public virtual void OnBeforeInitialize() {
   			if( trace) log.Trace("OnBeforeInitialize() - NotImplemented");
		}
		
		public virtual void OnInitialize() {
   			if( trace) log.Trace("OnInitialize() - NotImplemented");
		}
		
		public virtual void OnStartHistorical() {
   			if( trace) log.Trace("OnStartHistorical() - NotImplemented");
		}
			
		public virtual bool OnBeforeIntervalOpen() {
   			if( trace) log.Trace("OnBeforeIntervalOpen() - NotImplemented");
   			// Return false means never call this method again for performance.
   			return false;
		}

		public virtual bool OnBeforeIntervalOpen(Interval interval) {
   			if( trace) log.Trace("OnBeforeIntervalOpen("+interval+") - NotImplemented");
   			// Return false means never call this method again for performance.
   			return false;
		}

		public virtual bool OnIntervalOpen() {
   			if( trace) log.Trace("OnIntervalOpen() - NotImplemented");
   			// return false means the engine will never call this method again.
			return false;
		}

		public virtual bool OnIntervalOpen(Interval interval) {
   			if( trace) log.Trace("OnIntervalOpen("+interval+") - NotImplemented");
   			// return false means the engine will never call this method again.
			return false;
		}
		
		public virtual bool OnProcessTick(Tick tick) {
   			if( trace) log.Trace("OnProcessTick() - NotImplemented");
   			// return false means the engine will never call this method again.
			return false;
		}
		
		public virtual bool OnBeforeIntervalClose() {
   			if( trace) log.Trace("OnBeforeIntervalClose() - NotImplemented");
   			// return false means the engine will never call this method again.
			return false;
		}
		
		public virtual bool OnBeforeIntervalClose(Interval interval) {
   			if( trace) log.Trace("OnBeforeIntervalClose("+interval+") - NotImplemented");
   			// return false means the engine will never call this method again.
			return false;
		}
		
		public virtual bool OnIntervalClose() {
   			if( trace) log.Trace("OnIntervalClose() - NotImplemented");
   			// return false means the engine will never call this method again.
			return false;
		}
		
		public virtual bool OnIntervalClose(Interval interval) {
   			if( trace) log.Trace("OnIntervalClose("+interval+") - NotImplemented");
   			// return false means the engine will never call this method again.
			return false;
		}

		public virtual void OnEndHistorical() {
   			if( trace) log.Trace("OnEndHistorical() - NotImplemented");
   			// return false means the engine will never call this method again.
		}
		
		public virtual void Save( string fileName) {
			
		}
		
		public void AddDependency( ModelInterface formula) {
			chain.Dependencies.Add(formula.Chain);
		}

		[Browsable(false)]
		public bool IsStrategy {
			get { return isStrategy; }
		}
		
		[Browsable(false)]
		public bool IsIndicator{
			get { return isIndicator; }
		}
		
		[Browsable(false)]
		public virtual string Name {
			get { return name; }
			set { name = value; }
		}

		[Browsable(false)]
		public Chain Chain {
			get { return chain; }
			set { chain = value; }
		}
		
		[Obsolete("Please, use FullName property instead.",true)]
		public string LogName {
			get { return name.Equals(GetType().Name) ? name : name+"."+GetType().Name; }
		}

		string fullName;
		public virtual string FullName {
			get { return fullName; }
			set { fullName = value; }
		}
		
		public Integers Integers() {
			return Factory.Engine.Integers();
		}
		
		public Integers Integers(int capacity) {
			return Factory.Engine.Integers(capacity);
		}
		
		public Doubles Doubles() {
			return Factory.Engine.Doubles();
		}
		
		public Doubles Doubles(int capacity) {
			return Factory.Engine.Doubles(capacity);
		}
		
		public Doubles Doubles(object obj) {
			return Factory.Engine.Doubles(obj);
		}
		
		public Series<T> Series<T>() {
			return Factory.Engine.Series<T>();
		}
		
		[Browsable(false)]
		public virtual Drawing Drawing {
			get { return drawing; }
			set { drawing = value; }
		}
		
		[Obsolete("This method is never used. Override OnGetFitness() in a strategy instead.",true)]
		public virtual double Fitness {
			get { return 0; }
		}
		
		[Obsolete("Override OnGetFitness() or OnStatistics() in a strategy instead.",true)]
		public virtual string OnOptimizeResults() {
			throw new NotImplementedException();
		}
		
		[Browsable(false)]
		public Formula Formula {
			get { return formula; }
		}
		
		public void GotOrder(Order order) {
			throw new NotImplementedException();
		}
		public void GotFill(Trade fill) {
			throw new NotImplementedException();
		}
		public void GotOrderCancel(uint orderid) {
			throw new NotImplementedException();
		}
		public void GotPosition(Position pos) {
			throw new NotImplementedException();
		}
		 
		[Browsable(false)]
		public Provider Provider {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public bool IsOptimizeMode {
			get { return isOptimizeMode; }
			set { isOptimizeMode = value; }
		}

		public virtual string SymbolDefault {
			get { return symbolDefault; }
			set { symbolDefault = value; }
		}
	}
}
