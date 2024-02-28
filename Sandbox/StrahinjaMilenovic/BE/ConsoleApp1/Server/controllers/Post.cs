using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Server.controllers
{
    class Post
    {
        public Post(string id, string text)
        {
            this.id = id;
            this.text = text;
        }
        public string id;
        public string text;

        public override string ToString()
        {
            return "{" + $"\"id\": \"{id}\", \"text\": \"{text}\"" + "}";
        }
    }
}
