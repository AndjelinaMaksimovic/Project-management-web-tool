using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;


public class MySqlDatabaseAdapter
{
    private string connectionString;
    private MySqlConnection conn;

    public MySqlDatabaseAdapter(string connectionString)
    {
        this.connectionString = connectionString;
        this.conn = new MySqlConnection(connectionString);
    }
    public void connect()
    {
        try
        {
            conn.Open();

            // Create tables if they don't exists
            ExecuteQuery(
@"CREATE TABLE IF NOT EXISTS post (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    text VARCHAR(5000)
);");
        }
        catch (Exception err)
        {
            Console.WriteLine(err.ToString());
        }
    }

    public void ExecuteQuery(string query)
    {
        try
        {
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }
        catch (Exception err)
        {
            Console.WriteLine(err.ToString());
        }
    }

    public DataTable ExecuteQueryWithReturn(string query)
    {
        try
        {
            DataTable dataTable = new DataTable();
            MySqlCommand cmd = new MySqlCommand(query, conn);
            using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
            {
                // Fill the DataTable with the results of the SELECT query
                adapter.Fill(dataTable);
            }
            return dataTable;
        }
        catch (Exception err)
        {
            Console.WriteLine(err.ToString());
            return null;
        }
    }
}
public class MySqlConnector
{
    private static string connStr = "server=localhost;user=test;database=test_schema;port=3306;password=test1234";
    private static MySqlDatabaseAdapter adapter = new MySqlDatabaseAdapter(connStr);
    public static MySqlDatabaseAdapter GetAdapter()
    {
        return adapter;
    }
}