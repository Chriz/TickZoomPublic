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

#region Namespaces
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using TickZoom.Api;
using TickZoom.Common;
#endregion

namespace MiscTest
{
    public class MQ_BadFakeTick_2 : StrategyCommon
    {

        /// <summary>
        /// Contracts to trade
        /// </summary>
        public int Quantity
        { get { return quantity; } set { quantity = value; } }
        int quantity = 50;


        public MQ_BadFakeTick_2()
        {
        }

        public override void OnInitialize()
        {
            PositionSize.Size = Quantity;

        }

       	TimeStamp firstTime = new TimeStamp("2008-09-08 09:35:00");
       	TimeStamp secondTime = new TimeStamp("2008-09-08 09:36:00");
        public override bool OnIntervalClose()
        {
            if (Bars.EndTime[0] == firstTime)
            {
                Enter.BuyLimit(127.92);
            }
            if (Bars.EndTime[0] == secondTime)
            {
                Exit.SellStop(127.63);
            }

            return true;
        }
    }
}