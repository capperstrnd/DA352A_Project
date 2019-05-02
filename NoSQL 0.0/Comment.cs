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

        public ObjectId Id { get; set; }
        public ObjectId From { get; set; }
        public ObjectId To { get; set; }
        public string Text { get; set; }
        public string Date { get; set; }

        public Comment(ObjectId from, ObjectId to, string text, string date)
        {
            From = from;
            To = to;
            Text = text;
            Date = date;
        }
    }
}
