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
            const string connectionString = ""; // your connection string
            string selectQuery = File.ReadAllText(Directory.GetCurrentDirectory() + "\\Query.txt"); // query from txt file from current directory
            //const string selectQuery = "SELECT...";                                               // Or from this string

            DataTable table = ReadTable(connectionString, selectQuery);
            WriteToFile(table, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\SQL2CSV.csv", true, ","); //exported csv
        }

        public static DataTable ReadTable(string connectionString, string selectQuery)
        {
            using DataTable dataTable = new();
            using SqlConnection connection = new(connectionString);
            try
            {
                connection.Open();
                SqlCommand? command = new(selectQuery, connection);
                using SqlDataAdapter? adapter = new(command);
                adapter.Fill(dataTable);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            return dataTable;
        }

        public static void WriteToFile(DataTable dataSource, string fileOutputPath, bool firstRowIsColumnHeader = false, string seperator = ";")
        {
            using StreamWriter sw = new(fileOutputPath, false);
            int columns = dataSource.Columns.Count;

            if (!firstRowIsColumnHeader)
            {
                for (int i = 0; i < columns; i++)
                {
                    sw.Write(dataSource.Columns[i]);
                    if (i < columns - 1)
                        sw.Write(seperator);
                }

                sw.Write(sw.NewLine);
            }

            foreach (DataRow rows in dataSource.Rows)
            {
                for (int i = 0; i < columns; i++)
                {
                    if (!Convert.IsDBNull(rows[i]))
                        sw.Write(rows[i].ToString());
                    if (i < columns - 1)
                        sw.Write(seperator);
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }
    }
}

// Icon Author: Benjamin STAWARZ
// Icon License: Creative Commons (Attribution 3.0 Unported)