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
using System.Drawing;
using TickZoom.Api;

using TickZoom.Common;

namespace TickZoom
{
	/// <summary>
	/// Description of Line.
	/// </summary>
	public class WideChannel 
	{
		Log log = Factory.Log.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		Bars bars;
		int interceptBar = 0;
		int firstBar = int.MinValue;
		int lastBar;
		LinearRegression lrHigh = new LinearRegression();
		LinearRegression lrLow = new LinearRegression();
		int upperLineId = 0; // for charts
		int highLineId = 0; // for charts
		int lowLineId = 0; // for charts
		int lowerLineId = 0; // for reversal point, intrabar.
		double highestDev = 0;
		double lowestDev = 0;	
		int highestX = 0;
		int lowestX = 0;	
		int lowestLow = int.MaxValue;	
		double fttHighestHigh = double.MinValue;
		double fttLowestLow = double.MaxValue;	
		Color longColor = Color.Blue;
		Color shortColor = Color.Magenta;
		bool isCalculated = false;
		Trend direction = Trend.None;

		Trend traverseDirection = Trend.Flat;
		Integers highMaxVolume = Factory.Engine.Integers();
		Integers lowMaxVolume = Factory.Engine.Integers();
		
		public WideChannel(Trend direction, Bars bars) 
		{
			this.direction = direction;
			this.bars = bars;
		}
		
		public WideChannel( WideChannel other) {
			this.bars = other.bars;
			lrHigh = new LinearRegression(other.lrHigh.Coord);
			lrLow = new LinearRegression(other.lrLow.Coord);
		}
		
		public void ResetMaxVolume() {
			highMaxVolume = Factory.Engine.Integers();
			lowMaxVolume = Factory.Engine.Integers();
			UpdateTick();
		}
		
		public int GetHighYFromBar(int bar) {
			return GetHighY(bar-interceptBar);
		}
		
		public int GetLowYFromBar(int bar) {
			return GetLowY(bar-interceptBar);
		}
		
		public void addHigh(double bar) {
			lrHigh.addPoint(bar-interceptBar, bars.High[bars.CurrentBar-(int)bar]);
		}
		
		public void addLow(double bar) {
			lrLow.addPoint(bar-interceptBar, bars.Low[bars.CurrentBar-(int)bar]);
		}
		
		public void removePointsBefore( int bar) {
			lrHigh.removePointsBefore(bar);
			lrLow.removePointsBefore(bar);
		}
		
		public double CorrelationHigh {
			get { return lrHigh.Correlation; }
		}
		
		public double CorrelationLow {
			get { return lrLow.Correlation; }
		}
		
		public void Calculate() {
			if( direction == Trend.None ) {
				throw new ApplicationException("Must set direction before calculate.");
			}
			isCalculated = true;
			switch( direction) {
				case Trend.Down:
					lrHigh.calculate();
//					double mult = (1 - (1 / (double) lrHigh.Coord.Count));
//					lrHigh.Slope = lrHigh.Slope * mult;
					break;
				case Trend.Up:
					lrLow.calculate();
//					mult = (1 - (1 / (double) lrLow.Coord.Count));
//					lrLow.Slope = lrLow.Slope * mult;
					break;
			}
		}
		
		public void UpdateEnds() {
			if( !isCalculated) {
				throw new ApplicationException( "Linear Regression was never calculated.");
			}
			firstBar = int.MinValue;
			for(int i=0; i<lrHigh.Coord.Count; i++) {
				if( lrHigh.Coord[i].X > firstBar) {
					firstBar = (int) (lrHigh.Coord[i].X);
				}
			}
			for(int i=0; i<lrLow.Coord.Count; i++) {
				if( lrLow.Coord[i].X > firstBar) {
					firstBar = (int) (lrLow.Coord[i].X);
				}
			}
			lastBar = firstBar;
			for(int i=0; i<lrHigh.Coord.Count; i++) {
				if( lrHigh.Coord[i].X < lastBar) {
					lastBar = (int) (lrHigh.Coord[i].X);
				}
			}
			for(int i=0; i<lrLow.Coord.Count; i++) {
				if( lrLow.Coord[i].X < lastBar) {
					lastBar = (int) (lrLow.Coord[i].X);
				}
			}
		}
		
		bool drawLines = true;
		bool reDrawLines = true;
		Chart chart = null;

		int extend = 20;
		
		bool drawSolidLines = true;

		bool drawDashedLines = false;

		public void TryDraw() {
			try {
				
				Calculate();
				UpdateEnds();  // Was before calcMaxDev
				calcDeviation();
				if( drawLines) {
					DrawChannel();
				}
			} catch( ApplicationException ex) {
				log.Notice( ex.ToString() );
				return;
			}
			return;
		}
		
		public void TryExtend() {
			try {
				
				Calculate();
//				UpdateEnds();  // Was before calcMaxDev
//				calcMaxDev();
				if( drawLines) {
					if( reDrawLines) {
						RedrawChannel();
					} else {
						DrawChannel();
					}
				}
			} catch( ApplicationException ex) {
				log.Notice( ex.ToString() );
				return;
			}
			return;
		}
		
		public int GetHighY(int x) {
			return (int) (lrHigh.Intercept + x*lrHigh.Slope);
		}
		
		public int GetLowY(int x) {
			return (int) (lrLow.Intercept + x*lrLow.Slope);
		}
		
		public void calcDeviation() {
			// Find max deviation from the line.
			highestDev = 0;
			lowestDev = 0;
			switch(direction) {
				case Trend.Down:
//					for( int x = lastBar; x <= firstBar && firstBar-x < bars.Count; x++) {
						int x = (int)lrLow.Coord[0].X;
						int highY = GetHighY(x);
						double low = bars.Low[bars.CurrentBar-x];
						double lowDev = highY - low;
						if( lowDev > lowestDev) {
							lowestDev = lowDev;
						}
//					}
					for( int y = 0; y < lrHigh.Coord.Count; y++) {
						lrLow.addPoint(lrHigh.Coord[y].X,lrHigh.Coord[y].Y-lowestDev);
					}
					lrLow.calculate();
					break;
				case Trend.Up:
//					for( int x = lastBar; x <= firstBar && firstBar-x < bars.Count; x++) {
						x = (int)lrHigh.Coord[0].X;
						int lowY = GetLowY(x);
						double high = bars.High[bars.CurrentBar-x];
						double highDev = high - lowY;
						if( highDev > highestDev) {
							highestDev = highDev;
						}
//					}
					for( int y = 0; y < lrLow.Coord.Count; y++) {
						lrHigh.addPoint(lrLow.Coord[y].X,lrLow.Coord[y].Y+highestDev);
					}
					lrHigh.calculate();
					break;
			}
//			CalcFTT(bars);
		}
		
		private void CalcFTT(Bars bars) {
			if( highMaxVolume.Count==0) {
				highMaxVolume.Add(0);
			} else {
				highMaxVolume.Add(highMaxVolume[0]);
			}
			if( lowMaxVolume.Count==0) {
				lowMaxVolume.Add(0);
			} else {
				lowMaxVolume.Add(lowMaxVolume[0]);
			}
			if( bars.High[0] > fttHighestHigh && bars.High[0] > bars.High[1]) {
				if( bars.Volume[0] > highMaxVolume[0] ) {
					highMaxVolume[0] = bars.Volume[0];
				}
				fttHighestHigh = bars.High[0];
			}
			if( bars.Low[0] < fttLowestLow && bars.Low[0] < bars.Low[1] ) {
				if( bars.Volume[0] > lowMaxVolume[0]) {
					lowMaxVolume[0] = bars.Volume[0];
					log.Debug("Low Max Volume Set to " + lowMaxVolume[0]);
				}
				fttLowestLow = bars.Low[0];
			}
		}
		
		public void DrawChannel() {
			int extendBar = firstBar + interceptBar + extend;
			int extendHighY = GetHighYFromBar( extendBar);
			int extendLowY = GetLowYFromBar( extendBar);
			int lastHigh = GetHighY(lastBar);
			int lastLow = GetLowY(lastBar);
			int firstHigh = GetHighY(firstBar);
			int firstLow = GetLowY(firstBar);
			Color topColor;
			Color botColor;
			topColor = botColor = direction==Trend.Up ? longColor : shortColor;
			if( drawDashedLines) {
				upperLineId = chart.DrawLine( topColor, extendBar, extendHighY + highestDev,
					                                lastBar+interceptBar, lastHigh + highestDev, LineStyle.Dashed);
				lowerLineId = chart.DrawLine( botColor, extendBar, extendLowY - lowestDev,
						                                lastBar+interceptBar, lastLow - lowestDev, LineStyle.Dashed);
			}
			if( drawSolidLines) {
				highLineId = chart.DrawLine( topColor, extendBar, extendHighY,
					                                firstBar+interceptBar, firstHigh, LineStyle.Solid);
				lowLineId = chart.DrawLine( botColor, extendBar, extendLowY,
					                                firstBar+interceptBar, firstLow, LineStyle.Solid);
			}
		}

		public void RedrawChannel() {
			int extendBar = firstBar + interceptBar + extend;
			int extendHighY = GetHighYFromBar( extendBar);
			int extendLowY = GetLowYFromBar( extendBar);
			int y2High = GetHighY(lastBar);
			int y2Low = GetLowY(lastBar);
			Color topColor;
			Color botColor;
			topColor = botColor = direction==Trend.Up ? longColor : shortColor;
			if( drawDashedLines) {
				chart.ChangeLine( upperLineId, topColor, extendBar, extendHighY + highestDev,
			                 lastBar+interceptBar, y2High + highestDev, LineStyle.Dashed);
				chart.ChangeLine( lowerLineId, botColor, extendBar, extendLowY - lowestDev,
			                 lastBar+interceptBar, y2Low - lowestDev, LineStyle.Dashed);
			}
			if( drawSolidLines) {
				chart.ChangeLine( highLineId, botColor, extendBar, extendHighY,
		                 lastBar+interceptBar, y2High, LineStyle.Solid);
				chart.ChangeLine( lowLineId, botColor, extendBar, extendLowY,
		                 lastBar+interceptBar, y2Low, LineStyle.Solid);
			}
		}

		public double SlopeHigh {
			get { return lrHigh.Slope; }
			set { lrHigh.Slope = value; }
		}
		
		public double SlopeLow {
			get { return lrLow.Slope; }
			set { lrLow.Slope = value; }
		}
		
		// Try down and up true for slope = 0 (horizontal line)
		// so that we get correct top and bottom lines.
		public bool IsDown {
			get { return direction == Trend.Down; }
		}
		
		public bool IsUp {
			get { return direction == Trend.Up; }
		}
		
		public int Bar1 {
			get { return firstBar+interceptBar; }
		}
		
		public int Bar2 {
			get { return lastBar+interceptBar; }
		}
		
		public double Middle {
			get { 
				return (Top + Bottom)/2;
			}
		}
	
		public double Top {
			get {
				double line = High;
				return line + highestDev;
			}
		}
		
		public double Width {
			get {
				return Top - Bottom;
			}
		}
		
		public int High {
			get { return (int) GetHighYFromBar(bars.CurrentBar); }
		}
		
		public int Low {
			get { return (int) GetLowYFromBar(bars.CurrentBar); }
		}
		
		public double Bottom {
			get { 
				double line = Low;
				return line - lowestDev;
			}
		}
		
		public int Length {
			get{ return firstBar - lastBar; }
		}
		
		public double HighestDev {
			get { return highestDev; }
		}
		
		public double LowestDev {
			get { return lowestDev; }
		}
		
		public int UpperLineId {
			get { return upperLineId; }
			set { upperLineId = value; }
		}
		
		public int HighLineId {
			get { return highLineId; }
			set { highLineId = value; }
		}
		
		public int InterceptBar {
			get { return interceptBar; }
			set { interceptBar = value; }
		}
		
		public int LastLowBar {
			get { return lowestX + interceptBar; }
		}
		
		public int LastLowPrice {
			get { return lowestLow; }
		}
		
		public int LastHighBar {
			get { return highestX + interceptBar; }
		}
		
		public Color LongColor {
			get { return longColor; }
			set { longColor = value; }
		}
		
		public Color ShortColor {
			get { return shortColor; }
			set { shortColor = value; }
		}
		
		Tick tick;
		ChannelStatus status = ChannelStatus.Inside;
		
		public ChannelStatus Status {
			get { return status; }
		}
		
		private void UpdateTick() {
			if( tick.Ask > Top - 20) {
				status = ChannelStatus.Above;
			} else if( tick.Ask > High) {
				status = ChannelStatus.Upper;
			} else if( tick.Bid < Bottom + 20) {
				status = ChannelStatus.Below;
			} else if( tick.Bid < Low) {
				status = ChannelStatus.Lower;
			} else  {
				status = ChannelStatus.Inside;
			}
			switch( status) {
				case ChannelStatus.Above:
				case ChannelStatus.Upper:
					traverseDirection = Trend.Down;
					break;
				case ChannelStatus.Below:
				case ChannelStatus.Lower:
					traverseDirection = Trend.Up;
					break;
			}
			isLongFTT = highMaxVolume.Count>0 && IsUp && tick.Ask > fttHighestHigh && bars.Volume[0] < highMaxVolume[0] - 5000;
			isShortFTT = lowMaxVolume.Count>0 && IsDown && tick.Bid < fttLowestLow && bars.Volume[0] < lowMaxVolume[0] - 5000;
//			if( TradeSignal.IsShortFTT) {
//				TickConsole.WriteFile( "TradeSignal.IsShortFTT: isLow="+(lowMaxVolume.Count>0 && IsDown && tick.Bid < fttLowestLow)+", volume="+bars.Volume[0] +", lowMaxVolume="+(lowMaxVolume[0]-5000));
//			}
			isLongTraverse = highMaxVolume.Count>0 && IsUp && tick.Ask > fttHighestHigh && bars.Volume[0] > highMaxVolume[0];
			isShortTraverse = lowMaxVolume.Count>0 && IsDown && tick.Bid < fttLowestLow && bars.Volume[0] > lowMaxVolume[0];
		}
	
		public Tick Tick {
			get { return tick; }
			set { tick = value; UpdateTick(); }
		}
		
		public void LogShortTraverse() {
			log.Debug( "LogShortTraverse=" + (lowMaxVolume.Count>0 && IsDown && tick.Bid < fttLowestLow) +
			                       ", volume=" + bars.Volume[0] +
			                       ", low max volume=" +lowMaxVolume[0]);
		}
		
		bool isLongFTT = false;
		bool isShortFTT = false;
		bool isLongTraverse = false;
		bool isShortTraverse = false;
		
		public bool IsLongTraverse {
			get { return isLongTraverse; }
		}
		
		public bool IsShortTraverse {
			get { return isShortTraverse; }
		}

		public bool IsLongFTT {
			get { return isLongFTT; }
		}
		
		public bool IsShortFTT {
			get { 
//				if( TradeSignal.IsShortFTT && lowMaxVolume.Count>0) {
//					TickConsole.WriteFile( "TradeSignal.IsShortFTT: isLow="+(IsDown && tick.Bid < fttLowestLow)+", volume="+bars.Volume[0] +", lowMaxVolume="+(lowMaxVolume[0]-5000));
//				}
				return isShortFTT; }
		}
		
		public bool IsAbove {
			get { return status == ChannelStatus.Above; }
		}
		
		public bool IsGoingUp {
			get { return traverseDirection == Trend.Up; }
		}
		
		public bool IsGoingDown {
			get { return traverseDirection == Trend.Down; }
		}
		
		public bool IsBelow {
			get { return status == ChannelStatus.Below; }
		}
		
		public bool IsUpper {
			get { return status == ChannelStatus.Upper; }
		}
		
		public bool IsLower {
			get { return status == ChannelStatus.Lower; }
		}
		
		public bool IsInside {
			get { return status == ChannelStatus.Inside; }
		}
		
		public enum ChannelStatus {
			Above,
			Upper,
			Inside,
			Lower,
			Below,
		}
		
		public Trend TraverseDirection {
			get { return traverseDirection; }
		}
		
		public int HighMaxVolume {
			get { return highMaxVolume[0]; }
		}
		
		public int LowMaxVolume {
			get { return lowMaxVolume[0]; }
		}
		
		public bool IsHalfLowMaxVolume {
			get { return bars.Volume.Count>0 && lowMaxVolume.Count>0 && bars.Volume[0] * 2 < lowMaxVolume[0]; }
		}
		
		public bool IsHalfHighMaxVolume {
			get { return bars.Volume.Count>0 && lowMaxVolume.Count>0 && bars.Volume[0] * 2 < highMaxVolume[0]; }
		}
		
		public Trend Direction {
			get { return direction; }
		}
		
		public bool DrawLines {
			get { return drawLines; }
			set { drawLines = value; }
		}
		
		public bool ReDrawLines {
			get { return reDrawLines; }
			set { reDrawLines = value; }
		}
		
		public int Extend {
			get { return extend; }
			set { extend = value; }
		}
		
		public bool DrawSolidLines {
			get { return drawSolidLines; }
			set { drawSolidLines = value; }
		}
		
		public bool DrawDashedLines {
			get { return drawDashedLines; }
			set { drawDashedLines = value; }
		}
		
		public Chart Chart {
			get { return chart; }
			set { chart = value; }
		}
	}
}
