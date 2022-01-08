using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Manager
{
	public class Audit : IDisposable
	{
		
		private static EventLog customLog = null;
		const string SourceName = "DataManager.Audit";
		const string LogName = "MyDataTest";

		static Audit()
		{
			try
			{
                if (!EventLog.SourceExists(SourceName))
                {
                    EventLog.CreateEventSource(SourceName, LogName);
                }
                customLog = new EventLog(LogName, 
                    Environment.MachineName, SourceName);
			}
			catch (Exception e)
			{
				customLog = null;
				Console.WriteLine("Error while trying to create log handle. Error = {0}", e.Message);
			}
		}

		
		public static void WriteCentral(string dataWritten)
		{
            //TO DO
			
			if (customLog != null)
			{
                string WriteCentral = 
                    AuditEvents.WriteCentral;
                string message = String.Format(WriteCentral,
					dataWritten);
                customLog.WriteEntry(message,EventLogEntryType.Information);
            }
            else
			{
				throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", 
                    (int)AuditEventTypes.WriteCentral));
			}
		}

		public static void UpdateAllCentral(string updatedRecords)
		{
			//TO DO

			if (customLog != null)
			{
				string UpdateAllCentral =
					AuditEvents.UpdateAllCentral;
				string message = String.Format(UpdateAllCentral,
					updatedRecords);
				customLog.WriteEntry(message);
			}
			else
			{
				throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
					(int)AuditEventTypes.UpdateAllCentral));
			}
		}

		public static void UpdateConsumptionCentral(string updatedData)
		{
            //TO DO
            if (customLog != null)
            {
                string UpdateConsumptionCentral =
                    AuditEvents.UpdateConsumptionCentral;
                string message = String.Format(UpdateConsumptionCentral,
					updatedData);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.UpdateConsumptionCentral));
            }
        }

		public static void DeleteCentral(string deletedData)
		{
            if (customLog != null)
            {
                string DeleteCentral =
                    AuditEvents.DeleteCentral;
                string message = String.Format(DeleteCentral,
					deletedData);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.DeleteCentral));
            }
        }

		public void Dispose()
		{
			if (customLog != null)
			{
				customLog.Dispose();
				customLog = null;
			}
		}
	}
}
