using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamViewer.QuickSupport.Integration
{
    using System.Diagnostics;
    using System.IO;
    using System.Management.Instrumentation;
    using System.Threading;

    using NLog;

    using TeamViewer.QuickSupport.Integration.Properties;

    using TestStack.White;
    using TestStack.White.UIItems;
    using TestStack.White.UIItems.Finders;

    public class Automator
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        Application app;

        public static Process GetRunningProcess()
        {
            return Process.GetProcessesByName("TeamViewer").FirstOrDefault();
        }

        public string GetInstallationPath()
        {
            var x86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            var path = Path.Combine(x86, "TeamViewer\\TeamViewer.exe");

            if (File.Exists(path))
            {
                return path;
            }

            return null;
        }

        public string AlternativePathToTeamViewer { get; set; }

        public AuthInfo GetInfo()
        {
            var process = GetRunningProcess();
            if (process == null)
            {
                var path = GetInstallationPath();
                if (path == null)
                {
                    if (string.IsNullOrEmpty(AlternativePathToTeamViewer))
                    {
                        throw new FileNotFoundException(Resources.TeamViewerNotFound);
                    }
                    if (!File.Exists(AlternativePathToTeamViewer))
                    {
                        throw new FileNotFoundException(Resources.QuickSupportModuleIsMissing);
                    }
                    path = AlternativePathToTeamViewer;
                }

                app = Application.Launch(new ProcessStartInfo(path));
            }
            else
            {
                app = Application.Attach(process);
            }
            
            var authInfo = FindRepeat("Auth info", () =>
            {
                foreach (var window in app.GetWindows())
                {
                    //20098 automation id for ID
                    var id = window.Get<TextBox>(SearchCriteria.ByAutomationId("20098")).Text;
                    if (string.IsNullOrEmpty(id))
                    {
                        continue;
                    }
                    id = id.Replace(" ", "");
                    if (id == "-")
                    {
                        return null;
                    }

                    //20099 automation id for Password
                    var pass = window.Get<TextBox>(SearchCriteria.ByAutomationId("20099")).Text;
                    if (string.IsNullOrEmpty(pass))
                    {
                        continue;
                    }
                    if (pass == "-")
                    {
                        return null;
                    }

                    return new AuthInfo(id, pass);
                }

                return null;
            });

            return authInfo;
        }

        public static T FindRepeat<T>(string msg, Func<T> findFunc, TimeSpan? timeout = null) where T : class
        {
            if (!timeout.HasValue)
            {
                timeout = TimeSpan.FromSeconds(30);
            }
            
            var startTime = DateTime.Now;
            T res = null;
            while (DateTime.Now - startTime < timeout)
            {
                try
                {
                    res = findFunc();
                    if (res != null)
                    {
                        break;
                    }
                }
                catch (ApplicationException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Logger.Error(msg + "\n" + e);
                }
                Thread.Sleep(300);
            }
            if (res == null)
            {
                throw new InstanceNotFoundException(msg);
            }
            return res;
        }
    }
}
