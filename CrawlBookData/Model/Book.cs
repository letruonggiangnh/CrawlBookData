using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CrawlBookData.Model
{
    public class Book
    {
        public Book(string BookName, string BookImageUrl, string BookPrice, string Writer, string Publisher, string CoverType) 
        {
            this.BookName = BookName;
            this.BookImageUrl = BookImageUrl;
            this.BookPrice = BookPrice;
            this.Writer = Writer;
            this.Publisher = Publisher;
            this.CoverType = CoverType;
        }
        public string BookName { get; set; }
        public string BookImageUrl { get; set; }
        public string BookPrice { get; set; }
        public string Writer { get; set; }
        public string Publisher { get; set; }
        public string CoverType { get; set; }
    }
}
