using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using System.IO;
using System.Diagnostics;
using System.Reflection;


namespace WBid.WBidiPad.iOS.Log4Net
{
    /// <summary>
    /// Provides Logging functionality configurable through config file.
    /// </summary>

    public class Logger : ILogger
    {

        #region Private variables
        //Private variable declaration.
        private readonly log4net.ILog _logger;

        //Text separator string. This will be used in forming exception message details parsing.
        private const string Textseparator = "*********************************************";
        #endregion

        #region Construction
        /// <summary>
        /// Initializes new instance of log object
        /// </summary>
        public Logger(log4net.ILog log)
        {
            _logger = log;
        }
        #endregion

        #region Public properties

        #region IsDebugEnabled to identify DebugEnable status
        /// <summary>
        /// Provide DEBUG log enable status for conditional logging.
        /// </summary>
        public bool IsDebugEnabled
        {
            get { return _logger.IsDebugEnabled; }
        }
        #endregion

        #region IsErrorEnabled
        /// <summary>
        /// Provide ERROR log enable status for conditional logging.
        /// </summary>
        public bool IsErrorEnabled
        {
            get { return _logger.IsErrorEnabled; }
        }
        #endregion

        #region IsExceptionEnabled
        /// <summary>
        /// Provide Exceptions log enable status for conditional logging.
        /// </summary>
        public bool IsExceptionEnabled
        {
            // For now set the same as IsErrorEnabled
            get { return _logger.IsErrorEnabled; }
        }
        #endregion

        #region IsFatalEnabled
        /// <summary>
        /// Provide FATAL log enable status for conditional logging.
        /// </summary>
        public bool IsFatalEnabled
        {
            get { return _logger.IsFatalEnabled; }
        }
        #endregion

        #region IsInfoEnabled
        /// <summary>
        /// Provide INFO log enable status for conditional logging.
        /// </summary>
        public bool IsInfoEnabled
        {
            get { return _logger.IsInfoEnabled; }
        }
        #endregion

        #region IsWarnEnabled
        /// <summary>
        /// Provide WARN log enable status for conditional logging.
        /// </summary>
        public bool IsWarnEnabled
        {
            get { return _logger.IsWarnEnabled; }
        }
        #endregion

        #endregion

        #region Public methods
        #region ILogger Members
        #region Debug(object message) to log given Debug message string
        /// <summary>
        /// Log DEBUG message to configured adapters.
        /// </summary>
        /// <param name="message"></param>
        public void Debug(object message)
        {
            PrepareSourceClassDetails();
            _logger.Debug(message);
        }
        #endregion

        #region Info(object message) to log given Info message string
        /// <summary>
        /// Log INFO message to configured adapters.
        /// </summary>
        /// <param name="message"></param>
        public void Info(object message)
        {
            PrepareSourceClassDetails();
            _logger.Info(message);
        }
        #endregion

        #region Warn(object message) to log given Warn message string
        /// <summary>
        /// Log WARN message to configured adapters.
        /// </summary>
        /// <param name="message"></param>
        public void Warn(object message)
        {
            PrepareSourceClassDetails();
            _logger.Warn(message);
        }
        #endregion

        #region Error(object message) to log given Error message string
        /// <summary>
        /// Log ERROR message to configured adapters.
        /// </summary>
        /// <param name="message"></param>
        public void Error(object message)
        {
            PrepareSourceClassDetails();
            _logger.Error(message);
        }
        #endregion

        #region Exception(object message) to log given Exception message string
        /// <summary>
        /// Log Exception message to configured adapters.
        /// </summary>
        /// <param name="message"></param>
        public string Exception(object exceptionSource)
        {
            PrepareSourceClassDetails();

            // Extract Exeption message.
            Exception exception = exceptionSource as Exception;

            if (exception != null)
            {
                Exception InnerException = exception.InnerException;
                string message = exception.Message;
                string where = exception.StackTrace.Split(new string[] { " at " }, 2, StringSplitOptions.None)[1];
                string source = exception.Source;

                if (InnerException != null)
                {
                    if (InnerException.Message != null)
                    {
                        message = InnerException.Message;
                    }

                    if (InnerException.StackTrace != null)
                    {
                        where = InnerException.StackTrace.Split(new string[] { " at " }, 2, StringSplitOptions.None)[1];
                    }

                    source = InnerException.Source;

                    if (InnerException.InnerException != null)
                    {
                        if (InnerException.InnerException.Message != null)
                        {
                            message += " -> " + InnerException.InnerException.Message;
                        }

                        if (InnerException.InnerException.StackTrace != null)
                        {
                            where += "\r\n\r\n -> " + InnerException.InnerException.StackTrace.Split(new string[] { " at " },
                                2, StringSplitOptions.None)[1];
                        }

                        if (InnerException.InnerException.Source != null)
                        {
                            source += " -> " + InnerException.InnerException.Source;
                        }
                    }
                }

                if (where.Length > 1024)
                {
                    where = where.Substring(0, 1024);
                }

                _logger.Error(string.Format("\r\nDetails\t: {0}\r\nWhere\t: {1}\r\n\rSource\t: {2}\r\n*******************************************************************\r\n", message, where, source));

                // EmailHelper emailHelper = new EmailHelper();
                //emailHelper.SendErrorLogToAdmin("<br /> WbidMax Error Details. <br /><br />Error  :  " + message + "<br /><br /> Where  :  " + where + "<br /><br />Source   :  " + source + "<br /><br /> Date  :" + DateTime.Now + "<br /><br /> Data  :"+bidData, adminusermail, adminuserpassword, mailalias);
                var emailmessagecontent = "<br /> WbidMax Error Details. <br /><br />Error  :  " + message + "<br /><br /> Where  :  " + where + "<br /><br />Source   :  " + source + "<br /><br /> Date  :" + DateTime.Now;
                return emailmessagecontent;


            }
            else
            {

                _logger.Error("\r\nException Error: Unable access contents of exception.");
                return string.Empty;
            }
        }
        #endregion

        #region Fatal(object message) to log given Fatal message string
        /// <summary>
        /// Log FATAL message to configured adapters.
        /// </summary>
        /// <param name="message"></param>
        public void Fatal(object message)
        {
            PrepareSourceClassDetails();
            _logger.Fatal(message);
        }
        #endregion


        #region ShowCommonMessage
        /// <summary>
        /// Shows the common message.
        /// </summary>
        public void ShowCommonMessage()
        {
            var expectedFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WBidMax\\Logs\\WBidLog " + DateTime.Today.ToString("yyyyMMdd") + ".log");

            //MessageBox.Show(
            //    string.Format("Error: The application experienced an issue it could not handle and cannot complete the requested operation.\r\n\r\nYou should contact your systems admin and send the log file located at:\r\n{0}",
            //    expectedFile.Trim()), "Operation Error in Appraisal Prep Desktop");
        }
        #endregion

        #region DisplaySpecificMessage
        /// <summary>
        /// Shows the common message.
        /// </summary>
        public void DisplaySpecificMessage(string message)
        {
            var expectedFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WBidMax\\Logs\\WBidLog " + DateTime.Today.ToString("yyyyMMdd") + ".log");

            //MessageBox.Show(
            //    string.Format("{0}.\r\n\r\nYou should contact your systems admin and send the log file located at:\r\n{1}",
            //    message, expectedFile.Trim()), "Operation Error in Appraisal Prep Desktop");
        }
        #endregion
        #endregion

        #endregion

        #region Private methods

        #region ParseException(Exception exception) to parse exception object entire hierarchy
        /// <summary>
        /// Parse given exception object to prepare formatted log message.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns>string</returns>
        private string ParseException(Exception exception)
        {
            StringBuilder exceptionInfo = new StringBuilder();
            Exception currentException;	// Temp variable to hold InnerException object during the loop.

            try
            {
                currentException = exception;
                // Count variable to track the number of exceptions in the chain.
                int exceptionCount = 1;

                // Loop through each exception class in the chain of exception objects.
                do
                {
                    // Write title information for the exception object.
                    exceptionInfo.AppendFormat("{0}{0}{1}) Exception Information{0}{2}", Environment.NewLine, exceptionCount, Textseparator);
                    exceptionInfo.AppendFormat("{0}Exception Type: {1}", Environment.NewLine, currentException.GetType().FullName);

                    // Loop through the public properties of the exception object and record their value.
                    PropertyInfo[] publicProperties = currentException.GetType().GetProperties();

                    foreach (PropertyInfo propInfo in publicProperties)
                    {
                        // Here InnerException or StackTrace is not capturing. And This information is 
                        // captured later in the process.
                        if (propInfo.Name != "InnerException" && propInfo.Name != "StackTrace")
                        {
                            if (propInfo.GetValue(currentException, null) == null)
                            {
                                exceptionInfo.AppendFormat("{0}{1}: NULL", Environment.NewLine, propInfo.Name);
                            }
                            else
                            {
                                // writing the ToString() value of the property.
                                exceptionInfo.AppendFormat("{0}{1}: {2}", Environment.NewLine, propInfo.Name, propInfo.GetValue(currentException, null));
                            }
                        }
                    }

                    // Record the StackTrace with separate label.
                    if (currentException.StackTrace != null)
                    {
                        exceptionInfo.AppendFormat("{0}{0}StackTrace Information{0}{1}", Environment.NewLine, null);
                        exceptionInfo.AppendFormat("{0}{1}", Environment.NewLine, currentException.StackTrace);
                    }

                    // Reset the temp exception object and iterate the counter.
                    currentException = currentException.InnerException;
                    exceptionCount++;
                } while (currentException != null);

                exceptionInfo.AppendFormat("{0}{1}", Environment.NewLine, Textseparator);

            }
            catch (Exception exobj)
            {
                _logger.Fatal(exobj.ToString());
            }
            return exceptionInfo.ToString();
        }
        #endregion

        #region PrepareSourceClassDetails
        /// <summary>
        /// Prepares Source class details to indicate origin of logger method call.
        /// </summary>
        private static void PrepareSourceClassDetails()
        {
            StackTrace stack = new StackTrace();
            StackFrame frame = stack.GetFrame(2);
            //This property will be get used in Log4Net config file as part of PatternLayout definition
            //to get source class details from which logger method has been invoked.
            log4net.ThreadContext.Properties["SourceClass"] = frame.GetMethod().Name;
            //frame.GetMethod().DeclaringType + "." + frame.GetMethod().Name;
        }
        #endregion

        #endregion
    }
}