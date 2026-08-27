using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace ComputereMonitor;

public static class MultiLanguage
{
	private static Dictionary<string, string> DicLanguage = new Dictionary<string, string>();

	private static Dictionary<string, string> ReadXMLText(string frmName, string language)
	{
		try
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			XmlDocument xmlDocument = new XmlDocument();
			string text = AppDomain.CurrentDomain.BaseDirectory + "Languages\\" + language + ".xml";
			if (File.Exists(text))
			{
				xmlDocument.Load(text);
				foreach (XmlNode item in xmlDocument.DocumentElement.SelectNodes("frmMain[Name='" + frmName + "']/Controls/Control"))
				{
					dictionary.Add(item.Attributes["name"].Value, item.InnerText);
				}
				return dictionary;
			}
			return null;
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void SetLanguage(Form form)
	{
		string value = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings.Settings["Language"].Value;
		DicLanguage = ReadXMLText(form.Name, value);
		if (DicLanguage != null)
		{
			form.Text = GetLanguage(form.Name);
			SetControlsLanguage(form.Controls);
		}
	}

	private static string GetLanguage(string name)
	{
		if (DicLanguage.ContainsKey(name))
		{
			return DicLanguage[name];
		}
		return null;
	}

	private static void SetMenuLanguage(Form form)
	{
		FieldInfo[] fields = form.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
		for (int i = 0; i < fields.Length; i++)
		{
			switch (fields[i].FieldType.Name)
			{
			case "CheckBox":
				_ = (CheckBox)fields[i].GetValue(form);
				break;
			case "Form":
				foreach (ToolStripMenuItem item in ((ContextMenuStrip)fields[i].GetValue(form)).Items)
				{
					GetSetMenuStripItems(item);
				}
				break;
			case "ToolStrip":
				foreach (ToolStripItem item2 in ((ToolStrip)fields[i].GetValue(form)).Items)
				{
					string language2 = GetLanguage(item2.Name);
					if (!string.IsNullOrEmpty(language2))
					{
						item2.Text = language2;
					}
				}
				break;
			case "StatusStrip":
				foreach (ToolStripItem item3 in ((StatusStrip)fields[i].GetValue(form)).Items)
				{
					string language = GetLanguage(item3.Name);
					if (!string.IsNullOrEmpty(language))
					{
						item3.Text = language;
					}
				}
				break;
			}
		}
	}

	private static void GetSetMenuStripItems(ToolStripMenuItem menuItem)
	{
		string language = GetLanguage(menuItem.Name);
		if (!string.IsNullOrEmpty(language))
		{
			menuItem.Text = language;
		}
		foreach (ToolStripMenuItem dropDownItem in menuItem.DropDownItems)
		{
			language = GetLanguage(dropDownItem.Name);
			if (!string.IsNullOrEmpty(language))
			{
				dropDownItem.Text = language;
			}
			if (dropDownItem.DropDownItems != null)
			{
				GetSetMenuStripItems(dropDownItem);
			}
		}
	}

	private static void SetControlsLanguage(Control.ControlCollection controls)
	{
		foreach (Control control in controls)
		{
			string language = GetLanguage(control.Name);
			if (!string.IsNullOrEmpty(language))
			{
				control.Text = language;
			}
			if (control.Controls.Count > 0)
			{
				SetControlsLanguage(control.Controls);
			}
		}
	}

	public static string GetOtherLanguage(string name)
	{
		string value = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings.Settings["Language"].Value;
		string result = "";
		XmlDocument xmlDocument = new XmlDocument();
		string text = AppDomain.CurrentDomain.BaseDirectory + "Languages\\OtherLanguage.xml";
		if (File.Exists(text))
		{
			xmlDocument.Load(text);
			{
				foreach (XmlNode item in xmlDocument.DocumentElement.SelectNodes("/OtherLanguage/Text"))
				{
					if (item.Attributes["name"].Value == name)
					{
						result = ((!(value == EnumLanaguage.Chinese.ToString())) ? item.Attributes["en"].Value : item.Attributes["ch-z"].Value);
					}
				}
				return result;
			}
		}
		return null;
	}
}
