using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Manager
{
	public enum AuditEventTypes
	{
		WriteCentral = 0,
		UpdateConsumptionCentral = 1,
		DeleteCentral = 2,
		UpdateAllCentral = 3
	}

	public class AuditEvents
	{
		private static ResourceManager resourceManager = null;
		private static object resourceLock = new object();

		private static ResourceManager ResourceMgr
		{
			get
			{
				lock (resourceLock)
				{
					if (resourceManager == null)
					{
                        resourceManager = new ResourceManager
                            (typeof(AuditEventFile).ToString(),
                            Assembly.GetExecutingAssembly());
					}
					return resourceManager;
				}
			}
		}

		public static string WriteCentral
		{
			get
			{
                // TO DO
                return ResourceMgr.GetString(AuditEventTypes.WriteCentral.ToString()); 
			}
		}

		public static string UpdateConsumptionCentral
		{
			get
			{
                //TO DO
                return ResourceMgr.GetString(AuditEventTypes.UpdateConsumptionCentral.ToString());
            }
		}

		public static string DeleteCentral
		{
			get
			{
                //TO DO
                return ResourceMgr.GetString(AuditEventTypes.DeleteCentral.ToString());
            }
		}

		public static string UpdateAllCentral
		{
			get
			{
				//TO DO
				return ResourceMgr.GetString(AuditEventTypes.UpdateAllCentral.ToString());
			}
		}
	}
}
