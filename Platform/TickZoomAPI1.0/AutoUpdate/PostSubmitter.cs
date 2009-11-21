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
 * Date: 6/30/2009
 * Time: 5:40 PM
 * <http://www.tickzoom.org/wiki/Licenses>.
 */
#endregion

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace TickZoom.Api
{
	/// <summary>
	/// Submits post data to a url.
	/// </summary>
	internal class PostSubmitter
	{
		string downloadDirectory = @"c:\TickZoom\";
		string contentDisposition;
		string fullFileName;
		string baseName;
		string extension;
		string rootName;
		string fileVersion;
		string fileName;
		BackgroundWorker backgroundWorker;
		
		internal string ContentDisposition {
			get { return contentDisposition; }
			set { contentDisposition = value; }
		}
		
		internal string FullFileName {
			get { return fullFileName; }
			set { fullFileName = value; }
		}
		
		internal string BaseName {
			get { return baseName; }
			set { baseName = value; }
		}
		
		public string Extension {
			get { return extension; }
			set { extension = value; }
		}
		
		internal string RootName {
			get { return rootName; }
		}
		
		public string FileVersion {
			get { return fileVersion; }
			set { fileVersion = value; }
		}
		
		internal string FileName {
			get { return fileName; }
		}
		
		internal string DownloadDirectory {
			get { return downloadDirectory; }
			set { downloadDirectory = value + @"\"; }
		}
		
		/// <summary>
		/// determines what type of post to perform.
		/// </summary>
		internal enum PostTypeEnum
		{
			/// <summary>
			/// Does a get against the source.
			/// </summary>
			Get,
			/// <summary>
			/// Does a post against the source.
			/// </summary>
			Post
		}
		
		private string m_url=string.Empty;
		private NameValueCollection m_values=new NameValueCollection();
		private PostTypeEnum m_type=PostTypeEnum.Get;
		/// <summary>
		/// Default constructor.
		/// </summary>
		internal PostSubmitter()
		{
		}
		
		/// <summary>
		/// Constructor that accepts a url as a parameter
		/// </summary>
		/// <param name="url">The url where the post will be submitted to.</param>
		internal PostSubmitter(string url):this()
		{
			m_url=url;
		}
		
		/// <summary>
		/// Constructor allowing the setting of the url and items to post.
		/// </summary>
		/// <param name="url">the url for the post.</param>
		/// <param name="values">The values for the post.</param>
		internal PostSubmitter(string url, NameValueCollection values):this(url)
		{
			m_values=values;
		}
		
		/// <summary>
		/// Gets or sets the url to submit the post to.
		/// </summary>
		internal string Url
		{
			get
				{
					return m_url;
				}
		set
				{
					m_url=value;
				}
		}
		/// <summary>
		/// Gets or sets the name value collection of items to post.
		/// </summary>
		internal NameValueCollection PostItems
		{
			get
			{
				return m_values;
			}
			set
			{
				m_values=value;
			}
		}
		/// <summary>
		/// Gets or sets the type of action to perform against the url.
		/// </summary>
		internal PostTypeEnum Type
		{
		get
		{
		return m_type;
		}
		set
		{
		m_type=value;
		}
		}
		/// <summary>
		/// Posts the supplied data to specified url.
		/// </summary>
		/// <returns>a string containing the result of the post.</returns>
		internal string Post()
		{
			Log log = Factory.Log.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
			this.m_type=PostSubmitter.PostTypeEnum.Post;
			StringBuilder parameters=new StringBuilder();
			for (int i=0;i < m_values.Count;i++)
			{
				EncodeAndAddItem(ref parameters,m_values.GetKey(i),m_values[i]);
			}
			log.Debug("Post to " + m_url + " with parameters = " + System.Web.HttpUtility.UrlDecode(parameters.ToString()));
			string result=PostData(m_url,parameters.ToString());
			return result;
		}
		
		private bool CancelPending {
			get { return backgroundWorker !=null && backgroundWorker.CancellationPending; }
		}
			
		/// <summary>
		/// Posts data to a specified url. Note that this assumes that you have already url encoded the post data.
		/// </summary>
		/// <param name="postData">The data to post.</param>
		/// <param name="url">the url to post to.</param>
		/// <returns>Returns the result of the post.</returns>
		private string PostData(string url, string postData)
		{
			HttpWebRequest request=null;
			if (m_type==PostTypeEnum.Post)
			{
				Uri uri = new Uri(url);
				request = (HttpWebRequest) WebRequest.Create(uri);
				request.Method = "POST";
				request.ContentType = "application/x-www-form-urlencoded";
				request.ContentLength = postData.Length;
				using(Stream writeStream = request.GetRequestStream())
				{
					UTF8Encoding encoding = new UTF8Encoding();
					byte[] bytes = encoding.GetBytes(postData);
					writeStream.Write(bytes, 0, bytes.Length);
				}
			}
			else
			{
				Uri uri = new Uri(url + "?" + postData);
				request = (HttpWebRequest) WebRequest.Create(uri);
				request.Method = "GET";
			}
			
			string result=string.Empty;
			using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
			{
				contentDisposition = response.Headers.Get("Content-Disposition");
							
				using (Stream responseStream = response.GetResponseStream())
				{
					if( "application/octet-stream".Equals(response.ContentType)) {
				
						string[] contentDisp = contentDisposition.Split( new char[] { ';' } );
						string[] fileNameValue = contentDisp[1].Split( new char[] { '=' } );
						fullFileName = fileNameValue[1].Replace("\"","");
						fullFileName = fullFileName.Trim();
						int lastDot = fullFileName.LastIndexOf('.');
						baseName = fullFileName.Substring(0,lastDot);
						extension = fullFileName.Substring(lastDot+1);
						string[] baseNameParts = baseName.Split( new char[] { '-' } );
						rootName = baseNameParts[0];
						fileVersion = baseNameParts[1];
						fileName = rootName + "." + extension;
						long final = response.ContentLength;

						byte[] buffer = new byte[0x10000];
						int bytes;
						Directory.CreateDirectory(downloadDirectory);
						string tempFileName = fullFileName+"_temp";
						long current = 0;
						string text = "Downloading " + fullFileName;
						using( FileStream fileStream = new FileStream(downloadDirectory+tempFileName, FileMode.Create)) {
							responseStream.ReadTimeout = 10000;
							while ((bytes = responseStream.Read(buffer, 0, buffer.Length)) > 0) {
								if( CancelPending ) {
									result = "Download was interrupted.";
									break;
								}
								fileStream.Write(buffer, 0, bytes);
								current += bytes;
					    		if( backgroundWorker!=null && !backgroundWorker.CancellationPending) {
									backgroundWorker.ReportProgress(0, new ProgressImpl(text,current,final));
								}
							}
						}
						if( result == null || result.Length == 0) {
			    			File.Delete(downloadDirectory+fullFileName);
			    			File.Move(downloadDirectory+tempFileName,downloadDirectory+fullFileName);
						}
					} else {
						using (StreamReader readStream = new StreamReader (responseStream, Encoding.UTF8))
						{
							result = readStream.ReadToEnd();
						}
						return result;
					}
				}
			}
			return result;
		}
		/// <summary>
		/// Encodes an item and ads it to the string.
		/// </summary>
		/// <param name="baseRequest">The previously encoded data.</param>
		/// <param name="dataItem">The data to encode.</param>
		/// <returns>A string containing the old data and the previously encoded data.</returns>
		private void EncodeAndAddItem(ref StringBuilder baseRequest, string key, string dataItem)
		{
			if (baseRequest==null)
			{
				baseRequest=new StringBuilder();
			}
			if (baseRequest.Length!=0)
			{
				baseRequest.Append("&");
			}
			baseRequest.Append(key);
			baseRequest.Append("=");
			baseRequest.Append(System.Web.HttpUtility.UrlEncode(dataItem));
		}
		
		public BackgroundWorker BackgroundWorker {
			get { return backgroundWorker; }
			set { backgroundWorker = value; }
		}
	}
	
}			
