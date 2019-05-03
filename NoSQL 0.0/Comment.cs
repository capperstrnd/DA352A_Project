using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSQL_0._0
{
    class Comment
    {
        public ObjectId IdFrom { get; set; }
        public string Date { get; set; }
        public string Text { get; set; }

        public Comment(ObjectId from, string text, string date)
        {
            IdFrom = from;
            Text = text;
            Date = date;
        }
    }
}
