using System.Runtime.InteropServices;
using System.Text;

public static class IniFunc
{
	[DllImport("kernel32")]
	private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

	[DllImport("kernel32")]
	private static extern int WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpString, string lpFileName);

	public static string getString(string section, string key, string def, string filename)
	{
		StringBuilder stringBuilder = new StringBuilder(1024);
		GetPrivateProfileString(section, key, def, stringBuilder, 1024, filename);
		return stringBuilder.ToString();
	}

	public static void writeString(string section, string key, string val, string filename)
	{
		WritePrivateProfileString(section, key, val, filename);
	}
}
