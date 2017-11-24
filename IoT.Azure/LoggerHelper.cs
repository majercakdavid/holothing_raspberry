using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Azure
{
    public class LoggerHelper
    {
        public LoggerHelper() { }
        public static string log;
        public static void Log(Exception ex)
        {
            string innerException = "";
            string stackTrace = "";
            while (ex.InnerException != null) innerException += ex.InnerException;
            // maybe use wile loop as well.
            stackTrace = ex.StackTrace;
            log += stackTrace;
        }
    }
}
