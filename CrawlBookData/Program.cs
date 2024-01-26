using CrawlBookData.Model;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrawlBookData
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            await DownloadSachVanHocAsync();
            Console.WriteLine("Done");
        }

        public static async Task DownloadSachVanHocAsync() 
        {
            var web = new HtmlWeb();
            var listBook = new List<Book>();
            for (int i = 1; i <= 257; i++)
            {
                var document = await web.LoadFromWebAsync("https://www.fahasa.com/sach-trong-nuoc/van-hoc-trong-nuoc.html?order=num_orders&limit=24&p=" + i);
                var productHTMLElements = document.DocumentNode.QuerySelectorAll("div.ma-box-content");
                foreach (var htmlElement in productHTMLElements)
                {
                    string productDetailUrl = htmlElement.QuerySelector("div.images-container a").Attributes["href"].Value;
                    var productDetailDocument = await web.LoadFromWebAsync(productDetailUrl);
                    if (productDetailDocument.DocumentNode.QuerySelector("div.product-view-sa .product-view-sa_one .product-view-sa-supplier a") != null
                         && productDetailDocument.DocumentNode.QuerySelector("div.product-view-sa_one .product-view-sa-author  span:nth-child(2)") != null
                             && productDetailDocument.DocumentNode.QuerySelector("div.product-view-sa_two .product-view-sa-author  span:nth-child(2)") != null
                                && productDetailDocument.DocumentNode.QuerySelector("div.price-box .price") != null
                                    && productDetailDocument.DocumentNode.QuerySelector("div.product-essential-detail h1") != null
                                        && productDetailDocument.DocumentNode.QuerySelector("div.product-view-image-product img") != null)
                    {

                        string bookName = productDetailDocument.DocumentNode.QuerySelector("div.product-essential-detail h1").InnerText;
                        string bookNameFormatted = bookName.Trim();
                        string price = productDetailDocument.DocumentNode.QuerySelector("div.price-box .price").InnerText;
                        string supplier = productDetailDocument.DocumentNode.QuerySelector("div.product-view-sa_one .product-view-sa-supplier a").InnerText;
                        string writer = productDetailDocument.DocumentNode.QuerySelector("div.product-view-sa_one .product-view-sa-author  span:nth-child(2)").InnerText;
                        string coverType = productDetailDocument.DocumentNode.QuerySelector("div.product-view-sa_two .product-view-sa-author  span:nth-child(2)").InnerText;
                        string imgUrl = productDetailDocument.DocumentNode.QuerySelector("div.product-view-image-product img").Attributes["data-src"].Value;
                        Book book = new Book(bookNameFormatted, imgUrl, price, writer, supplier, coverType);
                        listBook.Add(book);
                        Console.WriteLine("Added");
                    }
                }
            }
        }
    }
}
