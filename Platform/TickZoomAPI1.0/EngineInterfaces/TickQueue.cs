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

namespace TickZoom.Api
{
    public delegate void StartEnqueue();
    public delegate void PauseEnqueue();
    public delegate void ResumeEnqueue();

    public interface TickQueue : FastQueue<TickBinary> {
    	
    }
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public interface FastQueue<T>
	{
		StartEnqueue StartEnqueue {
			get; set;
		}
		
		PauseEnqueue PauseEnqueue {
			get; set;
		}
		
		ResumeEnqueue ResumeEnqueue {
			get; set;
		}
		
	    void EnQueue(ref T o);
	    
	    void EnQueue(EntryType queueItemType, SymbolInfo symbol);
	    
	    void EnQueue(EntryType queueItemType, string error);
	    
	    void Dequeue(ref T tick);
	    
	    void Clear();

	    void Terminate();

	    void Terminate(Exception ex);
	    
	    int Count {
	    	get;
	    }
	    
	    int Timeout {
	    	get;
	    	set;
	    }
	    
	    bool IsStarted {
	    	get;
	    }
	    
	    bool IsPaused {
	    	get;
	    }
	    
	    void LogStats();
	    
	    bool CanEnqueue { 
	    	get;
	    }
	    
	    bool CanDequeue { 
	    	get;
	    }
	}
}
