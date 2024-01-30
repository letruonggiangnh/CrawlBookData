using CrawlBookData.Model;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Text.RegularExpressions;
namespace CrawlBookData
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            string sqlconnectStr = "Data Source=DESKTOP-PDDKOFN\\MSSQLSERVER01;Initial Catalog=UngDungBanSach;User ID=sa1;Password=Qaz@1234";
            await DownloadSachVanHocAsync(sqlconnectStr);
            Console.Beep();
        }
        static string RemoveSpecialCharacters(string input)
        {
            // Sử dụng biểu thức chính quy để xóa các kí tự đặc biệt
            string pattern = "[^0-9]";
            Regex regex = new Regex(pattern);
            return regex.Replace(input, "");
        }
        public static async Task DownloadSachVanHocAsync(string connectionString) 
        {
            var web = new HtmlWeb();
            var listBook = new List<Book>();
            var connection = new SqlConnection(connectionString);
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
                        string cleanedString = RemoveSpecialCharacters(price);
                        int formattedPrice = int.Parse(cleanedString);
                        string supplier = productDetailDocument.DocumentNode.QuerySelector("div.product-view-sa_one .product-view-sa-supplier a").InnerText;
                        string writer = productDetailDocument.DocumentNode.QuerySelector("div.product-view-sa_one .product-view-sa-author  span:nth-child(2)").InnerText;
                        string coverType = productDetailDocument.DocumentNode.QuerySelector("div.product-view-sa_two .product-view-sa-author  span:nth-child(2)").InnerText;
                        string imgUrl = productDetailDocument.DocumentNode.QuerySelector("div.product-view-image-product img").Attributes["data-src"].Value;

                        connection.Open();

                        using (SqlCommand checkCommand = new SqlCommand("SELECT COUNT (*) from product where product_name = @product_name", connection))
                        {
                            checkCommand.Parameters.AddWithValue("@product_name", bookNameFormatted);
                            int existingCount = (int)checkCommand.ExecuteScalar();
                            if (existingCount > 0)
                            {
                                Console.WriteLine("Duplicated data");
                            }
                            else 
                            {    
                                using (SqlCommand command = new SqlCommand("InsertBookData", connection))
                                {
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.Parameters.AddWithValue("@category_id", 1);
                                    command.Parameters.AddWithValue("@product_name", bookNameFormatted);
                                    command.Parameters.AddWithValue("@supplier", supplier);
                                    command.Parameters.AddWithValue("@author", writer);
                                    command.Parameters.AddWithValue("@cover_type", coverType);
                                    command.Parameters.AddWithValue("@price", formattedPrice);
                                    command.Parameters.AddWithValue("@image_url", imgUrl);

                                    command.ExecuteNonQuery();
                                    Console.WriteLine("Added to Database");
                                    
                                }
                            }
                        }
                        connection.Close();
                    }
                }
            }
        }
    }
}
