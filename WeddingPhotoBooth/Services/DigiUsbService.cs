/*using LibUsbDotNet;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using System;
using System.Net.Sockets;

namespace WeddingPhotoBooth.Services;

public class DigiUsbService : IDigiUsbService
{
    private const int VendorId = 0x16C0;
    private const int ProductId = 0x05DF;

    private UsbContext? _context;
    private IUsbDevice? _device;

    public bool Connect()
    {
        try
        {
            _context = new UsbContext();

            System.Diagnostics.Debug.WriteLine("USB Kontext erfolgreich erstellt");

            foreach (var usbDevice in _context.List())
            {
                System.Diagnostics.Debug.WriteLine(
                    $"VID:{usbDevice.VendorId:X4} PID:{usbDevice.ProductId:X4}"
                );

                if (usbDevice.VendorId == VendorId &&
                    usbDevice.ProductId == ProductId)
                    {
                        _device = usbDevice;
                        break;
                    }
            }

            if (_device == null)
            {
                System.Diagnostics.Debug.WriteLine("DigiUSB nicht gefunden");
                return false;
            }
            _device.Open();

            System.Diagnostics.Debug.WriteLine("DigiUSB verbunden");
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.ToString());
            return false;
        }
    }

    public bool Send(byte command)
    {
        if (_device == null)
            return false;

        try
        {

            var packet = new UsbSetupPacket(
                0x20,   // Host -> Device | Class | Interface
                0x09,   // SET_REPORT
                0, // Output Report
                0,
                1
            );

            byte[] data =
            {
                (byte)command
            };

            int transferred = _device.ControlTransfer(
                packet,
                data,
                0,
                data.Length
            );

             int transferred = _device.ControlTransfer(
                 packet,
                 dummy,
                 0,
                 dummy.Length
             );

            //bool result = transferred == buffer.Length;
            
            if (transferred > 0)
            {
                System.Diagnostics.Debug.WriteLine("Gesendet");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Fehler beim Senden");
            }

            System.Diagnostics.Debug.WriteLine(
                $"DigiUSB gesendet: {command}, transferred={transferred}"
            );
            return true;
            //return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            return false;
        }

    }

    public void Disconnect()
    {
        _device = null;

        _context?.Dispose();
        _context = null;
    }
}*/