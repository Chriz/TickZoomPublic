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
 * <http://www.tickzoom.org/wiki/Licenses>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Reflection;

using MBTCOMLib;
using MBTORDERSLib;
using MBTQUOTELib;
using TickZoom.Api;

namespace TickZoom.MBTrading
{
	/// <summary>
	/// Description of Instruments.
	/// </summary>
	public class InstrumentReaders
    {
		private static readonly Log log = Factory.Log.GetLogger(typeof(InstrumentReaders));
		private static readonly bool debug = log.IsDebugEnabled;
        private MbtQuotes m_Quotes;
        private MbtOrderClient m_OrderClient;
        private Dictionary<string, MbtPosition> positions = new Dictionary<string,MbtPosition>();
        private Collection<InstrumentReader> readers = new Collection<InstrumentReader>();
        private Receiver receiver;

        public InstrumentReaders(Receiver providerClient, MbtOrderClient orderClient, MbtQuotes quotes)
		{
        	this.receiver = providerClient;
        	m_OrderClient = orderClient;
            m_Quotes = quotes;
		}
        
        public int GetIndex(string Symbol)
        {
            for(int i=0;i<readers.Count;i++)
            {
                if(readers[i].Instrument.Symbol==Symbol) return i;
            }
            return -1;
        }
        
        public void Signal(string symbol, double signal) {
            int instrumentIndex = GetIndex(symbol);
            if (instrumentIndex != -1)
            {
            	readers[instrumentIndex].Signal(signal);
            }
        }
        
        public void AddQuotes(string symbol, bool saveFile)
        {
        	int i = GetIndex(symbol);
            if (i == -1)
            {
            	NewReader(symbol,QuoteType.Level1);
            } 
        }
        
        public void AddDepth(string symbol)
        {
        	int i = GetIndex(symbol);
            if (i == -1)
            {
            	NewReader(symbol,QuoteType.Level2);
            }
        }
        
        private void NewReader(string symbol, QuoteType quoteType) {
           	if( debug) log.Debug("NewReader for " + symbol + " with quote type " + quoteType);
 	      	SymbolInfo instrument = Factory.Symbol.LookupSymbol(symbol);
        	InstrumentReader reader = new InstrumentReader(m_OrderClient, m_Quotes, instrument);
        	reader.Receiver = receiver;
        	reader.QuoteType = quoteType;
        	reader.Initialize();
            readers.Add(reader);
            if (m_Quotes.ConnectionState == enumConnectionState.csLoggedIn) {
                reader.Connect();
            }
        }
        
        public void Remove(string Symbol)
        {
            int instrumentIndex = GetIndex(Symbol);
            if (instrumentIndex != -1)
            {
            	readers[instrumentIndex].Close();
                readers.RemoveAt(instrumentIndex);
            }
        }
        
        public void Clear()
        {
            for (int i = readers.Count-1; i >= 0; i--)
            {
            	readers[i].Close();
                readers.RemoveAt(i);
            }
        }
        public void Close()
        { 
            for (int i = readers.Count-1; i >= 0; i--)
            {
            	readers[i].Close();
                readers.RemoveAt(i);
            }
        }
        
        public void AdviseAll()
        {
            for (int i = 0; i < readers.Count; i++)
            {
            	readers[i].Connect();
            	if( debug) log.Debug("AdviseAll() advised symbol :" + readers[i].Instrument.Symbol);
            }
        }
        
        public int GetLastChangeTime()
        {
        	int lastChangeTime = 0;
            for (int i = 0; i < readers.Count; i++)
            {
            	if( readers[i].LastChangeTime > lastChangeTime) {
            		lastChangeTime = readers[i].LastChangeTime;
            	}
            }
            return lastChangeTime;
        }
        
        public void UnadviseAll()
        {
            for (int i = 0; i < readers.Count; i++)
            {
            	readers[i].Disconnect();
            }
        }
        
        void OnCancelPlaced(MbtOpenOrder order) {
        	for(int i=0; i<readers.Count; i++) {
        		if( readers[i].Instrument.Symbol == order.Symbol) {
        			readers[i].OnCancelPlaced(order);
        		}
        	}
	    }
        void OnReplacePlaced(MbtOpenOrder order) {
        	for(int i=0; i<readers.Count; i++) {
        		if( readers[i].Instrument.Symbol == order.Symbol) {
        			readers[i].OnReplacePlaced(order);
        		}
        	}
	    }
        void OnReplaceRejected(MbtOpenOrder order) {
        	for(int i=0; i<readers.Count; i++) {
        		if( readers[i].Instrument.Symbol == order.Symbol) {
        			readers[i].OnReplaceRejected(order);
        		}
        	}
	    }
        void OnCancelRejected(MbtOpenOrder order) {
        	for(int i=0; i<readers.Count; i++) {
        		if( readers[i].Instrument.Symbol == order.Symbol) {
        			readers[i].OnCancelRejected(order);
        		}
        	}
	    }
	    
        void OnClose(int x) {
        	for(int i=0; i<readers.Count; i++) {
       			readers[i].OnClose(x);
        	}
	    }

		void OnSubmit(MbtOpenOrder order) {
        	for(int i=0; i<readers.Count; i++) {
        		if( readers[i].Instrument.Symbol == order.Symbol) {
        			readers[i].OnSubmit(order);
        		}
        	}
	    }
        
        void OnAcknowledge(MbtOpenOrder order) {
        	for(int i=0; i<readers.Count; i++) {
        		if( readers[i].Instrument.Symbol == order.Symbol) {
        			readers[i].OnAcknowledge(order);
        		}
        	}
	    }
        void OnExecute(MbtOpenOrder order) { 
        	for(int i=0; i<readers.Count; i++) {
        		if( readers[i].Instrument.Symbol == order.Symbol) {
        			readers[i].OnExecute(order);
        		}
        	}
	    }
        void OnRemove(MbtOpenOrder order) { 
        	for(int i=0; i<readers.Count; i++) {
        		if( readers[i].Instrument.Symbol == order.Symbol) {
        			readers[i].OnRemove(order);
        		}
        	}
	    }
        void OnHistoryAdded(MBTORDERSLib.MbtOrderHistory orderhistory) {
        	for(int i=0; i<readers.Count; i++) {
        		if( readers[i].Instrument.Symbol == orderhistory.Symbol) {
        			readers[i].OnHistoryAdded(orderhistory);
        		}
        	}
	    }
        void OnPositionAdded(MBTORDERSLib.MbtPosition position) {
        	for(int i=0; i<readers.Count; i++) {
        		if( readers[i].Instrument.Symbol == position.Symbol) {
        			readers[i].OnPositionAdded(position);
        		}
        	}
	    }
        void OnPositionUpdated(MBTORDERSLib.MbtPosition position) {
        	for(int i=0; i<readers.Count; i++) {
        		if( readers[i].Instrument.Symbol == position.Symbol) {
        			readers[i].OnPositionUpdated(position);
        		}
        	}
	    }
        void OnBalanceUpdate(MBTORDERSLib.MbtAccount account) {
        	for(int i=0; i<readers.Count; i++) {
        		readers[i].OnBalanceUpdate(account);
        	}
	    }
        void OnDefaultAccountChanged(MBTORDERSLib.MbtAccount account) {
        	for(int i=0; i<readers.Count; i++) {
        		readers[i].OnDefaultAccountChanged(account);
        	}
	    }
        
        void OnAccountLoaded(MBTORDERSLib.MbtAccount account) {
        	for(int i=0; i<readers.Count; i++) {
        		readers[i].OnAccountLoaded(account);
        	}
	    }

		
		#region EventHandlers
	    public void AssignEventHandlers()
	    {
	        m_OrderClient.OnAccountLoaded += OnAccountLoaded;
	        m_OrderClient.OnAcknowledge += OnAcknowledge;
	        m_OrderClient.OnBalanceUpdate += OnAccountLoaded;
	        m_OrderClient.OnCancelPlaced += OnCancelPlaced;
	        m_OrderClient.OnCancelRejected += OnCancelRejected;
	        m_OrderClient.OnClose += OnClose;
	        m_OrderClient.OnSubmit += OnSubmit;
	        m_OrderClient.OnDefaultAccountChanged += OnDefaultAccountChanged;
	        m_OrderClient.OnExecute += OnExecute;
	        m_OrderClient.OnRemove += OnRemove;
	        m_OrderClient.OnHistoryAdded += OnHistoryAdded;
	        m_OrderClient.OnPositionAdded += OnPositionAdded;
	        m_OrderClient.OnPositionUpdated += OnPositionUpdated;
	        m_OrderClient.OnReplacePlaced += OnReplacePlaced;
	        m_OrderClient.OnReplaceRejected += OnReplaceRejected;
	    }
	    
	    public void RemoveEventHandlers()
	    {
	        m_OrderClient.OnAccountLoaded -= OnAccountLoaded;
	        m_OrderClient.OnAcknowledge -= OnAcknowledge;
	        m_OrderClient.OnBalanceUpdate -= OnAccountLoaded;
	        m_OrderClient.OnCancelPlaced -= OnCancelPlaced;
	        m_OrderClient.OnCancelRejected -= OnCancelRejected;
	        m_OrderClient.OnClose -= OnClose;
	        m_OrderClient.OnSubmit -= OnSubmit;
	        m_OrderClient.OnDefaultAccountChanged -= OnDefaultAccountChanged;
	        m_OrderClient.OnExecute -= OnExecute;
	        m_OrderClient.OnRemove -= OnRemove;
	        m_OrderClient.OnHistoryAdded -= OnHistoryAdded;
	        m_OrderClient.OnPositionAdded -= OnPositionAdded;
	        m_OrderClient.OnPositionUpdated -= OnPositionUpdated;
	        m_OrderClient.OnReplacePlaced -= OnReplacePlaced;
	        m_OrderClient.OnReplaceRejected -= OnReplaceRejected;
	    }
	    #endregion
	    
	    public void Add(string Symbol)
        {
            if (GetIndex(Symbol) == -1)
            {
	        	SymbolInfo instrument = Factory.Symbol.LookupSymbol(Symbol);
	        	InstrumentReader reader = new InstrumentReader(m_OrderClient, m_Quotes, instrument);
                readers.Add(reader);
                if (m_Quotes.ConnectionState == enumConnectionState.csLoggedIn) {
                	reader.Connect();
                }
            }
        }
        
		public Collection<InstrumentReader> Readers {
			get { return readers; }
		}
        
    }
}
