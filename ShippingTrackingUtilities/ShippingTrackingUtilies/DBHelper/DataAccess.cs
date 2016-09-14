using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;

using System.Data.SqlClient;
using System.IO;

namespace ShippingTrackingUtilities.DBHelper
{
    public class DataAccess
    {

        int _commandTimeOut = 600000;
        public string LastError = string.Empty;




        public DataAccess()
        {
            _commandTimeOut = 600000;
        }
        public DataAccess(int CommandTimeOut)
        {
            this._commandTimeOut = CommandTimeOut;
        }


        public bool HasErrors()
        {
            return this.LastError.Length > 0;
        }

        public int CommandTimeOut
        {
            get { return this._commandTimeOut; }
            set { this._commandTimeOut = value; }
        }

        public DataSet DataAdapter2DataSet(SqlCommand cmd, string ConnectionString)
        {
            LastError = string.Empty;
            SqlConnection cnn = new SqlConnection(ConnectionString);
            cmd.Connection = cnn;
            cmd.CommandTimeout = _commandTimeOut;
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            DataSet ds = new DataSet();
            try
            {
                da.Fill(ds, "dataTable");
                cmd.Connection.Close();
            }

            catch (Exception ex)
            {
                LastError = ex.Message;
                DataTable dt = new DataTable();
                DataColumn dc = new DataColumn("Error");
                dt.Columns.Add(dc);
                ds.Tables.Add(dt);
                Utilities.Utils.LogException(ex);

            }
            finally
            {
                if (cmd.Connection != null)
                {
                    cmd.Connection.Close();
                }
            }
            return ds;
        }




        public DataSet DataAdapter2DataSet(SqlCommand cmd)
        {
            if (cmd.Connection == null)
                return null;
            LastError = string.Empty;
            cmd.CommandTimeout = _commandTimeOut;
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            DataSet ds = new DataSet();
            try
            {
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                    da.Fill(ds, "dataTable");
                    cmd.Connection.Close();
                }
                else
                {
                    da.Fill(ds, "dataTable");
                }

            }

            catch (Exception ex)
            {
                LastError = ex.Message;
                DataTable dt = new DataTable();
                DataColumn dc = new DataColumn("Error");
                dt.Columns.Add(dc);
                ds.Tables.Add(dt);
                Utilities.Utils.LogException(ex);
            }
            finally
            {

            }
            return ds;
        }






        public DataSet DataReader2DataSet(SqlCommand cmd, string ConnectionString)
        {
            LastError = string.Empty;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            cmd.Connection = new SqlConnection(ConnectionString);
            try
            {
                cmd.Connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
            }

            catch (Exception ex)
            {
                LastError = ex.Message;
                DataColumn dc = new DataColumn("Error");
                dt.Columns.Add(dc);
                DataRow dr = dt.NewRow();
                dr["Error"] = ex.Message.ToString();
                dt.Rows.Add(dr);
                Utilities.Utils.LogException(ex);
            }
            finally
            {
                if (cmd.Connection != null)
                {
                    cmd.Connection.Close();
                }
            }
            ds.Tables.Add(dt);
            return ds;
        }


        public int ExecuteNonQuery(SqlCommand cmd, string ConnectionString)
        {
            LastError = string.Empty;
            int i = -1;
            SqlConnection cnn = new SqlConnection(ConnectionString);
            cmd.Connection = cnn;
            try
            {
                cmd.Connection.Open();
                i = cmd.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                LastError = ex.Message;
                i = -1;
                Utilities.Utils.LogException(ex);
            }
            finally
            {
                if (cmd.Connection != null)
                {
                    cmd.Connection.Close();
                }
            }
            return i;
        }

        public int ExecuteNonQuery(SqlCommand cmd)
        {
            int i = -1;
            LastError = string.Empty;
            try
            {
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                    i = cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
                else
                {
                    i = cmd.ExecuteNonQuery();
                }

            }

            catch (Exception ex)
            {
                LastError = ex.Message;
                i = -1;
                Utilities.Utils.LogException(ex);
            }
            finally
            {

            }
            return i;
        }

        public object ExecuteScalar(SqlCommand cmd, string ConnectionString)
        {
            LastError = string.Empty;
            object i = null;
            SqlConnection cnn = new SqlConnection(ConnectionString);
            cmd.Connection = cnn;
            try
            {
                cmd.Connection.Open();
                i = cmd.ExecuteScalar();
            }

            catch (Exception ex)
            {
                LastError = ex.Message;
                i = null;
                Utilities.Utils.LogException(ex);
            }
            finally
            {
                if (cmd.Connection != null)
                {
                    cmd.Connection.Close();
                }
            }
            return i;
        }

        public object ExecuteScalar(SqlCommand cmd)
        {
            object i = null;
            LastError = string.Empty;
            try
            {
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                    i = cmd.ExecuteScalar();
                    cmd.Connection.Close();
                }
                else
                {
                    i = cmd.ExecuteNonQuery();
                }

            }

            catch (Exception ex)
            {
                LastError = ex.Message;
                i = null;
                Utilities.Utils.LogException(ex);
            }
            finally
            {

            }
            return i;
        }



        public DataSet Run_SqlSP(string spName, List<SqlParameter> parameters, string connectionString)
        {
            SqlCommand cmd = new SqlCommand(spName);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = _commandTimeOut;
            cmd.Parameters.AddRange(parameters.ToArray());
            return DataAdapter2DataSet(cmd, connectionString);
        }


        public DataSet Run_SqlSP(string spName, List<SqlParameter> parameters, SqlConnection existingOpenedSQLConn)
        {
            SqlCommand cmd = new SqlCommand(spName);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = _commandTimeOut;
            cmd.Parameters.AddRange(parameters.ToArray());
            cmd.Connection = existingOpenedSQLConn;
            return DataAdapter2DataSet(cmd);
        }

        public int Run_SqlSPNonQuery(string spName, List<SqlParameter> parameters, string connectionString)
        {
            SqlCommand cmd = new SqlCommand(spName);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = _commandTimeOut;
            cmd.Parameters.AddRange(parameters.ToArray());
            return ExecuteNonQuery(cmd, connectionString);
        }


        public int Run_SqlSPNonQuery(string spName, List<SqlParameter> parameters, SqlConnection existingOpenedSQLConn)
        {
            SqlCommand cmd = new SqlCommand(spName);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = _commandTimeOut;
            cmd.Parameters.AddRange(parameters.ToArray());
            cmd.Connection = existingOpenedSQLConn;
            return ExecuteNonQuery(cmd);
        }



        public object Run_SqlSPExecScalar(string spName, List<SqlParameter> parameters, string connectionString)
        {
            SqlCommand cmd = new SqlCommand(spName);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = _commandTimeOut;
            cmd.Parameters.AddRange(parameters.ToArray());
            return ExecuteScalar(cmd, connectionString);
        }


        public object Run_SqlSPExecScalar(string spName, List<SqlParameter> parameters, SqlConnection existingOpenedSQLConn)
        {
            SqlCommand cmd = new SqlCommand(spName);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = _commandTimeOut;
            cmd.Parameters.AddRange(parameters.ToArray());
            cmd.Connection = existingOpenedSQLConn;
            return ExecuteScalar(cmd);
        }


        //------------

        public DataSet Run_SqlScript(string sqlScript, string connectionString)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sqlScript;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = _commandTimeOut;
            return DataAdapter2DataSet(cmd, connectionString);
        }
        public DataSet Run_SqlScript(string sqlScript, List<SqlParameter> prmts, string connectionString)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sqlScript;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = _commandTimeOut;
            if (prmts != null)
                foreach (SqlParameter p in prmts)
                    cmd.Parameters.Add(p);
            return DataAdapter2DataSet(cmd, connectionString);
        }

        public DataSet Run_SqlScript(string sqlScript, SqlConnection existingOpenedSQLConn)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sqlScript;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = _commandTimeOut;
            return DataAdapter2DataSet(cmd);
        }

        public int Run_SqlScriptNonQuery(string sqlScript, string connectionString)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sqlScript;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = _commandTimeOut;
            return ExecuteNonQuery(cmd, connectionString);
        }
        public int Run_SqlScriptNonQuery(string sqlScript, List<SqlParameter> prmts, string connectionString)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sqlScript;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = _commandTimeOut;
            if (prmts != null)
                foreach (SqlParameter p in prmts)
                    cmd.Parameters.Add(p);
            return ExecuteNonQuery(cmd, connectionString);
        }


        public int Run_SqlScriptNonQuery(string sqlScript, SqlConnection existingOpenedSQLConn)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sqlScript;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = _commandTimeOut;
            cmd.Connection = existingOpenedSQLConn;
            return ExecuteNonQuery(cmd);
        }


        public object Run_SqlScriptExecScalar(string sqlScript, List<SqlParameter> parameters, string connectionString)
        {
            SqlCommand cmd = new SqlCommand(sqlScript);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = _commandTimeOut;
            cmd.Parameters.AddRange(parameters.ToArray());
            return ExecuteScalar(cmd, connectionString);
        }

        public object Run_SqlScriptExecScalar(string sqlScript, string connectionString)
        {
            SqlCommand cmd = new SqlCommand(sqlScript);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = _commandTimeOut;
            return ExecuteScalar(cmd, connectionString);
        }

        public object Run_SqlScriptExecScalar(string sqlScript, SqlConnection existingOpenedSQLConn)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sqlScript;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = _commandTimeOut;
            cmd.Connection = existingOpenedSQLConn;
            return ExecuteScalar(cmd);
        }


        public object Run_SqlScriptExecScalar(string sqlScript, List<SqlParameter> parameters, SqlConnection existingOpenedSQLConn)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sqlScript;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = _commandTimeOut;
            cmd.Parameters.AddRange(parameters.ToArray());
            cmd.Connection = existingOpenedSQLConn;
            return ExecuteScalar(cmd);
        }

        public bool BulkCopy(DataTable dtSource, string destTableOnServer, bool bTruncateDestTable,
           Dictionary<string, string> mappingColumns, string connectionString)
        {
            bool r = false;
            try
            {
                string serverConnectionString = connectionString;

                LastError = string.Empty;
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
                LastError = ex.Message;
                r = false;
                Utilities.Utils.LogException(ex);
            }
            return r;
        }




    }
}
