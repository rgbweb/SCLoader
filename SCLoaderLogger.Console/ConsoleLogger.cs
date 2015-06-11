using SCLoaderShared.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCLoaderLogger.Console
{

    [Export(typeof(ILogger))]
    public class ConsoleLogger : ILogger
    {

        string ILogger.LoggerName
        {
            get
            {
                return "ConsoleLogger";
            }
        }

        void ILogger.Initialize()
        {
            // Nothing to initialize here
        }

        void ILogger.LogVerbose(string message, params object[] args)
        {

            System.Console.WriteLine(string.Format(message, args));

        }

        void ILogger.LogInformation(string message, params object[] args)
        {

            var prevColor = System.Console.ForegroundColor;

            System.Console.ForegroundColor = ConsoleColor.White;

            System.Console.WriteLine(string.Format(message, args));

            System.Console.ForegroundColor = prevColor;

        }

        void ILogger.LogException(string message, Exception exception, params object[] args)
        {

            var prevColor = System.Console.ForegroundColor;

            System.Console.ForegroundColor = ConsoleColor.Red;

            System.Console.WriteLine(string.Format(message, args));
            System.Console.WriteLine(exception.ToString());

            System.Console.ForegroundColor = prevColor;

        }

    }
}
