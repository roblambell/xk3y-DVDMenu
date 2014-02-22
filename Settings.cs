using System.Runtime.CompilerServices;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System;

namespace xk3yDVDMenu.Properties
{
[CompilerGenerated]
[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
internal sealed class Settings : ApplicationSettingsBase
{
	private static Settings defaultInstance;

	public static Settings Default
	{
		get
		{
			return Settings.defaultInstance;
		}
	}

	[DefaultSettingValue("")]
	[UserScopedSetting]
	[DebuggerNonUserCode]
	public string DVDStylePath
	{
		get
		{
			return (string)this["DVDStylePath"];
		}
		set
		{
			this["DVDStylePath"] = value;
		}
	}

	static Settings()
	{
		Settings.defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
	}

	public Settings()
	{
	}
}
}