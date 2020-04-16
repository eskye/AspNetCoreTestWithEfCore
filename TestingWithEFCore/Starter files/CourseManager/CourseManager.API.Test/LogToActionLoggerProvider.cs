using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Microsoft.Extensions.Logging;

namespace CourseManager.API.Test
{
    internal class LogToActionLoggerProvider : ILoggerProvider
    {
        private readonly Action<string> _efCoreLogAction;
        private readonly LogLevel _logLevel;

        public LogToActionLoggerProvider(Action<string> efCoreLogAction, LogLevel logLevel = LogLevel.Information)
        {
            if (!Enum.IsDefined(typeof(LogLevel), logLevel))
                throw new InvalidEnumArgumentException(nameof(logLevel), (int) logLevel, typeof(LogLevel));
            _efCoreLogAction = efCoreLogAction ?? throw new ArgumentNullException(nameof(efCoreLogAction));
            _logLevel = logLevel;
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public ILogger CreateLogger(string categoryName)
        {
           return new EfCoreLogger(_efCoreLogAction, _logLevel);
        }
    }
}
