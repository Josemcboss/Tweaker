using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace Tweaker.Services
{
    public class AioScreenService : IDisposable
    {
        private const ushort VendorId = 20785;  // 0x5131
        private const ushort ProductId = 8199;  // 0x2007

        private SafeFileHandle? _deviceHandle;
        private bool _isDisposed;

        // Background send loop state
        private Thread? _sendThread;
        private bool _isRunning;
        private readonly object _lock = new object();

        // Cached telemetry values
        private float _cpuTemp;
        private float _cpuLoad;
        private float _cpuPower = 45f;
        private float _cpuFreq = 3600f;
        private float _cpuVolt = 1.2f;
        private float _gpuTemp;
        private float _gpuLoad;
        private float _gpuPower = 150f;
        private float _gpuFreq = 1500f;
        private float _ramUsagePercent;
        private float _fanSpeedRpm;
        private float _pumpSpeedRpm;

        [StructLayout(LayoutKind.Sequential)]
        private struct SP_DEVICE_INTERFACE_DATA
        {
            public int cbSize;
            public Guid InterfaceClassGuid;
            public int Flags;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public int cbSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string DevicePath;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HIDD_ATTRIBUTES
        {
            public int Size;
            public ushort VendorID;
            public ushort ProductID;
            public ushort VersionNumber;
        }

        [DllImport("hid.dll", SetLastError = true)]
        private static extern void HidD_GetHidGuid(out Guid hidGuid);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetupDiGetClassDevs(ref Guid classGuid, string? enumerator, IntPtr hwndParent, int flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, IntPtr deviceInfoData, ref Guid interfaceClassGuid, int memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetupDiGetDeviceInterfaceDetail(
            IntPtr deviceInfoSet, 
            ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, 
            IntPtr deviceInterfaceDetailData, 
            int deviceInterfaceDetailDataSize, 
            ref int requiredSize, 
            IntPtr deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true, EntryPoint = "SetupDiGetDeviceInterfaceDetail")]
        private static extern bool SetupDiGetDeviceInterfaceDetail(
            IntPtr deviceInfoSet, 
            ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, 
            ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData, 
            int deviceInterfaceDetailDataSize, 
            ref int requiredSize, 
            IntPtr deviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

        [DllImport("hid.dll", SetLastError = true)]
        private static extern bool HidD_GetAttributes(SafeFileHandle hidDeviceObject, ref HIDD_ATTRIBUTES attributes);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_READ = 0x00000001;
        private const uint FILE_SHARE_WRITE = 0x00000002;
        private const uint OPEN_EXISTING = 3;

        public void Start()
        {
            lock (_lock)
            {
                if (_isRunning) return;
                _isRunning = true;
                _sendThread = new Thread(SendLoop)
                {
                    IsBackground = true,
                    Name = "AioScreenSendThread"
                };
                _sendThread.Start();
                System.Diagnostics.Debug.WriteLine("✅ AioScreenService: Hilo de envío (200ms) iniciado.");
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (!_isRunning) return;
                _isRunning = false;
            }
            if (_sendThread != null && _sendThread.IsAlive)
            {
                _sendThread.Join(500);
            }
            Disconnect();
        }

        public void UpdateValues(float cpuTemp, float cpuLoad, float cpuPower, float cpuFreq, float cpuVolt,
                                  float gpuTemp, float gpuLoad, float gpuPower, float gpuFreq,
                                  float ramUsagePercent, float fanSpeedRpm = 0, float pumpSpeedRpm = 0)
        {
            lock (_lock)
            {
                _cpuTemp = cpuTemp;
                _cpuLoad = cpuLoad;
                _cpuPower = cpuPower;
                _cpuFreq = cpuFreq;
                _cpuVolt = cpuVolt;
                _gpuTemp = gpuTemp;
                _gpuLoad = gpuLoad;
                _gpuPower = gpuPower;
                _gpuFreq = gpuFreq;
                _ramUsagePercent = ramUsagePercent;
                _fanSpeedRpm = fanSpeedRpm;
                _pumpSpeedRpm = pumpSpeedRpm;
            }
        }

        private void SendLoop()
        {
            while (true)
            {
                bool running;
                lock (_lock)
                {
                    running = _isRunning;
                }

                if (!running) break;

                try
                {
                    float cpuTemp, cpuLoad, cpuPower, cpuFreq, cpuVolt;
                    float gpuTemp, gpuLoad, gpuPower, gpuFreq;
                    float ramUsagePercent, fanSpeedRpm, pumpSpeedRpm;

                    lock (_lock)
                    {
                        cpuTemp = _cpuTemp;
                        cpuLoad = _cpuLoad;
                        cpuPower = _cpuPower;
                        cpuFreq = _cpuFreq;
                        cpuVolt = _cpuVolt;
                        gpuTemp = _gpuTemp;
                        gpuLoad = _gpuLoad;
                        gpuPower = _gpuPower;
                        gpuFreq = _gpuFreq;
                        ramUsagePercent = _ramUsagePercent;
                        fanSpeedRpm = _fanSpeedRpm;
                        pumpSpeedRpm = _pumpSpeedRpm;
                    }

                    SendTelemetry(
                        cpuTemp, cpuLoad, cpuPower, cpuFreq, cpuVolt,
                        gpuTemp, gpuLoad, gpuPower, gpuFreq,
                        ramUsagePercent, fanSpeedRpm, pumpSpeedRpm
                    );
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[AioScreenService] Error en bucle de envío: {ex.Message}");
                }

                Thread.Sleep(200); // Bucle rápido para evitar desconexiones / parpadeo de pantalla
            }
        }

        public bool Connect()
        {
            if (_deviceHandle != null && !_deviceHandle.IsInvalid)
            {
                return true;
            }

            try
            {
                Guid hidGuid;
                HidD_GetHidGuid(out hidGuid);

                IntPtr hDevInfo = SetupDiGetClassDevs(ref hidGuid, null, IntPtr.Zero, 0x10); // DIGCF_DEVICEINTERFACE | DIGCF_PRESENT
                if (hDevInfo == IntPtr.Zero || hDevInfo.ToInt64() == -1)
                {
                    return false;
                }

                SP_DEVICE_INTERFACE_DATA interfaceData = new SP_DEVICE_INTERFACE_DATA();
                interfaceData.cbSize = Marshal.SizeOf(interfaceData);

                int index = 0;
                while (SetupDiEnumDeviceInterfaces(hDevInfo, IntPtr.Zero, ref hidGuid, index++, ref interfaceData))
                {
                    int requiredSize = 0;
                    // First call to get the size
                    SetupDiGetDeviceInterfaceDetail(hDevInfo, ref interfaceData, IntPtr.Zero, 0, ref requiredSize, IntPtr.Zero);

                    SP_DEVICE_INTERFACE_DETAIL_DATA detailData = new SP_DEVICE_INTERFACE_DETAIL_DATA();
                    detailData.cbSize = IntPtr.Size == 8 ? 8 : 4 + Marshal.SystemDefaultCharSize;

                    if (SetupDiGetDeviceInterfaceDetail(hDevInfo, ref interfaceData, ref detailData, 256, ref requiredSize, IntPtr.Zero))
                    {
                        SafeFileHandle handle = CreateFile(detailData.DevicePath, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
                        if (!handle.IsInvalid)
                        {
                            HIDD_ATTRIBUTES attrs = new HIDD_ATTRIBUTES();
                            attrs.Size = Marshal.SizeOf(attrs);
                            if (HidD_GetAttributes(handle, ref attrs))
                            {
                                if (attrs.VendorID == VendorId && attrs.ProductID == ProductId)
                                {
                                    _deviceHandle = handle;
                                    SetupDiDestroyDeviceInfoList(hDevInfo);
                                    System.Diagnostics.Debug.WriteLine("✅ AioScreenService: Dispositivo LCD detectado y conectado.");
                                    return true;
                                }
                            }
                            handle.Close();
                        }
                    }
                }
                SetupDiDestroyDeviceInfoList(hDevInfo);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AioScreenService] Error al conectar: {ex.Message}");
            }

            return false;
        }

        public bool SendTelemetry(float cpuTemp, float cpuLoad, float cpuPower, float cpuFreq, float cpuVolt,
                                   float gpuTemp, float gpuLoad, float gpuPower, float gpuFreq,
                                   float ramUsagePercent, float fanSpeedRpm = 0, float pumpSpeedRpm = 0)
        {
            if (_deviceHandle == null || _deviceHandle.IsInvalid)
            {
                if (!Connect())
                {
                    return false;
                }
            }

            try
            {
                byte[] report = new byte[65];
                report[0] = 0; // Report ID
                report[1] = 0;
                report[2] = 1;
                report[3] = 2;

                // SendValueArray mappings
                int[] sendValues = new int[35];

                // CPU Temperature
                double cpuTempFloor = Math.Floor(cpuTemp);
                double cpuTempFraction = cpuTemp - cpuTempFloor;
                sendValues[0] = Convert.ToInt16(cpuTempFloor);
                sendValues[1] = Convert.ToInt16(cpuTempFraction * 100.0);
                sendValues[2] = 0; // Celsius

                // CPU Load
                sendValues[3] = Convert.ToInt16(Math.Floor(cpuLoad));

                // CPU Power
                double cpuPowerFloor = Math.Floor(cpuPower);
                sendValues[4] = Convert.ToInt16(cpuPowerFloor % 100.0);
                sendValues[5] = Convert.ToInt16((cpuPower - cpuPowerFloor) * 100.0);

                // CPU Frequency
                double cpuFreqFloor = Math.Floor(cpuFreq);
                sendValues[6] = Convert.ToInt16(cpuFreqFloor / 100.0);
                sendValues[7] = Convert.ToInt16(cpuFreqFloor % 100.0);

                // CPU Voltage
                double cpuVoltFloor = Math.Floor(cpuVolt);
                sendValues[8] = Convert.ToInt16(cpuVoltFloor);
                sendValues[9] = Convert.ToInt16((cpuVolt - cpuVoltFloor) * 100.0);

                // GPU Temperature
                double gpuTempFloor = Math.Floor(gpuTemp);
                double gpuTempFraction = gpuTemp - gpuTempFloor;
                sendValues[10] = Convert.ToInt16(gpuTempFloor);
                sendValues[11] = Convert.ToInt16(gpuTempFraction * 100.0);
                sendValues[12] = 0; // Celsius

                // GPU Load
                sendValues[13] = Convert.ToInt16(Math.Floor(gpuLoad));

                // GPU Power
                double gpuPowerFloor = Math.Floor(gpuPower);
                sendValues[14] = Convert.ToInt16(gpuPowerFloor % 100.0);
                sendValues[15] = Convert.ToInt16((gpuPower - gpuPowerFloor) * 100.0);

                // GPU Frequency
                double gpuFreqFloor = Math.Floor(gpuFreq);
                sendValues[16] = Convert.ToInt16(gpuFreqFloor / 100.0);
                sendValues[17] = Convert.ToInt16(gpuFreqFloor % 100.0);

                // Fan Speeds
                double fanSpeedFloor = Math.Floor(fanSpeedRpm);
                sendValues[18] = Convert.ToInt16(fanSpeedFloor / 100.0);
                sendValues[19] = Convert.ToInt16(fanSpeedFloor % 100.0);

                // Pump Speeds
                double pumpSpeedFloor = Math.Floor(pumpSpeedRpm);
                sendValues[20] = Convert.ToInt16(pumpSpeedFloor / 100.0);
                sendValues[21] = Convert.ToInt16(pumpSpeedFloor % 100.0);

                // Time fields
                var now = DateTime.Now;
                string yearStr = now.Year.ToString();
                sendValues[22] = Convert.ToInt16(yearStr.Substring(0, 2));
                sendValues[23] = Convert.ToInt16(yearStr.Substring(2, 2));
                sendValues[24] = now.Month;
                sendValues[25] = now.Day;
                sendValues[26] = now.Hour;
                sendValues[27] = now.Minute;
                sendValues[28] = now.Second;
                sendValues[29] = (int)now.DayOfWeek;

                // RAM Usage
                sendValues[30] = Convert.ToInt16(Math.Floor(ramUsagePercent));

                // High parts of Power
                sendValues[31] = Convert.ToInt16(cpuPowerFloor / 100.0);
                sendValues[32] = Convert.ToInt16(gpuPowerFloor / 100.0);
                sendValues[33] = Convert.ToInt16(gpuPowerFloor / 100.0);
                sendValues[34] = Convert.ToInt16(gpuPowerFloor / 100.0);

                // Copy to report
                for (int i = 0; i < 33; i++)
                {
                    report[4 + i] = (byte)sendValues[i];
                }

                // Write to HID device
                using (var stream = new FileStream(_deviceHandle!, FileAccess.Write, 65, false))
                {
                    stream.Write(report, 0, 65);
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AioScreenService] Error al enviar telemetría: {ex.Message}");
                Disconnect();
            }

            return false;
        }

        public void Disconnect()
        {
            if (_deviceHandle != null && !_deviceHandle.IsInvalid)
            {
                _deviceHandle.Close();
                _deviceHandle = null;
            }
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                Stop();
                _isDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        ~AioScreenService()
        {
            Dispose();
        }
    }
}
