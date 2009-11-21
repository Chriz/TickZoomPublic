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
using System.Diagnostics;
using System.Threading;

using TickZoom.Api;

namespace TickZoom.TickUtil
{
	public class FastQueueImpl<T> where T : struct
	{
		readonly Log log;
		readonly bool debug;
		readonly bool trace;
		string name;
		long lockSpins = 0;
		long lockCount = 0;
		long enqueueSpins = 0;
		long dequeueSpins = 0;
		int enqueueSleepCounter = 0;
		int dequeueSleepCounter = 0;
		long enqueueConflicts = 0;
		long dequeueConflicts = 0;
	    int isLocked = 0;
	    readonly int spinCycles = 1000;
	    int timeout = 30000; // milliseconds
	    private static Pool<Queue<T>> queuePool;
	    Queue<T> queue;
	    volatile bool terminate = false;
	    int processorCount = Environment.ProcessorCount;
		bool isStarted = false;
		bool isPaused = false;
		StartEnqueue startEnqueue;
		PauseEnqueue pauseEnqueue;
		ResumeEnqueue resumeEnqueue;
	    int maxSize;
	    int lowWaterMark;
	    int highWaterMark;
	    private static readonly List<FastQueueImpl<T>> queueList = new List<FastQueueImpl<T>>();
	    private static readonly object locker = new object();
		Exception exception;

        public FastQueueImpl(object name)
            : this(name, 20000)
        {

        }

	    public FastQueueImpl(object name, int maxSize) {
			log = Factory.Log.GetLogger("TickZoom.TickUtil.FastQueue."+name);
			debug = log.IsDebugEnabled;
			trace = log.IsTraceEnabled;
	    	log.Debug("Created with capacity " + maxSize);
            if( name is string)
            {
                this.name = (string) name;
            } else if( name is Type)
            {
                this.name = ((Type) name).Name;
            }
			this.maxSize = maxSize;
			this.lowWaterMark = maxSize / 2;
			this.highWaterMark = maxSize / 2;
			queue = QueuePool.Create();
			queue.Clear();
			lock( locker) {
				 queueList.Add(this);
			}
	    }
	    
		private bool SpinLockNB() {
	    	while( isLocked == 1 || Interlocked.CompareExchange(ref isLocked,1,0) == 1) Interlocked.Increment(ref lockSpins);
	    	return true;
	    }
	    
	    private void SpinUnLock() {
	    	isLocked = 0;
	    }
	    
	    public bool CanEnqueue {
	    	get { return queue != null && queue.Count<maxSize && isLocked == 0; }
	    }
	    
	    public bool CanDequeue {
	    	get { 
		    	if( !isStarted) { 
		    		if( !StartDequeue()) return false;
		    	}
	    		return queue!=null && queue.Count>0 && isLocked == 0;
	    	}
	    }
	    
	    public bool EnQueueStruct(ref T tick)
	    {
            if( terminate) {
	    		if( exception != null) {
	    			throw new ApplicationException("Enqueue failed.",exception);
	    		} else {
            		throw new QueueException(EntryType.Terminate);
	    		}
            }
            // If the queue is full, wait for an item to be removed
            if( queue.Count>=maxSize) return false;
            if( !SpinLockNB()) return false;
           	queue.Enqueue(tick);
            SpinUnLock();
            return true;
	    }
	    T empty = new T();
	    public bool DequeueStruct(ref T tick)
	    {
            if( terminate) {
	    		if( exception != null) {
	    			throw new ApplicationException("Dequeue failed.",exception);
	    		} else {
	            	throw new QueueException(EntryType.Terminate);
	    		}
            }
	    	tick = empty;
	    	if( !isStarted) { 
	    		if( !StartDequeue()) return false;
	    	}
	        if( queue.Count==0) return false;
	    	if( !SpinLockNB()) return false;
            tick = queue.Dequeue();
            SpinUnLock();
            return true;
	    }
	    
	    public void Clear() {
	    	log.Debug("Clear called");
	    	if( !terminate) {
	    		while( !SpinLockNB()) ;
		        queue.Clear();
		        SpinUnLock();
	    	}
	    }
	    
	    public void Flush() {
	    	log.Debug("Flush called");
	    	while(!terminate && queue.Count>0) {
	    		Factory.Parallel.Yield();
	    	}
	    }
	    
	    public void Terminate(Exception ex) {
	    	exception = ex;
	    	Terminate();
	    }
	    
	    public void Terminate() {
	    	if( debug) log.Debug("Terminate was called.");
	   		SpinUnLock();
	    	terminate = true;
	        if( queue!=null) {
		        QueuePool.Free(queue);
		        queue=null;
	        }
	    }
	
	    public int Count {
	    	get { if(!terminate) {
	    			return queue.Count;
	    		} else {
	    			return 0;
	    		}
	    	}
	    }
	    
		public long EnqueueConflicts {
			get { return enqueueConflicts; }
		}
	    
		public long DequeueConflicts {
			get { return dequeueConflicts; }
		}
		
		public StartEnqueue StartEnqueue {
			get { return startEnqueue; }
			set { startEnqueue = value;	}
		}
	
		private bool StartDequeue()
		{
			if( debug) log.Debug("StartDequeue called");
			if( !SpinLockNB()) return false;
			isStarted = true;
			if( StartEnqueue != null) {
		    	if( debug) log.Debug("Calling StartEnqueue called");
				StartEnqueue();
			}
	        SpinUnLock();			
	        return true;
		}
		
		public int Timeout {
			get { return timeout; }
			set { timeout = value; }
		}
		
		public bool IsStarted {
			get { return isStarted; }
		}
		
		public void Pause() {
	    	log.Debug("Pause called");
			if( !isPaused) {
				isPaused = true;
				if( PauseEnqueue != null) {
					PauseEnqueue();
				}
			}
		}
		
		public void Resume() {
	    	log.Debug("Resume called");
			if( isPaused) {
				isPaused = false;
				if( ResumeEnqueue != null) {
					ResumeEnqueue();
				}
			}
		}
		
		public ResumeEnqueue ResumeEnqueue {
			get { return resumeEnqueue; }
			set { resumeEnqueue = value; }
		}
		
		public PauseEnqueue PauseEnqueue {
			get { return pauseEnqueue; }
			set { pauseEnqueue = value; }
		}
		
		public bool IsPaused {
			get { return isPaused; }
		}
		
		public void LogStatistics() {
			if( debug) {
				double average = lockCount == 0 ? 0 : ((lockSpins*spinCycles)/lockCount);
				log.Debug("Queue Name="+name+
			    " items="+Count+
			    " locks( count="+lockCount + 
			    " spins="+lockSpins*spinCycles+
			    " average="+average+
				") enqueue( conflicts="+enqueueConflicts+
				" spins="+enqueueSpins+
				" sleeps="+enqueueSleepCounter+
				") dequeue( conflicts="+dequeueConflicts+
				" spins="+dequeueSpins+
				" sleeps="+dequeueSleepCounter+
				")");
			}
		}
	    
		public static Pool<Queue<T>> QueuePool {
	    	get { if( queuePool == null) {
	    			lock(locker) {
	    				if( queuePool == null) {
	    					queuePool = new Pool<Queue<T>>();
	    				}
	    			}
				}
	    		return queuePool;
	    	}
		}
			
		public static void LogAllStatistics() {
			lock(locker) {
				for( int i=0;i<queueList.Count; i++) {
					FastQueueImpl<T> queue = queueList[i];
					queue.LogStatistics();
				}
			}
		}
	}
}


