using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace SQLCSVExporter
{
    internal static class Program
    {
        static void Main()
        {
            const string connectionString = @"SERVER=SERVER-ARCAEVO\SQL;DATABASE=ADB_LASERPOINT;TRUSTED_CONNECTION=Yes;MultipleActiveResultSets=True";
            const string selectQuery = "SELECT CONCAT(TRIM(alias), ',', TRIM(cd_ar)) AS 'ALIAS,CODICE' FROM ARAlias";

            DataTable table = ReadTable(connectionString, selectQuery);
            WriteToFile(table, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Nyx_Alias.csv", true, ",");
        }

        public static DataTable ReadTable(string connectionString, string selectQuery)
        {
            using DataTable returnValue = new();
            SqlConnection conn = new(connectionString);

            try
            {
                conn.Open();
                SqlCommand? command = new SqlCommand(selectQuery, conn);
                using SqlDataAdapter? adapter = new SqlDataAdapter(command);
                adapter.Fill(returnValue);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return returnValue;
        }

        public static void WriteToFile(DataTable dataSource, string fileOutputPath, bool firstRowIsColumnHeader = false, string seperator = ";")
        {
            using StreamWriter sw = new(fileOutputPath, false);
            int icolcount = dataSource.Columns.Count;

            if (!firstRowIsColumnHeader)
            {
                for (int i = 0; i < icolcount; i++)
                {
                    sw.Write(dataSource.Columns[i]);
                    if (i < icolcount - 1)
                        sw.Write(seperator);
                }

                sw.Write(sw.NewLine);
            }

            foreach (DataRow drow in dataSource.Rows)
            {
                for (int i = 0; i < icolcount; i++)
                {
                    if (!Convert.IsDBNull(drow[i]))
                        sw.Write(drow[i].ToString());
                    if (i < icolcount - 1)
                        sw.Write(seperator);
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }
    }
}