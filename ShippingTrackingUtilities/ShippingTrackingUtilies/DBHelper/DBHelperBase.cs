using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Data;
using System.Data.SqlClient;


namespace ShippingTrackingUtilities.DBHelper
{
    public abstract class DBHelperBase
    {

        private string m_LastError = string.Empty;
        public string LastError { get { return m_LastError; } }

        private Exception m_LastException = null;
        public Exception LastException { get { return m_LastException; } }


        protected string m_ConnectionString;
        protected DataAccess m_Dac
        {
            get
            {
                if (m_DacLazy == null)
                    m_DacLazy = new DataAccess();
                return m_DacLazy;
            }
        }
        private static DataAccess m_DacLazy;


        protected bool HasErrors()
        {
            return m_Dac.HasErrors();
        }

        public DBHelperBase()
        {
            DoInit();
        }


        protected DataAccess GetDac()
        {
            if (m_DacLazy == null)
                m_DacLazy = new DataAccess();
            return m_DacLazy;
        }

        /// <summary>
        /// A place holder to initialize objects and calling methods.
        /// </summary>
        protected virtual void DoInit()
        {
        }


        /// <summary>
        /// Reset LastError to string.empty and LastException to Null. This is usually at beginning of each method.
        /// </summary>
        protected virtual void ClearLastError()
        {
            m_LastError = string.Empty;
            m_LastException = null;
        }


        protected virtual void LogException(Exception ex)
        {
            try
            {
                Utilities.Utils.LogException(ex);
                /*StreamWriter sw = new StreamWriter(@"ZuckShipRush_ErrorLog.txt", true);
                sw.WriteLine("");
                sw.WriteLine("");
                sw.WriteLine(DateTime.Now.ToString());
                sw.WriteLine("Error Message: " + ex.Message);
                sw.WriteLine("STrace: " + ex.StackTrace);
                sw.WriteLine(new string('-', 200));
                sw.Close(); */
            }
            catch { }
        }

        /// <summary>
        /// Assign LastError and LastException based on the current exception, it also calls LogException method.
        /// </summary>
        /// <param name="ex"></param>
        protected virtual void CatchException(Exception ex)
        {
            m_LastError = ex.Message;
            m_LastException = ex;
            LogException(ex);
            Console.WriteLine(ex);
        }

        /// <summary>
        /// Returns a basic datatable with 2 fields, Key and Value, both strings, Key is unique.
        /// </summary>
        /// <returns></returns>
        protected virtual DataTable GetKeyValueDataTable()
        {
            return GetKeyValueDataTable(true);
        }

        /// <summary>
        /// Returns a basic datatable with 2 fields, Key and Value, both strings, isKeyUnique defines if the Key field is unique.
        /// </summary>
        /// <param name="isKeyUnique"></param>
        /// <returns></returns>
        protected virtual DataTable GetKeyValueDataTable(bool isKeyUnique)
        {
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn("Key");
            dc.Unique = isKeyUnique;
            dt.Columns.Add(dc);
            dt.Columns.Add("Value");
            return dt;
        }

        protected virtual string PrepareTSqlIN<T>(List<T> items)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("IN (");
            foreach (T itm in items)
            {
                if (itm is string)
                    sb.Append("'" + itm.ToString().Replace("'", "") + "',");
                else sb.Append(itm + ",");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(")");
            return sb.ToString();
        }
        protected bool BulkCopy(DataTable dtSource, string destTableOnServer, bool bTruncateDestTable,
           Dictionary<string, string> mappingColumns)
        {
            bool r = false;
            try
            {
                string serverConnectionString = this.m_ConnectionString;

                ClearLastError();
                if (bTruncateDestTable)
                {
                    SqlConnection cnnDest = new SqlConnection(serverConnectionString);
                    SqlCommand cmdDest = new SqlCommand("truncate table " + destTableOnServer, cnnDest);
                    cnnDest.Open();
                    cmdDest.ExecuteNonQuery();
                    cnnDest.Close();
                }


                SqlBulkCopy sbc = new SqlBulkCopy(serverConnectionString
                       , SqlBulkCopyOptions.TableLock);
                sbc.BulkCopyTimeout = 2000000;
                foreach (var map in mappingColumns)
                {
                    sbc.ColumnMappings.Add(map.Key, map.Value);
                }
                // Copying data to destination
                sbc.DestinationTableName = destTableOnServer;
                sbc.WriteToServer(dtSource);
                // Closing connection and the others
                sbc.Close();
                r = true;
            }
            catch (Exception ex)
            {
                r = false;
                CatchException(ex);
            }
            return r;
        }


        #region IDisposable Members

        public virtual void Dispose()
        {
            //throw new NotImplementedException();

        }

        #endregion
    }
}
