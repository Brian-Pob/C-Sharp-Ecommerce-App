using Library.GUI_App.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Library.GUI_App.Services
{
    public abstract class Service
    {
        private List<Product> productList;
        public List<Product> Products { get { return productList; } }

        private static Service current;
        public static Service Current { get; }

        public bool Save(string filename)
        {
            try
            {
                var Json = JsonConvert.SerializeObject(productList);
                File.WriteAllText(filename, Json);
                return true;
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public bool Load(string filename)
        {
            try
            {
                var Json = File.ReadAllText(filename);
                productList = JsonConvert.DeserializeObject<List<Product>>(Json) ?? new List<Product>();
                return true;
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public void List()
        {
            foreach (Product product in Products)
            {
                Console.WriteLine(product);
            }
        }
    }
}
