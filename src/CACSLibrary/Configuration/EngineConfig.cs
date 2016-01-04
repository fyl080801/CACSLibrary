using System;
using System.Configuration;

namespace CACSLibrary.Configuration
{
	/// <summary>
	/// ÒıÇæÅäÖÃ
	/// </summary>
	public class EngineConfig : ConfigurationSection
	{
		/// <summary>
		/// ÈİÆ÷ÀàĞÍ
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
