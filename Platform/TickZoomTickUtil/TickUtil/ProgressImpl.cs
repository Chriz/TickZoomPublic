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
using TickZoom.Api;

namespace TickZoom.TickUtil
{
	/// <summary>
	/// Description of Progress.
	/// </summary>
	public struct ProgressImpl : Progress
	{
		string text; // this is just an example member, replace it with your own struct members!
		Int64 current;
		Int64 final;
		
		public ProgressImpl( string text, Int64 current, Int64 final) {
			this.text = text;
			this.current = current;
			this.final = final;
		}
		public string Text {
			get { return text; }
		}
		
		public long Current {
			get { return current; }
		}
		
		public long Final {
			get { return final; }
		}
	}
}
