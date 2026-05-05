using System.Data;
using System.Data.SqlClient;

namespace AdminJustAndJustice.Models
{
    public class DBConnection
    {
        public static string connectionString = string.Empty;

        static DBConnection()
        {
            try
            {
                connectionString = new ConfigurationBuilder().AddJsonFile($"appsettings.json").Build().GetSection("ConnectionString").Value;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static int ExecuteNonQuery(string commandText, params SqlParameter[] commandParameters)
        {
            int k = 0;
            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand(commandText, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(commandParameters);
                    connection.Open();
                    k = command.ExecuteNonQuery();
                }
                return k;
            }
            catch (Exception)
            {
                return k;
            }
        }

        public static DataSet ExecuteQuery(string commandText, params SqlParameter[] parameters)
        {
            DataSet ds = new DataSet();
            try
            {
                using var connection = new SqlConnection(connectionString);
                using var command = new SqlCommand(commandText, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 120;
                command.Parameters.AddRange(parameters);
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(ds);
                connection.Close();
            }
            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Code");
                dt.Columns.Add("Remark");
                DataRow dr = dt.NewRow();
                dr["Code"] = "1";
                dr["Remark"] = ex.Message;
                dt.Rows.Add(dr);
                ds.Tables.Add(dt);
            }
            return ds;
        }
        public static async Task<DataSet> ExecuteQueryAsync(string commandText, params SqlParameter[] parameters)
        {
            DataSet ds = new DataSet();
            try
            {
                using var connection = new SqlConnection(connectionString);
                using var command = new SqlCommand(commandText, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 120
                };
                command.Parameters.AddRange(parameters);
                using var da = new SqlDataAdapter(command);

                await connection.OpenAsync();
                da.Fill(ds); 
            }
            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("code");
                dt.Columns.Add("mess");
                DataRow dr = dt.NewRow();
                dr["code"] = "1";
                dr["mess"] = ex.Message;
                dt.Rows.Add(dr);
                ds.Tables.Add(dt);
            }
            return ds;
        }
        public static async Task<DataSet> ApiExecuteQueryAsync(string commandText, params SqlParameter[] parameters)
        {
            DataSet ds = new DataSet();
            try
            {
                using var connection = new SqlConnection(connectionString);
                using var command = new SqlCommand(commandText, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 120
                };
                command.Parameters.AddRange(parameters);
                using var da = new SqlDataAdapter(command);

                await connection.OpenAsync();
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("code");
                dt.Columns.Add("mess");
                DataRow dr = dt.NewRow();
                dr["code"] = "1";
                dr["mess"] = ex.Message;
                dt.Rows.Add(dr);
                ds.Tables.Add(dt);
  
            }
            return ds;
        }
    }

    internal class DALBase
    {
        private string strCon = "";
        private System.Data.SqlClient.SqlConnection objConnection;
        private System.Data.DataSet dsResultSet;

        public DALBase()
        {
            this.strCon = new ConfigurationBuilder().AddJsonFile($"appsettings.json").Build().GetSection("ConnectionString").Value;
        }

        public void Create_Connection()
        {
            this.objConnection = null;
            try
            {
                this.objConnection = new System.Data.SqlClient.SqlConnection(this.strCon);
                if (this.objConnection.State == System.Data.ConnectionState.Closed || this.objConnection == null)
                {
                    this.objConnection.Open();
                }
            }
            catch
            {
            }
        }

        public void Close_Connection()
        {
            try
            {
                if (this.objConnection.State == System.Data.ConnectionState.Open || this.objConnection != null)
                {
                    this.objConnection.Close();
                    this.objConnection = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public System.Data.DataSet ExecuteProcedure(string SPName, string[] pName, string[] pValue)
        {
            this.dsResultSet = null;
            try
            {
                this.dsResultSet = new System.Data.DataSet();
                this.Create_Connection();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = this.objConnection;
                sqlCommand.CommandText = SPName;
                sqlCommand.CommandType = CommandType.StoredProcedure;
                for (int i = 0; i < pName.Length; i++)
                {
                    sqlCommand.Parameters.AddWithValue(pName[i], string.IsNullOrEmpty(pValue[i]) ? null : pValue[i]);
                }
                sqlDataAdapter.SelectCommand = sqlCommand;
                sqlDataAdapter.Fill(this.dsResultSet);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                this.Close_Connection();
            }
            return this.dsResultSet;
        }

        public System.Data.DataSet ExecuteProcedureWithoutPara(string SPName)
        {
            this.dsResultSet = null;
            try
            {
                this.dsResultSet = new System.Data.DataSet();
                this.Create_Connection();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = this.objConnection;
                sqlCommand.CommandText = SPName;
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlDataAdapter.SelectCommand = sqlCommand;
                sqlDataAdapter.Fill(this.dsResultSet);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                this.Close_Connection();
            }
            return this.dsResultSet;
        }

        public Int32 ExecuteProcedureINSERT(string SPName, string[] pName, string[] pValue)
        {
            int result = 0;
            try
            {
                this.dsResultSet = new System.Data.DataSet();
                this.Create_Connection();
                // SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = this.objConnection;
                sqlCommand.CommandText = SPName;
                sqlCommand.CommandType = CommandType.StoredProcedure;
                for (int i = 0; i < pName.Length; i++)
                {
                    sqlCommand.Parameters.AddWithValue(pName[i], string.IsNullOrEmpty(pValue[i]) ? null : pValue[i]);
                }
                // sqlDataAdapter.SelectCommand = sqlCommand;
                //sqlDataAdapter.Fill(this.dsResultSet);
                result = sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                this.Close_Connection();
            }
            return result;
        }
    }
}
