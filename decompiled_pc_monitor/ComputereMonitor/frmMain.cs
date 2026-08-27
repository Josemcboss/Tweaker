using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using CyUSB;
using IWshRuntimeLibrary;

namespace ComputereMonitor;

public class frmMain : Form
{
	private List<int> ListSelectindex = new List<int>();

	private int CPUTempSelectIndex;

	private int CPUUsageSelectIndex;

	private int CPUPowerSelectIndex;

	private int CPUFrequenceSelectIndex;

	private int CPUVoltagesSelectIndex;

	private int GPUTempSelectIndex;

	private int GPUUsageSelectIndex;

	private int GPUPowerSelectIndex;

	private int GPUFrequenceSelectIndex;

	private int FansSelectIndex;

	private int WatarSelectIndex;

	public const int WM_SYSCOMMAND = 274;

	public const int SC_MOVE = 61456;

	public const int HTCAPTION = 2;

	private int[] SendValueArray = new int[40];

	private int[] ValueArray = new int[30];

	public static bool Connect_Copy;

	public static bool Connect_Flag;

	private int[] lcd_buf = new int[16];

	private CyHidDevice myHidDevice;

	private USBDeviceList usbDevices;

	private const int VID = 20785;

	private const int PID = 8199;

	private int xiaoshu;

	private int error_shandeng;

	private byte sendi;

	public double SendValue_CPUTemp;

	public double SendValue_CPUUsage;

	public double SendValue_CPUPower;

	public double SendValue_CPUFrequence;

	public double SendValue_CPUVoltages;

	public double SendValue_GPUTemp;

	public double SendValue_GPUUsage;

	public double SendValue_GPUPower;

	public double SendValue_GPUFrequence;

	public double SendValue_WaterCool;

	public double SendValue_Fans;

	private IContainer components;

	private NotifyIcon notifyIcon1;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem toolStripMenuItem1;

	private Label lab_CPUTemp;

	private ComboBox cmb_FansSelect;

	private Label lab_FANSRPM;

	private Label lab_CPUUsage;

	private System.Windows.Forms.Timer timer1;

	private ComboBox cmb_CPUUsage;

	private Label lab_Value3;

	private Label lab_Value2;

	private Label lab_Value1;

	private Panel panel8;

	private ComboBox cmb_CPUT;

	private Label label4;

	private Label label5;

	private Label label6;

	private Label label3;

	private Label label2;

	private Label label1;

	private Label label7;

	private Label label8;

	private ComboBox cmb_WaterCoolSelect;

	private Label lab_WaterCoolFans;

	private Label lab_Value4;

	private Panel panel1;

	private CheckBox cbx_ShowCentigrade;

	private CheckBox cbx_AutoStartEnable;

	private Label lab_PicWaterCoolFans;

	private Label lab_PicFansSpeed;

	private Label lab_PicGPUPower;

	private Label lab_PicCPUPower;

	private Label lab_PicCPUVoltages;

	private Label lab_PicGPUUSage;

	private Label lab_PicCPUUSage;

	private Label lab_PicGPUFre;

	private Label lab_PicCPUFre;

	private Label lab_PicGPUTemp;

	private Label lab_PicCPUTemp;

	private ComboBox cmb_CPUVoltages;

	private Label lab_CPUVoltages;

	private Label label16;

	private Label label17;

	private ComboBox cmb_CPUFrequence;

	private Label lab_CPUFrequence;

	private Label label13;

	private Label label14;

	private ComboBox cmb_CPUPower;

	private Label lab_CPUPower;

	private Label label10;

	private Label label11;

	private ComboBox cmb_GPUFrequence;

	private Label lab_GPUFrequecne;

	private Label label19;

	private Label label20;

	private ComboBox cmb_GPUPower;

	private Label lab_GPUPower;

	private Label label22;

	private Label label23;

	private ComboBox cmb_GPUUsage;

	private Label lab_GPUTemp;

	private Label lab_GPUUsage;

	private ComboBox cmb_GPUT;

	private Label label26;

	private Label label27;

	private Label label28;

	private Label label29;

	private Button btn_Close;

	private Button btn_Min;

	private Label lab_PicRAMUSage;

	private Label lab_MainBoardName;

	private Label label9;

	private Label lab_MainBoardUsage;

	private Label label12;

	[DllImport("user32.dll")]
	public static extern bool ReleaseCapture();

	[DllImport("user32.dll")]
	public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

	public frmMain()
	{
		InitializeComponent();
		USB_Init();
		base.WindowState = FormWindowState.Minimized;
		base.ShowInTaskbar = false;
		Hide();
		notifyIcon1.Visible = true;
		base.Icon = null;
	}

	public void LoadBGImage(string Path)
	{
		try
		{
			if (ClsMain.BGImagePath != Path)
			{
				Image backgroundImage = Image.FromFile(Path);
				panel8.BackgroundImage = backgroundImage;
			}
			ClsMain.BGImagePath = Path;
		}
		catch (Exception)
		{
		}
	}

	public void LoadControl()
	{
		try
		{
			cbx_AutoStartEnable.Checked = ClsMain.AutoStartEnable;
			cbx_ShowCentigrade.Checked = ClsMain.Showcentigrade_Flag;
			ClsMain.labvalue_PicRAMUsage = lab_PicRAMUSage;
			ClsMain.labvalue_CPUT = lab_PicCPUTemp;
			ClsMain.labvalue_CPUUsage = lab_PicCPUUSage;
			ClsMain.labvalue_CPUPower = lab_PicCPUPower;
			ClsMain.labvalue_CPUFrequence = lab_PicCPUFre;
			ClsMain.labvalue_CPUVoltages = lab_PicCPUVoltages;
			ClsMain.labvalue_GPUT = lab_PicGPUTemp;
			ClsMain.labvalue_GPUUsage = lab_PicGPUUSage;
			ClsMain.labvalue_GPUPower = lab_PicGPUPower;
			ClsMain.labvalue_GPUFrequence = lab_PicGPUFre;
			ClsMain.labvalue_FansRPM = lab_PicFansSpeed;
			ClsMain.labvalue_WatarRPM = lab_PicWaterCoolFans;
			ClsMain.labvalue_RAMName2 = lab_MainBoardName;
			ClsMain.labvalue_RAMUsage2 = lab_MainBoardUsage;
			ClsMain.labvalue_CPUT2 = lab_CPUTemp;
			ClsMain.labvalue_CPUUsage2 = lab_CPUUsage;
			ClsMain.labvalue_CPUPower2 = lab_CPUPower;
			ClsMain.labvalue_CPUFrequence2 = lab_CPUFrequence;
			ClsMain.labvalue_CPUVoltages2 = lab_CPUVoltages;
			ClsMain.labvalue_GPUT2 = lab_GPUTemp;
			ClsMain.labvalue_GPUUsage2 = lab_GPUUsage;
			ClsMain.labvalue_GPUPower2 = lab_GPUPower;
			ClsMain.labvalue_GPUFrequence2 = lab_GPUFrequecne;
			ClsMain.labvalue_FansRPM2 = lab_FANSRPM;
			ClsMain.labvalue_WatarRPM2 = lab_WaterCoolFans;
			ClsMain.cmbValue_CPUT = cmb_CPUT;
			ClsMain.cmbValue_CPUUsage = cmb_CPUUsage;
			ClsMain.cmbValue_CPUPower = cmb_CPUPower;
			ClsMain.cmbValue_CPUFrequence = cmb_CPUFrequence;
			ClsMain.cmbValue_CPUVoltages = cmb_CPUVoltages;
			ClsMain.cmbValue_GPUT = cmb_GPUT;
			ClsMain.cmbValue_GPUUsage = cmb_GPUUsage;
			ClsMain.cmbValue_GPUPower = cmb_GPUPower;
			ClsMain.cmbValue_GPUFrequence = cmb_GPUFrequence;
			ClsMain.cmbValue_FansRPM = cmb_FansSelect;
			ClsMain.cmbValue_WatarRPM = cmb_WaterCoolSelect;
		}
		catch (Exception)
		{
		}
	}

	public static bool SetMeStartByAutoStartPath(bool onOff, out string errMsg)
	{
		errMsg = string.Empty;
		try
		{
			string fileName = Process.GetCurrentProcess().MainModule.FileName;
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
			string fileName2 = Path.GetFileName(fileName);
			if (onOff)
			{
				ShortCutCreate(fileName2, fileName, folderPath, out errMsg);
			}
			else
			{
				ShortCutDelete(fileName, folderPath, out errMsg);
			}
			return true;
		}
		catch (Exception ex)
		{
			errMsg = ex.Message + ex.StackTrace;
			return false;
		}
	}

	public static List<string> GetDirectoryFileList(string target)
	{
		List<string> list = new List<string>();
		list.Clear();
		string[] files = Directory.GetFiles(target, "*.lnk");
		if (files == null || files.Length == 0)
		{
			return list;
		}
		for (int i = 0; i < files.Length; i++)
		{
			list.Add(files[i]);
		}
		return list;
	}

	public static string GetAppPathViaShortCut(string shortCutPath)
	{
		try
		{
			WshShell wshShell = (WshShell)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8")));
			return ((IWshShortcut)(dynamic)wshShell.CreateShortcut(shortCutPath)).TargetPath;
		}
		catch
		{
			return null;
		}
	}

	public static bool ShortCutExist(string path, string target)
	{
		bool result = false;
		foreach (string directoryFile in GetDirectoryFileList(target))
		{
			if (path == GetAppPathViaShortCut(directoryFile))
			{
				result = true;
			}
		}
		return result;
	}

	public static bool ShortCutDelete(string appPath, string sysAutoStartPath, out string errMsg)
	{
		bool result = false;
		errMsg = string.Empty;
		try
		{
			foreach (string directoryFile in GetDirectoryFileList(sysAutoStartPath))
			{
				if (appPath == GetAppPathViaShortCut(directoryFile))
				{
					File.Delete(directoryFile);
					result = true;
				}
			}
		}
		catch (Exception ex)
		{
			errMsg = ex.Message + ex.StackTrace;
			result = false;
		}
		return result;
	}

	public static bool ShortCutCreate(string name, string appPath, string sysAutoStartPath, out string errMsg)
	{
		errMsg = string.Empty;
		bool flag = false;
		try
		{
			if (ShortCutExist(appPath, sysAutoStartPath))
			{
				return false;
			}
			if (!Directory.Exists(sysAutoStartPath))
			{
				Directory.CreateDirectory(sysAutoStartPath);
			}
			string pathLink = Path.Combine(sysAutoStartPath, $"{name}.lnk");
			WshShell wshShell = (WshShell)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8")));
			IWshShortcut obj = (IWshShortcut)(dynamic)wshShell.CreateShortcut(pathLink);
			obj.TargetPath = appPath;
			obj.WorkingDirectory = Path.GetDirectoryName(appPath);
			obj.WindowStyle = 1;
			obj.Save();
			return true;
		}
		catch (Exception ex)
		{
			errMsg = ex.Message + ex.StackTrace;
			return false;
		}
	}

	private void frmMain_Load(object sender, EventArgs e)
	{
		string path = Application.StartupPath + "\\ico\\UI_DISConnect.png";
		LoadBGImage(path);
		base.MaximizeBox = false;
		if (ClsMain.AutoStartEnable)
		{
			string errMsg = "";
			SetMeStartByAutoStartPath(onOff: true, out errMsg);
		}
		ClsMain.GetParam();
		LoadControl();
		base.Visible = false;
		frmMain_Resize(null, null);
		lab_Value1.Select();
		new Thread(Thread_GetPCParam).Start();
		new Thread(Thread_Send).Start();
		ClsMain.SetAppendTxt("加载界面成功", "HT");
		notifyIcon1_DoubleClick(null, null);
		SetBtnStyle(btn_Close);
		SetBtnStyle(btn_Min);
	}

	private void frmMain_Resize(object sender, EventArgs e)
	{
		if (base.WindowState == FormWindowState.Minimized)
		{
			base.ShowInTaskbar = false;
			base.Visible = false;
			notifyIcon1.Visible = true;
		}
	}

	private void notifyIcon1_DoubleClick(object sender, EventArgs e)
	{
		base.Visible = true;
		base.WindowState = FormWindowState.Normal;
		Show();
	}

	private void toolStripMenuItem1_Click(object sender, EventArgs e)
	{
		Close();
	}

	public void Thread_GetPCParam()
	{
		int num = 0;
		while (true)
		{
			num++;
			try
			{
				GetPCParam();
			}
			catch (Exception ex)
			{
				ClsMain.SetAppendTxt("获取参数线程ERR:" + ex.Message, "HT");
			}
			Thread.Sleep(100);
			Application.DoEvents();
		}
	}

	public void GetPCParam()
	{
		string devicename = "";
		string sensorname = "";
		string s = "";
		int _raw_value = 0;
		float _min_value = 0f;
		float _max_value = 0f;
		int _sensor_id = 0;
		ClsMain.CPUT2 = new List<KeyValue>();
		ClsMain.CPUUsage2 = new List<KeyValue>();
		ClsMain.CPUPower2 = new List<KeyValue>();
		ClsMain.CPUFrequence2 = new List<KeyValue>();
		ClsMain.CPUVoltages2 = new List<KeyValue>();
		ClsMain.GPUT2 = new List<KeyValue>();
		ClsMain.GPUUsage2 = new List<KeyValue>();
		ClsMain.GPUPower2 = new List<KeyValue>();
		ClsMain.GPUFrequence2 = new List<KeyValue>();
		ClsMain.FansRPM2 = new List<KeyValue>();
		ClsMain.WatarRPM2 = new List<KeyValue>();
		ClsMain.Frequence2 = new List<KeyValue>();
		float _value = 0f;
		int numberOfDevices = Program.pSDK.GetNumberOfDevices();
		for (int i = 0; i < numberOfDevices; i++)
		{
			devicename = Program.pSDK.GetDeviceName(i);
			int numberOfSensors = Program.pSDK.GetNumberOfSensors(i, 4096);
			for (int j = 0; j < numberOfSensors; j++)
			{
				if (!Program.pSDK.GetSensorInfos(i, j, 4096, ref _sensor_id, ref sensorname, ref _raw_value, ref _value, ref _min_value, ref _max_value) || !(_value > 0f - CPUIDSDK.MAX_FLOAT))
				{
					continue;
				}
				s = Convert.ToString(Math.Round(_value, 2));
				if (sensorname.ToLower().Contains("cpu") || sensorname.ToLower().Contains("#") || sensorname.ToLower().Contains("vdd"))
				{
					if (ClsMain.CPUVoltages.Where((KeyValue A) => A.Key == devicename + "_" + sensorname).ToList().Count == 0)
					{
						KeyValue item = new KeyValue(devicename + "_" + sensorname, s);
						ClsMain.CPUVoltages.Add(item);
					}
					else
					{
						ClsMain.CPUVoltages.Where((KeyValue A) => A.Key == devicename + "_" + sensorname).ToList().ForEach(delegate(KeyValue B)
						{
							B.Value = s;
						});
					}
					KeyValue item2 = new KeyValue(devicename + "_" + sensorname, s);
					ClsMain.CPUVoltages2.Add(item2);
				}
				s += "  V";
			}
			numberOfSensors = Program.pSDK.GetNumberOfSensors(i, 8192);
			for (int j = 0; j < numberOfSensors; j++)
			{
				if (!Program.pSDK.GetSensorInfos(i, j, 8192, ref _sensor_id, ref sensorname, ref _raw_value, ref _value, ref _min_value, ref _max_value) || !Program.pSDK.IS_F_DEFINED(_value))
				{
					continue;
				}
				s = Convert.ToString(Math.Round(_value, 0));
				if (!ClsMain.Showcentigrade_Flag)
				{
					double c = Convert.ToDouble(s);
					s = ((int)Math.Floor(ClsMain.GetF(c))).ToString();
				}
				if (sensorname == "Package" || sensorname.ToLower().Contains("core") || sensorname.ToLower().Contains("#"))
				{
					if (ClsMain.CPUT.Where((KeyValue A) => A.Key == devicename + "_" + sensorname).ToList().Count == 0)
					{
						KeyValue item3 = new KeyValue(devicename + "_" + sensorname, s);
						ClsMain.CPUT.Add(item3);
					}
					else
					{
						ClsMain.CPUT.Where((KeyValue A) => A.Key == devicename + "_" + sensorname).ToList().ForEach(delegate(KeyValue B)
						{
							B.Value = s;
						});
					}
					KeyValue item4 = new KeyValue(devicename + "_" + sensorname, s);
					ClsMain.CPUT2.Add(item4);
				}
				if (sensorname.ToLower().Contains("gpu") || sensorname.ToLower().Contains("hot") || sensorname.ToLower().Contains("total"))
				{
					if (ClsMain.GPUT.Where((KeyValue A) => A.Key == devicename + "_" + sensorname).ToList().Count == 0)
					{
						KeyValue item5 = new KeyValue(devicename + "_" + sensorname, s);
						ClsMain.GPUT.Add(item5);
					}
					else
					{
						ClsMain.GPUT.Where((KeyValue A) => A.Key == devicename + "_" + sensorname).ToList().ForEach(delegate(KeyValue B)
						{
							B.Value = s;
						});
					}
					KeyValue item6 = new KeyValue(devicename + "_" + sensorname, s);
					ClsMain.GPUT2.Add(item6);
				}
				s += "  °C";
			}
			numberOfSensors = Program.pSDK.GetNumberOfSensors(i, 12288);
			for (int j = 0; j < numberOfSensors; j++)
			{
				if (!Program.pSDK.GetSensorInfos(i, j, 12288, ref _sensor_id, ref sensorname, ref _raw_value, ref _value, ref _min_value, ref _max_value) || !(Math.Round(_value, 0) >= 0.0))
				{
					continue;
				}
				s = Convert.ToString(Math.Round(_value, 0));
				_ = s == "0";
				if (ClsMain.FansRPM.Where((KeyValue A) => A.Key == sensorname).ToList().Count == 0)
				{
					KeyValue item7 = new KeyValue(sensorname, s);
					ClsMain.FansRPM.Add(item7);
				}
				else
				{
					ClsMain.FansRPM.Where((KeyValue A) => A.Key == sensorname).ToList().ForEach(delegate(KeyValue B)
					{
						B.Value = s;
					});
				}
				KeyValue item8 = new KeyValue(sensorname, s);
				ClsMain.FansRPM2.Add(item8);
				if (ClsMain.WatarRPM.Where((KeyValue A) => A.Key == sensorname).ToList().Count == 0)
				{
					KeyValue item9 = new KeyValue(sensorname, s);
					ClsMain.WatarRPM.Add(item9);
				}
				else
				{
					ClsMain.WatarRPM.Where((KeyValue A) => A.Key == sensorname).ToList().ForEach(delegate(KeyValue B)
					{
						B.Value = s;
					});
				}
				KeyValue item10 = new KeyValue(sensorname, s);
				ClsMain.WatarRPM2.Add(item10);
				s += "  RPM";
			}
			numberOfSensors = Program.pSDK.GetNumberOfSensors(i, 61440);
			for (int j = 0; j < numberOfSensors; j++)
			{
				if (!Program.pSDK.GetSensorInfos(i, j, 61440, ref _sensor_id, ref sensorname, ref _raw_value, ref _value, ref _min_value, ref _max_value) || !(_value > 0f))
				{
					continue;
				}
				s = Convert.ToString(Math.Round(_value, 1));
				s = Convert.ToDouble(s).ToString("F1");
				if (sensorname.ToLower().Contains("core") || sensorname.ToLower().Contains("#") || sensorname.ToLower().Contains("cpu"))
				{
					if (ClsMain.CPUFrequence.Where((KeyValue A) => A.Key == devicename + "_" + sensorname).ToList().Count == 0)
					{
						KeyValue item11 = new KeyValue(devicename + "_" + sensorname, s);
						ClsMain.CPUFrequence.Add(item11);
					}
					else
					{
						ClsMain.CPUFrequence.Where((KeyValue A) => A.Key == devicename + "_" + sensorname).ToList().ForEach(delegate(KeyValue B)
						{
							B.Value = s;
						});
					}
					KeyValue item12 = new KeyValue(devicename + "_" + sensorname, s);
					ClsMain.CPUFrequence2.Add(item12);
				}
				if (!sensorname.ToLower().Contains("video") && !sensorname.ToLower().Contains("memory") && !sensorname.ToLower().Contains("gpu") && !sensorname.ToLower().Contains("graphics"))
				{
					continue;
				}
				if (ClsMain.GPUFrequence.Where((KeyValue A) => A.Key == devicename + "_" + sensorname).ToList().Count == 0)
				{
					KeyValue item13 = new KeyValue(devicename + "_" + sensorname, s);
					ClsMain.GPUFrequence.Add(item13);
				}
				else
				{
					ClsMain.GPUFrequence.Where((KeyValue A) => A.Key == devicename + "_" + sensorname).ToList().ForEach(delegate(KeyValue B)
					{
						B.Value = s;
					});
				}
				KeyValue item14 = new KeyValue(devicename + "_" + sensorname, s);
				ClsMain.GPUFrequence2.Add(item14);
			}
			numberOfSensors = Program.pSDK.GetNumberOfSensors(i, 57344);
			for (int j = 0; j < numberOfSensors; j++)
			{
				if (!Program.pSDK.GetSensorInfos(i, j, 57344, ref _sensor_id, ref sensorname, ref _raw_value, ref _value, ref _min_value, ref _max_value) || !(Math.Round(_value, 0) >= 0.0))
				{
					continue;
				}
				s = Convert.ToString(Math.Round(_value, 0));
				if (sensorname.ToLower().Contains("system memory"))
				{
					ClsMain.MainBoardName = devicename;
					ClsMain.MainBoardUsage = Convert.ToInt16(s);
				}
				if (sensorname == "Package" || sensorname.ToLower().Contains("core") || sensorname.ToLower().Contains("processor") || sensorname.ToLower().Contains("cpu") || sensorname.ToLower().Contains("#") || sensorname.ToLower().Contains("package"))
				{
					if (ClsMain.CPUUsage.Where((KeyValue A) => A.Key == sensorname).ToList().Count == 0)
					{
						KeyValue item15 = new KeyValue(sensorname, s);
						ClsMain.CPUUsage.Add(item15);
					}
					else
					{
						ClsMain.CPUUsage.Where((KeyValue A) => A.Key == sensorname).ToList().ForEach(delegate(KeyValue B)
						{
							B.Value = s;
						});
					}
					KeyValue item16 = new KeyValue(sensorname, s);
					ClsMain.CPUUsage2.Add(item16);
				}
				if (sensorname.ToLower().Contains("gpu") || sensorname.ToLower().Contains("memory") || sensorname.ToLower().Contains("video") || sensorname.ToLower().Contains("3d"))
				{
					if (ClsMain.GPUUsage.Where((KeyValue A) => A.Key == sensorname).ToList().Count == 0)
					{
						KeyValue item17 = new KeyValue(sensorname, s);
						ClsMain.GPUUsage.Add(item17);
					}
					else
					{
						ClsMain.GPUUsage.Where((KeyValue A) => A.Key == sensorname).ToList().ForEach(delegate(KeyValue B)
						{
							B.Value = s;
						});
					}
					KeyValue item18 = new KeyValue(sensorname, s);
					ClsMain.GPUUsage2.Add(item18);
				}
				s += "  %";
			}
			numberOfSensors = Program.pSDK.GetNumberOfSensors(i, 20480);
			for (int j = 0; j < numberOfSensors; j++)
			{
				if (!Program.pSDK.GetSensorInfos(i, j, 20480, ref _sensor_id, ref sensorname, ref _raw_value, ref _value, ref _min_value, ref _max_value) || !(_value > 0f))
				{
					continue;
				}
				s = Convert.ToString(Math.Round(_value, 1));
				if (sensorname == "Package" || sensorname.ToLower().Contains("core") || sensorname.ToLower().Contains("cpu") || sensorname.ToLower().Contains("#") || sensorname.ToLower().Contains("package"))
				{
					if (ClsMain.CPUPower.Where((KeyValue A) => A.Key == sensorname).ToList().Count == 0)
					{
						KeyValue item19 = new KeyValue(sensorname, s);
						ClsMain.CPUPower.Add(item19);
					}
					else
					{
						ClsMain.CPUPower.Where((KeyValue A) => A.Key == sensorname).ToList().ForEach(delegate(KeyValue B)
						{
							B.Value = s;
						});
					}
					KeyValue item20 = new KeyValue(sensorname, s);
					ClsMain.CPUPower2.Add(item20);
				}
				if (sensorname.ToLower().Contains("gpu") || sensorname.ToLower().Contains("memory") || sensorname.ToLower().Contains("video") || sensorname.ToLower().Contains("3d"))
				{
					if (ClsMain.GPUPower.Where((KeyValue A) => A.Key == sensorname).ToList().Count == 0)
					{
						KeyValue item21 = new KeyValue(sensorname, s);
						ClsMain.GPUPower.Add(item21);
					}
					else
					{
						ClsMain.GPUPower.Where((KeyValue A) => A.Key == sensorname).ToList().ForEach(delegate(KeyValue B)
						{
							B.Value = s;
						});
					}
					KeyValue item22 = new KeyValue(sensorname, s);
					ClsMain.GPUPower2.Add(item22);
				}
				s += "  Watts";
			}
			numberOfSensors = Program.pSDK.GetNumberOfSensors(i, 16384);
			for (int j = 0; j < numberOfSensors; j++)
			{
				if (Program.pSDK.GetSensorInfos(i, j, 16384, ref _sensor_id, ref sensorname, ref _raw_value, ref _value, ref _min_value, ref _max_value) && _value > 0f)
				{
					s = Convert.ToString(Math.Round(_value, 1));
					s += "  Amps";
				}
			}
			numberOfSensors = Program.pSDK.GetNumberOfSensors(i, 24576);
			for (int j = 0; j < numberOfSensors; j++)
			{
				if (Program.pSDK.GetSensorInfos(i, j, 24576, ref _sensor_id, ref sensorname, ref _raw_value, ref _value, ref _min_value, ref _max_value))
				{
					s = Convert.ToString(Math.Round(_value, 0));
					s += "  %";
				}
			}
			numberOfSensors = Program.pSDK.GetNumberOfSensors(i, 40960);
			for (int j = 0; j < numberOfSensors; j++)
			{
				if (Program.pSDK.GetSensorInfos(i, j, 40960, ref _sensor_id, ref sensorname, ref _raw_value, ref _value, ref _min_value, ref _max_value) && _value > 0f)
				{
					s = Convert.ToString(Math.Round(_value, 0));
					s += "  mWh";
				}
			}
			numberOfSensors = Program.pSDK.GetNumberOfSensors(i, 49152);
			for (int j = 0; j < numberOfSensors; j++)
			{
				if (Program.pSDK.GetSensorInfos(i, j, 49152, ref _sensor_id, ref sensorname, ref _raw_value, ref _value, ref _min_value, ref _max_value))
				{
					s = Convert.ToString(Math.Round(_value, 0));
					s += "  %";
				}
			}
		}
		ShowCmb();
		try
		{
			List<KeyValue> list = ClsMain.CPUT.Where((KeyValue A) => A.Key == ClsMain.cmbValue_CPUT_selectText.Split(':')[0]).ToList();
			if (list.Count > 0)
			{
				KeyValue keyValue = list[0];
				if (ClsMain.Showcentigrade_Flag)
				{
					ClsMain.SetDisplayLbl(ClsMain.labvalue_CPUT, keyValue.Value + "℃");
					ClsMain.SetDisplayLbl(ClsMain.labvalue_CPUT2, keyValue.Value + "℃");
				}
				else
				{
					ClsMain.SetDisplayLbl(ClsMain.labvalue_CPUT, keyValue.Value + "℉");
					ClsMain.SetDisplayLbl(ClsMain.labvalue_CPUT2, keyValue.Value + "℉");
				}
				SendValue_CPUTemp = Convert.ToDouble(keyValue.Value);
			}
			else
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUT, "-1", "select");
			}
			List<KeyValue> list2 = ClsMain.CPUUsage.Where((KeyValue A) => A.Key == ClsMain.cmbValue_CPUUsage_selectText.Split(':')[0]).ToList();
			if (list2.Count > 0)
			{
				KeyValue keyValue2 = list2[0];
				ClsMain.SetDisplayLbl(ClsMain.labvalue_CPUUsage, keyValue2.Value + "%");
				ClsMain.SetDisplayLbl(ClsMain.labvalue_CPUUsage2, keyValue2.Value + "%");
				SendValue_CPUUsage = Convert.ToDouble(keyValue2.Value);
			}
			else
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUUsage, "-1", "select");
			}
			List<KeyValue> list3 = ClsMain.CPUPower.Where((KeyValue A) => A.Key == ClsMain.cmbValue_CPUPower_selectText.Split(':')[0]).ToList();
			if (list3.Count > 0)
			{
				KeyValue keyValue3 = list3[0];
				ClsMain.SetDisplayLbl(ClsMain.labvalue_CPUPower, keyValue3.Value + "W");
				ClsMain.SetDisplayLbl(ClsMain.labvalue_CPUPower2, keyValue3.Value + "W");
				SendValue_CPUPower = Convert.ToDouble(keyValue3.Value);
			}
			else
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUPower, "-1", "select");
			}
			List<KeyValue> list4 = ClsMain.CPUFrequence.Where((KeyValue A) => A.Key == ClsMain.cmbValue_CPUFrequence_selectText.Split(':')[0]).ToList();
			if (list4.Count > 0)
			{
				KeyValue keyValue4 = list4[0];
				ClsMain.SetDisplayLbl(ClsMain.labvalue_CPUFrequence, keyValue4.Value + "MHZ");
				ClsMain.SetDisplayLbl(ClsMain.labvalue_CPUFrequence2, keyValue4.Value + "MHZ");
				SendValue_CPUFrequence = Convert.ToDouble(keyValue4.Value);
			}
			else
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUFrequence, "-1", "select");
			}
			List<KeyValue> list5 = ClsMain.CPUVoltages.Where((KeyValue A) => A.Key == ClsMain.cmbValue_CPUVoltages_selectText.Split(':')[0]).ToList();
			if (list5.Count > 0)
			{
				KeyValue keyValue5 = list5[0];
				ClsMain.SetDisplayLbl(ClsMain.labvalue_CPUVoltages, keyValue5.Value + "V");
				ClsMain.SetDisplayLbl(ClsMain.labvalue_CPUVoltages2, keyValue5.Value + "V");
				SendValue_CPUVoltages = Convert.ToDouble(keyValue5.Value);
			}
			else
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUVoltages, "-1", "select");
			}
			List<KeyValue> list6 = ClsMain.GPUT.Where((KeyValue A) => A.Key == ClsMain.cmbValue_GPUT_selectText.Split(':')[0]).ToList();
			if (list6.Count > 0)
			{
				KeyValue keyValue6 = list6[0];
				if (ClsMain.Showcentigrade_Flag)
				{
					ClsMain.SetDisplayLbl(ClsMain.labvalue_GPUT, keyValue6.Value + "℃");
					ClsMain.SetDisplayLbl(ClsMain.labvalue_GPUT2, keyValue6.Value + "℃");
				}
				else
				{
					ClsMain.SetDisplayLbl(ClsMain.labvalue_GPUT, keyValue6.Value + "℉");
					ClsMain.SetDisplayLbl(ClsMain.labvalue_GPUT2, keyValue6.Value + "℉");
				}
				SendValue_GPUTemp = Convert.ToDouble(keyValue6.Value);
			}
			else
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUT, "-1", "select");
			}
			List<KeyValue> list7 = ClsMain.GPUUsage.Where((KeyValue A) => A.Key == ClsMain.cmbValue_GPUUsage_selectText.Split(':')[0]).ToList();
			if (list7.Count > 0)
			{
				KeyValue keyValue7 = list7[0];
				ClsMain.SetDisplayLbl(ClsMain.labvalue_GPUUsage, keyValue7.Value + "%");
				ClsMain.SetDisplayLbl(ClsMain.labvalue_GPUUsage2, keyValue7.Value + "%");
				SendValue_GPUUsage = Convert.ToDouble(keyValue7.Value);
			}
			else
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUUsage, "-1", "select");
			}
			List<KeyValue> list8 = ClsMain.GPUPower.Where((KeyValue A) => A.Key == ClsMain.cmbValue_GPUPower_selectText.Split(':')[0]).ToList();
			if (list8.Count > 0)
			{
				KeyValue keyValue8 = list8[0];
				ClsMain.SetDisplayLbl(ClsMain.labvalue_GPUPower, keyValue8.Value + "W");
				ClsMain.SetDisplayLbl(ClsMain.labvalue_GPUPower2, keyValue8.Value + "W");
				SendValue_GPUPower = Convert.ToDouble(keyValue8.Value);
			}
			else
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUPower, "-1", "select");
			}
			List<KeyValue> list9 = ClsMain.GPUFrequence.Where((KeyValue A) => A.Key == ClsMain.cmbValue_GPUFrequence_selectText.Split(':')[0]).ToList();
			if (list9.Count > 0)
			{
				KeyValue keyValue9 = list9[0];
				ClsMain.SetDisplayLbl(ClsMain.labvalue_GPUFrequence, keyValue9.Value + "MHZ");
				ClsMain.SetDisplayLbl(ClsMain.labvalue_GPUFrequence2, keyValue9.Value + "MHZ");
				SendValue_GPUFrequence = Convert.ToDouble(keyValue9.Value);
			}
			else
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUFrequence, "-1", "select");
			}
			List<KeyValue> list10 = ClsMain.FansRPM.Where((KeyValue A) => A.Key == ClsMain.cmbValue_FansRPM_selectText.Split(':')[0]).ToList();
			if (list10.Count > 0)
			{
				KeyValue keyValue10 = list10[0];
				ClsMain.SetDisplayLbl(ClsMain.labvalue_FansRPM, keyValue10.Value + "RPM");
				ClsMain.SetDisplayLbl(ClsMain.labvalue_FansRPM2, keyValue10.Value + "RPM");
				SendValue_Fans = Convert.ToDouble(keyValue10.Value);
			}
			else
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_FansRPM, "-1", "select");
			}
			List<KeyValue> list11 = ClsMain.WatarRPM.Where((KeyValue A) => A.Key == ClsMain.cmbValue_WatarRPM_selectText.Split(':')[0]).ToList();
			if (list11.Count > 0)
			{
				KeyValue keyValue11 = list11[0];
				ClsMain.SetDisplayLbl(ClsMain.labvalue_WatarRPM, keyValue11.Value + "RPM");
				ClsMain.SetDisplayLbl(ClsMain.labvalue_WatarRPM2, keyValue11.Value + "RPM");
				SendValue_WaterCool = Convert.ToDouble(keyValue11.Value);
			}
			else
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_WatarRPM, "-1", "select");
			}
			ClsMain.SetDisplayLbl(ClsMain.labvalue_PicRAMUsage, ClsMain.MainBoardUsage + "%");
			ClsMain.SetDisplayLbl(ClsMain.labvalue_RAMName2, ClsMain.MainBoardName + ":Usage:");
			ClsMain.SetDisplayLbl(ClsMain.labvalue_RAMUsage2, ClsMain.MainBoardUsage + "%");
			double num = Math.Floor(SendValue_CPUTemp);
			double num2 = SendValue_CPUTemp - num;
			SendValueArray[0] = Convert.ToInt16(num);
			SendValueArray[1] = Convert.ToInt16(num2 * 100.0);
			if (ClsMain.Showcentigrade_Flag)
			{
				SendValueArray[2] = 0;
			}
			else
			{
				SendValueArray[2] = 1;
			}
			SendValueArray[3] = Convert.ToInt16(Math.Floor(SendValue_CPUUsage));
			double num3 = Math.Floor(SendValue_CPUPower);
			double value = Math.Floor(num3 / 100.0);
			double value2 = Math.Floor(num3 % 100.0);
			double num4 = SendValue_CPUPower - num3;
			SendValueArray[4] = Convert.ToInt16(value2);
			SendValueArray[5] = Convert.ToInt16(num4 * 100.0);
			double num5 = Math.Floor(SendValue_CPUFrequence);
			double value3 = Math.Floor(num5 / 100.0);
			double value4 = Math.Floor(num5 % 100.0);
			SendValueArray[6] = Convert.ToInt16(value3);
			SendValueArray[7] = Convert.ToInt16(value4);
			double num6 = Math.Floor(SendValue_CPUVoltages);
			Math.Floor(SendValue_CPUVoltages % 1.0);
			SendValueArray[8] = Convert.ToInt16(num6);
			SendValueArray[9] = Convert.ToInt16(num6 * 100.0);
			double num7 = Math.Floor(SendValue_GPUTemp);
			double num8 = SendValue_GPUTemp - num7;
			SendValueArray[10] = Convert.ToInt16(num7);
			SendValueArray[11] = Convert.ToInt16(num8 * 100.0);
			if (ClsMain.Showcentigrade_Flag)
			{
				SendValueArray[12] = 0;
			}
			else
			{
				SendValueArray[12] = 1;
			}
			SendValueArray[13] = Convert.ToInt16(Math.Floor(SendValue_GPUUsage));
			double num9 = Math.Floor(SendValue_GPUPower);
			double value5 = Math.Floor(num9 / 100.0);
			double value6 = Math.Floor(num9 % 100.0);
			double num10 = SendValue_GPUPower - num9;
			SendValueArray[14] = Convert.ToInt16(value6);
			SendValueArray[15] = Convert.ToInt16(num10 * 100.0);
			double num11 = Math.Floor(SendValue_GPUFrequence);
			double value7 = Math.Floor(num11 / 100.0);
			double value8 = Math.Floor(num11 % 100.0);
			SendValueArray[16] = Convert.ToInt16(value7);
			SendValueArray[17] = Convert.ToInt16(value8);
			double num12 = Math.Floor(SendValue_Fans);
			double value9 = Math.Floor(num12 / 100.0);
			double value10 = Math.Floor(num12 % 100.0);
			SendValueArray[18] = Convert.ToInt16(value9);
			SendValueArray[19] = Convert.ToInt16(value10);
			double num13 = Math.Floor(SendValue_WaterCool);
			double value11 = Math.Floor(num13 / 100.0);
			double value12 = Math.Floor(num13 % 100.0);
			SendValueArray[20] = Convert.ToInt16(value11);
			SendValueArray[21] = Convert.ToInt16(value12);
			SendValueArray[22] = Convert.ToInt16(DateTime.Now.Year.ToString().Substring(0, 2));
			SendValueArray[23] = Convert.ToInt16(DateTime.Now.Year.ToString().Substring(2, 2));
			SendValueArray[24] = DateTime.Now.Month;
			SendValueArray[25] = DateTime.Now.Day;
			SendValueArray[26] = DateTime.Now.Hour;
			SendValueArray[27] = DateTime.Now.Minute;
			SendValueArray[28] = DateTime.Now.Second;
			SendValueArray[29] = (int)DateTime.Now.DayOfWeek;
			SendValueArray[30] = ClsMain.MainBoardUsage;
			SendValueArray[31] = Convert.ToInt16(value);
			SendValueArray[32] = Convert.ToInt16(value5);
			SendValueArray[33] = Convert.ToInt16(value5);
			SendValueArray[34] = Convert.ToInt16(value5);
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("转换数据ERR:" + ex.Message, "HT");
		}
	}

	public void ShowCmb()
	{
		try
		{
			if (ClsMain.CPUT2.Count != ClsMain.cmbValue_CPUT.Items.Count)
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUT, "", "clear");
				for (int i = 0; i < ClsMain.CPUT2.Count; i++)
				{
					ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUT, ClsMain.CPUT2[i].Key + ":" + ClsMain.CPUT2[i].Value, "add");
					if (ClsMain.CPUT2[i].Key == ClsMain.CPUTempSelectName.Split(':')[0] && !ClsMain.CPUTempDrop)
					{
						CPUTempSelectIndex = i;
						if (ClsMain.cmbValue_CPUT.Items.Count > 0)
						{
							ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUT, CPUTempSelectIndex.ToString(), "select");
						}
					}
				}
			}
			else
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUT, ClsMain.CPUTempSelectName.Split(':')[0], "TEXT");
				if (ClsMain.CPUTempDrop && !ClsMain.CPUTempDropEnd)
				{
					ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUT, "", "clear");
					ClsMain.CPUTempDropEnd = true;
				}
				if (!ClsMain.CPUTempDrop)
				{
					if (ClsMain.CPUT2.Count == 0)
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUT, "", "TEXT");
					}
					else if (CPUTempSelectIndex < ClsMain.CPUT2.Count && CPUTempSelectIndex >= 0)
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUT, ClsMain.CPUT2[CPUTempSelectIndex].Key + ":" + ClsMain.CPUT2[CPUTempSelectIndex].Value, "TEXT");
					}
					else
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUT, "", "TEXT");
					}
				}
			}
			if (ClsMain.CPUUsage2.Count != ClsMain.cmbValue_CPUUsage.Items.Count)
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUUsage, "", "clear");
				for (int j = 0; j < ClsMain.CPUUsage2.Count; j++)
				{
					ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUUsage, ClsMain.CPUUsage2[j].Key + ":" + ClsMain.CPUUsage2[j].Value, "add");
					if (ClsMain.CPUUsage2[j].Key == ClsMain.CPUUsageSelectName.Split(':')[0] && !ClsMain.CPUUsageDrop)
					{
						CPUUsageSelectIndex = j;
						if (ClsMain.cmbValue_CPUUsage.Items.Count > 0)
						{
							ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUUsage, CPUUsageSelectIndex.ToString(), "select");
						}
					}
				}
			}
			else
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUUsage, ClsMain.CPUUsageSelectName.Split(':')[0], "TEXT");
				if (ClsMain.CPUUsageDrop && !ClsMain.CPUUsageDropEnd)
				{
					ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUUsage, "", "clear");
					ClsMain.CPUUsageDropEnd = true;
				}
				if (!ClsMain.CPUUsageDrop)
				{
					if (ClsMain.CPUUsage2.Count == 0)
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUUsage, "", "TEXT");
					}
					else if (CPUUsageSelectIndex < ClsMain.CPUUsage2.Count && CPUUsageSelectIndex >= 0)
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUUsage, ClsMain.CPUUsage2[CPUUsageSelectIndex].Key + ":" + ClsMain.CPUUsage2[CPUUsageSelectIndex].Value, "TEXT");
					}
					else
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUUsage, "", "TEXT");
					}
				}
			}
			if (ClsMain.CPUPower2.Count != ClsMain.cmbValue_CPUPower.Items.Count)
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUPower, "", "clear");
				for (int k = 0; k < ClsMain.CPUPower2.Count; k++)
				{
					ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUPower, ClsMain.CPUPower2[k].Key + ":" + ClsMain.CPUPower2[k].Value, "add");
					if (ClsMain.CPUPower2[k].Key == ClsMain.CPUPowerSelectName.Split(':')[0] && !ClsMain.CPUPowerDrop)
					{
						CPUPowerSelectIndex = k;
						if (ClsMain.cmbValue_CPUPower.Items.Count > 0)
						{
							ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUPower, CPUPowerSelectIndex.ToString(), "select");
						}
					}
				}
			}
			else
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUPower, ClsMain.CPUPowerSelectName.Split(':')[0], "TEXT");
				if (ClsMain.CPUPowerDrop && !ClsMain.CPUPowerDropEnd)
				{
					ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUPower, "", "clear");
					ClsMain.CPUPowerDropEnd = true;
				}
				if (!ClsMain.CPUPowerDrop)
				{
					if (ClsMain.CPUPower2.Count == 0)
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUPower, "", "TEXT");
					}
					else if (CPUPowerSelectIndex < ClsMain.CPUPower2.Count && CPUPowerSelectIndex >= 0)
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUPower, ClsMain.CPUPower2[CPUPowerSelectIndex].Key + ":" + ClsMain.CPUPower2[CPUPowerSelectIndex].Value, "TEXT");
					}
					else
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUPower, "", "TEXT");
					}
				}
			}
			if (ClsMain.CPUFrequence2.Count != ClsMain.cmbValue_CPUFrequence.Items.Count)
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUFrequence, "", "clear");
				for (int l = 0; l < ClsMain.CPUFrequence2.Count; l++)
				{
					ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUFrequence, ClsMain.CPUFrequence2[l].Key + ":" + ClsMain.CPUFrequence2[l].Value, "add");
					if (ClsMain.CPUFrequence2[l].Key == ClsMain.CPUFrequenceSelectName.Split(':')[0] && !ClsMain.CPUFrequenceDrop)
					{
						CPUFrequenceSelectIndex = l;
						if (ClsMain.cmbValue_CPUFrequence.Items.Count > 0)
						{
							ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUFrequence, CPUFrequenceSelectIndex.ToString(), "select");
						}
					}
				}
			}
			else
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUFrequence, ClsMain.CPUFrequenceSelectName.Split(':')[0], "TEXT");
				if (ClsMain.CPUFrequenceDrop && !ClsMain.CPUFrequenceDropEnd)
				{
					ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUFrequence, "", "clear");
					ClsMain.CPUFrequenceDropEnd = true;
				}
				if (!ClsMain.CPUFrequenceDrop)
				{
					if (ClsMain.CPUFrequence2.Count == 0)
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUFrequence, "", "TEXT");
					}
					else if (CPUFrequenceSelectIndex < ClsMain.CPUFrequence2.Count && CPUFrequenceSelectIndex >= 0)
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUFrequence, ClsMain.CPUFrequence2[CPUFrequenceSelectIndex].Key + ":" + ClsMain.CPUFrequence2[CPUFrequenceSelectIndex].Value, "TEXT");
					}
					else
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUFrequence, "", "TEXT");
					}
				}
			}
			if (ClsMain.CPUVoltages2.Count != ClsMain.cmbValue_CPUVoltages.Items.Count)
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUVoltages, "", "clear");
				for (int m = 0; m < ClsMain.CPUVoltages2.Count; m++)
				{
					ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUVoltages, ClsMain.CPUVoltages2[m].Key + ":" + ClsMain.CPUVoltages2[m].Value, "add");
					if (ClsMain.CPUVoltages2[m].Key == ClsMain.CPUVoltagesSelectName.Split(':')[0] && !ClsMain.CPUVoltagesDrop)
					{
						CPUVoltagesSelectIndex = m;
						if (ClsMain.cmbValue_CPUVoltages.Items.Count > 0)
						{
							ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUVoltages, CPUVoltagesSelectIndex.ToString(), "select");
						}
					}
				}
			}
			else
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUVoltages, ClsMain.CPUVoltagesSelectName.Split(':')[0], "TEXT");
				if (ClsMain.CPUVoltagesDrop && !ClsMain.CPUVoltagesDropEnd)
				{
					ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUVoltages, "", "clear");
					ClsMain.CPUVoltagesDropEnd = true;
				}
				if (!ClsMain.CPUVoltagesDrop)
				{
					if (ClsMain.CPUVoltages2.Count == 0)
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUVoltages, "", "TEXT");
					}
					else if (CPUVoltagesSelectIndex < ClsMain.CPUVoltages2.Count && CPUVoltagesSelectIndex >= 0)
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUVoltages, ClsMain.CPUVoltages2[CPUVoltagesSelectIndex].Key + ":" + ClsMain.CPUVoltages2[CPUVoltagesSelectIndex].Value, "TEXT");
					}
					else
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_CPUVoltages, "", "TEXT");
					}
				}
			}
			if (ClsMain.GPUT2.Count != ClsMain.cmbValue_GPUT.Items.Count)
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUT, "", "clear");
				for (int n = 0; n < ClsMain.GPUT2.Count; n++)
				{
					ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUT, ClsMain.GPUT2[n].Key + ":" + ClsMain.GPUT2[n].Value, "add");
					if (ClsMain.GPUT2[n].Key == ClsMain.GPUTempSelectName.Split(':')[0] && !ClsMain.GPUTempDrop)
					{
						GPUTempSelectIndex = n;
						if (ClsMain.cmbValue_GPUT.Items.Count > 0)
						{
							ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUT, GPUTempSelectIndex.ToString(), "select");
						}
					}
				}
			}
			else
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUT, ClsMain.GPUTempSelectName.Split(':')[0], "TEXT");
				if (ClsMain.GPUTempDrop && !ClsMain.GPUTempDropEnd)
				{
					ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUT, "", "clear");
					ClsMain.GPUTempDropEnd = true;
				}
				if (!ClsMain.GPUTempDrop)
				{
					if (ClsMain.GPUT2.Count == 0)
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUT, "", "TEXT");
					}
					else if (GPUTempSelectIndex < ClsMain.GPUT2.Count && GPUTempSelectIndex >= 0)
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUT, ClsMain.GPUT2[GPUTempSelectIndex].Key + ":" + ClsMain.GPUT2[GPUTempSelectIndex].Value, "TEXT");
					}
					else
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUT, "", "TEXT");
					}
				}
			}
			if (ClsMain.GPUUsage2.Count != ClsMain.cmbValue_GPUUsage.Items.Count)
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUUsage, "", "clear");
				for (int num = 0; num < ClsMain.GPUUsage2.Count; num++)
				{
					ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUUsage, ClsMain.GPUUsage2[num].Key + ":" + ClsMain.GPUUsage2[num].Value, "add");
					if (ClsMain.GPUUsage2[num].Key == ClsMain.GPUUsageSelectName.Split(':')[0] && !ClsMain.GPUUsageDrop)
					{
						GPUUsageSelectIndex = num;
						if (ClsMain.cmbValue_GPUUsage.Items.Count > 0)
						{
							ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUUsage, GPUUsageSelectIndex.ToString(), "select");
						}
					}
				}
			}
			else
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUUsage, ClsMain.GPUUsageSelectName.Split(':')[0], "TEXT");
				if (ClsMain.GPUUsageDrop && !ClsMain.GPUUsageDropEnd)
				{
					ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUUsage, "", "clear");
					ClsMain.GPUUsageDropEnd = true;
				}
				if (!ClsMain.GPUUsageDrop)
				{
					if (ClsMain.GPUUsage2.Count == 0)
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUUsage, "", "TEXT");
					}
					else if (GPUUsageSelectIndex < ClsMain.GPUUsage2.Count && GPUUsageSelectIndex >= 0)
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUUsage, ClsMain.GPUUsage2[GPUUsageSelectIndex].Key + ":" + ClsMain.GPUUsage2[GPUUsageSelectIndex].Value, "TEXT");
					}
					else
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUUsage, "", "TEXT");
					}
				}
			}
			if (ClsMain.GPUPower2.Count != ClsMain.cmbValue_GPUPower.Items.Count)
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUPower, "", "clear");
				for (int num2 = 0; num2 < ClsMain.GPUPower2.Count; num2++)
				{
					ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUPower, ClsMain.GPUPower2[num2].Key + ":" + ClsMain.GPUPower2[num2].Value, "add");
					if (ClsMain.GPUPower2[num2].Key == ClsMain.GPUPowerSelectName.Split(':')[0] && !ClsMain.GPUPowerDrop)
					{
						GPUPowerSelectIndex = num2;
						if (ClsMain.cmbValue_GPUPower.Items.Count > 0)
						{
							ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUPower, GPUPowerSelectIndex.ToString(), "select");
						}
					}
				}
			}
			else
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUPower, ClsMain.GPUPowerSelectName.Split(':')[0], "TEXT");
				if (ClsMain.GPUPowerDrop && !ClsMain.GPUPowerDropEnd)
				{
					ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUPower, "", "clear");
					ClsMain.GPUPowerDropEnd = true;
				}
				if (!ClsMain.GPUPowerDrop)
				{
					if (ClsMain.GPUPower2.Count == 0)
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUPower, "", "TEXT");
					}
					else if (GPUPowerSelectIndex < ClsMain.GPUPower2.Count && GPUPowerSelectIndex >= 0)
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUPower, ClsMain.GPUPower2[GPUPowerSelectIndex].Key + ":" + ClsMain.GPUPower2[GPUPowerSelectIndex].Value, "TEXT");
					}
					else
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUPower, "", "TEXT");
					}
				}
			}
			if (ClsMain.GPUFrequence2.Count != ClsMain.cmbValue_GPUFrequence.Items.Count)
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUFrequence, "", "clear");
				for (int num3 = 0; num3 < ClsMain.GPUFrequence2.Count; num3++)
				{
					ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUFrequence, ClsMain.GPUFrequence2[num3].Key + ":" + ClsMain.GPUFrequence2[num3].Value, "add");
					if (ClsMain.GPUFrequence2[num3].Key == ClsMain.GPUFrequenceSelectName.Split(':')[0] && !ClsMain.GPUFrequenceDrop)
					{
						GPUFrequenceSelectIndex = num3;
						if (ClsMain.cmbValue_GPUFrequence.Items.Count > 0)
						{
							ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUFrequence, GPUFrequenceSelectIndex.ToString(), "select");
						}
					}
				}
			}
			else
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUFrequence, ClsMain.GPUFrequenceSelectName.Split(':')[0], "TEXT");
				if (ClsMain.GPUFrequenceDrop && !ClsMain.GPUFrequenceDropEnd)
				{
					ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUFrequence, "", "clear");
					ClsMain.GPUFrequenceDropEnd = true;
				}
				if (!ClsMain.GPUFrequenceDrop)
				{
					if (ClsMain.GPUFrequence2.Count == 0)
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUFrequence, "", "TEXT");
					}
					else if (GPUFrequenceSelectIndex < ClsMain.GPUFrequence2.Count && GPUFrequenceSelectIndex >= 0)
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUFrequence, ClsMain.GPUFrequence2[GPUFrequenceSelectIndex].Key + ":" + ClsMain.GPUFrequence2[GPUFrequenceSelectIndex].Value, "TEXT");
					}
					else
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_GPUFrequence, "", "TEXT");
					}
				}
			}
			if (ClsMain.FansRPM2.Count != ClsMain.cmbValue_FansRPM.Items.Count)
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_FansRPM, "", "clear");
				for (int num4 = 0; num4 < ClsMain.FansRPM2.Count; num4++)
				{
					ClsMain.SetDisplayCmb(ClsMain.cmbValue_FansRPM, ClsMain.FansRPM2[num4].Key + ":" + ClsMain.FansRPM2[num4].Value, "add");
					if (ClsMain.FansRPM2[num4].Key == ClsMain.FANSSelectName.Split(':')[0] && !ClsMain.FansDrop)
					{
						FansSelectIndex = num4;
						if (ClsMain.cmbValue_FansRPM.Items.Count > 0)
						{
							ClsMain.SetDisplayCmb(ClsMain.cmbValue_FansRPM, FansSelectIndex.ToString(), "select");
						}
					}
				}
			}
			else
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_FansRPM, ClsMain.FANSSelectName.Split(':')[0], "TEXT");
				if (ClsMain.FansDrop && !ClsMain.FansDropEnd)
				{
					ClsMain.SetDisplayCmb(ClsMain.cmbValue_FansRPM, "", "clear");
					ClsMain.FansDropEnd = true;
				}
				if (!ClsMain.FansDrop)
				{
					if (ClsMain.FansRPM2.Count == 0)
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_FansRPM, "", "TEXT");
					}
					else if (FansSelectIndex < ClsMain.FansRPM2.Count && FansSelectIndex >= 0)
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_FansRPM, ClsMain.FansRPM2[FansSelectIndex].Key + ":" + ClsMain.FansRPM2[FansSelectIndex].Value, "TEXT");
					}
					else
					{
						ClsMain.SetDisplayCmb(ClsMain.cmbValue_FansRPM, "", "TEXT");
					}
				}
			}
			if (ClsMain.WatarRPM2.Count != ClsMain.cmbValue_WatarRPM.Items.Count)
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_WatarRPM, "", "clear");
				for (int num5 = 0; num5 < ClsMain.WatarRPM.Count; num5++)
				{
					ClsMain.SetDisplayCmb(ClsMain.cmbValue_WatarRPM, ClsMain.WatarRPM2[num5].Key + ":" + ClsMain.WatarRPM2[num5].Value, "add");
					if (ClsMain.WatarRPM2[num5].Key == ClsMain.WatarSelectName.Split(':')[0] && !ClsMain.WaterCoolDrop)
					{
						WatarSelectIndex = num5;
						if (ClsMain.cmbValue_WatarRPM.Items.Count > 0)
						{
							ClsMain.SetDisplayCmb(ClsMain.cmbValue_WatarRPM, WatarSelectIndex.ToString(), "select");
						}
					}
				}
				return;
			}
			ClsMain.SetDisplayCmb(ClsMain.cmbValue_WatarRPM, ClsMain.WatarSelectName.Split(':')[0], "TEXT");
			if (ClsMain.WaterCoolDrop && !ClsMain.WaterCoolDropEnd)
			{
				ClsMain.SetDisplayCmb(ClsMain.cmbValue_WatarRPM, "", "clear");
				ClsMain.WaterCoolDropEnd = true;
			}
			if (!ClsMain.WaterCoolDrop)
			{
				if (ClsMain.WatarRPM2.Count == 0)
				{
					ClsMain.SetDisplayCmb(ClsMain.cmbValue_WatarRPM, "", "TEXT");
				}
				else if (WatarSelectIndex < ClsMain.WatarRPM2.Count && WatarSelectIndex >= 0)
				{
					ClsMain.SetDisplayCmb(ClsMain.cmbValue_WatarRPM, ClsMain.WatarRPM2[WatarSelectIndex].Key + ":" + ClsMain.WatarRPM2[WatarSelectIndex].Value, "TEXT");
				}
				else
				{
					ClsMain.SetDisplayCmb(ClsMain.cmbValue_WatarRPM, "", "TEXT");
				}
			}
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("刷新显示CMB下拉框ERR:" + ex.Message, "HT");
		}
	}

	public void Thread_Send()
	{
		while (true)
		{
			try
			{
				ShowConnect();
				SendData2();
			}
			catch (Exception ex)
			{
				ClsMain.SetAppendTxt("线程发送数据ERR:" + ex.Message, "HT");
			}
			Thread.Sleep(200);
			Application.DoEvents();
		}
	}

	public void ShowConnect()
	{
		try
		{
			if (myHidDevice == null)
			{
				Connect_Flag = false;
				try
				{
					string path = Application.StartupPath + "\\ico\\UI_DISConnect.png";
					LoadBGImage(path);
				}
				catch (Exception ex)
				{
					ClsMain.SetAppendTxt("切换图片1ERR:" + ex.Message, "HT");
				}
				Get_Devices();
				Thread.Sleep(250);
				Application.DoEvents();
			}
			if (myHidDevice != null)
			{
				Connect_Flag = true;
				myHidDevice.Outputs.DataBuf[0] = myHidDevice.Outputs.ID;
				try
				{
					string path2 = Application.StartupPath + "\\ico\\UI_Connect.png";
					LoadBGImage(path2);
					return;
				}
				catch (Exception ex2)
				{
					ClsMain.SetAppendTxt("切换图片2ERR:" + ex2.Message, "HT");
					return;
				}
			}
		}
		catch (Exception ex3)
		{
			ClsMain.SetAppendTxt("切换图片3ERR:" + ex3.Message, "HT");
		}
	}

	private void cmb_CPUT_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			CPUTempSelectIndex = cmb_CPUT.SelectedIndex;
			ClsMain.CPUTempSelectName = cmb_CPUT.Text;
			IniFunc.writeString("system", "CPUTempSelectName", ClsMain.CPUTempSelectName, ClsMain.iniPath);
			ClsMain.cmbValue_CPUT_selectText = ClsMain.CPUTempSelectName;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示CPUT异常1:" + ex.Message, "HT");
		}
	}

	private void cmb_FansSelect_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			FansSelectIndex = cmb_FansSelect.SelectedIndex;
			ClsMain.FANSSelectName = cmb_FansSelect.Text;
			IniFunc.writeString("system", "FANSSelectName", ClsMain.FANSSelectName, ClsMain.iniPath);
			ClsMain.cmbValue_FansRPM_selectText = ClsMain.FANSSelectName;
		}
		catch (Exception)
		{
		}
	}

	private void cmb_watarcoolselsct_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			CPUUsageSelectIndex = cmb_CPUUsage.SelectedIndex;
			ClsMain.CPUUsageSelectName = cmb_CPUUsage.Text;
			IniFunc.writeString("system", "CPUUsageSelectName", ClsMain.CPUUsageSelectName, ClsMain.iniPath);
			ClsMain.cmbValue_CPUUsage_selectText = ClsMain.CPUUsageSelectName;
		}
		catch (Exception)
		{
		}
	}

	private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
	{
		byte[] array = new byte[64];
		array[1] = 15;
		send_usb_data(array, 64);
		try
		{
			CloseAllProcesses(Process.GetCurrentProcess().ProcessName);
		}
		catch (Exception)
		{
		}
	}

	public static void CloseAllProcesses(string processName)
	{
		Process[] processesByName = Process.GetProcessesByName(processName);
		foreach (Process process in processesByName)
		{
			try
			{
				process.Kill();
				process.WaitForExit();
			}
			catch (Exception)
			{
			}
		}
	}

	public bool send_usb_data(byte[] data, int length)
	{
		bool result = false;
		try
		{
			if (myHidDevice == null)
			{
				return result;
			}
			if (myHidDevice != null)
			{
				myHidDevice.Outputs.DataBuf[0] = myHidDevice.Outputs.ID;
				for (int i = 1; i <= length; i++)
				{
					myHidDevice.Outputs.DataBuf[i] = data[i - 1];
				}
				result = myHidDevice.WriteOutput();
				return result;
			}
			return result;
		}
		catch (Exception)
		{
			myHidDevice = null;
			return result;
		}
	}

	public int receive_usb_data(byte[] data)
	{
		int num = 0;
		if (myHidDevice != null)
		{
			if (myHidDevice.ReadInput())
			{
				num = myHidDevice.Inputs.RptByteLen;
				if (num != 0)
				{
					for (int i = 0; i < num - 1; i++)
					{
						data[i] = myHidDevice.Inputs.DataBuf[i + 1];
					}
				}
			}
			else if (myHidDevice.ReadInput())
			{
				num = myHidDevice.Inputs.RptByteLen;
				if (num != 0)
				{
					for (int j = 0; j < num - 1; j++)
					{
						data[j] = myHidDevice.Inputs.DataBuf[j + 1];
					}
				}
			}
		}
		return num;
	}

	private void USB_Init()
	{
		usbDevices = new USBDeviceList(4);
		usbDevices.DeviceAttached += UsbDevices_DeviceAttached;
		usbDevices.DeviceRemoved += UsbDevices_DeviceRemoved;
		Get_Devices();
	}

	private void Get_Devices()
	{
		try
		{
			Action method = delegate
			{
				usbDevices.Dispose();
				usbDevices = new USBDeviceList(4);
				myHidDevice = usbDevices[20785, 8199] as CyHidDevice;
				List<string> list = new List<string>();
				for (int i = 0; i < usbDevices.Count; i++)
				{
					list.Add(usbDevices[i].ProductID + ":" + usbDevices[i].VendorID);
				}
			};
			Invoke(method);
		}
		catch (Exception)
		{
		}
	}

	private void UsbDevices_DeviceRemoved(object sender, EventArgs e)
	{
		USBEventArgs e2 = e as USBEventArgs;
		if (e2.VendorID == 20785 && e2.ProductID == 8199)
		{
			myHidDevice = null;
		}
	}

	private void UsbDevices_DeviceAttached(object sender, EventArgs e)
	{
		USBEventArgs e2 = e as USBEventArgs;
		if (e2.VendorID == 20785)
		{
			_ = e2.ProductID;
			_ = 8199;
		}
	}

	public void SendData2()
	{
		try
		{
			byte[] array = new byte[64];
			array[0] = 0;
			array[1] = 1;
			array[2] = 2;
			for (int i = 0; i < 33; i++)
			{
				array[3 + i] = (byte)SendValueArray[i];
			}
			send_usb_data(array, 64);
		}
		catch (Exception)
		{
		}
	}

	private void cmb_CPUT_DropDown(object sender, EventArgs e)
	{
		try
		{
			ClsMain.CPUTempDrop = true;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示CPUT异常2:" + ex.Message, "HT");
		}
	}

	private void cmb_CPUT_DropDownClosed(object sender, EventArgs e)
	{
		try
		{
			ClsMain.CPUTempDrop = false;
			ClsMain.CPUTempDropEnd = false;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示CPUT异常3:" + ex.Message, "HT");
		}
		lab_Value1.Select();
	}

	private void cmb_FansSelect_DropDownClosed(object sender, EventArgs e)
	{
		try
		{
			ClsMain.FansDrop = false;
			ClsMain.FansDropEnd = false;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示FansSelectl异常2:" + ex.Message, "HT");
		}
		lab_Value1.Select();
	}

	private void cmb_FansSelect_DropDown(object sender, EventArgs e)
	{
		try
		{
			ClsMain.FansDrop = true;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示FansSelectl异常1:" + ex.Message, "HT");
		}
	}

	private void label3_Click(object sender, EventArgs e)
	{
	}

	private void cmb_Frequenceselsct_DropDown(object sender, EventArgs e)
	{
		try
		{
			ClsMain.WaterCoolDrop = true;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示WaterCool异常1:" + ex.Message, "HT");
		}
	}

	private void cmb_Frequenceselsct_DropDownClosed(object sender, EventArgs e)
	{
		try
		{
			ClsMain.WaterCoolDrop = false;
			ClsMain.WaterCoolDropEnd = false;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示WaterCool异常2:" + ex.Message, "HT");
		}
		lab_Value1.Select();
	}

	private void cmb_Frequenceselsct_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			WatarSelectIndex = cmb_WaterCoolSelect.SelectedIndex;
			ClsMain.WatarSelectName = cmb_WaterCoolSelect.Text;
			IniFunc.writeString("system", "WatarSelectName", ClsMain.WatarSelectName, ClsMain.iniPath);
			ClsMain.cmbValue_WatarRPM_selectText = ClsMain.WatarSelectName;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示WaterCool异常3:" + ex.Message, "HT");
		}
	}

	private void cbx_ShowCentigrade_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			ClsMain.Showcentigrade_Flag = cbx_ShowCentigrade.Checked;
			IniFunc.writeString("system", "Showcentigrade_Flag", ClsMain.Showcentigrade_Flag ? "1" : "0", ClsMain.iniPath);
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示cbx_ShowCentigrade异常:" + ex.Message, "HT");
		}
	}

	private void cbx_AutoStartEnable_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			ClsMain.AutoStartEnable = cbx_AutoStartEnable.Checked;
			IniFunc.writeString("system", "AutoStartEnable", ClsMain.AutoStartEnable ? "1" : "0", ClsMain.iniPath);
			if (ClsMain.AutoStartEnable)
			{
				string errMsg = "";
				SetMeStartByAutoStartPath(onOff: true, out errMsg);
			}
			else
			{
				string errMsg2 = "";
				SetMeStartByAutoStartPath(onOff: false, out errMsg2);
			}
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示cbx_AutoStartEnable异常:" + ex.Message, "HT");
		}
	}

	private void panel1_Paint(object sender, PaintEventArgs e)
	{
	}

	private void lab_Value1_Click(object sender, EventArgs e)
	{
	}

	private void cmb_CPUUsage_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			CPUUsageSelectIndex = cmb_CPUUsage.SelectedIndex;
			ClsMain.CPUUsageSelectName = cmb_CPUUsage.Text;
			IniFunc.writeString("system", "CPUUsageSelectName", ClsMain.CPUUsageSelectName, ClsMain.iniPath);
			ClsMain.cmbValue_CPUUsage_selectText = ClsMain.CPUUsageSelectName;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示CPUUsage异常1:" + ex.Message, "HT");
		}
	}

	private void cmb_CPUUsage_DropDown(object sender, EventArgs e)
	{
		try
		{
			ClsMain.CPUUsageDrop = true;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示CPUUsage异常2:" + ex.Message, "HT");
		}
	}

	private void cmb_CPUUsage_DropDownClosed(object sender, EventArgs e)
	{
		try
		{
			ClsMain.CPUUsageDrop = false;
			ClsMain.CPUUsageDropEnd = false;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示CPUUsage异常3:" + ex.Message, "HT");
		}
		lab_Value1.Select();
	}

	private void cmb_CPUPower_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			CPUPowerSelectIndex = cmb_CPUPower.SelectedIndex;
			ClsMain.CPUPowerSelectName = cmb_CPUPower.Text;
			IniFunc.writeString("system", "CPUPowerSelectName", ClsMain.CPUPowerSelectName, ClsMain.iniPath);
			ClsMain.cmbValue_CPUPower_selectText = ClsMain.CPUPowerSelectName;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示CPUPower异常1:" + ex.Message, "HT");
		}
	}

	private void cmb_CPUPower_DropDown(object sender, EventArgs e)
	{
		try
		{
			ClsMain.CPUPowerDrop = true;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示CPUPower异常2:" + ex.Message, "HT");
		}
	}

	private void cmb_CPUPower_DropDownClosed(object sender, EventArgs e)
	{
		try
		{
			ClsMain.CPUPowerDrop = false;
			ClsMain.CPUPowerDropEnd = false;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示CPUPower异常3:" + ex.Message, "HT");
		}
		lab_Value1.Select();
	}

	private void cmb_CPUFrequence_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			CPUFrequenceSelectIndex = cmb_CPUFrequence.SelectedIndex;
			ClsMain.CPUFrequenceSelectName = cmb_CPUFrequence.Text;
			IniFunc.writeString("system", "CPUFrequenceSelectName", ClsMain.CPUFrequenceSelectName, ClsMain.iniPath);
			ClsMain.cmbValue_CPUFrequence_selectText = ClsMain.CPUFrequenceSelectName;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示CPUFrequence异常1:" + ex.Message, "HT");
		}
	}

	private void cmb_CPUFrequence_DropDown(object sender, EventArgs e)
	{
		try
		{
			ClsMain.CPUFrequenceDrop = true;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示CPUFrequence异常2:" + ex.Message, "HT");
		}
	}

	private void cmb_CPUFrequence_DropDownClosed(object sender, EventArgs e)
	{
		try
		{
			ClsMain.CPUFrequenceDrop = false;
			ClsMain.CPUFrequenceDropEnd = false;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示CPUFrequence异常3:" + ex.Message, "HT");
		}
		lab_Value1.Select();
	}

	private void cmb_CPUVoltages_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			CPUVoltagesSelectIndex = cmb_CPUVoltages.SelectedIndex;
			ClsMain.CPUVoltagesSelectName = cmb_CPUVoltages.Text;
			IniFunc.writeString("system", "CPUVoltagesSelectName", ClsMain.CPUVoltagesSelectName, ClsMain.iniPath);
			ClsMain.cmbValue_CPUVoltages_selectText = ClsMain.CPUVoltagesSelectName;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示CPUVoltages异常1:" + ex.Message, "HT");
		}
	}

	private void cmb_CPUVoltages_DropDown(object sender, EventArgs e)
	{
		try
		{
			ClsMain.CPUVoltagesDrop = true;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示CPUVoltages异常2:" + ex.Message, "HT");
		}
	}

	private void cmb_CPUVoltages_DropDownClosed(object sender, EventArgs e)
	{
		try
		{
			ClsMain.CPUVoltagesDrop = false;
			ClsMain.CPUVoltagesDropEnd = false;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示CPUVoltages异常3:" + ex.Message, "HT");
		}
		lab_Value1.Select();
	}

	private void cmb_GPUT_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			GPUTempSelectIndex = cmb_GPUT.SelectedIndex;
			ClsMain.GPUTempSelectName = cmb_GPUT.Text;
			IniFunc.writeString("system", "GPUTempSelectName", ClsMain.GPUTempSelectName, ClsMain.iniPath);
			ClsMain.cmbValue_GPUT_selectText = ClsMain.GPUTempSelectName;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示GPUT异常1:" + ex.Message, "HT");
		}
	}

	private void cmb_GPUT_DropDown(object sender, EventArgs e)
	{
		try
		{
			ClsMain.GPUTempDrop = true;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示GPUT异常2:" + ex.Message, "HT");
		}
	}

	private void cmb_GPUT_DropDownClosed(object sender, EventArgs e)
	{
		try
		{
			ClsMain.GPUTempDrop = false;
			ClsMain.GPUTempDropEnd = false;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示GPUT异常3:" + ex.Message, "HT");
		}
		lab_Value1.Select();
	}

	private void cmb_GPUUsage_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			GPUUsageSelectIndex = cmb_GPUUsage.SelectedIndex;
			ClsMain.GPUUsageSelectName = cmb_GPUUsage.Text;
			IniFunc.writeString("system", "GPUUsageSelectName", ClsMain.GPUUsageSelectName, ClsMain.iniPath);
			ClsMain.cmbValue_GPUUsage_selectText = ClsMain.GPUUsageSelectName;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示GPUUsage异常1:" + ex.Message, "HT");
		}
	}

	private void cmb_GPUUsage_DropDown(object sender, EventArgs e)
	{
		try
		{
			ClsMain.GPUUsageDrop = true;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示GPUUsage异常2:" + ex.Message, "HT");
		}
	}

	private void cmb_GPUUsage_DropDownClosed(object sender, EventArgs e)
	{
		try
		{
			ClsMain.GPUUsageDrop = false;
			ClsMain.GPUUsageDropEnd = false;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示GPUUsage异常3:" + ex.Message, "HT");
		}
		lab_Value1.Select();
	}

	private void cmb_GPUPower_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			GPUPowerSelectIndex = cmb_GPUPower.SelectedIndex;
			ClsMain.GPUPowerSelectName = cmb_GPUPower.Text;
			IniFunc.writeString("system", "GPUPowerSelectName", ClsMain.GPUPowerSelectName, ClsMain.iniPath);
			ClsMain.cmbValue_GPUPower_selectText = ClsMain.GPUPowerSelectName;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示GPUPower异常1:" + ex.Message, "HT");
		}
	}

	private void cmb_GPUPower_DropDown(object sender, EventArgs e)
	{
		try
		{
			ClsMain.GPUPowerDrop = true;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示GPUPower异常2:" + ex.Message, "HT");
		}
	}

	private void cmb_GPUPower_DropDownClosed(object sender, EventArgs e)
	{
		try
		{
			ClsMain.GPUPowerDrop = false;
			ClsMain.GPUPowerDropEnd = false;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示GPUPower异常3:" + ex.Message, "HT");
		}
		lab_Value1.Select();
	}

	private void cmb_GPUFrequence_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			GPUFrequenceSelectIndex = cmb_GPUFrequence.SelectedIndex;
			ClsMain.GPUFrequenceSelectName = cmb_GPUFrequence.Text;
			IniFunc.writeString("system", "GPUFrequenceSelectName", ClsMain.GPUFrequenceSelectName, ClsMain.iniPath);
			ClsMain.cmbValue_GPUFrequence_selectText = ClsMain.GPUFrequenceSelectName;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示GPUFrequence异常1:" + ex.Message, "HT");
		}
	}

	private void SetBtnStyle(Button btn)
	{
		btn.FlatStyle = FlatStyle.Flat;
		btn.ForeColor = Color.Transparent;
		btn.BackColor = Color.Transparent;
		btn.FlatAppearance.BorderSize = 0;
		btn.FlatAppearance.MouseOverBackColor = Color.Transparent;
		btn.FlatAppearance.MouseDownBackColor = Color.Transparent;
		btn.Text = "";
	}

	private void panel8_Paint(object sender, PaintEventArgs e)
	{
	}

	private void cmb_GPUFrequence_DropDown(object sender, EventArgs e)
	{
		try
		{
			ClsMain.GPUFrequenceDrop = true;
		}
		catch (Exception ex)
		{
			ClsMain.SetAppendTxt("显示GPUFrequence异常2:" + ex.Message, "HT");
		}
	}

	private void cmb_GPUFrequence_DropDownClosed(object sender, EventArgs e)
	{
		try
		{
			ClsMain.GPUFrequenceDrop = false;
			ClsMain.GPUFrequenceDropEnd = false;
		}
		catch (Exception)
		{
		}
		try
		{
			lab_Value1.Select();
		}
		catch (Exception ex2)
		{
			ClsMain.SetAppendTxt("显示GPUFrequence异常3:" + ex2.Message, "HT");
		}
	}

	private void btn_Min_Click(object sender, EventArgs e)
	{
		try
		{
			base.WindowState = FormWindowState.Minimized;
			base.ShowInTaskbar = false;
			Hide();
			notifyIcon1.Visible = true;
		}
		catch (Exception)
		{
		}
	}

	private void btn_Close_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void panel8_MouseDown(object sender, MouseEventArgs e)
	{
		ReleaseCapture();
		SendMessage(base.Handle, 274, 61458, 0);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ComputereMonitor.frmMain));
		this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.panel8 = new System.Windows.Forms.Panel();
		this.lab_PicRAMUSage = new System.Windows.Forms.Label();
		this.btn_Close = new System.Windows.Forms.Button();
		this.btn_Min = new System.Windows.Forms.Button();
		this.cbx_AutoStartEnable = new System.Windows.Forms.CheckBox();
		this.lab_PicWaterCoolFans = new System.Windows.Forms.Label();
		this.lab_PicFansSpeed = new System.Windows.Forms.Label();
		this.lab_PicGPUPower = new System.Windows.Forms.Label();
		this.cbx_ShowCentigrade = new System.Windows.Forms.CheckBox();
		this.lab_PicCPUPower = new System.Windows.Forms.Label();
		this.lab_PicCPUVoltages = new System.Windows.Forms.Label();
		this.lab_PicGPUUSage = new System.Windows.Forms.Label();
		this.lab_PicCPUUSage = new System.Windows.Forms.Label();
		this.lab_PicGPUFre = new System.Windows.Forms.Label();
		this.lab_PicCPUFre = new System.Windows.Forms.Label();
		this.lab_PicGPUTemp = new System.Windows.Forms.Label();
		this.lab_PicCPUTemp = new System.Windows.Forms.Label();
		this.panel1 = new System.Windows.Forms.Panel();
		this.label12 = new System.Windows.Forms.Label();
		this.lab_MainBoardUsage = new System.Windows.Forms.Label();
		this.lab_MainBoardName = new System.Windows.Forms.Label();
		this.label9 = new System.Windows.Forms.Label();
		this.cmb_GPUFrequence = new System.Windows.Forms.ComboBox();
		this.lab_GPUFrequecne = new System.Windows.Forms.Label();
		this.label19 = new System.Windows.Forms.Label();
		this.label20 = new System.Windows.Forms.Label();
		this.cmb_GPUPower = new System.Windows.Forms.ComboBox();
		this.lab_GPUPower = new System.Windows.Forms.Label();
		this.label22 = new System.Windows.Forms.Label();
		this.label23 = new System.Windows.Forms.Label();
		this.cmb_GPUUsage = new System.Windows.Forms.ComboBox();
		this.lab_GPUTemp = new System.Windows.Forms.Label();
		this.lab_GPUUsage = new System.Windows.Forms.Label();
		this.cmb_GPUT = new System.Windows.Forms.ComboBox();
		this.label26 = new System.Windows.Forms.Label();
		this.label27 = new System.Windows.Forms.Label();
		this.label28 = new System.Windows.Forms.Label();
		this.label29 = new System.Windows.Forms.Label();
		this.cmb_CPUVoltages = new System.Windows.Forms.ComboBox();
		this.lab_CPUVoltages = new System.Windows.Forms.Label();
		this.label16 = new System.Windows.Forms.Label();
		this.label17 = new System.Windows.Forms.Label();
		this.cmb_CPUFrequence = new System.Windows.Forms.ComboBox();
		this.lab_CPUFrequence = new System.Windows.Forms.Label();
		this.label13 = new System.Windows.Forms.Label();
		this.label14 = new System.Windows.Forms.Label();
		this.cmb_CPUPower = new System.Windows.Forms.ComboBox();
		this.lab_CPUPower = new System.Windows.Forms.Label();
		this.label10 = new System.Windows.Forms.Label();
		this.label11 = new System.Windows.Forms.Label();
		this.lab_Value3 = new System.Windows.Forms.Label();
		this.label7 = new System.Windows.Forms.Label();
		this.lab_Value2 = new System.Windows.Forms.Label();
		this.cmb_CPUUsage = new System.Windows.Forms.ComboBox();
		this.lab_FANSRPM = new System.Windows.Forms.Label();
		this.lab_CPUTemp = new System.Windows.Forms.Label();
		this.label8 = new System.Windows.Forms.Label();
		this.lab_CPUUsage = new System.Windows.Forms.Label();
		this.cmb_CPUT = new System.Windows.Forms.ComboBox();
		this.cmb_WaterCoolSelect = new System.Windows.Forms.ComboBox();
		this.lab_WaterCoolFans = new System.Windows.Forms.Label();
		this.label6 = new System.Windows.Forms.Label();
		this.label1 = new System.Windows.Forms.Label();
		this.label4 = new System.Windows.Forms.Label();
		this.cmb_FansSelect = new System.Windows.Forms.ComboBox();
		this.label3 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.label5 = new System.Windows.Forms.Label();
		this.lab_Value1 = new System.Windows.Forms.Label();
		this.lab_Value4 = new System.Windows.Forms.Label();
		this.contextMenuStrip1.SuspendLayout();
		this.panel8.SuspendLayout();
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
		this.notifyIcon1.Icon = (System.Drawing.Icon)resources.GetObject("notifyIcon1.Icon");
		this.notifyIcon1.Text = "PC MonitoringV2.3";
		this.notifyIcon1.Visible = true;
		this.notifyIcon1.DoubleClick += new System.EventHandler(notifyIcon1_DoubleClick);
		this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.toolStripMenuItem1 });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(142, 26);
		this.contextMenuStrip1.Text = "close";
		this.toolStripMenuItem1.Name = "toolStripMenuItem1";
		this.toolStripMenuItem1.Size = new System.Drawing.Size(141, 22);
		this.toolStripMenuItem1.Text = "CloseHWM";
		this.toolStripMenuItem1.Click += new System.EventHandler(toolStripMenuItem1_Click);
		this.timer1.Interval = 500;
		this.panel8.BackgroundImage = (System.Drawing.Image)resources.GetObject("panel8.BackgroundImage");
		this.panel8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
		this.panel8.Controls.Add(this.lab_PicRAMUSage);
		this.panel8.Controls.Add(this.btn_Close);
		this.panel8.Controls.Add(this.btn_Min);
		this.panel8.Controls.Add(this.cbx_AutoStartEnable);
		this.panel8.Controls.Add(this.lab_PicWaterCoolFans);
		this.panel8.Controls.Add(this.lab_PicFansSpeed);
		this.panel8.Controls.Add(this.lab_PicGPUPower);
		this.panel8.Controls.Add(this.cbx_ShowCentigrade);
		this.panel8.Controls.Add(this.lab_PicCPUPower);
		this.panel8.Controls.Add(this.lab_PicCPUVoltages);
		this.panel8.Controls.Add(this.lab_PicGPUUSage);
		this.panel8.Controls.Add(this.lab_PicCPUUSage);
		this.panel8.Controls.Add(this.lab_PicGPUFre);
		this.panel8.Controls.Add(this.lab_PicCPUFre);
		this.panel8.Controls.Add(this.lab_PicGPUTemp);
		this.panel8.Controls.Add(this.lab_PicCPUTemp);
		this.panel8.Controls.Add(this.panel1);
		this.panel8.Controls.Add(this.lab_Value1);
		this.panel8.Controls.Add(this.lab_Value4);
		this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel8.Location = new System.Drawing.Point(0, 0);
		this.panel8.Name = "panel8";
		this.panel8.Size = new System.Drawing.Size(700, 989);
		this.panel8.TabIndex = 17;
		this.panel8.Paint += new System.Windows.Forms.PaintEventHandler(panel8_Paint);
		this.panel8.MouseDown += new System.Windows.Forms.MouseEventHandler(panel8_MouseDown);
		this.lab_PicRAMUSage.AutoSize = true;
		this.lab_PicRAMUSage.BackColor = System.Drawing.Color.Transparent;
		this.lab_PicRAMUSage.Font = new System.Drawing.Font("思源黑体", 18f);
		this.lab_PicRAMUSage.Location = new System.Drawing.Point(347, 626);
		this.lab_PicRAMUSage.Name = "lab_PicRAMUSage";
		this.lab_PicRAMUSage.Size = new System.Drawing.Size(63, 35);
		this.lab_PicRAMUSage.TabIndex = 52;
		this.lab_PicRAMUSage.Text = "00%";
		this.btn_Close.BackColor = System.Drawing.Color.Transparent;
		this.btn_Close.ForeColor = System.Drawing.Color.Black;
		this.btn_Close.Location = new System.Drawing.Point(661, 3);
		this.btn_Close.Name = "btn_Close";
		this.btn_Close.Size = new System.Drawing.Size(38, 22);
		this.btn_Close.TabIndex = 51;
		this.btn_Close.Text = "Close";
		this.btn_Close.UseVisualStyleBackColor = false;
		this.btn_Close.Click += new System.EventHandler(btn_Close_Click);
		this.btn_Min.BackColor = System.Drawing.Color.Transparent;
		this.btn_Min.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		this.btn_Min.ForeColor = System.Drawing.Color.Black;
		this.btn_Min.Location = new System.Drawing.Point(623, 3);
		this.btn_Min.Name = "btn_Min";
		this.btn_Min.Size = new System.Drawing.Size(36, 22);
		this.btn_Min.TabIndex = 50;
		this.btn_Min.Text = "Min";
		this.btn_Min.UseVisualStyleBackColor = false;
		this.btn_Min.Click += new System.EventHandler(btn_Min_Click);
		this.cbx_AutoStartEnable.AutoSize = true;
		this.cbx_AutoStartEnable.BackColor = System.Drawing.Color.Transparent;
		this.cbx_AutoStartEnable.Location = new System.Drawing.Point(513, 171);
		this.cbx_AutoStartEnable.Name = "cbx_AutoStartEnable";
		this.cbx_AutoStartEnable.Size = new System.Drawing.Size(168, 16);
		this.cbx_AutoStartEnable.TabIndex = 23;
		this.cbx_AutoStartEnable.Text = "Auto-Start at Pc Startup";
		this.cbx_AutoStartEnable.UseVisualStyleBackColor = false;
		this.cbx_AutoStartEnable.CheckedChanged += new System.EventHandler(cbx_AutoStartEnable_CheckedChanged);
		this.lab_PicWaterCoolFans.AutoSize = true;
		this.lab_PicWaterCoolFans.BackColor = System.Drawing.Color.Transparent;
		this.lab_PicWaterCoolFans.Font = new System.Drawing.Font("思源黑体", 18f);
		this.lab_PicWaterCoolFans.Location = new System.Drawing.Point(563, 626);
		this.lab_PicWaterCoolFans.Name = "lab_PicWaterCoolFans";
		this.lab_PicWaterCoolFans.Size = new System.Drawing.Size(116, 35);
		this.lab_PicWaterCoolFans.TabIndex = 38;
		this.lab_PicWaterCoolFans.Text = "0000RPM";
		this.lab_PicFansSpeed.AutoSize = true;
		this.lab_PicFansSpeed.BackColor = System.Drawing.Color.Transparent;
		this.lab_PicFansSpeed.Font = new System.Drawing.Font("思源黑体", 18f);
		this.lab_PicFansSpeed.Location = new System.Drawing.Point(563, 380);
		this.lab_PicFansSpeed.Name = "lab_PicFansSpeed";
		this.lab_PicFansSpeed.Size = new System.Drawing.Size(116, 35);
		this.lab_PicFansSpeed.TabIndex = 37;
		this.lab_PicFansSpeed.Text = "0000RPM";
		this.lab_PicGPUPower.AutoSize = true;
		this.lab_PicGPUPower.BackColor = System.Drawing.Color.Transparent;
		this.lab_PicGPUPower.Font = new System.Drawing.Font("思源黑体", 18f);
		this.lab_PicGPUPower.Location = new System.Drawing.Point(574, 509);
		this.lab_PicGPUPower.Name = "lab_PicGPUPower";
		this.lab_PicGPUPower.Size = new System.Drawing.Size(82, 35);
		this.lab_PicGPUPower.TabIndex = 36;
		this.lab_PicGPUPower.Text = "00.0W";
		this.cbx_ShowCentigrade.AutoSize = true;
		this.cbx_ShowCentigrade.BackColor = System.Drawing.Color.Transparent;
		this.cbx_ShowCentigrade.Location = new System.Drawing.Point(513, 149);
		this.cbx_ShowCentigrade.Name = "cbx_ShowCentigrade";
		this.cbx_ShowCentigrade.Size = new System.Drawing.Size(138, 16);
		this.cbx_ShowCentigrade.TabIndex = 22;
		this.cbx_ShowCentigrade.Text = "Show Centigrade(℃)";
		this.cbx_ShowCentigrade.UseVisualStyleBackColor = false;
		this.cbx_ShowCentigrade.CheckedChanged += new System.EventHandler(cbx_ShowCentigrade_CheckedChanged);
		this.lab_PicCPUPower.AutoSize = true;
		this.lab_PicCPUPower.BackColor = System.Drawing.Color.Transparent;
		this.lab_PicCPUPower.Font = new System.Drawing.Font("思源黑体", 18f);
		this.lab_PicCPUPower.Location = new System.Drawing.Point(574, 261);
		this.lab_PicCPUPower.Name = "lab_PicCPUPower";
		this.lab_PicCPUPower.Size = new System.Drawing.Size(82, 35);
		this.lab_PicCPUPower.TabIndex = 35;
		this.lab_PicCPUPower.Text = "00.0W";
		this.lab_PicCPUVoltages.AutoSize = true;
		this.lab_PicCPUVoltages.BackColor = System.Drawing.Color.Transparent;
		this.lab_PicCPUVoltages.Font = new System.Drawing.Font("思源黑体", 18f);
		this.lab_PicCPUVoltages.Location = new System.Drawing.Point(347, 380);
		this.lab_PicCPUVoltages.Name = "lab_PicCPUVoltages";
		this.lab_PicCPUVoltages.Size = new System.Drawing.Size(75, 35);
		this.lab_PicCPUVoltages.TabIndex = 34;
		this.lab_PicCPUVoltages.Text = "00.0V";
		this.lab_PicGPUUSage.AutoSize = true;
		this.lab_PicGPUUSage.BackColor = System.Drawing.Color.Transparent;
		this.lab_PicGPUUSage.Font = new System.Drawing.Font("思源黑体", 18f);
		this.lab_PicGPUUSage.Location = new System.Drawing.Point(347, 509);
		this.lab_PicGPUUSage.Name = "lab_PicGPUUSage";
		this.lab_PicGPUUSage.Size = new System.Drawing.Size(63, 35);
		this.lab_PicGPUUSage.TabIndex = 33;
		this.lab_PicGPUUSage.Text = "00%";
		this.lab_PicCPUUSage.AutoSize = true;
		this.lab_PicCPUUSage.BackColor = System.Drawing.Color.Transparent;
		this.lab_PicCPUUSage.Font = new System.Drawing.Font("思源黑体", 18f);
		this.lab_PicCPUUSage.Location = new System.Drawing.Point(347, 261);
		this.lab_PicCPUUSage.Name = "lab_PicCPUUSage";
		this.lab_PicCPUUSage.Size = new System.Drawing.Size(63, 35);
		this.lab_PicCPUUSage.TabIndex = 32;
		this.lab_PicCPUUSage.Text = "00%";
		this.lab_PicGPUFre.AutoSize = true;
		this.lab_PicGPUFre.BackColor = System.Drawing.Color.Transparent;
		this.lab_PicGPUFre.Font = new System.Drawing.Font("思源黑体", 18f);
		this.lab_PicGPUFre.Location = new System.Drawing.Point(90, 626);
		this.lab_PicGPUFre.Name = "lab_PicGPUFre";
		this.lab_PicGPUFre.Size = new System.Drawing.Size(117, 35);
		this.lab_PicGPUFre.TabIndex = 31;
		this.lab_PicGPUFre.Text = "0000MHZ";
		this.lab_PicCPUFre.AutoSize = true;
		this.lab_PicCPUFre.BackColor = System.Drawing.Color.Transparent;
		this.lab_PicCPUFre.Font = new System.Drawing.Font("思源黑体", 18f);
		this.lab_PicCPUFre.Location = new System.Drawing.Point(90, 380);
		this.lab_PicCPUFre.Name = "lab_PicCPUFre";
		this.lab_PicCPUFre.Size = new System.Drawing.Size(117, 35);
		this.lab_PicCPUFre.TabIndex = 30;
		this.lab_PicCPUFre.Text = "0000MHZ";
		this.lab_PicGPUTemp.AutoSize = true;
		this.lab_PicGPUTemp.BackColor = System.Drawing.Color.Transparent;
		this.lab_PicGPUTemp.Font = new System.Drawing.Font("思源黑体", 18f);
		this.lab_PicGPUTemp.Location = new System.Drawing.Point(108, 509);
		this.lab_PicGPUTemp.Name = "lab_PicGPUTemp";
		this.lab_PicGPUTemp.Size = new System.Drawing.Size(78, 35);
		this.lab_PicGPUTemp.TabIndex = 29;
		this.lab_PicGPUTemp.Text = "000℃";
		this.lab_PicCPUTemp.AutoSize = true;
		this.lab_PicCPUTemp.BackColor = System.Drawing.Color.Transparent;
		this.lab_PicCPUTemp.Font = new System.Drawing.Font("思源黑体", 18f);
		this.lab_PicCPUTemp.Location = new System.Drawing.Point(108, 261);
		this.lab_PicCPUTemp.Name = "lab_PicCPUTemp";
		this.lab_PicCPUTemp.Size = new System.Drawing.Size(78, 35);
		this.lab_PicCPUTemp.TabIndex = 28;
		this.lab_PicCPUTemp.Text = "000℃";
		this.panel1.BackColor = System.Drawing.Color.Transparent;
		this.panel1.Controls.Add(this.label12);
		this.panel1.Controls.Add(this.lab_MainBoardUsage);
		this.panel1.Controls.Add(this.lab_MainBoardName);
		this.panel1.Controls.Add(this.label9);
		this.panel1.Controls.Add(this.cmb_GPUFrequence);
		this.panel1.Controls.Add(this.lab_GPUFrequecne);
		this.panel1.Controls.Add(this.label19);
		this.panel1.Controls.Add(this.label20);
		this.panel1.Controls.Add(this.cmb_GPUPower);
		this.panel1.Controls.Add(this.lab_GPUPower);
		this.panel1.Controls.Add(this.label22);
		this.panel1.Controls.Add(this.label23);
		this.panel1.Controls.Add(this.cmb_GPUUsage);
		this.panel1.Controls.Add(this.lab_GPUTemp);
		this.panel1.Controls.Add(this.lab_GPUUsage);
		this.panel1.Controls.Add(this.cmb_GPUT);
		this.panel1.Controls.Add(this.label26);
		this.panel1.Controls.Add(this.label27);
		this.panel1.Controls.Add(this.label28);
		this.panel1.Controls.Add(this.label29);
		this.panel1.Controls.Add(this.cmb_CPUVoltages);
		this.panel1.Controls.Add(this.lab_CPUVoltages);
		this.panel1.Controls.Add(this.label16);
		this.panel1.Controls.Add(this.label17);
		this.panel1.Controls.Add(this.cmb_CPUFrequence);
		this.panel1.Controls.Add(this.lab_CPUFrequence);
		this.panel1.Controls.Add(this.label13);
		this.panel1.Controls.Add(this.label14);
		this.panel1.Controls.Add(this.cmb_CPUPower);
		this.panel1.Controls.Add(this.lab_CPUPower);
		this.panel1.Controls.Add(this.label10);
		this.panel1.Controls.Add(this.label11);
		this.panel1.Controls.Add(this.lab_Value3);
		this.panel1.Controls.Add(this.label7);
		this.panel1.Controls.Add(this.lab_Value2);
		this.panel1.Controls.Add(this.cmb_CPUUsage);
		this.panel1.Controls.Add(this.lab_FANSRPM);
		this.panel1.Controls.Add(this.lab_CPUTemp);
		this.panel1.Controls.Add(this.label8);
		this.panel1.Controls.Add(this.lab_CPUUsage);
		this.panel1.Controls.Add(this.cmb_CPUT);
		this.panel1.Controls.Add(this.cmb_WaterCoolSelect);
		this.panel1.Controls.Add(this.lab_WaterCoolFans);
		this.panel1.Controls.Add(this.label6);
		this.panel1.Controls.Add(this.label1);
		this.panel1.Controls.Add(this.label4);
		this.panel1.Controls.Add(this.cmb_FansSelect);
		this.panel1.Controls.Add(this.label3);
		this.panel1.Controls.Add(this.label2);
		this.panel1.Controls.Add(this.label5);
		this.panel1.Location = new System.Drawing.Point(5, 689);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(694, 306);
		this.panel1.TabIndex = 27;
		this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(panel1_Paint);
		this.label12.AutoSize = true;
		this.label12.BackColor = System.Drawing.Color.Transparent;
		this.label12.Location = new System.Drawing.Point(559, 277);
		this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.label12.Name = "label12";
		this.label12.Size = new System.Drawing.Size(65, 12);
		this.label12.TabIndex = 58;
		this.label12.Text = "RAM Usage:";
		this.lab_MainBoardUsage.AutoSize = true;
		this.lab_MainBoardUsage.BackColor = System.Drawing.Color.Transparent;
		this.lab_MainBoardUsage.Location = new System.Drawing.Point(624, 277);
		this.lab_MainBoardUsage.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.lab_MainBoardUsage.Name = "lab_MainBoardUsage";
		this.lab_MainBoardUsage.Size = new System.Drawing.Size(23, 12);
		this.lab_MainBoardUsage.TabIndex = 57;
		this.lab_MainBoardUsage.Text = "00%";
		this.lab_MainBoardName.AutoSize = true;
		this.lab_MainBoardName.BackColor = System.Drawing.Color.Transparent;
		this.lab_MainBoardName.Location = new System.Drawing.Point(126, 277);
		this.lab_MainBoardName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.lab_MainBoardName.Name = "lab_MainBoardName";
		this.lab_MainBoardName.Size = new System.Drawing.Size(83, 12);
		this.lab_MainBoardName.TabIndex = 56;
		this.lab_MainBoardName.Text = "MainBoardName";
		this.label9.AutoSize = true;
		this.label9.BackColor = System.Drawing.Color.Transparent;
		this.label9.Location = new System.Drawing.Point(13, 277);
		this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.label9.Name = "label9";
		this.label9.Size = new System.Drawing.Size(89, 12);
		this.label9.TabIndex = 55;
		this.label9.Text = "RAM/MainBoard:";
		this.cmb_GPUFrequence.FormattingEnabled = true;
		this.cmb_GPUFrequence.Location = new System.Drawing.Point(123, 230);
		this.cmb_GPUFrequence.Margin = new System.Windows.Forms.Padding(2);
		this.cmb_GPUFrequence.Name = "cmb_GPUFrequence";
		this.cmb_GPUFrequence.Size = new System.Drawing.Size(391, 20);
		this.cmb_GPUFrequence.TabIndex = 52;
		this.cmb_GPUFrequence.DropDown += new System.EventHandler(cmb_GPUFrequence_DropDown);
		this.cmb_GPUFrequence.SelectedIndexChanged += new System.EventHandler(cmb_GPUFrequence_SelectedIndexChanged);
		this.cmb_GPUFrequence.DropDownClosed += new System.EventHandler(cmb_GPUFrequence_DropDownClosed);
		this.lab_GPUFrequecne.AutoSize = true;
		this.lab_GPUFrequecne.BackColor = System.Drawing.Color.Transparent;
		this.lab_GPUFrequecne.Font = new System.Drawing.Font("宋体", 9.75f);
		this.lab_GPUFrequecne.Location = new System.Drawing.Point(624, 233);
		this.lab_GPUFrequecne.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.lab_GPUFrequecne.Name = "lab_GPUFrequecne";
		this.lab_GPUFrequecne.Size = new System.Drawing.Size(56, 13);
		this.lab_GPUFrequecne.TabIndex = 51;
		this.lab_GPUFrequecne.Text = "0000MHZ";
		this.label19.AutoSize = true;
		this.label19.BackColor = System.Drawing.Color.Transparent;
		this.label19.Location = new System.Drawing.Point(535, 233);
		this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.label19.Name = "label19";
		this.label19.Size = new System.Drawing.Size(89, 12);
		this.label19.TabIndex = 54;
		this.label19.Text = "GPU Frequence:";
		this.label20.AutoSize = true;
		this.label20.BackColor = System.Drawing.Color.Transparent;
		this.label20.Location = new System.Drawing.Point(31, 233);
		this.label20.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.label20.Name = "label20";
		this.label20.Size = new System.Drawing.Size(71, 12);
		this.label20.TabIndex = 53;
		this.label20.Text = "GPU Select:";
		this.cmb_GPUPower.FormattingEnabled = true;
		this.cmb_GPUPower.Location = new System.Drawing.Point(123, 206);
		this.cmb_GPUPower.Margin = new System.Windows.Forms.Padding(2);
		this.cmb_GPUPower.Name = "cmb_GPUPower";
		this.cmb_GPUPower.Size = new System.Drawing.Size(391, 20);
		this.cmb_GPUPower.TabIndex = 48;
		this.cmb_GPUPower.DropDown += new System.EventHandler(cmb_GPUPower_DropDown);
		this.cmb_GPUPower.SelectedIndexChanged += new System.EventHandler(cmb_GPUPower_SelectedIndexChanged);
		this.cmb_GPUPower.DropDownClosed += new System.EventHandler(cmb_GPUPower_DropDownClosed);
		this.lab_GPUPower.AutoSize = true;
		this.lab_GPUPower.BackColor = System.Drawing.Color.Transparent;
		this.lab_GPUPower.Font = new System.Drawing.Font("宋体", 9.75f);
		this.lab_GPUPower.Location = new System.Drawing.Point(624, 209);
		this.lab_GPUPower.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.lab_GPUPower.Name = "lab_GPUPower";
		this.lab_GPUPower.Size = new System.Drawing.Size(42, 13);
		this.lab_GPUPower.TabIndex = 47;
		this.lab_GPUPower.Text = "00.0W";
		this.label22.AutoSize = true;
		this.label22.BackColor = System.Drawing.Color.Transparent;
		this.label22.Location = new System.Drawing.Point(559, 209);
		this.label22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.label22.Name = "label22";
		this.label22.Size = new System.Drawing.Size(65, 12);
		this.label22.TabIndex = 50;
		this.label22.Text = "GPU Power:";
		this.label23.AutoSize = true;
		this.label23.BackColor = System.Drawing.Color.Transparent;
		this.label23.Location = new System.Drawing.Point(31, 209);
		this.label23.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.label23.Name = "label23";
		this.label23.Size = new System.Drawing.Size(71, 12);
		this.label23.TabIndex = 49;
		this.label23.Text = "GPU Select:";
		this.cmb_GPUUsage.FormattingEnabled = true;
		this.cmb_GPUUsage.Location = new System.Drawing.Point(123, 182);
		this.cmb_GPUUsage.Margin = new System.Windows.Forms.Padding(2);
		this.cmb_GPUUsage.Name = "cmb_GPUUsage";
		this.cmb_GPUUsage.Size = new System.Drawing.Size(391, 20);
		this.cmb_GPUUsage.TabIndex = 42;
		this.cmb_GPUUsage.DropDown += new System.EventHandler(cmb_GPUUsage_DropDown);
		this.cmb_GPUUsage.SelectedIndexChanged += new System.EventHandler(cmb_GPUUsage_SelectedIndexChanged);
		this.cmb_GPUUsage.DropDownClosed += new System.EventHandler(cmb_GPUUsage_DropDownClosed);
		this.lab_GPUTemp.AutoSize = true;
		this.lab_GPUTemp.BackColor = System.Drawing.Color.Transparent;
		this.lab_GPUTemp.Font = new System.Drawing.Font("宋体", 9.75f);
		this.lab_GPUTemp.Location = new System.Drawing.Point(624, 162);
		this.lab_GPUTemp.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.lab_GPUTemp.Name = "lab_GPUTemp";
		this.lab_GPUTemp.Size = new System.Drawing.Size(41, 13);
		this.lab_GPUTemp.TabIndex = 39;
		this.lab_GPUTemp.Text = "37°C";
		this.lab_GPUUsage.AutoSize = true;
		this.lab_GPUUsage.BackColor = System.Drawing.Color.Transparent;
		this.lab_GPUUsage.Font = new System.Drawing.Font("宋体", 9.75f);
		this.lab_GPUUsage.Location = new System.Drawing.Point(624, 185);
		this.lab_GPUUsage.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.lab_GPUUsage.Name = "lab_GPUUsage";
		this.lab_GPUUsage.Size = new System.Drawing.Size(28, 13);
		this.lab_GPUUsage.TabIndex = 41;
		this.lab_GPUUsage.Text = "00%";
		this.cmb_GPUT.FormattingEnabled = true;
		this.cmb_GPUT.Location = new System.Drawing.Point(123, 158);
		this.cmb_GPUT.Margin = new System.Windows.Forms.Padding(2);
		this.cmb_GPUT.Name = "cmb_GPUT";
		this.cmb_GPUT.Size = new System.Drawing.Size(391, 20);
		this.cmb_GPUT.TabIndex = 40;
		this.cmb_GPUT.DropDown += new System.EventHandler(cmb_GPUT_DropDown);
		this.cmb_GPUT.SelectedIndexChanged += new System.EventHandler(cmb_GPUT_SelectedIndexChanged);
		this.cmb_GPUT.DropDownClosed += new System.EventHandler(cmb_GPUT_DropDownClosed);
		this.label26.AutoSize = true;
		this.label26.BackColor = System.Drawing.Color.Transparent;
		this.label26.Location = new System.Drawing.Point(523, 162);
		this.label26.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.label26.Name = "label26";
		this.label26.Size = new System.Drawing.Size(101, 12);
		this.label26.TabIndex = 45;
		this.label26.Text = "GPU Temperature:";
		this.label27.AutoSize = true;
		this.label27.BackColor = System.Drawing.Color.Transparent;
		this.label27.Location = new System.Drawing.Point(31, 162);
		this.label27.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.label27.Name = "label27";
		this.label27.Size = new System.Drawing.Size(71, 12);
		this.label27.TabIndex = 43;
		this.label27.Text = "GPU Select:";
		this.label28.AutoSize = true;
		this.label28.BackColor = System.Drawing.Color.Transparent;
		this.label28.Location = new System.Drawing.Point(559, 185);
		this.label28.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.label28.Name = "label28";
		this.label28.Size = new System.Drawing.Size(65, 12);
		this.label28.TabIndex = 46;
		this.label28.Text = "GPU Usage:";
		this.label29.AutoSize = true;
		this.label29.BackColor = System.Drawing.Color.Transparent;
		this.label29.Location = new System.Drawing.Point(31, 185);
		this.label29.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.label29.Name = "label29";
		this.label29.Size = new System.Drawing.Size(71, 12);
		this.label29.TabIndex = 44;
		this.label29.Text = "GPU Select:";
		this.cmb_CPUVoltages.FormattingEnabled = true;
		this.cmb_CPUVoltages.Location = new System.Drawing.Point(123, 110);
		this.cmb_CPUVoltages.Margin = new System.Windows.Forms.Padding(2);
		this.cmb_CPUVoltages.Name = "cmb_CPUVoltages";
		this.cmb_CPUVoltages.Size = new System.Drawing.Size(391, 20);
		this.cmb_CPUVoltages.TabIndex = 36;
		this.cmb_CPUVoltages.DropDown += new System.EventHandler(cmb_CPUVoltages_DropDown);
		this.cmb_CPUVoltages.SelectedIndexChanged += new System.EventHandler(cmb_CPUVoltages_SelectedIndexChanged);
		this.cmb_CPUVoltages.DropDownClosed += new System.EventHandler(cmb_CPUVoltages_DropDownClosed);
		this.lab_CPUVoltages.AutoSize = true;
		this.lab_CPUVoltages.BackColor = System.Drawing.Color.Transparent;
		this.lab_CPUVoltages.Font = new System.Drawing.Font("宋体", 9.75f);
		this.lab_CPUVoltages.Location = new System.Drawing.Point(624, 113);
		this.lab_CPUVoltages.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.lab_CPUVoltages.Name = "lab_CPUVoltages";
		this.lab_CPUVoltages.Size = new System.Drawing.Size(42, 13);
		this.lab_CPUVoltages.TabIndex = 35;
		this.lab_CPUVoltages.Text = "00.0V";
		this.label16.AutoSize = true;
		this.label16.BackColor = System.Drawing.Color.Transparent;
		this.label16.Location = new System.Drawing.Point(541, 113);
		this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.label16.Name = "label16";
		this.label16.Size = new System.Drawing.Size(83, 12);
		this.label16.TabIndex = 38;
		this.label16.Text = "CPU Voltages:";
		this.label17.AutoSize = true;
		this.label17.BackColor = System.Drawing.Color.Transparent;
		this.label17.Location = new System.Drawing.Point(31, 113);
		this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.label17.Name = "label17";
		this.label17.Size = new System.Drawing.Size(71, 12);
		this.label17.TabIndex = 37;
		this.label17.Text = "CPU Select:";
		this.cmb_CPUFrequence.FormattingEnabled = true;
		this.cmb_CPUFrequence.Location = new System.Drawing.Point(123, 86);
		this.cmb_CPUFrequence.Margin = new System.Windows.Forms.Padding(2);
		this.cmb_CPUFrequence.Name = "cmb_CPUFrequence";
		this.cmb_CPUFrequence.Size = new System.Drawing.Size(391, 20);
		this.cmb_CPUFrequence.TabIndex = 32;
		this.cmb_CPUFrequence.DropDown += new System.EventHandler(cmb_CPUFrequence_DropDown);
		this.cmb_CPUFrequence.SelectedIndexChanged += new System.EventHandler(cmb_CPUFrequence_SelectedIndexChanged);
		this.cmb_CPUFrequence.DropDownClosed += new System.EventHandler(cmb_CPUFrequence_DropDownClosed);
		this.lab_CPUFrequence.AutoSize = true;
		this.lab_CPUFrequence.BackColor = System.Drawing.Color.Transparent;
		this.lab_CPUFrequence.Font = new System.Drawing.Font("宋体", 9.75f);
		this.lab_CPUFrequence.Location = new System.Drawing.Point(624, 89);
		this.lab_CPUFrequence.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.lab_CPUFrequence.Name = "lab_CPUFrequence";
		this.lab_CPUFrequence.Size = new System.Drawing.Size(56, 13);
		this.lab_CPUFrequence.TabIndex = 31;
		this.lab_CPUFrequence.Text = "0000MHZ";
		this.label13.AutoSize = true;
		this.label13.BackColor = System.Drawing.Color.Transparent;
		this.label13.Location = new System.Drawing.Point(535, 89);
		this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.label13.Name = "label13";
		this.label13.Size = new System.Drawing.Size(89, 12);
		this.label13.TabIndex = 34;
		this.label13.Text = "CPU Frequence:";
		this.label14.AutoSize = true;
		this.label14.BackColor = System.Drawing.Color.Transparent;
		this.label14.Location = new System.Drawing.Point(31, 89);
		this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.label14.Name = "label14";
		this.label14.Size = new System.Drawing.Size(71, 12);
		this.label14.TabIndex = 33;
		this.label14.Text = "CPU Select:";
		this.cmb_CPUPower.FormattingEnabled = true;
		this.cmb_CPUPower.Location = new System.Drawing.Point(123, 62);
		this.cmb_CPUPower.Margin = new System.Windows.Forms.Padding(2);
		this.cmb_CPUPower.Name = "cmb_CPUPower";
		this.cmb_CPUPower.Size = new System.Drawing.Size(391, 20);
		this.cmb_CPUPower.TabIndex = 28;
		this.cmb_CPUPower.DropDown += new System.EventHandler(cmb_CPUPower_DropDown);
		this.cmb_CPUPower.SelectedIndexChanged += new System.EventHandler(cmb_CPUPower_SelectedIndexChanged);
		this.cmb_CPUPower.DropDownClosed += new System.EventHandler(cmb_CPUPower_DropDownClosed);
		this.lab_CPUPower.AutoSize = true;
		this.lab_CPUPower.BackColor = System.Drawing.Color.Transparent;
		this.lab_CPUPower.Font = new System.Drawing.Font("宋体", 9.75f);
		this.lab_CPUPower.Location = new System.Drawing.Point(624, 65);
		this.lab_CPUPower.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.lab_CPUPower.Name = "lab_CPUPower";
		this.lab_CPUPower.Size = new System.Drawing.Size(42, 13);
		this.lab_CPUPower.TabIndex = 27;
		this.lab_CPUPower.Text = "00.0W";
		this.label10.AutoSize = true;
		this.label10.BackColor = System.Drawing.Color.Transparent;
		this.label10.Location = new System.Drawing.Point(559, 65);
		this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.label10.Name = "label10";
		this.label10.Size = new System.Drawing.Size(65, 12);
		this.label10.TabIndex = 30;
		this.label10.Text = "CPU Power:";
		this.label11.AutoSize = true;
		this.label11.BackColor = System.Drawing.Color.Transparent;
		this.label11.Location = new System.Drawing.Point(31, 65);
		this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.label11.Name = "label11";
		this.label11.Size = new System.Drawing.Size(71, 12);
		this.label11.TabIndex = 29;
		this.label11.Text = "CPU Select:";
		this.lab_Value3.AutoSize = true;
		this.lab_Value3.BackColor = System.Drawing.Color.Transparent;
		this.lab_Value3.Font = new System.Drawing.Font("思源黑体", 36f);
		this.lab_Value3.Location = new System.Drawing.Point(616, 274);
		this.lab_Value3.Name = "lab_Value3";
		this.lab_Value3.Size = new System.Drawing.Size(128, 70);
		this.lab_Value3.TabIndex = 6;
		this.lab_Value3.Text = "00%";
		this.lab_Value3.Visible = false;
		this.label7.AutoSize = true;
		this.label7.BackColor = System.Drawing.Color.Transparent;
		this.label7.Location = new System.Drawing.Point(565, 255);
		this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(59, 12);
		this.label7.TabIndex = 25;
		this.label7.Text = "Fans RPM:";
		this.lab_Value2.AutoSize = true;
		this.lab_Value2.BackColor = System.Drawing.Color.Transparent;
		this.lab_Value2.Font = new System.Drawing.Font("思源黑体", 36f);
		this.lab_Value2.Location = new System.Drawing.Point(606, 274);
		this.lab_Value2.Name = "lab_Value2";
		this.lab_Value2.Size = new System.Drawing.Size(138, 70);
		this.lab_Value2.TabIndex = 5;
		this.lab_Value2.Text = "0000";
		this.lab_Value2.Visible = false;
		this.cmb_CPUUsage.FormattingEnabled = true;
		this.cmb_CPUUsage.Location = new System.Drawing.Point(123, 38);
		this.cmb_CPUUsage.Margin = new System.Windows.Forms.Padding(2);
		this.cmb_CPUUsage.Name = "cmb_CPUUsage";
		this.cmb_CPUUsage.Size = new System.Drawing.Size(391, 20);
		this.cmb_CPUUsage.TabIndex = 15;
		this.cmb_CPUUsage.DropDown += new System.EventHandler(cmb_CPUUsage_DropDown);
		this.cmb_CPUUsage.SelectedIndexChanged += new System.EventHandler(cmb_CPUUsage_SelectedIndexChanged);
		this.cmb_CPUUsage.DropDownClosed += new System.EventHandler(cmb_CPUUsage_DropDownClosed);
		this.lab_FANSRPM.AutoSize = true;
		this.lab_FANSRPM.BackColor = System.Drawing.Color.Transparent;
		this.lab_FANSRPM.Font = new System.Drawing.Font("宋体", 9.75f);
		this.lab_FANSRPM.Location = new System.Drawing.Point(624, 137);
		this.lab_FANSRPM.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.lab_FANSRPM.Name = "lab_FANSRPM";
		this.lab_FANSRPM.Size = new System.Drawing.Size(56, 13);
		this.lab_FANSRPM.TabIndex = 11;
		this.lab_FANSRPM.Text = "0000RPM";
		this.lab_CPUTemp.AutoSize = true;
		this.lab_CPUTemp.BackColor = System.Drawing.Color.Transparent;
		this.lab_CPUTemp.Font = new System.Drawing.Font("宋体", 9.75f);
		this.lab_CPUTemp.Location = new System.Drawing.Point(624, 18);
		this.lab_CPUTemp.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.lab_CPUTemp.Name = "lab_CPUTemp";
		this.lab_CPUTemp.Size = new System.Drawing.Size(48, 13);
		this.lab_CPUTemp.TabIndex = 6;
		this.lab_CPUTemp.Text = "000°C";
		this.label8.AutoSize = true;
		this.label8.BackColor = System.Drawing.Color.Transparent;
		this.label8.Location = new System.Drawing.Point(1, 255);
		this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.label8.Name = "label8";
		this.label8.Size = new System.Drawing.Size(101, 12);
		this.label8.TabIndex = 24;
		this.label8.Text = "WaterCoolSelect:";
		this.lab_CPUUsage.AutoSize = true;
		this.lab_CPUUsage.BackColor = System.Drawing.Color.Transparent;
		this.lab_CPUUsage.Font = new System.Drawing.Font("宋体", 9.75f);
		this.lab_CPUUsage.Location = new System.Drawing.Point(624, 41);
		this.lab_CPUUsage.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.lab_CPUUsage.Name = "lab_CPUUsage";
		this.lab_CPUUsage.Size = new System.Drawing.Size(28, 13);
		this.lab_CPUUsage.TabIndex = 13;
		this.lab_CPUUsage.Text = "00%";
		this.cmb_CPUT.FormattingEnabled = true;
		this.cmb_CPUT.Location = new System.Drawing.Point(123, 14);
		this.cmb_CPUT.Margin = new System.Windows.Forms.Padding(2);
		this.cmb_CPUT.Name = "cmb_CPUT";
		this.cmb_CPUT.Size = new System.Drawing.Size(391, 20);
		this.cmb_CPUT.TabIndex = 7;
		this.cmb_CPUT.DropDown += new System.EventHandler(cmb_CPUT_DropDown);
		this.cmb_CPUT.SelectedIndexChanged += new System.EventHandler(cmb_CPUT_SelectedIndexChanged);
		this.cmb_CPUT.DropDownClosed += new System.EventHandler(cmb_CPUT_DropDownClosed);
		this.cmb_WaterCoolSelect.FormattingEnabled = true;
		this.cmb_WaterCoolSelect.Location = new System.Drawing.Point(123, 252);
		this.cmb_WaterCoolSelect.Margin = new System.Windows.Forms.Padding(2);
		this.cmb_WaterCoolSelect.Name = "cmb_WaterCoolSelect";
		this.cmb_WaterCoolSelect.Size = new System.Drawing.Size(391, 20);
		this.cmb_WaterCoolSelect.TabIndex = 23;
		this.cmb_WaterCoolSelect.DropDown += new System.EventHandler(cmb_Frequenceselsct_DropDown);
		this.cmb_WaterCoolSelect.SelectedIndexChanged += new System.EventHandler(cmb_Frequenceselsct_SelectedIndexChanged);
		this.cmb_WaterCoolSelect.DropDownClosed += new System.EventHandler(cmb_Frequenceselsct_DropDownClosed);
		this.lab_WaterCoolFans.AutoSize = true;
		this.lab_WaterCoolFans.BackColor = System.Drawing.Color.Transparent;
		this.lab_WaterCoolFans.Font = new System.Drawing.Font("宋体", 9.75f);
		this.lab_WaterCoolFans.Location = new System.Drawing.Point(624, 255);
		this.lab_WaterCoolFans.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.lab_WaterCoolFans.Name = "lab_WaterCoolFans";
		this.lab_WaterCoolFans.Size = new System.Drawing.Size(56, 13);
		this.lab_WaterCoolFans.TabIndex = 22;
		this.lab_WaterCoolFans.Text = "0000RPM";
		this.label6.AutoSize = true;
		this.label6.BackColor = System.Drawing.Color.Transparent;
		this.label6.Location = new System.Drawing.Point(523, 18);
		this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(101, 12);
		this.label6.TabIndex = 19;
		this.label6.Text = "CPU Temperature:";
		this.label1.AutoSize = true;
		this.label1.BackColor = System.Drawing.Color.Transparent;
		this.label1.Location = new System.Drawing.Point(31, 18);
		this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(71, 12);
		this.label1.TabIndex = 16;
		this.label1.Text = "CPU Select:";
		this.label4.AutoSize = true;
		this.label4.BackColor = System.Drawing.Color.Transparent;
		this.label4.Location = new System.Drawing.Point(559, 41);
		this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(65, 12);
		this.label4.TabIndex = 21;
		this.label4.Text = "CPU Usage:";
		this.cmb_FansSelect.FormattingEnabled = true;
		this.cmb_FansSelect.Location = new System.Drawing.Point(123, 134);
		this.cmb_FansSelect.Margin = new System.Windows.Forms.Padding(2);
		this.cmb_FansSelect.Name = "cmb_FansSelect";
		this.cmb_FansSelect.Size = new System.Drawing.Size(391, 20);
		this.cmb_FansSelect.TabIndex = 9;
		this.cmb_FansSelect.DropDown += new System.EventHandler(cmb_FansSelect_DropDown);
		this.cmb_FansSelect.SelectedIndexChanged += new System.EventHandler(cmb_FansSelect_SelectedIndexChanged);
		this.cmb_FansSelect.DropDownClosed += new System.EventHandler(cmb_FansSelect_DropDownClosed);
		this.label3.AutoSize = true;
		this.label3.BackColor = System.Drawing.Color.Transparent;
		this.label3.Location = new System.Drawing.Point(31, 41);
		this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(71, 12);
		this.label3.TabIndex = 18;
		this.label3.Text = "CPU Select:";
		this.label3.Click += new System.EventHandler(label3_Click);
		this.label2.AutoSize = true;
		this.label2.BackColor = System.Drawing.Color.Transparent;
		this.label2.Location = new System.Drawing.Point(25, 137);
		this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(77, 12);
		this.label2.TabIndex = 17;
		this.label2.Text = "FANS Select:";
		this.label5.AutoSize = true;
		this.label5.BackColor = System.Drawing.Color.Transparent;
		this.label5.Location = new System.Drawing.Point(565, 137);
		this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(59, 12);
		this.label5.TabIndex = 20;
		this.label5.Text = "FANS RPM:";
		this.lab_Value1.AutoSize = true;
		this.lab_Value1.BackColor = System.Drawing.Color.Transparent;
		this.lab_Value1.Font = new System.Drawing.Font("思源黑体", 18f);
		this.lab_Value1.Location = new System.Drawing.Point(574, 669);
		this.lab_Value1.Name = "lab_Value1";
		this.lab_Value1.Size = new System.Drawing.Size(78, 35);
		this.lab_Value1.TabIndex = 4;
		this.lab_Value1.Text = "000℃";
		this.lab_Value1.Visible = false;
		this.lab_Value1.Click += new System.EventHandler(lab_Value1_Click);
		this.lab_Value4.AutoSize = true;
		this.lab_Value4.BackColor = System.Drawing.Color.Transparent;
		this.lab_Value4.Font = new System.Drawing.Font("宋体", 36f);
		this.lab_Value4.Location = new System.Drawing.Point(684, 1003);
		this.lab_Value4.Name = "lab_Value4";
		this.lab_Value4.Size = new System.Drawing.Size(116, 48);
		this.lab_Value4.TabIndex = 26;
		this.lab_Value4.Text = "0000";
		this.lab_Value4.Visible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(700, 989);
		base.Controls.Add(this.panel8);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "frmMain";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(frmMain_FormClosing);
		base.Load += new System.EventHandler(frmMain_Load);
		base.Resize += new System.EventHandler(frmMain_Resize);
		this.contextMenuStrip1.ResumeLayout(false);
		this.panel8.ResumeLayout(false);
		this.panel8.PerformLayout();
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		base.ResumeLayout(false);
	}
}
