using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using Center.Model;
using System.Threading.Tasks;

namespace Center
{
    class SQL
    {
        SqlConnection connection;
        string conStringTesterDb = @"Data Source=10.4.24.114;Initial Catalog=dailyreport;User ID=user;Password=user;Min Pool Size=10;Connection Lifetime=120;Connection Timeout=60;";
        string conStringTraceSheetDb = @"Data Source=192.168.122.2;Initial Catalog=MANUFASPCPD;User ID=admin;Password=Buitanphat0201@;Min Pool Size=10;Connection Lifetime=120;Connection Timeout=60;";
        string conStringTraceSheetDb1 = @"Data Source=192.168.122.2;Initial Catalog=F2Database;User ID=admin;Password=Buitanphat0201@;Min Pool Size=10;Connection Lifetime=120;Connection Timeout=60;";
        private static string connection_manuafa { get; } = "Data Source=192.168.122.2;Initial Catalog=MANUFASPCPD;User ID=admin;Password=Buitanphat0201@";
        //private static string query_insert_user { get; } = "insert MANUFA_F2_Users values (@ins,@msnv,@po)";
        private static string query_insert_user { get; } = "IF EXISTS (SELECT 1 FROM [MANUFASPCPD].[dbo].[MANUFA_F2_Users] WHERE [nameMachine] = @machine)\r\nBEGIN\r\n    -- Nếu đã tồn tại Ins_Key -> Cập nhật MSNV và PO_Check\r\n    UPDATE [MANUFASPCPD].[dbo].[MANUFA_F2_Users]\r\n    SET [MSNV] = @msnv,\r\n        [PO_Check] = @po,\r\n\t\t[dateCreated] = GETDATE(),\r\n\t\t[Ins_Key] = @ins\r\n    WHERE [nameMachine] = @machine;\r\nEND\r\nELSE\r\nBEGIN\r\n    -- Nếu chưa tồn tại -> Thêm mới bản ghi\r\n    INSERT INTO [MANUFASPCPD].[dbo].[MANUFA_F2_Users] ([Ins_Key], [MSNV], [PO_Check], [dateCreated],[nameMachine])\r\n    VALUES (@ins, @msnv, @po,GETDATE(),@machine);\r\nEND";


        //string connect = "user id=SNKTR2K;password=SNKTR2K;" +
        //                           "data source=(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST=192.168.0.9)" +
        //                            "(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=SNKT.spclt.com.vn)))";

        public async Task<bool> InsertUser(Users _user)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connection_manuafa))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand(query_insert_user, connection))
                    {
                        command.Parameters.AddWithValue("@ins", _user.ins);
                        command.Parameters.AddWithValue("@msnv", _user.msnv);
                        command.Parameters.AddWithValue("@po", _user.po);
                        command.Parameters.AddWithValue("@machine", _user.machine);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inserting user: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false; // Trả về false nếu có lỗi xảy ra
            }
        }
        public void sqlDataAdapterFillDatatable(string sql, ref DataTable dt)
        {
            connection = new SqlConnection(conStringTraceSheetDb);
            SqlCommand command = new SqlCommand();
            connection.Open();
            using (SqlDataAdapter adapter = new SqlDataAdapter())
            {
                command.CommandText = sql;
                command.Connection = connection;
                adapter.SelectCommand = command;
                adapter.Fill(dt);
            }
            connection.Close();
        }

        public void sqlDataAdapterFillDatatableMachineConfig(string sql, ref DataTable dt)
        {
            connection = new SqlConnection(conStringTraceSheetDb1);
            SqlCommand command = new SqlCommand();
            connection.Open();
            using (SqlDataAdapter adapter = new SqlDataAdapter())
            {
                command.CommandText = sql;
                command.Connection = connection;
                adapter.SelectCommand = command;
                adapter.Fill(dt);
            }
            connection.Close();
        }

        public int sqlExecuteNonQuery(string sql)
        {
            int rowsAffected = 0; // Biến để lưu số lượng hàng bị ảnh hưởng
            try
            {
                using (SqlConnection connection = new SqlConnection(conStringTraceSheetDb))
                {
                    connection.Open(); // Mở kết nối
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        rowsAffected = command.ExecuteNonQuery(); // Thực thi câu lệnh SQL
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL ExecuteNonQuery method failed." + "\r\n" + ex.Message,
                                "Database Response", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                // rowsAffected sẽ là 0 nếu xảy ra lỗi
            }
            
            return rowsAffected; // Trả về số lượng hàng bị ảnh hưởng
        }
        public string sqlExecuteScalarString(string sql)
        {
            string response;
            try
            {
                connection = new SqlConnection(conStringTraceSheetDb);
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                response = Convert.ToString(command.ExecuteScalar());
                connection.Close();
                return response;
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL executeschalar moethod failed." + "\r\n" + ex.Message
                                , "Database Responce", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                connection.Close();
                return String.Empty;
            }
        }

        public int sqlExecuteScalar(string query)
        {
            int result = 0;

            using (SqlConnection conn = new SqlConnection(conStringTraceSheetDb1))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    object obj = cmd.ExecuteScalar();
                    if (obj != null && obj != DBNull.Value)
                    {
                        result = Convert.ToInt32(obj);
                    }
                }
            }

            return result;
        }


        public void sqlDataAdapterFillDatatableDB(string sql, ref DataTable dt)
        {
            connection = new SqlConnection(conStringTesterDb);
            SqlCommand command = new SqlCommand();
            connection.Open();
            using (SqlDataAdapter adapter = new SqlDataAdapter())
            {
                command.CommandText = sql;
                command.Connection = connection;
                adapter.SelectCommand = command;
                adapter.Fill(dt);
            }
            connection.Close();
        }
        
        public void getAutoCompleteData(string sql, ref TextBox txt)
        {
            txt.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txt.AutoCompleteSource = AutoCompleteSource.CustomSource;
            AutoCompleteStringCollection DataCollection = new AutoCompleteStringCollection();

            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommand command;
            DataSet ds = new DataSet();
            try
            {
                connection = new SqlConnection(conStringTraceSheetDb);
                connection.Open();
                command = new SqlCommand(sql, connection);
                adapter.SelectCommand = command;
                adapter.Fill(ds);
                adapter.Dispose();
                command.Dispose();
                connection.Close();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    DataCollection.Add(row[0].ToString());
                }
                txt.AutoCompleteCustomSource = DataCollection;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection! ");
                connection.Close();
            }
        }
        public void getAutoCompleteLabelData(string sql, ref Label txt)
        {
         
          //  AutoCompleteStringCollection DataCollection = new AutoCompleteStringCollection();

            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommand command;
            DataSet ds = new DataSet();
            try
            {
                connection = new SqlConnection(conStringTraceSheetDb);
                connection.Open();
                command = new SqlCommand(sql, connection);
                adapter.SelectCommand = command;
                adapter.Fill(ds);
                adapter.Dispose();
                command.Dispose();
                connection.Close();
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    txt.Text = ds.Tables[0].Rows[0][0].ToString();
                    // ds.Clear();
                }
                //  txt.AutoCompleteCustomSource = DataCollection;
            }
            catch (Exception ex)
            {
              //  MessageBox.Show("Can not open connection! ");
                connection.Close();
            }
        }
    }
}
