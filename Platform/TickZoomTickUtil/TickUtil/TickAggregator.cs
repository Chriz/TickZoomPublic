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
 * Date: 5/25/2009
 * Time: 3:36 PM
 * <http://www.tickzoom.org/wiki/Licenses>.
 */
#endregion


using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using TickZoom;
using TickZoom.Api;
using TickZoom.TickUtil;

namespace TickZoom.TickUtil
{
	public class TickAggregator : Provider {
		List<SymbolQueue> symbolQueues = new List<SymbolQueue>();
		Receiver receiver;
		static readonly Log log = Factory.Log.GetLogger(typeof(TickAggregator));
		static readonly bool debug = log.IsDebugEnabled;
		static readonly bool trace = log.IsTraceEnabled;
		private int id;
		private static int staticId;
		private static object locker = new object();
		private Task runTask;
		
		public TickAggregator() {
			if(debug) log.Debug("Constructor");
			lock(locker) {
				staticId++;
				this.id = staticId;
			}
			symbolQueues = new List<SymbolQueue>();
		}
		
		public void Add(SymbolInfo symbol, Provider provider, TimeStamp lastTime) {
			if(debug) log.Debug("Add("+symbol+","+provider+")");
	    	SymbolQueue sq = new SymbolQueue(symbol, provider, lastTime);
			this.symbolQueues.Add( sq);
		}

		bool isStarted = false;
		public void Start(Receiver receiver) {
			if( isStarted) {
				if( debug) log.Debug("Start - already started");
			} else {
				if( debug) log.Debug("Start");
				this.receiver = receiver;
		        isStarted = true;
		        runTask = Factory.Parallel.Loop(tick,Process);
			}
		}
		
		bool isProcessStarted = false;

		TickBinary tick = new TickBinary();
		private void ProcessStartup() {
			for(int i=0; i<symbolQueues.Count; i++) {
				log.Notice("Initializing Symbol Input Queue for " + symbolQueues[i].Symbol);
				SymbolQueue symbolQueue = symbolQueues[i];
				symbolQueue.Receive(ref tick);
				symbolQueue.NextTick = tick;
				symbolQueue.NextTick.Symbol = symbolQueue.Symbol.BinaryIdentifier;
				if( debug) log.Debug("Initial tick has symbol '" + symbolQueue.NextTick.Symbol +"'");
	   		}
			receiver.OnRealTime(null);
		}
		
		int countLog = 0;
		private bool Process() {
			if( !isProcessStarted) {
				ProcessStartup();
				isProcessStarted = false;
			}
			if( symbolQueues.Count == 0 ||
			   !receiver.CanReceive) {
				return false;
			}
			int nextQueue = 0;
   			for( int i=1; i<symbolQueues.Count; i++) {
				if( symbolQueues[i].NextTick.UtcTime < symbolQueues[nextQueue].NextTick.UtcTime ) {
	   				nextQueue = i;
		   		}
	   		}
			tick = symbolQueues[nextQueue].NextTick;
			if( debug && countLog < 5) {
				log.Symbol = tick.Symbol.ToString();
				log.TimeStamp = tick.UtcTime;
				log.Debug("Queuing tick with symbol=" + tick.Symbol.ToString() + " " + tick);
				countLog++;
			} else if( trace) {
				log.Symbol = tick.Symbol.ToString();
				log.TimeStamp = tick.UtcTime;
				log.Trace("Queuing tick with symbol=" + tick.Symbol + ", " + tick);
			}
			
			receiver.OnSend(ref tick);
			try {
				SymbolQueue inputQueue = symbolQueues[nextQueue];
				inputQueue.Receive(ref tick);
				inputQueue.NextTick = tick;
				inputQueue.NextTick.Symbol = inputQueue.Symbol.BinaryIdentifier;
				return true;
			} catch( QueueException ex) {
				if( ex.EntryType == EntryType.EndHistorical) {
					if( symbolQueues.Count <= 1) {
						receiver.OnStop();
						Factory.Parallel.CurrentTask.Stop();
					} else {
						symbolQueues.RemoveAt(nextQueue);
					}
				} else {
					throw new ApplicationException("Queue returned invalid entry type: " + ex.EntryType, ex);
				}
			}
			return true;
		}
		
		public void Stop(Receiver receiver)
		{
			runTask.Stop();
			for( int i=0; i<symbolQueues.Count; i++) {
				symbolQueues[i].Provider.Stop(symbolQueues[i]);
			}
			runTask.Join();
		}
		
		public void StartSymbol(Receiver receiver, SymbolInfo symbol, TimeStamp lastTimeStamp)
		{
			log.Warn("StartSymbol("+symbol+","+lastTimeStamp+") NOT IMPLEMENTED");
		}
		
		public void StopSymbol(Receiver receiver, SymbolInfo symbol)
		{
			throw new NotImplementedException();
		}
		
		public void LoadSymbol(string symbol, TimeStamp startTime, TimeStamp endTime)
		{
			throw new NotImplementedException();
		}
		
		public void PositionChange(Receiver receiver, SymbolInfo symbol, double position)
		{
			throw new NotImplementedException();
		}
		
		public void Stop()
		{
			throw new NotImplementedException();
		}
	}
}
