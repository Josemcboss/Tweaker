using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace ComputereMonitor;

public class Info
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
	private class MEMORYSTATUSEX
	{
		public uint dwLength;

		public uint dwMemoryLoad;

		public ulong ullTotalPhys;

		public ulong ullAvailPhys;

		public ulong ullTotalPageFile;

		public ulong ullAvailPageFile;

		public ulong ullTotalVirtual;

		public ulong ullAvailVirtual;

		public ulong ullAvailExtendedVirtual;

		public MEMORYSTATUSEX()
		{
			dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
		}
	}

	public class commomObjInfos
	{
		public float vid { get; set; }

		public float fanSpeed { get; set; }

		public float currentRefreshRate { get; set; }

		public float utilization { get; set; }

		public float powerDraw { get; set; }

		public float temperature { get; set; }

		public int sensorid { get; set; }

		public string name { get; set; }

		public string serialNum { get; set; }
	}

	public class CPUInfos : commomObjInfos
	{
	}

	public class MemInfos : commomObjInfos
	{
		public float used;

		public float total;

		public float free;
	}

	public class MemLayoutInfos : commomObjInfos
	{
		public float size { get; set; }
	}

	public class GraphicsInfos : commomObjInfos
	{
		public float memCurrentRefreshRate { get; set; }

		public float memUtilization { get; set; }
	}

	public class diskLayoutInfos : commomObjInfos
	{
		public float size;
	}

	public class SysInfo
	{
		public int pid = Process.GetCurrentProcess().Id;

		public CPUInfos cpu { get; set; } = new CPUInfos();

		public MemInfos mem { get; set; } = new MemInfos();

		public List<MemLayoutInfos> memLayout { get; set; } = new List<MemLayoutInfos>();

		public List<GraphicsInfos> graphics { get; set; } = new List<GraphicsInfos>();

		public List<CPUInfos> cpuLayout { get; set; } = new List<CPUInfos>();

		public List<diskLayoutInfos> diskLayout { get; set; } = new List<diskLayoutInfos>();
	}

	private int proc_index;

	private int core_index;

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern bool GlobalMemoryStatusEx([In][Out] MEMORYSTATUSEX lpBuffer);

	public SysInfo GetSysInfo(out string OutStr)
	{
		OutStr = "";
		SysInfo sysInfo = new SysInfo();
		MEMORYSTATUSEX mEMORYSTATUSEX = new MEMORYSTATUSEX();
		if (GlobalMemoryStatusEx(mEMORYSTATUSEX))
		{
			sysInfo.mem.total = mEMORYSTATUSEX.ullTotalPhys;
			sysInfo.mem.free = mEMORYSTATUSEX.ullAvailPhys;
			sysInfo.mem.used = sysInfo.mem.total - sysInfo.mem.free;
			double num = sysInfo.mem.used / sysInfo.mem.total * 100f;
			sysInfo.mem.utilization = (float)num;
		}
		CPUIDSDK pSDK = Program.pSDK;
		Dictionary<uint, object> dictionary = new Dictionary<uint, object>();
		dictionary[4u] = sysInfo.cpuLayout;
		dictionary[32u] = sysInfo.graphics;
		dictionary[16u] = sysInfo.diskLayout;
		dictionary[2048u] = sysInfo.memLayout;
		Dictionary<uint, object> dictionary2 = new Dictionary<uint, object>();
		dictionary2[4u] = new CPUInfos();
		dictionary2[32u] = new GraphicsInfos();
		dictionary2[16u] = new diskLayoutInfos();
		dictionary2[2048u] = new MemLayoutInfos();
		Dictionary<uint, string> dictionary3 = new Dictionary<uint, string>();
		dictionary3[4096u] = "vid";
		dictionary3[8192u] = "temperature";
		dictionary3[12288u] = "fanSpeed";
		dictionary3[61440u] = "currentRefreshRate";
		dictionary3[57344u] = "utilization";
		dictionary3[20480u] = "powerDraw";
		int numberOfDevices = pSDK.GetNumberOfDevices();
		for (int i = 0; i < numberOfDevices; i++)
		{
			Program.pSDK.GetDeviceName(i);
			uint deviceClass = (uint)pSDK.GetDeviceClass(i);
			dictionary.ContainsKey(deviceClass);
			if (!dictionary.ContainsKey(deviceClass))
			{
				continue;
			}
			dynamic val = dictionary[deviceClass];
			dynamic val2 = dictionary2[deviceClass];
			object obj = Activator.CreateInstance((Type)(object)val2.GetType());
			if (obj == null)
			{
				continue;
			}
			string deviceName = Program.pSDK.GetDeviceName(i);
			PropertyInfo property = obj.GetType().GetProperty("name");
			if (property != null && property.CanWrite)
			{
				property.SetValue(obj, deviceName);
			}
			foreach (KeyValuePair<uint, string> item in dictionary3)
			{
				int key = (int)item.Key;
				getSensorValue(i, key, obj, item.Value);
			}
			string deviceSerialNumber = pSDK.GetDeviceSerialNumber(i);
			PropertyInfo property2 = obj.GetType().GetProperty("serialNum");
			if (property2 != null && property2.CanWrite)
			{
				property2.SetValue(obj, deviceSerialNumber);
			}
			dynamic method = val.GetType().GetMethod("Add");
			if (method != null)
			{
				method.Invoke(val, new object[1] { obj });
			}
		}
		int count = sysInfo.memLayout.Count;
		int numberOfMemoryDevices = pSDK.GetNumberOfMemoryDevices();
		List<float> list = new List<float>();
		for (int i = 0; i < numberOfMemoryDevices; i++)
		{
			int _size = 0;
			int _total_width = 0;
			int _data_width = 0;
			int _speed = 0;
			string _szFormat = "SODIMM";
			string _szDesignation = "";
			string _szType = "";
			pSDK.GetDeviceName(i);
			pSDK.GetMemoryDeviceInfos(i, ref _size, ref _szFormat);
			pSDK.GetMemoryDeviceInfosExt(i, ref _szDesignation, ref _szType, ref _total_width, ref _data_width, ref _speed);
			if (_size > 0)
			{
				list.Add(_size);
			}
		}
		int num2 = Math.Min(count, list.Count);
		for (int i = 0; i < num2; i++)
		{
			MemLayoutInfos memLayoutInfos = sysInfo.memLayout[i];
			PropertyInfo property3 = memLayoutInfos.GetType().GetProperty("size");
			if (property3 != null && property3.CanWrite)
			{
				int num3 = (int)list[i];
				property3.SetValue(memLayoutInfos, num3);
			}
		}
		string text = JsonConvert.SerializeObject(sysInfo, Formatting.Indented);
		Console.WriteLine(22 + text);
		OutStr = text;
		return sysInfo;
		static void getSensorValue(int device_indexProps, int _sensor_type, dynamic commonObj, string text2)
		{
			int num4 = 0;
			int _sensor_id = 0;
			int _raw_value = 0;
			string _szName = "";
			float _value = 0f;
			float _min_value = 0f;
			float _max_value = 0f;
			int numberOfSensors = Program.pSDK.GetNumberOfSensors(device_indexProps, _sensor_type);
			for (num4 = 0; num4 < numberOfSensors; num4++)
			{
				Program.pSDK.GetSensorInfos(device_indexProps, num4, _sensor_type, ref _sensor_id, ref _szName, ref _raw_value, ref _value, ref _min_value, ref _max_value);
				if (Program.pSDK.IS_F_DEFINED(_value))
				{
					PropertyInfo propertyInfo = commonObj.GetType().GetProperty(text2);
					if (propertyInfo != null && propertyInfo.CanWrite)
					{
						propertyInfo.SetValue(commonObj, _value);
					}
					PropertyInfo propertyInfo2 = commonObj.GetType().GetProperty("sensorid");
					if (propertyInfo2 != null && propertyInfo2.CanWrite)
					{
						propertyInfo2.SetValue(commonObj, _sensor_id);
					}
				}
			}
		}
	}
}
