using System.Diagnostics;
using System.IO;
using WeddingPhotoBooth.Models;

namespace WeddingPhotoBooth.Services;

public class SessionService
{
    private readonly Settings _settings;

    public PhotoSession CurrentSession { get; private set; }

    public SessionService(Settings settings)
    {
        _settings = settings;
        CurrentSession = CreateNewSession(true);
    }

    public PhotoSession CreateNewSession(bool fake=false)
    {
        string folder = Path.Combine(
            _settings.SaveDirectory,
            DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));

        if (!fake) {          
            Directory.CreateDirectory(folder);
        }

        CurrentSession = new PhotoSession
        {
            SessionFolder = folder
        };

        return CurrentSession;
    }

    public void AddPhoto(string sourceFile)
    {

        if (!File.Exists(sourceFile))
            return;
        
        string fileName = Path.GetFileName(sourceFile);

        string destination =
            Path.Combine(
                CurrentSession.SessionFolder,
                fileName);



        File.Move(sourceFile, destination);

        CurrentSession.Photos.Add(destination);
        System.Diagnostics.Debug.WriteLine(
        $"Foto verschoben: {destination}");
    }
}