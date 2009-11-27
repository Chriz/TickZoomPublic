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
 * Date: 3/13/2009
 * Time: 10:14 PM
 * <http://www.tickzoom.org/wiki/Licenses>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ArrowDirection = TickZoom.Api.ArrowDirection;

using TickZoom;
using TickZoom.Api;
using ZedGraph;

namespace TickZoom
{
	/// Description of UserControl1.
	/// </summary>
	public partial class ChartControl : UserControl, TickZoom.Api.Chart
	{
		private static Log log;
		private static bool debug;
		private static bool trace;
	    TimeStamp firstTime;
		StockPointList stockPointList;
		List<PointPairList> lineList;
		List<Indicator> indicators; 
		object chartLocker = new Object();
		bool showPriceGraph = true;
		int objectId = 0;
		float _clusterWidth = 0;
		StrategySupportInterface strategyForTrades;
		bool isDynamicUpdate = false;
		Color[] colors = { Color.Black, Color.Red, Color.FromArgb(194,171,213), Color.FromArgb (250,222,130) } ;
		bool isAudioNotify = false;
		bool isAutoScroll = true;
		bool isCompactMode = false;
		ChartType chartType = ChartType.Bar;
		Interval intervalChartDisplay;
		Interval intervalChartBar;
		Interval intervalChartUpdate;
		string storageFolder;
        private SynchronizationContext context;
        SymbolInfo symbol;
		
	    public ChartControl()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			context = SynchronizationContext.Current;
            if(context == null)
            {
                context = new SynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(context);
            }

			InitializeComponent();
		    stockPointList = new StockPointList();
		    lineList = new List<PointPairList>();
		    indicators = new List<Indicator>();
		    try {
				log = Factory.Log.GetLogger(typeof(ChartControl));
				debug = log.IsDebugEnabled;
				trace = log.IsTraceEnabled;
				Interval intervalChartDisplay = Factory.Engine.DefineInterval(BarUnit.Day,1);
				Interval intervalChartBar = Factory.Engine.DefineInterval(BarUnit.Day,1);
				Interval intervalChartUpdate = Factory.Engine.DefineInterval(BarUnit.Day,1);
	       		storageFolder = Factory.Settings["AppDataFolder"];
	       		if( storageFolder == null) {
	       			throw new ApplicationException( "Must set AppDataFolder property in app.config");
	       		}
		    } catch( Exception) {
				// This exception means we're running inside the form designer.
				// TODO: find a better way to determine if running in form designer mode.
		    }
	    }
		
		public void ChartLoad(object sender, EventArgs e)
		{
			if( debug) log.Debug("ChartLoad()");
		   DrawChart();
		   // Size the control to fill the form with a margin
		   SetSize();			
		}
		
		public void ChartResize(object sender, EventArgs e)
		{
			if( debug) log.Debug("ChartResize()");
			SetSize();
		}
		
		private void SetSize()
		{
//			int width = ClientRectangle.Width - 25;
//			int height = ClientRectangle.Height - toolStripStatusXY.Height - logTextBox.Height - 40;
//			if( dataGraph.Size.Width == width &&
//			   dataGraph.Size.Height == height) {
//				// Size has already been set.
//				return;
//			}
//			    
//			// Leave a small margin around the outside of the control
//			
//			logTextBox.Location = new Point( 10, ClientRectangle.Height - logTextBox.Height - toolStripStatusXY.Height - 10);
//			logTextBox.Width = width;
//			checkBoxOnTop.Location = new Point(width - 55,5);
//			audioNotify.Location = new Point(width - 145,5);
//			dataGraph.Location = new System.Drawing.Point( 10, 30 );
//			dataGraph.Size = new Size( width, height);
		}
		
		private delegate void WriteLineDelegate(string text);		
		
		public void WriteLine(string text) {
		    if(InvokeRequired)
		    {
		        // This is a worker thread so delegate the task.
		        logTextBox.Invoke(new WriteLineDelegate(WriteLine), text);
		    }
		    else
		    {
		        // This is the UI thread so perform the task.
		        logTextBox.Text += text + Environment.NewLine;
	                logTextBox.SelectionStart = logTextBox.Text.Length;
	                logTextBox.ScrollToCaret();
		    }		
		}
		public void AudioNotify(Audio clip) {
			if( isAudioNotify) {
				string fileName = "";
				try {
					switch( clip) {
						case Audio.RisingVolume:
							fileName = storageFolder + @"\Media\risingVolume.wav";
						    SoundPlayer simpleSound = new SoundPlayer(fileName);
						    simpleSound.Play();
						    break;
						case Audio.IntervalChime:
						    fileName = storageFolder + @"\Media\intervalChime.wav";
						    simpleSound = new SoundPlayer(fileName);
						    simpleSound.Play();
						    break;
					}
				} catch( Exception e) {
					log.Notice("Error playing " + fileName + ": " + e);
				}
			}
		}
	
		private void InitializeChart( )
		{
			MasterPane master = dataGraph.MasterPane;
			// Vertical pan and zoom not allowed
			dataGraph.IsEnableVPan = false;
			dataGraph.IsEnableVZoom = false;
			
			// Fill background
			master.Fill = new Fill( Color.White, Color.FromArgb( 220, 220, 255 ), 45.0f );
			// Clear out the initial GraphPane
			master.PaneList.Clear();
			
			//Show masterpane title.
			master.Title.IsVisible = false;
			
			// Leave a margin around the masterpane, but only small gap between panes.
			master.Margin.All = 10;
			master.InnerPaneGap = 5;
			
			priceGraphPane = createPane();
			
			string[] yLables = { "Fun 1", "Fun 2", "Fun 3" };
			ColorSymbolRotator rotator = new ColorSymbolRotator();
		}
		
		void setLayout() {
			layoutLastPane();
			MasterPane master = dataGraph.MasterPane;
			master.IsFontsScaled = false;
	//			PaneList paneList = master.PaneList;
			using ( Graphics g = this.CreateGraphics() )
			{
				
				float[] proportions = new float[master.PaneList.Count];
				int[] layout = new int[master.PaneList.Count];
				layout[0] = 1;
				proportions[0] = Math.Max(4,master.PaneList.Count);
				for( int i = 1; i< master.PaneList.Count; i++) {
					layout[i] = 1;
					proportions[i] = 1;
				}
				master.IsCommonScaleFactor = true;
				master.SetLayout(g, true, layout, proportions);
				
				// Synchronize the Axes
				dataGraph.IsAutoScrollRange = true;
				dataGraph.IsShowHScrollBar = true;
				dataGraph.IsSynchronizeXAxes = true;
				dataGraph.IsShowDrawObjectTags = true;
				
				// Horizontal pan and zoom allowed
				dataGraph.IsEnableHPan = true;
				dataGraph.IsEnableHZoom = true;
				
				// Vertical pan and zoom not allowed
				dataGraph.IsEnableVPan = false;
				dataGraph.IsEnableVZoom = false;
			}
		}
		
		GraphPane createPane() {
			MasterPane master = dataGraph.MasterPane;
			// Create a new graph -- dimensions to be set later by MasterPane Layout
			GraphPane myPaneT = new GraphPane( new Rectangle( 10, 10, 10, 10 ),
				"",
				"Time, Days",
				symbol.Symbol );
			
			myPaneT.Fill.IsVisible = false;
			
			// pretty it up a little
	//			myPaneT.Chart.Fill = new Fill( Color.White, Color.LightGoldenrodYellow, 45.0f );
			myPaneT.Chart.Fill = new Fill( Color.White, Color.White, 45.0f );
			myPaneT.Border = new Border(true, Color.Black, 2);
			// set the dimension so fonts look bigger.
			myPaneT.BaseDimension = 3.0F;
			
			// Hide the titles
			myPaneT.XAxis.Title.IsVisible = false;
			if( isCompactMode) {
				myPaneT.YAxis.Title.IsVisible = false;
				myPaneT.YAxis.Scale.IsVisible = false;
				myPaneT.YAxis.MinSpace = 10;
			} else {
				myPaneT.YAxis.Title.IsVisible = true;
				myPaneT.YAxis.Scale.IsVisible = true;
				myPaneT.YAxis.MinSpace = 80;
			}
			myPaneT.YAxis.Title.IsOmitMag = false;
			
			myPaneT.Legend.IsVisible = false;
			myPaneT.Border.IsVisible = false;
			myPaneT.Title.IsVisible = false;
			
			// Get rid of the tics that are outside the chart rect
			myPaneT.XAxis.MajorTic.IsOutside = false;
			myPaneT.XAxis.MinorTic.IsOutside = false;
			
			myPaneT.XAxis.Scale.IsVisible = false;
			
			// Show the X grids
			myPaneT.XAxis.MajorGrid.IsVisible = false;
			myPaneT.XAxis.MinorGrid.IsVisible = false;
			// Remove all margins
			myPaneT.Margin.All = 0;
	    
			// Use DateAsOrdinal to skip weekend gaps
			if( chartType == ChartType.Bar) {
				myPaneT.XAxis.Type = AxisType.DateAsOrdinal;
				_clusterWidth = 1.25F;
			} else {
				myPaneT.XAxis.Type = AxisType.Date;
			    XDate size = new XDate(0d);
			    size.AddSeconds(intervalChartDisplay.Seconds);
			    _clusterWidth = size;
			}
	   
			// Except, leave some top margin on the first GraphPane
			if ( master.PaneList.Count == 0 )
				myPaneT.Margin.Top = 20;
			
			
			if ( master.PaneList.Count > 0 ) {
				 myPaneT.YAxis.Scale.IsSkipLastLabel = true;
			}
			
			// This sets the minimum amount of space for the left and right side, respectively
			// The reason for this is so that the ChartRect's all end up being the same size.
				
			myPaneT.IsFontsScaled = false;
			myPaneT.IsBoundedRanges = true;
			master.Add( myPaneT);
			return myPaneT;
		}
		
		void layoutLastPane() {
			MasterPane master = dataGraph.MasterPane;
			GraphPane myPaneT = master.PaneList[master.PaneList.Count-1];
			myPaneT.Margin.Bottom = 10;
		}
	
		public List<Indicator> Indicators {
			get { return indicators; }
		}
		
		public int DrawText(string text, Color color, int bar, double y, Positioning orient) {
			double x = barToXAxis(bar);
			TextObj textObj = new TextObj(text, x,y);
			textObj.IsClippedToChartRect=true;
			switch( orient) {
				case Positioning.UpperLeft:
					textObj.Location.AlignH = AlignH.Left;
					textObj.Location.AlignV = AlignV.Top;
					break;
				case Positioning.LowerLeft:
					textObj.Location.AlignH = AlignH.Left;
					textObj.Location.AlignV = AlignV.Bottom;
					break;
				case Positioning.LowerRight:
					textObj.Location.AlignH = AlignH.Right;
					textObj.Location.AlignV = AlignV.Bottom;
					break;
				case Positioning.UpperRight:
					textObj.Location.AlignH = AlignH.Right;
					textObj.Location.AlignV = AlignV.Top;
					break;
			}
			textObj.FontSpec.FontColor = color;
			textObj.FontSpec.Border.IsVisible = false;
			textObj.FontSpec.Fill.IsVisible = false;
			objectId++;
		   
			textObjs.Add(objectId,textObj);
		    priceGraphPane.GraphObjList.Add(textObj);
		   
			return objectId;
		}
		
		public int DrawBox(Color color, int bar, double y) {
			double width = 0.25;
			double height = 10;
			double x = barToXAxis(bar);
			double xwidth = barToXAxis(x+width) - barToXAxis(x);
			BoxObj box = new BoxObj(x-xwidth/2,y+height/2,xwidth,height,color,color);
			box.IsClippedToChartRect=true;
			objectId++;
			
			graphObjs.Add(objectId,box);
		    priceGraphPane.GraphObjList.Add(box);
			return objectId;
		}
		
		public int DrawBox(Color color, int bar, double y, double width, double height) {
			double x = barToXAxis(bar);
			BoxObj box = new BoxObj(x,y,width,height,color,color);
			box.IsClippedToChartRect=true;
			box.ZOrder = ZOrder.E_BehindCurves;
			objectId++;
			graphObjs.Add(objectId,box);
		    priceGraphPane.GraphObjList.Add(box);
			return objectId;
		}
		
		Dictionary<int,GraphObj> graphObjs = new Dictionary<int,GraphObj>();
		Dictionary<int,GraphObj> textObjs = new Dictionary<int,GraphObj>();
		
		public int DrawArrow( Color color, float size, int bar1, double y1, int bar2, double y2) {
			return 0;
		}
		
		public int DrawArrow( ArrowDirection direction, Color color, float size, int bar, double price) {
			ArrowObj arrow = CreateArrow(direction,color,size,bar,price);
			objectId++;
			graphObjs.Add(objectId,arrow);
		    priceGraphPane.GraphObjList.Add(arrow);
			return objectId;
		}
		/// <summary>
		/// Draws a trade and annotateg with hover message if possible.
		/// </summary>
		/// <param name="order"></param>
		/// <param name="fillPrice"></param>
		/// <param name="resultingPosition"></param>
		/// <returns></returns>
		public int DrawTrade(LogicalOrder order, double fillPrice, double resultingPosition)
		{
			Color color = Color.Empty;
			ArrowDirection direction = ArrowDirection.Up;
			if( order.TradeDirection == TradeDirection.Exit) {
				color = Color.Black;	
			} else {
				switch( order.Type) {
					case OrderType.BuyLimit:
					case OrderType.BuyStop:
					case OrderType.BuyMarket:
						color = Color.Green;
						direction = ArrowDirection.Up;
						break;
					case OrderType.SellLimit:
					case OrderType.SellStop:
					case OrderType.SellMarket:
						color = Color.Red;
						direction = ArrowDirection.Down;
						break;
					default: 
						throw new ApplicationException("Unknown OrderType " + order.Type + " for drawing a trade.");
				}
			}
			// One ticket open on tickzoom is to draw arrows to scale based
			// on the price range of the chart. This numbers for size and position
			// were hard code, calibrated to Forex prices.
			ArrowObj arrow = CreateArrow( direction, color, 12.5f, ChartBars.BarCount, fillPrice);
			StringBuilder sb = new StringBuilder();
			if( order.Tag != null) {
				sb.AppendLine(order.Tag);
			}
			sb.Append(order.TradeDirection);
			sb.Append(" ");
			sb.AppendLine(order.Type.ToString());
			if( order.Price > 0) {
				sb.Append("at ");
				sb.AppendLine(order.Price.ToString());
			}
			sb.Append("size ");
			sb.AppendLine(order.Positions.ToString());
			sb.Append("filled ");
			sb.AppendLine(fillPrice.ToString());
			sb.Append("new positions ");
			sb.AppendLine(resultingPosition.ToString());
			arrow.Tag = sb.ToString();
			objectId++;
			graphObjs.Add(objectId,arrow);
		    priceGraphPane.GraphObjList.Add(arrow);
			return objectId;
		}
		
		private ArrowObj CreateArrow( ArrowDirection direction, Color color, float size, int bar, double price) {
			double x = barToXAxis(bar);
			double tipPrice = price;
			double tipBar = bar;
			switch( direction) {
				case ArrowDirection.Up:
					price -= symbol.MinimumTick; 
					break;
				case ArrowDirection.Down:
					price += symbol.MinimumTick; 
					break;
			}
			ArrowObj arrow = new ArrowObj(color,size,bar,price,tipBar,tipPrice);
			arrow.IsClippedToChartRect=true;
			arrow.Location.CoordinateFrame = CoordType.AxisXYScale;
			return arrow;
		}
	
		public int DrawLine( Color color, Point p1, Point p2, LineStyle style) {
			LineObj line = CreateLine(color,p1.X,p1.Y,p2.X,p2.Y,style);
			objectId++;
			graphObjs.Add(objectId,line);
		    priceGraphPane.GraphObjList.Add(line);
			return objectId;
		}
		
		public float ClusterWidth {
			get { return _clusterWidth; }
		}
		
		public double barToXAxis(double bar) {
			double ret = 0;
			if( priceGraphPane.XAxis.Scale.IsAnyOrdinal) {
				ret = bar;
			} else {
				ret = stockPointList[stockPointList.Count-1].X + (bar-(stockPointList.Count-1))*ClusterWidth;
				ret-=ClusterWidth;
			}
			return ret;
		}
		
		public int DrawLine( Color color, int bar1, double y1, int bar2, double y2, LineStyle style) {
			
			double x1 = barToXAxis(bar1);
			double x2 = barToXAxis(bar2);
			LineObj line = CreateLine(color,x1,y1,x2,y2,style);
			objectId++;
			graphObjs.Add(objectId,line);
		    priceGraphPane.GraphObjList.Add(line);
			return priceGraphPane.GraphObjList.Count-1;
		}
		
		public void ChangeLine( int lineId, Color color, int bar1, double y1, int bar2, double y2, LineStyle style) {
			LineObj line = CreateLine(color,barToXAxis(bar1),y1,barToXAxis(bar2),y2,style);
	//			graphObjs[lineId] = line;
			priceGraphPane.GraphObjList[lineId] = line;
		}
		
		private LineObj CreateLine( Color color, double x1, double y1, double x2, double y2, LineStyle style) {
			LineObj line = new LineObj(color,x1,y1,x2,y2);
			line.IsClippedToChartRect=true;
			line.Location.CoordinateFrame = CoordType.AxisXYScale;
			switch( style) {
				case LineStyle.Dashed:
					line.Line.Style = System.Drawing.Drawing2D.DashStyle.Custom;
					line.Line.DashOn = 7;
					line.Line.DashOff = 5;
					break;
				case LineStyle.Solid:
					line.Line.Style = System.Drawing.Drawing2D.DashStyle.Solid;
					break;
				case LineStyle.Dotted:
					line.Line.Style = System.Drawing.Drawing2D.DashStyle.Dot;
					break;
			}
			line.Line.Width = 2;
			return line;
		}
		
		Dictionary<ModelInterface,PointPairList> repeatList = new Dictionary<ModelInterface,PointPairList>();
		
		int lastColorValue = 2;
		public void AddBar( Bars updateSeries, Bars displaySeries) {
			lock( chartLocker) {
	    		if( firstTime == TimeStamp.MinValue) {
	    			firstTime = updateSeries.Time[0];
	    		}
				// Set the default bar color
	        	lastColorValue = 2;
		        //if price is increasing color=black, else color=red
		        lastColorValue = displaySeries.Close[0] > displaySeries.Open[0] ? 0 : 1;
				
				UpdateIndicators(updateSeries);
			    
			    if( showPriceGraph) {
			    	UpdatePriceGraph(displaySeries,updateSeries);
			    }
			}
		}
		
		string paintBarName = "";
				
		// Update lines for all indicators.
		private void UpdateIndicators(Bars updateSeries) {
		    for( int j = 0; j < indicators.Count; j++) {
    			Indicator indicator = indicators[j];
    			if( lineList.Count <= j) {
					lineList.Add( new PointPairList());
    			}
    			UpdateIndicator(j,indicator,updateSeries);
			}
		}
		
		private void UpdateIndicator(int index, Indicator indicator, Bars updateSeries) {
			if( indicator.Count > 0) { 
				while( lineList[index].Count <= updateSeries.CurrentBar) {
					lineList[index].Add(double.NaN,double.NaN);
				}
				switch( indicator.Drawing.GraphType ) {
					case GraphType.PaintBar:
						UpdatePaintBar(indicator);
						break;
					case GraphType.Histogram:
						UpdateHistoGram(index,indicator,updateSeries);
						break;
					default:
						UpdateOrdinaryIndicator(index,indicator,updateSeries);
						break;
				}
			}
		}

		private void UpdateOrdinaryIndicator(int index, Indicator indicator, Bars updateSeries) {
			int colorIndex = 1;
			try {
				colorIndex = indicator.Drawing.ColorIndex;
			} catch ( ApplicationException) {
			}
			double val = indicator[0];
			double time = (double) updateSeries.Time[0];
			PointPair ppair;
			ppair = lineList[index][updateSeries.CurrentBar];
			ppair.X = time;
			ppair.Y = val;
			ppair.Z = colorIndex;
		}
		
		private void UpdatePaintBar(Indicator indicator) {
			if( paintBarName.Length > 0) {
				throw new ApplicationException( "Only one paint bar: " + paintBarName + 
				                           ". Found conflicting paint bar: " + indicator.Name);
			} else {
				paintBarName = indicator.Name;
			}
			lastColorValue = (int) indicator[0];
		}

		private void UpdateHistoGram(int index, Indicator indicator, Bars updateSeries) {
			int colorIndex = 1;
			try {
				colorIndex = indicator.Drawing.ColorIndex;
			} catch ( ApplicationException) {
			}
			double val = indicator[0];
			double time = (double) updateSeries.Time[0];
			PointPair ppair;
			int startBar = 0; 
			ppair = lineList[index][updateSeries.CurrentBar];
			ppair.X = time;
			ppair.Y = val;
			ppair.Z = colorIndex;
			startBar ++;
			PointPairList ppList;
			if( repeatList.TryGetValue(indicator, out ppList) == false) {
				ppList = new PointPairList();
				repeatList[indicator] = ppList;
			}
			while( ppList.Count <= updateSeries.CurrentBar) {
				ppList.Add(double.NaN,double.NaN);
			}
			// TODO: Make pplist an array. Wrap Indicator in IndicatorGraph object.
// 			for( int repeat=1; repeat < indicator.GraphRepeat && repeat < indicator.Count; repeat++, startBar++) {
//	    		val = indicator[startBar];
//	    		colorIndex = 1;
//				if( formula.checkWaitTillReady()) {
//					try {
//			    		colorIndex = indicator.GetColorIndex(startBar);
//					} catch ( BeyondCircularException e) {
//						formula.WaitTillReady.Add( e);
//					}
//	    		}
//	    		ppair = ppList[series.CurrentBar];
//	    		ppair.X = time;
//	    		ppair.Y = val;
//	    		ppair.Z = colorIndex;
// 			}
		}
		
		private void UpdatePriceGraph(Bars displaySeries, Bars updateSeries) {
    		StockPt pt;
    		if( stockPointList.Count < updateSeries.BarCount ) {
				double time = (double) updateSeries.Time[0];
    			time = (double) updateSeries.Time[0];
				double high = displaySeries.High[0];
		        double low = displaySeries.Low[0];
		        double open = displaySeries.Open[0];
		        double close = displaySeries.Close[0];
				pt = new StockPt( time, displaySeries.High[0],
		                                 displaySeries.Low[0],
		                                 displaySeries.Open[0],
		                                 displaySeries.Close[0], 10000 );
		        //if price is increasing color=black, else color=red
		        pt.ColorValue = lastColorValue;
				stockPointList.Add( pt );
    		} else {
			    pt = (StockPt) stockPointList.GetAt(updateSeries.CurrentBar);
			    // Update the bar on the chart.
				pt.High = displaySeries.High[0];
				pt.Low = displaySeries.Low[0];
				pt.Open = displaySeries.Open[0];
		        pt.Close = displaySeries.Close[0];
		        pt.ColorValue = lastColorValue;
    		}
    		UpdateScaleCheck( displaySeries );
		}
		
		public void InitializeTick(Tick tick) {
       		context.Send(new SendOrPostCallback(
       		delegate(object state)
       	    {
			    InitializeChart( );
				if( isDynamicUpdate && !priceGraphPane.XAxis.Scale.IsAnyOrdinal) {
			    	ZoomToLast( dataGraph.GraphPane);
				}
       		}), null);
		}
		
		protected void MenuClick_ZoomToLast( object sender, EventArgs e )
		{
			if ( dataGraph.MasterPane != null )
			{
				GraphPane pane = dataGraph.MasterPane.FindPane( _menuClickPt );
				ZoomToLast( dataGraph.GraphPane);
			}
		}
		
		public void ZoomToLast( GraphPane primaryPane)
		{
			if ( primaryPane != null )
			{
				primaryPane.IsBoundedRanges = false;
				primaryPane.YAxis.Scale.MaxAuto = true;
				primaryPane.YAxis.Scale.MinAuto = true;
				primaryPane.XAxis.Scale.MaxAuto = true;
				primaryPane.XAxis.Scale.MinAuto = true;
				
				
				if( stockPointList == null || stockPointList.Count == 0) {
					dataGraph.RestoreScale( primaryPane);
					return;
				}
				
				primaryPane.AxisChange();
				
				primaryPane.IsBoundedRanges = true;
				Scale yScale = primaryPane.YAxis.Scale;
				double price = stockPointList[stockPointList.Count-1].Y;
				Scale xScale = primaryPane.XAxis.Scale;
				double time;
				if( xScale.IsAnyOrdinal) {
					time = stockPointList.Count;
				} else {
					time = stockPointList[stockPointList.Count-1].X;
				}
				double yHeight = Math.Min(yScale.Max - yScale.Min, 800);
				double xWidth = ClusterWidth * 40;
				
				yScale.Max = price + yHeight/2;
				yScale.Min = price - yHeight/2;
				
				xScale.Max = time + xWidth * .30;
				xScale.Min = time - xWidth * .70;
			}
		}
		
//		void SetDefaultScale(object sender, EventArgs e) {
//			SetDefaultScale( spl[spl.Count-1].Y, spl[spl.Count-1].X);
//		}
		
//		private void SetDefaultScale(double price, double time) {
//			double yHeight = 800;
//			Scale yScale = priceGraphPane.YAxis.Scale;
//			yScale.Max = price + yHeight/2;
//			yScale.Min = price - yHeight/2;
//			double xWidth = ClusterWidth * 40;
//			Scale xScale = priceGraphPane.XAxis.Scale;
//			xScale.Max = time + xWidth * .30;
//			xScale.Min = time - xWidth * .70;
//			SetCommonXScale( xScale.Min, xScale.Max);
//			dataGraph.AxisChange();
//			dataGraph.Invalidate();
//		}
		
		double lastHigh;
		double lastLow;
		double lastTime;
		double lastBar;
		
		void UpdateScaleCheck(Bars series) {
			lastHigh = series.High[0];
			lastLow = series.Low[0];
			lastTime = (double) series.Time[0];
			lastBar = series.CurrentBar;	
		}
	
		bool KeepWithinScale() {
			Scale yScale = priceGraphPane.YAxis.Scale;
			if( stockPointList != null && (
				stockPointList.Count == 2 ||
				stockPointList.Count == 4 ||
				stockPointList.Count == 8 ||
				stockPointList.Count == 16 ||
				stockPointList.Count == 32 ) ) {
				ZoomToLast(dataGraph.GraphPane);
			}
			double yHeight = yScale.Max - yScale.Min;
			double yUpperLimit = yScale.Max - yHeight/4;
			double yLowerLimit = yScale.Min + yHeight/4;
			bool reset = false;
			if( lastHigh > yUpperLimit ) {
				resetYScale = -1;
			} else if( lastLow < yLowerLimit ) {
				resetYScale = 1;
			} else {
				resetYScale = 0;
			}
			if( resetYScale != 0 ) {
				if( lastHigh > yScale.Max || lastLow < yScale.Min ) {
					ZoomToLast(dataGraph.GraphPane);
				}
				double yMax = MoveByPixels(yScale,yScale.Max,resetYScaleSpeed*resetYScale);
				double yMin = MoveByPixels(yScale,yScale.Min,resetYScaleSpeed*resetYScale);
				if( !double.IsNaN(yMax) && !double.IsNaN(yMin)) {
					yScale.Max = yMax;
					yScale.Min = yMin;
					reset = true;
				}
	        }
			Scale xScale = priceGraphPane.XAxis.Scale;
			double xCurrent;
			if( priceGraphPane.XAxis.Scale.IsAnyOrdinal) {
				xCurrent = lastBar;
			} else {
				xCurrent = lastTime;
			}
			double xWidth = xScale.Max - xScale.Min;
			double xUpperLimit = xScale.Max - xWidth/6;
			double xLowerLimit = xScale.Max - xWidth/3;
			if( xCurrent > xUpperLimit) {
				resetXScale = true;
			}
			if( resetXScale && xCurrent > xLowerLimit) {
				if( xCurrent > xScale.Max) {
					ZoomToLast(dataGraph.GraphPane);
					resetXScaleSpeed *= 1.5f;
					log.Debug("resetXScaleSpeed = " + resetXScaleSpeed);
				}
				xScale.Min = MoveByPixels(xScale,xScale.Min,resetXScaleSpeed);
				xScale.Max = MoveByPixels(xScale,xScale.Max,resetXScaleSpeed);
//				xScale.Min = MoveByPixels(xScale,xScale.Min,1);
//				xScale.Max = MoveByPixels(xScale,xScale.Max,1);
				reset = true;
			} else {
				resetXScale = false;
			}
			return reset;
		}
		
		double MoveByPixels( Scale scale, double value, float movePixels) {
			float rawPixels = scale.Transform(value);
			float currPixels = (float) Math.Round(rawPixels);
			return scale.ReverseTransform(currPixels+movePixels);
		}
		
		void SetCommonXScale() {
			Scale xScale = priceGraphPane.XAxis.Scale;
			double min = xScale.Min;
			double max = xScale.Max;
			if( !double.IsNaN(max) && !double.IsNaN(min) ) {
				if(  dataGraph != null && dataGraph.MasterPane != null) {
					PaneList list = dataGraph.MasterPane.PaneList;
					for( int i=0; i<list.Count; i++) {
						list[i].XAxis.Scale.Max = max;
						list[i].XAxis.Scale.Min = min;
					}
				}
			}
		}
		
		bool resetXScale = false;
		float resetXScaleSpeed = 1;
		int resetYScale = 0;
		float resetYScaleSpeed = 1;
	
		public bool IsValid {
			get { return ChartBars != null && DisplayBars != null && UpdateBars != null; }
		}
		
		OHLCBarItem ohlcCurve;
		GraphPane priceGraphPane;
		
		public void DrawChart() {
			if( debug) log.Debug("ChartResize()");
       		context.Send(new SendOrPostCallback(
       		delegate(object state)
       	    {
       			try {
					// Setup the gradient fill...
					// Use Red for negative days and black for positive days
					Fill myFill = new Fill( colors );
					myFill.Type = FillType.GradientByColorValue;
					myFill.SecondaryValueGradientColor = Color.Empty;
					myFill.RangeMin = 0;
					myFill.RangeMax = colors.Length-1;
					
					//Create the OHLC and assign it a Fill
					ohlcCurve = priceGraphPane.AddOHLCBar( "Price", stockPointList, Color.Empty );
					ohlcCurve.Bar.GradientFill = myFill;
					ohlcCurve.Bar.Size = ClusterWidth;
					ohlcCurve.Bar.IsAutoSize = true;
					
					if( priceGraphPane != null && dataGraph.MasterPane != null) {
						CreateIndicators();
					}
					
					if( isDynamicUpdate) {
						ZoomToLast(dataGraph.GraphPane);
					}
					
					setLayout();
					// Calculate the Axis Scale Ranges
					dataGraph.AxisChange();
       			} catch (Exception ex) {
       				log.Error("ERROR: DrawChart ", ex);
       			}
       		}), null);
		}
		
	    Dictionary<string,GraphPane> signalPaneList = new Dictionary<string,GraphPane>();
	    Dictionary<string,GraphPane> secondaryPaneList = new Dictionary<string,GraphPane>();
	    
	    bool layoutChange = false;
		void CreateIndicators() {
		   //Create the indicators
		   for( int i = 0; i < indicators.Count; i++) {
	   			   ModelInterface indicator = indicators[i];
			   if( lineList.Count <= i) {
				   lineList.Add( new PointPairList());
			   } 
		   	   PointPairList pplist = lineList[i];
		   	   if(!indicator.Drawing.AlreadyDrawn) {
					Color color = indicator.Drawing.Color;
					GraphPane gp;
					string groupName = indicator.Drawing.GroupName;
					switch( indicator.Drawing.GraphType ) {
						case GraphType.FilledLine:
						case GraphType.Line:
					   	   	switch( indicator.Drawing.PaneType )
					   	   	{
					   	   		case PaneType.Primary:
									LineItem indCurve = priceGraphPane.AddCurve( indicator.Name, pplist, indicator.Drawing.Color, SymbolType.None);
									indCurve.Line.IsOptimizedDraw = true;
									indCurve.IsY2Axis = false;
							   	   	indicator.Drawing.AlreadyDrawn = true;
							   	   	layoutChange = true;
							   	   	if( indicator.Drawing.GraphType == GraphType.FilledLine) {
								   	   	indCurve.Line.Fill = new Fill( indicator.Drawing.Color );
							   	   	}
									break;
								case PaneType.Secondary:
						   	   		if( secondaryPaneList.TryGetValue( groupName, out gp) == false) {
						   	   			gp = createPane();
						   	   			gp.YAxis.Title.Text = groupName;
							   	   		secondaryPaneList[groupName] = gp;
						   	   		}
							   	   	indicator.Drawing.AlreadyDrawn = true;
							   	   	layoutChange = true;
						   	   		indCurve = gp.AddCurve( indicator.Name, pplist, color, SymbolType.None);
									if( ! Double.IsNaN(indicator.Drawing.ScaleMax)) {
										gp.YAxis.Scale.Max = indicator.Drawing.ScaleMax;
									}
									if( ! Double.IsNaN(indicator.Drawing.ScaleMin)) {
										gp.YAxis.Scale.Min = indicator.Drawing.ScaleMin;
									}
							   	   	if( indicator.Drawing.GraphType == GraphType.FilledLine) {
								   	   	indCurve.Line.Fill = new Fill( indicator.Drawing.Color );
							   	   	}
						   	   		break;
						   	   	case PaneType.OverlayPrimary:
									indCurve = priceGraphPane.AddCurve( indicator.Name, pplist, indicator.Drawing.Color, SymbolType.None);
									indCurve.IsY2Axis = true;
							   	   	indicator.Drawing.AlreadyDrawn = true;
							   	   	layoutChange = true;
									if( ! Double.IsNaN(indicator.Drawing.ScaleMax)) {
										priceGraphPane.Y2Axis.Scale.Max = indicator.Drawing.ScaleMax;
									}
									if( ! Double.IsNaN(indicator.Drawing.ScaleMin)) {
										priceGraphPane.Y2Axis.Scale.Min = indicator.Drawing.ScaleMin;
									}
							   	   	if( indicator.Drawing.GraphType == GraphType.FilledLine) {
								   	   	indCurve.Line.Fill = new Fill( indicator.Drawing.Color );
							   	   	}
									break;
					   	   	}
					   	   	break;
					   	case GraphType.Histogram:
				   	   		if( secondaryPaneList.TryGetValue( groupName, out gp) == false) {
				   	   			gp = createPane();
				   	   			gp.YAxis.Title.Text = groupName;
					   	   		secondaryPaneList[groupName] = gp;
				   	   		}
					   	   	indicator.Drawing.AlreadyDrawn = true;
					   	   	layoutChange = true;
					   	   	// Setup to either red or Black fill color.
				   	   		Fill myFill = new Fill( colors);
							myFill.Type = FillType.GradientByZ;
							myFill.RangeMin = 1;
							myFill.RangeMax = 4;
							myFill.RangeDefault = 1;
							
							myFill.SecondaryValueGradientColor = Color.Empty;
							
							BarItem indBar;
							
		   					PointPairList ppList;
	    						if( repeatList.TryGetValue(indicator, out ppList) == false) {
	    							ppList = new PointPairList();
	    							repeatList[indicator] = ppList;
	    					}
		   					
				   	   		indBar = gp.AddBar( indicator.Name, pplist, color);
						    gp.BarSettings.ClusterScaleWidth = ClusterWidth;
						    BarSettings settings = gp.BarSettings;
							indBar.Bar.Fill = myFill;
							indBar.Bar.Border.IsVisible=false;
							
							gp.BarSettings.ClusterScaleWidthAuto=false;
							if( ! Double.IsNaN(indicator.Drawing.ScaleMax)) {
								gp.YAxis.Scale.Max = indicator.Drawing.ScaleMax;
							}
							if( ! Double.IsNaN(indicator.Drawing.ScaleMin)) {
								gp.YAxis.Scale.Min = indicator.Drawing.ScaleMin;
							}
							break;
					}
	
		   	   }
		   }
		   
		   //Create the signals
		   for( int i = 0; i < lineList.Count; i++) {
	   			   ModelInterface indicator = indicators[i];
		   	   PointPairList pplist = lineList[i];
		   	   if( !indicator.Drawing.AlreadyDrawn) {
				   switch( indicator.Drawing.PaneType ) {
			   	   	case PaneType.Signal:
			   	   		GraphPane gp;
			   	   		if( signalPaneList.TryGetValue( indicators[i].Name, out gp) == false) {
			   	   			gp = createPane();
			   	   			gp.YAxis.Title.Text = indicator.Drawing.GroupName;
				   	   		signalPaneList[indicators[i].Name] = gp;
			   	   		}
						gp.AddCurve( indicators[i].Name, pplist, indicators[i].Drawing.Color, SymbolType.None);
			   	   		indicator.Drawing.AlreadyDrawn = true;
				   	   	layoutChange = true;
						break;
		   	   		}
		   	   }
		   }
		}
		
		private bool DataGraphMouseMoveEvent(ZedGraph.ZedGraphControl sender, System.Windows.Forms.MouseEventArgs e)
		{
			try { 
		        GraphPane myPane = dataGraph.GraphPane;
				// Save the mouse location
				PointF mousePt = new PointF( e.X, e.Y );
				int dragIndex;
				StockPt startPair;
				
				GraphPane.Default.NearestTol = 200.00f;
				
				// find the point that was clicked, and make sure the point list is editable
				// and that it's a primary Y axis (the first Y or Y2 axis)
				double curX, curY;
		        myPane.ReverseTransform( mousePt, out curX, out curY);
	        	dragIndex = 0;
				// save the starting point information
				if( stockPointList.Count > 0 ) {
					if( priceGraphPane.XAxis.Scale.IsAnyOrdinal) {
						dragIndex = Math.Min(stockPointList.Count-1,Math.Max(0,(int) Math.Round(curX-1)));
					} else {
						for( int i = 0; i<stockPointList.Count; i++) {
							if( stockPointList[i].X-(ClusterWidth/2) <= curX) {
								dragIndex = i;
							}
						}
					}
					startPair = (StockPt) stockPointList[dragIndex];
					DateTime time = DateTime.FromOADate(startPair.X);
					if( isCompactMode) {
						toolStripStatusXY.Text = time.ToLongTimeString() + ", " +
							startPair.Close.ToString("f0");
					} else {
						    toolStripStatusXY.Text = time.ToLongDateString() + " " + 
							time.ToLongTimeString() + ", " +
							"O:" + startPair.Open.ToString(",0.000") + ", " + 
							"H:" + startPair.High.ToString(",0.000") + ", " + 
							"L:" + startPair.Low.ToString(",0.000") + ", " + 
							"C:" + startPair.Close.ToString(",0.000") + ", " +
							"Bar: " + (dragIndex+1) + ", " +
							"Period: " + intervalChartBar;
					}
				}
			} catch( Exception ex) {
				log.Notice(ex.ToString());
			}
		   // Return false to indicate we have not processed the MouseMoveEvent
		   // ZedGraphControl should still go ahead and handle it
		   return false;
		}
		
		void refreshTick(object sender, EventArgs e)
		{
			try {
				if( Visible) {
					lock( chartLocker) {
						System.Windows.Forms.Timer timer = (System.Windows.Forms.Timer) sender;
						if( layoutChange) {
							setLayout();
							layoutChange = false;
						}
						if( stockPointList.Count > 0 && isDynamicUpdate ) {
							if( isAutoScroll ) {
								bool refresh = false;
//								for( int i=0; i<resetXScaleSpeed; i++) {
									if( KeepWithinScale()) {
										timer.Interval = 20;
										SetCommonXScale();
										dataGraph.AxisChange();
										dataGraph.Refresh();
										refresh = true;
									} else {
										timer.Interval = 20;
									}
//								}
								if( !refresh) {
									SetCommonXScale();
									dataGraph.AxisChange();
									dataGraph.Refresh();
								}
							}
						}
					}
				}
			} catch( Exception ex) {
				log.Notice( ex.ToString());
			}
		}
		
		public bool ShowPriceGraph {
			get { return showPriceGraph; }
			set { showPriceGraph = value; }
		}
		
		public StrategySupportInterface StrategyForTrades {
			get { return strategyForTrades; }
			set { strategyForTrades = value; }
		}
		
		public bool IsDynamicUpdate {
			get { return isDynamicUpdate; }
			set { isDynamicUpdate = value; }
		}
		
		
		public bool ProcessKeys(Keys keyData) {
			bool blnProcess = false;
			if( keyData == Keys.Up ) {
				strategyForTrades.Position.Signal = 1;
				blnProcess = true;
			}
			if( keyData == Keys.Down ) {
				strategyForTrades.Position.Signal = -1;
				blnProcess = true;
			}
			if( keyData == Keys.Right) {
				strategyForTrades.Position.Signal = - strategyForTrades.Position.Signal;
				blnProcess = true;
			}
			if( keyData == Keys.Insert || keyData == Keys.D0 || keyData == Keys.NumPad0 ) {
				// Go flat.
				strategyForTrades.Position.Signal = 0;
				blnProcess = true;
			}
			return blnProcess;
		}
		
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (ProcessKeys(keyData) == true) {
				return true;
			} else {
				return base.ProcessCmdKey(ref msg, keyData);
			}
		}
		
		// TODO: Move the TopMost setting to the form
//		void CheckBoxOnTopCheckStateChanged(object sender, EventArgs e)
//		{
//			if( checkBoxOnTop.Checked) {
//				TopMost = true;
//			} else {
//				TopMost = false;
//			}
//		}
		
		
		void ButtonVolumeTestClick(object sender, EventArgs e)
		{
		}
		
		void AudioNotifyCheckStateChanged(object sender, EventArgs e)
		{
			if( audioNotify.Checked) {
				isAudioNotify = true;
//				// For volume test.
//				AudioNotify( Audio.RisingVolume);
//				Thread.Sleep(5000);
//				AudioNotify( Audio.IntervalChime);
			} else {
				isAudioNotify = false;
			}
		}
		
		internal Point _menuClickPt;
		
		void DataGraphContextMenuBuilder(ZedGraphControl sender, ContextMenuStrip menuStrip, Point mousePt, ZedGraph.ZedGraphControl.ContextMenuObjectState objState)
		{
			
			_menuClickPt = this.PointToClient( Control.MousePosition );
			GraphPane pane = dataGraph.MasterPane.FindPane( _menuClickPt );
			
			try { 
				ToolStripMenuItem item;
				if( IsDynamicUpdate) {
					// create a new menu item
					item = new ToolStripMenuItem();
					// This is the user-defined Tag so you can find this menu item later if necessary
					item.Name = "zoom_last";
					item.Tag = "zoom_last";
					// This is the text that will show up in the menu
					item.Text = "Zoom to Last";
					// Add a handler that will respond when that menu item is selected
					item.Click += new System.EventHandler( MenuClick_ZoomToLast );
					// Add the menu item to the menu
					menuStrip.Items.Add( item );
				}
				if( IsDynamicUpdate) {
					// create a new menu item
					item = new ToolStripMenuItem();
					// This is the user-defined Tag so you can find this menu item later if necessary
					item.Name = "auto_scroll";
					item.Tag = "auto_scroll";
					// This is the text that will show up in the menu
					if( isAutoScroll) {
						item.Text = "Disable Auto Scroll";
					} else {
						item.Text = "Enable Auto Scroll";
					}
					// Add a handler that will respond when that menu item is selected
					item.Click += new System.EventHandler( ToggleAutoScroll );
					// Add the menu item to the menu
					menuStrip.Items.Add( item );
				}
				
				// create a new menu item
				item = new ToolStripMenuItem();
				// This is the user-defined Tag so you can find this menu item later if necessary
				item.Name = "compact_mode";
				item.Tag = "compact_mode";
				// This is the text that will show up in the menu
				if( isCompactMode) {
					item.Text = "Disable Compact";
				} else {
					item.Text = "Enable Compact";
				}
				// Add a handler that will respond when that menu item is selected
				item.Click += new System.EventHandler( ToggleCompactMode );
				// Add the menu item to the menu
				menuStrip.Items.Add( item );
			} catch( Exception ex) {
				log.Notice(ex.ToString());
			}
		}
		
		void ToggleAutoScroll(object sender, EventArgs e) {
			isAutoScroll = !isAutoScroll;	
		}
			
		void ToggleCompactMode(object sender, EventArgs e) {
			isCompactMode = !isCompactMode;	
			for( int i=0; i<dataGraph.MasterPane.PaneList.Count; i++) {
				GraphPane myPaneT = dataGraph.MasterPane.PaneList[i];
				if( isCompactMode) {
					myPaneT.YAxis.Title.IsVisible = false;
					myPaneT.YAxis.Scale.IsVisible = false;
					myPaneT.YAxis.MinSpace = 10;
				} else {
					myPaneT.YAxis.Title.IsVisible = true;
					myPaneT.YAxis.Scale.IsVisible = true;
					myPaneT.YAxis.MinSpace = 80;
				}
			}
		}
		
		public ChartType ChartType {
			get { return chartType; }
			set { chartType = value; }
		}
		
		Bars updateBars = null;
		public Bars UpdateBars {
			get { return updateBars; }
			set { updateBars = value; }
		}

		Bars displayBars;
		public Bars DisplayBars {
			get { return displayBars; }
			set { displayBars = value; }
		}
		
		Bars chartBars;
		public Bars ChartBars {
			get { return chartBars; }
			set { chartBars = value; }
		}

		
		public Interval IntervalChartDisplay {
			get { return intervalChartDisplay; }
			set { intervalChartDisplay = value; }
		}
		
		public Interval IntervalChartBar {
			get { return intervalChartBar; }
			set { intervalChartBar = value; }
		}
		
		public Interval IntervalChartUpdate {
			get { return intervalChartUpdate; }
			set { intervalChartUpdate = value; }
		}
        
		public string Symbol {
			get { return symbol.Symbol; }
			set { symbol = Factory.Symbol.LookupSymbol(value); }
		}
		
		public StockPointList StockPointList {
			get { return stockPointList; }
		}
	}
}
