﻿#region Copyright
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
 * Date: 11/24/2009
 * Time: 5:57 PM
 * <http://www.tickzoom.org/wiki/Licenses>.
 */
#endregion

using System;
using TickZoom.Api;
using log4net;
using log4net.Appender;

namespace TickZoom.Logging
{
    /// <summary>
    /// My log4net Rolling file appender.
    /// </summary>
    public class FileAppender : log4net.Appender.FileAppender
    {
//        public FileAppender()
//        {
//            // set the log level
////            LogManager.GetRepository().Threshold = LogManager.GetRepository().LevelMap[logLevel];
//        }

        public override string File
        {
            get
            {
                return base.File;
            }

            set
            {
                try
                {
                    // get the log directory
                    string logDirectory = Factory.Settings["AppDataFolder"];

                    // get the log file name from the config file.
                    string logFileName = value.Replace("AppDataFolder",logDirectory);

                    base.File = logFileName;
                }
                catch (Exception)
                {
                    base.File = value;
                }
            }
        }
    }
}