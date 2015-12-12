using System;
using System.Collections;

namespace CACSLibrary.Caching
{
    /// <summary>
    /// 
    /// </summary>
	public class PriorityDateComparer : IComparer
	{
		Hashtable unsortedItems;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="unsortedItems"></param>
		public PriorityDateComparer(Hashtable unsortedItems)
		{
			this.unsortedItems = unsortedItems;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
		public int Compare(object x, object y)
		{
			CacheItem itemx = (CacheItem)this.unsortedItems[(string)x];
			CacheItem itemy = (CacheItem)this.unsortedItems[(string)y];
			int result;
			lock (itemy)
			{
				lock (itemx)
				{
					if (itemy == null && itemx == null)
					{
						result = 0;
					}
					else
					{
						if (itemx == null)
						{
							result = -1;
						}
						else
						{
							if (itemy == null)
							{
								result = 1;
							}
							else
							{
								result = ((itemx.ScavengingPriority == itemy.ScavengingPriority) ? itemx.LastAccessedTime.CompareTo(itemy.LastAccessedTime) : (itemx.ScavengingPriority - itemy.ScavengingPriority));
							}
						}
					}
				}
			}
			return result;
		}
	}
}
