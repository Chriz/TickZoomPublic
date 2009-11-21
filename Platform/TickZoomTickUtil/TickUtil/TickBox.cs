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
 * Date: 2/28/2009
 * Time: 10:27 PM
 * <http://www.tickzoom.org/wiki/Licenses>.
 */
#endregion

using System;
using System.IO;
using TickZoom.Api;

namespace TickZoom.TickUtil
{

	/// <summary>
	/// Description of TickDOM.
	/// </summary>
	public class TickBox : TickIO
	{
		private TickImpl tick;
		
		public void init(TickBinary tickBinary) {
			tick.init(tick);
		}
		
		public void init(TickIO tickIO) {
			tick.init(tickIO);
		}
		
		public void init(TickIO tickIO, byte contentMask){
			tick.init(tickIO,contentMask);
		}
		
		public void init(TimeStamp utcTime, double dBid, double dAsk) {
			tick.init(utcTime,dBid,dAsk);
		}

		public void init(TimeStamp utcTime, double price, int size) {
			tick.init(utcTime,price,size);
		}

		public void init(TimeStamp utcTime, byte side, double price, int size) {
			init(utcTime, side, price, size);
		}
		
		public void init(TimeStamp utcTime, byte side, double dPrice, int size, double dBid, double dAsk) {
			init(utcTime, side, dPrice, size, dBid, dAsk);
		}

		public void init(TimeStamp utcTime, byte side, double price, int size, double dBid, double dAsk, ushort[] bidSize, ushort[] askSize) {
			init(utcTime, side, price, size, dBid, dAsk, bidSize, askSize);
		}

		public TickImpl Tick {
			get { return tick; }
			set { tick = value; }
		}

		public int BidDepth {
			get { return tick.BidDepth; }
		}
		
		public int AskDepth {
			get { return tick.AskDepth; }
		}
		
		public override string ToString() {
			return tick.ToString();
		}
		
		public void ToWriter(MemoryStream writer) {
			tick.ToWriter(writer);
		}
		
		public byte DataVersion {
			get { return tick.DataVersion; }
		}
		
		public double Bid {
			get { return tick.Bid; }
		}
		
		public double Ask {
			get { return tick.Ask; }
		}
		
		public byte Side {
			get { return tick.Side; }
		}
		
		public double Price {
			get { return tick.Price; }
		}
		
		public int Size {
			get { return tick.Size; }
		}
		
		public int Sentiment {
			get { return tick.Sentiment; }
		}
		
		public int Volume {
			get { return tick.Size; }
		}
		
		public ushort AskLevel(int level) {
			return tick.AskLevel(level);
		}
		
		public ushort BidLevel(int level) {
			return tick.BidLevel(level);
		}
		
		public TimeStamp Time {
			get { return tick.Time; }
			set { tick.Time = value; }
		}
		
		public int CompareTo(TickBox other)
		{
			return tick.CompareTo(other.tick);
		}
		
		public override int GetHashCode()
		{
			return tick.GetHashCode();
		}
		
		public override bool Equals(object other)
		{
			TickBox box = other as TickBox;
			return CompareTo(box) == 0;
		}
		
		public bool Equals(TickBox other)
		{
			return CompareTo(other) == 0;
		}
		
		public int FromReader(BinaryReader reader)
		{
			return tick.FromReader(reader);
		}
		
		public TickBinary Extract() {
			return tick.Extract();
		}
		
		public byte ContentMask {
			get { return tick.ContentMask; }
		}
		
		public long lBid {
			get { return tick.lBid; }
			set { tick.lBid = value; }
		}
		
		public long lAsk {
			get { return tick.lAsk; }
			set { tick.lAsk = value; }
		}
		
		public long lPrice {
			get { return tick.lPrice; }
			set { tick.lPrice = value; }
		}
		
		public bool IsRealTime {
			get { return tick.IsRealTime; }
			set { tick.IsRealTime = value; }
		}
		
		public TimeStamp UtcTime {
			get { return tick.UtcTime; }
			set { tick.UtcTime = value; }
		}
		
		public ulong lSymbol {
			get { return tick.lSymbol; }
			set { tick.lSymbol = value; }
		}
		
		public string Symbol {
			get { return tick.Symbol; }
			set { tick.Symbol = value; }
		}
		
		public int DomLevels {
			get { return tick.DomLevels; }
		}
		
		public bool IsTrade {
			get { return tick.IsTrade; }
		}
		
		public bool IsSimulateTicks {
			get { return tick.IsSimulateTicks; }
			set { tick.IsSimulateTicks = value; }
		}
		
		public object ToPosition() {
			return tick.Time;
		}
		
//		public int Length {
//			get { return tick.Length; }
//		}
	}
}
