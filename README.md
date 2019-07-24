# New Document# TeamViewer.QuickSupport.Integration
TeamViewer QuickSupport Integration for .net applications.
<a href="https://www.nuget.org/packages/TeamViewer.QuickSupport.Integration/">Nuget Package</a>


## Sample use:
### Code configuration
```csharp
try
{
    var automator = new TeamViewer.QuickSupport.Integration.Automator();
    var isTeamViewerInstalled = automator.GetInstallationPath() != null;
    if (!isTeamViewerInstalled)
    {
        var quickSupportDownloader = new TeamViewer.QuickSupport.Integration.QuickSupportDownloader();
        //you can specify alternative path to QuickSupport
        //quickSupportDownloader.DownloadUrl = @"http://yourdomain.com/somepath/TeamViewerQS.exe"

        var isQuickSupportDownloaded = System.IO.File.Exists(quickSupportDownloader.DownloadPath);
        if (!isQuickSupportDownloaded)
        {
            //QuickSupport is not downloaded, so download it
            quickSupportDownloader.Update();
        }
        else
        {
            //if it's downloaded try to update. it uses Etag from Http Response to check if file changed
            UpdateQuickSupportInBackground(quickSupportDownloader);
        }
        //Switch deafult path to QuickSupport path
        automator.AlternativePathToTeamViewer = quickSupportDownloader.DownloadPath;
    }
    var info = automator.GetInfo();
    //success
    //info.ID and info.Password
}
catch (Exception ex)
{
  //oops something goes wrong
}

private static void UpdateQuickSupportInBackground(QuickSupportDownloader quickSupportDownloader)
{
    Task.Factory.StartNew(() =>
     {
         try
         {
             quickSupportDownloader.Update();
         }
         catch (Exception updateEx)
         {
             Debug.WriteLine("Failed to update QuickSupport: " + updateEx);
         }
     });
}
```

## Check if TeamViewer is running:
### Code configuration
```csharp
var process = Automator.GetRunningProcess();
if (process != null)
{
  //TeamViewer is running
}
else
{
  //TeamViewr not is running
}
```

## Known problems
### Exception at --> automator.GetInfo();
Usually, it's occurs when TeamViewer is running as admin, and your app is running as user.

More information in [Issue](https://github.com/ihtfw/TeamViewer.QuickSupport.Integration/issues/1)



### Running within a Windows Service
When a service is running under the system group it is assigned a Session ID of 0. When running under Session ID 0, services cannot interact with the user's desktop or display windows in their current session. This prevents the library from launching Team Viewer and TestStack.White from reading Team Viewer's form info.

The best way around this is to use [this](https://www.codeproject.com/Articles/35773/Subverting-Vista-UAC-in-Both-and-bit-Archite) example to launch a small exe that runs the example above and uses named pipes to pass the info back to the service.

The main file needed is [ApplicationLoader.cs](/Examples/ApplicationLoader.cs) and using it looks like this:

```csharp
// spawns a child process in the current logged in user's context to bypass Windows Service restrictions
// on interacting with the desktop
bool didApplicationStart = ApplicationLoader.StartProcessAndBypassUAC(pathToExe, out ApplicationLoader.PROCESS_INFORMATION procInfo);
```