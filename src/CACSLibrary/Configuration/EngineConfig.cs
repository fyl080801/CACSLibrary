using System;
using System.Configuration;

namespace CACSLibrary.Configuration
{
	/// <summary>
	/// ��������
	/// </summary>
	public class EngineConfig : ConfigurationSection
	{
		/// <summary>
		/// ��������
		/// </summary>
		[ConfigurationProperty("containerType")]
		public string ContainerType
		{
			get
			{
				return base["containerType"] as string;
			}
		}
	}
}
