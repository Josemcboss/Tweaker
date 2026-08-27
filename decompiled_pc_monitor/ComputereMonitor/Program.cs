using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace ComputereMonitor;

internal static class Program
{
	public static CPUIDSDK pSDK;

	private static volatile bool _shouldStop = false;

	private static string dllpath = Application.StartupPath + "\\";

	[STAThread]
	private static void Main()
	{
		if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length - 1 > 0)
		{
			ProjectData.EndApp();
			return;
		}
		int _version = 0;
		int _errorcode = 0;
		int _extended_errorcode = 0;
		Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
		Application.ThreadException += Application_ThreadException;
		AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(defaultValue: false);
		Thread thread = new Thread(ThreadLoop);
		try
		{
			pSDK = new CPUIDSDK();
			pSDK.CreateInstance();
			bool flag = pSDK.Init(dllpath, "cpuidsdk.dll", 2147483647u, ref _errorcode, ref _extended_errorcode);
			if (_errorcode != 0L)
			{
				MessageBox.Show((uint)_errorcode switch
				{
					1u => (uint)_extended_errorcode switch
					{
						1u => "You are running a trial version of the DLL SDK. In order to make it work, please run CPU-Z at the same time.", 
						2u => "Evaluation version has expired.", 
						_ => "Eval version error " + _extended_errorcode, 
					}, 
					2u => "Driver error " + _extended_errorcode, 
					4u => "Virtual machine detected.", 
					8u => "SDK mutex locked.", 
					_ => "Error code 0x%X" + _errorcode, 
				}, "CPUID SDK Error 1212");
			}
			if (flag)
			{
				pSDK.GetDllVersion(ref _version);
				thread.Start();
				Application.Run(new frmMain());
				_shouldStop = true;
				thread.Join();
			}
			pSDK.Close();
			pSDK.DestroyInstance();
		}
		catch (Exception ex)
		{
			MessageBox.Show(GetExceptionMsg(ex, string.Empty), "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
	{
		MessageBox.Show(GetExceptionMsg(e.Exception, e.ToString()), "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
	}

	private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		MessageBox.Show(GetExceptionMsg(e.ExceptionObject as Exception, e.ToString()), "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
	}

	private static string GetExceptionMsg(Exception ex, string backStr)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("****************************异常文本****************************");
		stringBuilder.AppendLine("【出现时间】：" + DateTime.Now.ToString());
		if (ex != null)
		{
			stringBuilder.AppendLine("【异常类型】：" + ex.GetType().Name);
			stringBuilder.AppendLine("【异常信息】：" + ex.Message);
			stringBuilder.AppendLine("【堆栈调用】：" + ex.StackTrace);
		}
		else
		{
			stringBuilder.AppendLine("【未处理异常】：" + backStr);
		}
		stringBuilder.AppendLine("***************************************************************");
		return stringBuilder.ToString();
	}

	public static void ThreadLoop()
	{
		while (!_shouldStop)
		{
			pSDK.RefreshInformation();
			Thread.Sleep(1000);
		}
	}
}
