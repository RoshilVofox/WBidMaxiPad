using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace WBid.WBidiPad.iOS.Log4Net
{
    /// <summary>
    /// Define methods for Logger functionality.
    /// </summary>

    public interface ILogger
    {

        #region Public properties

        #region IsDebugEnabled
        /// <summary>
        /// Provide DebugEnable status information for conditional logging.
        /// </summary>
        bool IsDebugEnabled { get; }
        #endregion

        #region IsInfoEnabled
        /// <summary>
        /// Provide InfoEnable status information for conditional logging.
        /// </summary>
        bool IsInfoEnabled { get; }
        #endregion

        #region IsWarnEnabled
        /// <summary>
        /// Provide WarnEnable status information for conditional logging.
        /// </summary>
        bool IsWarnEnabled { get; }
        #endregion

        #region IsErrorEnabled
        /// <summary>
        /// Provide ErrorEnable status information for conditional logging.
        /// </summary>
        bool IsErrorEnabled { get; }
        #endregion

        #region IsExceptionEnabled
        /// <summary>
        /// Provide ErrorEnable status information for conditional logging.
        /// </summary>
        bool IsExceptionEnabled { get; }
        #endregion

        #region IsFatalEnabled
        /// <summary>
        /// Provide FatalEnable status information for conditional logging.
        /// </summary>
        bool IsFatalEnabled { get; }
        #endregion

        #endregion

        #region Public methods

        #region Debug(object message)
        /// <summary>
        /// Log Debug message
        /// </summary>
        /// <param name="message"></param>
        void Debug(object message);
        #endregion

        #region Info(object message)
        /// <summary>
        /// Log Info message
        /// </summary>
        /// <param name="message"></param>
        void Info(object message);
        #endregion

        #region Warn(object message)
        /// <summary>
        /// Log Warn message
        /// </summary>
        /// <param name="message"></param>
        void Warn(object message);
        #endregion

        #region Error(object message)
        /// <summary>
        /// Log Error message
        /// </summary>
        /// <param name="message"></param>
        void Error(object message);
        #endregion

        #region Exception(object message)



        /// <summary>
        /// Log Error message
        /// </summary>
        /// <param name="message"></param>
        string Exception(object exceptionSource);
        #endregion

        #region Fatal(object message)
        /// <summary>
        /// Log Fatal message
        /// </summary>
        /// <param name="message"></param>
        void Fatal(object message);
        #endregion

        #region ShowCommonMessage
        /// <summary>
        /// Shows the common message.
        /// </summary>
        void ShowCommonMessage();
        #endregion

        #region DisplaySpecificMessage
        /// <summary>
        /// Shows the common message.
        /// </summary>
        void DisplaySpecificMessage(string message);
        #endregion
        #endregion
    }
}