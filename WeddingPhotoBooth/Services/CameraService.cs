using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WeddingPhotoBooth.Services;

public class CameraService
{
    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private const int SW_RESTORE = 9;

    public async Task<bool> TriggerCaptureAsync(string processName)
    {
        processName = processName.Replace(".exe", "").Trim();

        var process = Process.GetProcessesByName(processName).FirstOrDefault();

        if (process == null || process.MainWindowHandle == IntPtr.Zero)
            return false;

        ShowWindow(process.MainWindowHandle, SW_RESTORE);
        SetForegroundWindow(process.MainWindowHandle);

        // Kurze Pause, damit das Fenster wirklich aktiv wird
        await Task.Delay(250);

        SendKeys.SendWait("{ENTER}");

        await Task.Delay(300);
        return true;
    }
}