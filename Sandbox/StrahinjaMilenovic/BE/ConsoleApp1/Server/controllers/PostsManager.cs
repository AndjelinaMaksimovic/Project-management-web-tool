using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Server.controllers
{
    internal class PostsManager
    {
        private MySqlDatabaseAdapter adapter = MySqlConnector.GetAdapter();
        public string GetPost(string id)
        {
            DataTable table = adapter.ExecuteQueryWithReturn($"SELECT * FROM post WHERE id={id}");
            DataRow row = table.Rows[0];
            Post np = new Post(row[table.Columns[0]].ToString(), row[table.Columns[1]].ToString());
            return np.ToString();
        }
        public void UpdatePost(string id, string newText)
        {
            adapter.ExecuteQuery($"UPDATE post SET text='{newText}' WHERE id={id}");
        }
        public void DeletePost(string id)
        {
            adapter.ExecuteQuery($"DELETE FROM post WHERE id={id}");
        }
        public void CreatePost(string text)
        {
            adapter.ExecuteQuery($"INSERT INTO post (text) VALUES ('{text}')");
        }
        // public List<Post> GetAllPosts()
        public string GetAllPosts()
        {
            DataTable table = adapter.ExecuteQueryWithReturn("SELECT * FROM post;");
            string JSONresponse = "[";
            List<Post> posts = new List<Post>();
            int i = 0;
            foreach (DataRow row in table.Rows)
            {
                Post np = new Post(row[table.Columns[0]].ToString(), row[table.Columns[1]].ToString());
                JSONresponse = JSONresponse + np.ToString() + ((i < table.Rows.Count - 1) ? "," : "");
                posts.Add(new Post(row[table.Columns[0]].ToString(), row[table.Columns[1]].ToString()));
                /*
                foreach (DataColumn column in table.Columns)
                {
                    Console.Write(row[column] + "\t");
                }
                Console.WriteLine();
                */
                Console.WriteLine(posts[0].ToString());
                i++;
            }
            JSONresponse = JSONresponse + "]";
            return JSONresponse;
        }
    }
}
