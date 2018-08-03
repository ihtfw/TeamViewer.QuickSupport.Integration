# TeamViewer.QuickSupport.Integration
TeamViewer QuickSupport Integration for .net applications.
<a href="https://www.nuget.org/packages/TeamViewer.QuickSupport.Integration/">Nuget Package</a>


##Sample use:
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

##Check if TeamViewer is running:
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




