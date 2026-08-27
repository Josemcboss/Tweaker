using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace ComputereMonitor;

public class ClsMain
{
	private delegate void SetLblValue(Label mlbl, string InputString);

	private delegate void SetcmbValue(ComboBox mcmb, string InputString, string state);

	public static string MainBoardName = "";

	public static int MainBoardUsage = 0;

	public static bool AutoStartEnable = true;

	public static bool Showcentigrade_Flag = true;

	public static List<bool> List_Drop = new List<bool>();

	public static bool CPUTempDrop = false;

	public static bool CPUUsageDrop = false;

	public static bool CPUPowerDrop = false;

	public static bool CPUFrequenceDrop = false;

	public static bool CPUVoltagesDrop = false;

	public static bool GPUTempDrop = false;

	public static bool GPUUsageDrop = false;

	public static bool GPUPowerDrop = false;

	public static bool GPUFrequenceDrop = false;

	public static bool FansDrop = false;

	public static bool WaterCoolDrop = false;

	public static List<bool> List_DropEnd = new List<bool>();

	public static bool CPUTempDropEnd = false;

	public static bool CPUUsageDropEnd = false;

	public static bool CPUPowerDropEnd = false;

	public static bool CPUFrequenceDropEnd = false;

	public static bool CPUVoltagesDropEnd = false;

	public static bool GPUTempDropEnd = false;

	public static bool GPUUsageDropEnd = false;

	public static bool GPUPowerDropEnd = false;

	public static bool GPUFrequenceDropEnd = false;

	public static bool FansDropEnd = false;

	public static bool WaterCoolDropEnd = false;

	public static int Count = 0;

	public static List<KeyValue> CPUT = new List<KeyValue>();

	public static List<KeyValue> CPUUsage = new List<KeyValue>();

	public static List<KeyValue> CPUPower = new List<KeyValue>();

	public static List<KeyValue> CPUFrequence = new List<KeyValue>();

	public static List<KeyValue> CPUVoltages = new List<KeyValue>();

	public static List<KeyValue> GPUT = new List<KeyValue>();

	public static List<KeyValue> GPUUsage = new List<KeyValue>();

	public static List<KeyValue> GPUPower = new List<KeyValue>();

	public static List<KeyValue> GPUFrequence = new List<KeyValue>();

	public static List<KeyValue> FansRPM = new List<KeyValue>();

	public static List<KeyValue> WatarRPM = new List<KeyValue>();

	public static List<KeyValue> Frequence = new List<KeyValue>();

	public static List<List<KeyValue>> List_All2 = new List<List<KeyValue>>();

	public static List<KeyValue> CPUT2 = new List<KeyValue>();

	public static List<KeyValue> CPUUsage2 = new List<KeyValue>();

	public static List<KeyValue> CPUPower2 = new List<KeyValue>();

	public static List<KeyValue> CPUFrequence2 = new List<KeyValue>();

	public static List<KeyValue> CPUVoltages2 = new List<KeyValue>();

	public static List<KeyValue> GPUT2 = new List<KeyValue>();

	public static List<KeyValue> GPUUsage2 = new List<KeyValue>();

	public static List<KeyValue> GPUPower2 = new List<KeyValue>();

	public static List<KeyValue> GPUFrequence2 = new List<KeyValue>();

	public static List<KeyValue> FansRPM2 = new List<KeyValue>();

	public static List<KeyValue> WatarRPM2 = new List<KeyValue>();

	public static List<KeyValue> Frequence2 = new List<KeyValue>();

	public static List<string> ListSelectName = new List<string>();

	public static string CPUTempSelectName = "";

	public static string CPUUsageSelectName = "";

	public static string CPUPowerSelectName = "";

	public static string CPUFrequenceSelectName = "";

	public static string CPUVoltagesSelectName = "";

	public static string GPUTempSelectName = "";

	public static string GPUUsageSelectName = "";

	public static string GPUPowerSelectName = "";

	public static string GPUFrequenceSelectName = "";

	public static string FANSSelectName = "";

	public static string WatarSelectName = "";

	public static string iniPath = Application.StartupPath + "\\config.ini";

	public static string BGImagePath = "";

	public static Label labvalue_PicRAMUsage = new Label();

	public static Label labvalue_RAMName2 = new Label();

	public static Label labvalue_RAMUsage2 = new Label();

	public static Label labvalue_CPUT = new Label();

	public static Label labvalue_CPUUsage = new Label();

	public static Label labvalue_CPUPower = new Label();

	public static Label labvalue_CPUFrequence = new Label();

	public static Label labvalue_CPUVoltages = new Label();

	public static Label labvalue_GPUT = new Label();

	public static Label labvalue_GPUUsage = new Label();

	public static Label labvalue_GPUPower = new Label();

	public static Label labvalue_GPUFrequence = new Label();

	public static Label labvalue_FansRPM = new Label();

	public static Label labvalue_WatarRPM = new Label();

	public static Label labvalue_CPUT2 = new Label();

	public static Label labvalue_CPUUsage2 = new Label();

	public static Label labvalue_CPUPower2 = new Label();

	public static Label labvalue_CPUFrequence2 = new Label();

	public static Label labvalue_CPUVoltages2 = new Label();

	public static Label labvalue_GPUT2 = new Label();

	public static Label labvalue_GPUUsage2 = new Label();

	public static Label labvalue_GPUPower2 = new Label();

	public static Label labvalue_GPUFrequence2 = new Label();

	public static Label labvalue_FansRPM2 = new Label();

	public static Label labvalue_WatarRPM2 = new Label();

	public static List<ComboBox> List_cmb = new List<ComboBox>();

	public static ComboBox cmbValue_CPUT = new ComboBox();

	public static ComboBox cmbValue_CPUUsage = new ComboBox();

	public static ComboBox cmbValue_CPUPower = new ComboBox();

	public static ComboBox cmbValue_CPUFrequence = new ComboBox();

	public static ComboBox cmbValue_CPUVoltages = new ComboBox();

	public static ComboBox cmbValue_GPUT = new ComboBox();

	public static ComboBox cmbValue_GPUUsage = new ComboBox();

	public static ComboBox cmbValue_GPUPower = new ComboBox();

	public static ComboBox cmbValue_GPUFrequence = new ComboBox();

	public static ComboBox cmbValue_FansRPM = new ComboBox();

	public static ComboBox cmbValue_WatarRPM = new ComboBox();

	public static string cmbValue_CPUT_selectText = "";

	public static string cmbValue_CPUUsage_selectText = "";

	public static string cmbValue_CPUPower_selectText = "";

	public static string cmbValue_CPUFrequence_selectText = "";

	public static string cmbValue_CPUVoltages_selectText = "";

	public static string cmbValue_GPUT_selectText = "";

	public static string cmbValue_GPUUsage_selectText = "";

	public static string cmbValue_GPUPower_selectText = "";

	public static string cmbValue_GPUFrequence_selectText = "";

	public static string cmbValue_FansRPM_selectText = "";

	public static string cmbValue_WatarRPM_selectText = "";

	public static void SetDisplayLbl(Label mlbl, string mValue)
	{
		if (mlbl.InvokeRequired)
		{
			SetLblValue method = SetDisplayLbl;
			mlbl.Invoke(method, mlbl, mValue);
		}
		else
		{
			mlbl.Text = mValue;
		}
	}

	public static void SetAppendTxt(string TmpStr, string PortName, int mIndex = 999)
	{
		string text = Strings.Format(DateAndTime.Now, "yyyyMMdd");
		string path = Application.StartupPath + "\\LOG\\" + text;
		if (mIndex != 999)
		{
			PortName = PortName + "_" + mIndex;
		}
		try
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			StreamWriter streamWriter = new StreamWriter(Application.StartupPath + "\\LOG\\" + Strings.Format(DateAndTime.Now, "yyyyMMdd") + "\\" + Strings.Format(DateAndTime.Now, "yyyyMMdd") + "_" + PortName + ".txt", append: true, Encoding.Default);
			streamWriter.WriteLine(Strings.Format(DateAndTime.Now, "yyyy-MM-dd HH:mm:ss fff") + "---" + TmpStr);
			streamWriter.Flush();
			streamWriter.Close();
		}
		catch (Exception)
		{
		}
	}

	public static void SetDisplayCmb(ComboBox mcmb, string mValue, string state)
	{
		if (mcmb.InvokeRequired)
		{
			SetcmbValue method = SetDisplayCmb;
			mcmb.Invoke(method, mcmb, mValue, state);
			return;
		}
		try
		{
			if (state == "clear")
			{
				mcmb.Items.Clear();
			}
			if (state == "add")
			{
				mcmb.Items.Add(mValue);
			}
			if (state == "select")
			{
				if (mValue == "-1")
				{
					mcmb.Text = "";
				}
				mcmb.SelectedIndex = Convert.ToInt16(mValue);
			}
			if (state == "TEXT")
			{
				mcmb.Text = mValue;
			}
		}
		catch (Exception ex)
		{
			SetAppendTxt("显示CMB异常:" + ex.Message, "HT");
		}
	}

	public static bool CheckListHave(List<KeyValue> ListCheck, string StrKey)
	{
		bool result = false;
		try
		{
			if (ListCheck.Where((KeyValue A) => A.Key == StrKey).ToList().Count > 0)
			{
				result = true;
			}
		}
		catch (Exception)
		{
		}
		return result;
	}

	public static double GetF(double C)
	{
		double result = 0.0;
		try
		{
			result = C * 1.8 + 32.0;
		}
		catch (Exception)
		{
		}
		return result;
	}

	public static void GetParam()
	{
		try
		{
			Showcentigrade_Flag = IniFunc.getString("system", "Showcentigrade_Flag", "ERR", iniPath) == "1";
			CPUTempSelectName = IniFunc.getString("system", "CPUTempSelectName", "ERR", iniPath);
			CPUUsageSelectName = IniFunc.getString("system", "CPUUsageSelectName", "ERR", iniPath);
			CPUPowerSelectName = IniFunc.getString("system", "CPUPowerSelectName", "ERR", iniPath);
			CPUFrequenceSelectName = IniFunc.getString("system", "CPUFrequenceSelectName", "ERR", iniPath);
			CPUVoltagesSelectName = IniFunc.getString("system", "CPUVoltagesSelectName", "ERR", iniPath);
			GPUTempSelectName = IniFunc.getString("system", "GPUTempSelectName", "ERR", iniPath);
			GPUUsageSelectName = IniFunc.getString("system", "GPUUsageSelectName", "ERR", iniPath);
			GPUPowerSelectName = IniFunc.getString("system", "GPUPowerSelectName", "ERR", iniPath);
			GPUFrequenceSelectName = IniFunc.getString("system", "GPUFrequenceSelectName", "ERR", iniPath);
			FANSSelectName = IniFunc.getString("system", "FANSSelectName", "ERR", iniPath);
			WatarSelectName = IniFunc.getString("system", "WatarSelectName", "ERR", iniPath);
			AutoStartEnable = IniFunc.getString("system", "AutoStartEnable", "ERR", iniPath) == "1";
		}
		catch (Exception ex)
		{
			MessageBox.Show("GetParam is Err:" + ex.Message);
		}
	}
}
