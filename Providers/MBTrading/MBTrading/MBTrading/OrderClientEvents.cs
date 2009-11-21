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

namespace TickZoom.MBTrading
{
	/// <summary>
	/// This class serves only to make sure we have a complete list
	/// of the events since. If were missing any the compiler will say so.
	/// </summary>
    public class OrderClientEvents : MBTORDERSLib._IMbtOrderClientEvents
    {
        void MBTORDERSLib._IMbtOrderClientEvents.OnConnect(int x){}
        void MBTORDERSLib._IMbtOrderClientEvents.OnClose(int x) { }
        void MBTORDERSLib._IMbtOrderClientEvents.OnLogonSucceed() { }
        void MBTORDERSLib._IMbtOrderClientEvents.OnSubmit(MBTORDERSLib.MbtOpenOrder order){}
        void MBTORDERSLib._IMbtOrderClientEvents.OnAcknowledge(MBTORDERSLib.MbtOpenOrder order){}
        void MBTORDERSLib._IMbtOrderClientEvents.OnExecute(MBTORDERSLib.MbtOpenOrder order){}
        void MBTORDERSLib._IMbtOrderClientEvents.OnRemove(MBTORDERSLib.MbtOpenOrder order){}
        void MBTORDERSLib._IMbtOrderClientEvents.OnHistoryAdded(MBTORDERSLib.MbtOrderHistory orderhistory){}
        void MBTORDERSLib._IMbtOrderClientEvents.OnPositionAdded(MBTORDERSLib.MbtPosition position){}
        void MBTORDERSLib._IMbtOrderClientEvents.OnPositionUpdated(MBTORDERSLib.MbtPosition position){}
        void MBTORDERSLib._IMbtOrderClientEvents.OnBalanceUpdate(MBTORDERSLib.MbtAccount account){}
        void MBTORDERSLib._IMbtOrderClientEvents.OnDefaultAccountChanged(MBTORDERSLib.MbtAccount account){}
        void MBTORDERSLib._IMbtOrderClientEvents.OnAccountUnavailable(MBTORDERSLib.MbtAccount account){}
        void MBTORDERSLib._IMbtOrderClientEvents.OnAccountLoading(MBTORDERSLib.MbtAccount account){}
        void MBTORDERSLib._IMbtOrderClientEvents.OnAccountLoaded(MBTORDERSLib.MbtAccount account){}
        void MBTORDERSLib._IMbtOrderClientEvents.OnCancelPlaced(MBTORDERSLib.MbtOpenOrder order){}
        void MBTORDERSLib._IMbtOrderClientEvents.OnReplacePlaced(MBTORDERSLib.MbtOpenOrder order){}
        void MBTORDERSLib._IMbtOrderClientEvents.OnReplaceRejected(MBTORDERSLib.MbtOpenOrder order){}
        void MBTORDERSLib._IMbtOrderClientEvents.OnCancelRejected(MBTORDERSLib.MbtOpenOrder order) { }
        void MBTORDERSLib._IMbtOrderClientEvents.OnPositionStrategyGroupRemoved(MBTORDERSLib.MbtPositionStrategyGroup group) { }
        void MBTORDERSLib._IMbtOrderClientEvents.OnPositionStrategyGroupUpdated(MBTORDERSLib.MbtPositionStrategyGroup group) { }
        void MBTORDERSLib._IMbtOrderClientEvents.OnPositionStrategyGroupAdded(MBTORDERSLib.MbtPositionStrategyGroup group) { }
    }
}
