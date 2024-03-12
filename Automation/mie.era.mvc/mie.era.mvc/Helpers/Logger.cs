using log4net;
using System.Diagnostics;
using System.Reflection;

namespace mie.era.mvc.Helpers
{
    public class MyLogger
    {
        private readonly ILog _logger;

        public MyLogger()
        {
            // Initialize log4net with the configuration file
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));
            _logger = LogManager.GetLogger(typeof(MyLogger));
        }

        public void Log(string logString)
        {
            string methodName = GetCallingMethodName();
            _logger.Info($"[{methodName}] - {logString}");
        }

        public void LogWithElapsedTime(string logString, int elapsedTime)
        {
            string methodName = GetCallingMethodName();
            _logger.Info($"[{methodName}] - {logString} (Elapsed Time: {elapsedTime} seconds)");
        }

        private string GetCallingMethodName()
        {
            MethodBase method = new StackFrame(2).GetMethod();
            return $"{method.DeclaringType?.FullName}.{method.Name}";
        }
    }
}
