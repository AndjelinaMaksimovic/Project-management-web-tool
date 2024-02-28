using System;

class ConsoleApp
{
    static void Main(string[] args)
    {
        MySqlConnector.GetAdapter().connect();
        string[] prefixes = { "http://localhost:8080/" }; // Define the URLs where the server listens for requests
        var server = new SimpleHttpServer(prefixes);

        server.Start();

        Console.WriteLine("Press any key to stop the server...");
        Console.ReadKey();

        server.Stop();
    }
}
/*
class ConsoleApp1
{
    static void Main(string[] args)
    {
        string connStr = "server=localhost;user=test;database=test_schema;port=3306;password=test1234";
        MySqlDatabaseAdapter adapter = new MySqlDatabaseAdapter(connStr);
        adapter.connect();
        // adapter.ExecuteQuery("INSERT INTO user (ime, godine) VALUES (\"test2\", 20)");
        DataTable table = adapter.ExecuteQueryWithReturn("SELECT * FROM user;");

        foreach (DataRow row in table.Rows)
        {
            foreach (DataColumn column in table.Columns)
            {
                Console.Write(row[column] + "\t");
            }
            Console.WriteLine();
        }

        string[] prefixes = { "http://localhost:8080/" }; // Define the URLs where the server listens for requests
        var server = new SimpleHttpServer(prefixes);

        server.Start();

        Console.WriteLine("Press any key to stop the server...");
        Console.ReadKey();

        server.Stop();
    }
}
*/