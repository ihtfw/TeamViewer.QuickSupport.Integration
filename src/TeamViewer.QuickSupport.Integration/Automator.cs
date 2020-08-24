using System;
using System.Linq;
using System.Threading;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using TeamViewer.QuickSupport.Integration.Exceptions;

namespace TeamViewer.QuickSupport.Integration
{
    using System.Diagnostics;
    using System.IO;

    public class Automator
    {
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

        public bool AllowToKillTeamViewer { get; set; } = true;

        public string AlternativePathToTeamViewer { get; set; }

        public AuthInfo GetInfo()
        {
            var app = StartTeamViewer();
            using var automation = new UIA3Automation();

            var topLevelWindows = app.GetAllTopLevelWindows(automation);
            if (AllowToKillTeamViewer && !topLevelWindows.Any() && !app.HasExited)
            {
                app.Kill();
                app = StartTeamViewer();

                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(1000);
                    topLevelWindows = app.GetAllTopLevelWindows(automation);
                    if (topLevelWindows.Any())
                    {
                        break;
                    }
                }
            }

            if (!topLevelWindows.Any())
            {
                throw new AutomatorException("Failed to get main window");
            }

            foreach (var window in topLevelWindows)
            {
                //20098 automation id for ID
                var idTextBox = GetIDTextBox(window);

                if (idTextBox == null)
                {
                    var panel = window.GetByNameOrDefault("Navigation");
                    if (panel == null)
                        continue;
                        
                    var button = panel.GetByNameOrDefault("Удалённое управление", "Удаленное управление", "Remote Control", "Віддалене керування", "Controllo remoto")
                                 ?? panel.GetByIdOrDefault("2");
                    if (button == null)
                    {
                        continue;
                    }
                    button.Click();

                    idTextBox = GetIDTextBox(window);
                    if (idTextBox == null)
                    {
                        throw new AutomatorException("Failed to find TextBox with ID");
                    }
                }

                var id = idTextBox.Text;
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
                var passwordTextBox = GetPasswordTextBox(window);
                if (passwordTextBox == null)
                {
                    continue;
                }

                var pass = passwordTextBox.Text;
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
        }

        private TextBox GetPasswordTextBox(Window window)
        {
            var names = new[] { "ПАРОЛЬ", "PASSWORD" };
            var descendants = window.FindAllDescendants(f => f.ByControlType(ControlType.Edit));
            foreach (var descendant in descendants.Where(d => !string.IsNullOrEmpty(d.Name)))
            {
                if (names.Contains(descendant.Name.ToUpper()))
                {
                    return descendant.AsTextBox();
                }
            }

            foreach (var descendant in descendants.Where(d => !string.IsNullOrEmpty(d.AutomationId)))
            {
                if (descendant.AutomationId == "20099")
                {
                    return descendant.AsTextBox();
                }
            }

            return null;
        }
        private TextBox GetIDTextBox(Window window)
        {
            var names = new[] {"ВАШ ID", "YOUR ID", "IL TUO ID"};
            var descendants = window.FindAllDescendants(f => f.ByControlType(ControlType.Edit));
            foreach (var descendant in descendants.Where(d => !string.IsNullOrEmpty(d.Name)))
            {
                if (names.Contains(descendant.Name.ToUpper()))
                {
                    return descendant.AsTextBox();
                }
            }
            
            foreach (var descendant in descendants.Where(d => !string.IsNullOrEmpty(d.AutomationId)))
            {
                if (descendant.AutomationId == "20098")
                {
                    return descendant.AsTextBox();
                }
            }

            return null;
        }

        private Application StartTeamViewer()
        {
            var process = GetRunningProcess();
            if (process != null)
            {
                return Application.Attach(process);
            }

            var path = GetInstallationPath();
            if (path == null)
            {
                if (string.IsNullOrEmpty(AlternativePathToTeamViewer))
                {
                    throw new FileNotFoundException("TeamViewer is not installed and no path for QuickSupport module is provided");
                }
                if (!File.Exists(AlternativePathToTeamViewer))
                {
                    throw new FileNotFoundException("QuickSupport module not found");
                }
                path = AlternativePathToTeamViewer;
            }

            return Application.Launch(new ProcessStartInfo(path)
            {
                Arguments = "--dre"
            });
        }
    }
}