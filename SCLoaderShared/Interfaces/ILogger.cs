using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCLoaderShared.Interfaces
{
    public interface ILogger
    {

        void LogVerbose(string message, params object[] args);

        //void VerboseInline(string message, params object[] args);

        void LogException(string message, Exception exception, params object[] args);

    }
}
