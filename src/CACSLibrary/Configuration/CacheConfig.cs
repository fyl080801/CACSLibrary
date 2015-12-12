using System;
using System.Configuration;

namespace CACSLibrary.Configuration
{
	/// <summary>
	/// 
	/// </summary>
	public class CacheConfig : ConfigurationSection
	{
		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("backingStore")]
		public string BackingStore
		{
			get
			{
				return base["backingStore"] as string;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("numberToRemoveWhenScavenging", DefaultValue = 8), IntegerValidator]
		public int NumberToRemoveWhenScavenging
		{
			get
			{
				return (int)base["numberToRemoveWhenScavenging"];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("maximumElementsInCacheBeforeScavenging", DefaultValue = 16), IntegerValidator]
		public int MaximumElementsInCacheBeforeScavenging
		{
			get
			{
				return (int)base["maximumElementsInCacheBeforeScavenging"];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationProperty("expirationPollTimeout", DefaultValue = 128), IntegerValidator]
		public int ExpirationPollTimeout
		{
			get
			{
				return (int)base["expirationPollTimeout"];
			}
		}
	}
}
