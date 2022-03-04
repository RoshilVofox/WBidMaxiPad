using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using System.Diagnostics;

namespace WBid.WBidiPad.iOS.Log4Net
{
    /// <summary>
    /// Provides instance of a Logger class with respective dependencies. 
    /// </summary>
    public sealed class LoggerFactory
    {
        static LoggerFactory()
        {
           // log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("Log4net.Config"));
        }

        #region Public methods
        #region GetLogger() to PLATO Logger component
        /// <summary>
        /// Returns a new instance of the Logger class.
        /// </summary>
        public static Logger GetLogger()
        {
            //Getting Caller class type by using stack trace details.
            //This type information will be used in getting logger component from log4net.
            StackTrace stack = new StackTrace();
            StackFrame frame = stack.GetFrame(1);
            Logger logger = new Logger(log4net.LogManager.GetLogger(frame.GetMethod().DeclaringType));
            return logger;
        }
        #endregion
        #endregion
    }
}