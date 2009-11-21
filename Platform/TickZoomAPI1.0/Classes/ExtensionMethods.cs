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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace TickZoom.Api
{
	public static class ExtensionMethods {
		public const double ConvertDouble = 1000000000D;
		public const int PricePrecision = 9;

		public static long ToLong( this double value) {
			return Convert.ToInt64(Math.Round(value,PricePrecision)*ConvertDouble);
		}
		
		public static double ToDouble( this long value) {
			return Math.Round(value / ConvertDouble, PricePrecision );
		}
		
		public static double Round( this double value) {
			return Math.Round(value, PricePrecision);
		}
		
		private static readonly object encodeLocker = new object();
		public static ulong ToULong(this string symbol) {
			lock( encodeLocker) {
				ulong result = Factory.Symbol.LookupSymbol(symbol).BinaryIdentifier;
				return result;
			}
		}
		
		#region base64
		public static readonly int Combinations = 26 + 26 + 10 + 2;
		
		public static ulong SymbolToULong(string symbol) {
			symbol = symbol.Substring(0,Math.Min(symbol.Length,9));
			char[] chars = symbol.ToCharArray();
			ulong result = 0;
			for( int i=chars.Length-1; i>=0;i--) {
				char chr = chars[i];
				ulong digit = (ulong) CharToInt(chr);
				ulong pow = (ulong) Math.Pow(Combinations,i);
				result += digit * pow ;
			}
			return result;
		}
			
		public static string ULongToSymbol(ulong symbol) {
			char[] chars = new char[10];
			int index=0;
			for( int i=0; i<chars.Length;i++) {
				ulong pow = (ulong) Math.Pow(Combinations,i);
				if( pow == 0) {
					System.Diagnostics.Debugger.Break();
				}
				ulong remain = (ulong) (symbol / pow);
				ulong ulDigit = remain % (ulong) Combinations;
				int digit = (int) ulDigit;
				if( digit > 0) {
					chars[index] = IntToChar(digit);
					index ++;
				}
			}
			return new string(chars,0,index);
		}
		
		public static int CharToInt(char chr) {
			int result = 0;
			if( chr >= 'A' && chr <= 'Z') {
				result = chr - 'A' + 1;
			} else if( chr >= 'a' && chr <= 'z') {
				result = chr - 'a' + 27;
			} else if( chr >= '0' && chr <= '9') {
				result = chr - '0' + 53;
			} else if( chr == '/') {
				result = 63;
			} else {
				throw new FormatException("Invalid symbol character: '"+chr+"'");
			}
			return result;
		}
		
		public static char IntToChar(int digit) {
			char result;
			if( digit < 27) {
				result = (char) ('A' + digit - 1);
			} else if( digit < 53) {
				result = (char) ('a' + digit - 27);
			} else if( digit < 63) {
				result = (char) ('0' + digit - 53);
			} else if( digit == 63) {
				result = '/';
			} else {
				throw new FormatException("Invalid symbol digit: '"+digit+"'");
			}
			return result;
		}
		#endregion
		
		private static readonly object decodeLocker = new object();
		private static readonly ASCIIEncoding decoder = new ASCIIEncoding();
		public static string ToSymbol(this ulong symbol) {
			lock( decodeLocker) {
				string result = Factory.Symbol.LookupSymbol(symbol).Symbol;
				return result;
			}
		}
	}
}
