namespace ComputereMonitor;

public class KeyValue
{
	public string Key = "";

	public string Value = "";

	public KeyValue(string devicename, string s)
	{
		Key = devicename;
		Value = s;
	}
}
