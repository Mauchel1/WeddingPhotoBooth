using System;
using System.IO.Ports;
using System.Threading;

public class NanoController : IDisposable
{
    private readonly SerialPort _port;

    private NanoController(SerialPort port)
    {
        _port = port;
    }

    public static NanoController? Connect()
    {
        foreach (string portName in SerialPort.GetPortNames())
        {
            try
            {
                SerialPort port = new SerialPort(portName, 115200);

                port.NewLine = "\n";
                port.ReadTimeout = 1000;
                port.WriteTimeout = 1000;

                port.Open();

                // Nano startet nach dem Öffnen neu
                Thread.Sleep(2000);

                // Empfangspuffer leeren
                port.DiscardInBuffer();

                // Ping senden
                port.WriteLine("PING");

                Thread.Sleep(500);


                while (port.BytesToRead > 0)
                {
                    string response = port.ReadLine().Trim();
                    System.Diagnostics.Debug.WriteLine($"Antwort: {response}");

                    if (response == "PHOTOBOX_NANO")
                    {
                        return new NanoController(port);
                    }
                }

                port.Close();
            }
            catch
            {
                // Falls geöffnet, wieder schließen
                System.Diagnostics.Debug.WriteLine("Fehler beim Öffnen des Ports.");
            }
        }

        return null;
    }

    public bool RelayOn()
    {
        _port.WriteLine("RELAY ON");
        Thread.Sleep(500);

        return _port.ReadLine().Trim() == "OK";
    }

    public bool RelayOff()
    {
        _port.WriteLine("RELAY OFF");
        Thread.Sleep(500);

        return _port.ReadLine().Trim() == "OK";
    }

    public void Dispose()
    {
        if (_port != null)
        {
            if (_port.IsOpen)
                _port.Close();

            _port.Dispose();
        }
    }
}