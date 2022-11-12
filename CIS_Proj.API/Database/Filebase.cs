using Newtonsoft.Json;
using Library.GUI_App.Models;

namespace CIS_Proj.API.Database
{
    public class Filebase
    {
        private string _root;
        private string _quantityRoot;
        private string _weightRoot;
        private string _cartRoot;
        private JsonSerializerSettings options = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        private static Filebase? _instance;


        public static Filebase Current
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new Filebase();
                }

                return _instance;
            }
        }

        private Filebase()
        {
            _root = "C:\\temp";
            _quantityRoot = $"{_root}\\QuantityProducts";
            _weightRoot = $"{_root}\\WeightProducts";
            _cartRoot = $"{_root}\\Carts";
            InitializeDirectories();
            //Demo();
            
        }
        
        private void Demo()
        {
            testWeightProducts.ForEach(p => AddOrUpdateInventory(p));
            testQuantityProducts.ForEach(p => AddOrUpdateInventory(p));

            testQList.ForEach(p => AddOrUpdateCart("default", p));
            testQList.ForEach(p => AddOrUpdateCart("other", p));
            testWList.ForEach(p => AddOrUpdateCart("other", p));
        }

        private bool InitializeDirectories()
        {
            try
            {
                if (!Directory.Exists(_root))
                {
                    Directory.CreateDirectory(_root);
                } 
                if (!Directory.Exists(_quantityRoot))
                {
                    Directory.CreateDirectory(_quantityRoot);
                }
                if (!Directory.Exists(_weightRoot))
                {
                    Directory.CreateDirectory(_weightRoot);
                }
                if (!Directory.Exists(_cartRoot))
                {
                    Directory.CreateDirectory(_cartRoot);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Product AddOrUpdateInventory(Product product)
        {
            if (product == null)
            {
                return new Product();
            }
            
            //set up a new Id if one doesn't already exist
            if (product.Id == -1)
            {
                product.Id = NextId;
            }

            //go to the right place]
            string path = "";
            if (product is ProductByQuantity)
            {
                path = $"{_quantityRoot}\\{product.Id}.json";
            }
            else if (product is ProductByWeight)
            {
                path = $"{_weightRoot}\\{product.Id}.json";
            }

            Delete(path);

            //write the file
            File.WriteAllText(path, JsonConvert.SerializeObject(product, options));

            //return the item, which now has an id
            return product;
        }

        public Product AddOrUpdateCart(string cartName, Product product)
        {
            // item being added to cart already has an Id

            string path = $"{_cartRoot}\\{cartName}\\{product.Id}.json";
            
            Delete(path);

            //write the file
            try
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(product, options));

            }
            catch (Exception)
            {
                CreateCart(cartName);
                File.WriteAllText(path, JsonConvert.SerializeObject(product, options));
            }
            // return the item
            return product;
        }

        public void CreateCart(string cartName)
        {
            string path = $"{_cartRoot}\\{cartName}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        
        public bool Delete(string path)
        {
            //if the item has been previously persisted
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                //blow it up
                File.Delete(path);
                return true;
            }
            return false;
            //TODO: refer to AddOrUpdate for an idea of how you can implement this.
        }

        public bool DeleteCart(string cartName)
        {
            string path = $"{_cartRoot}\\{cartName}";
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                return true;
            }
            return false;
        }
        public List<ProductByQuantity> QuantityProducts
        {
            get
            {
                var root = new DirectoryInfo(_quantityRoot);
                var _products = new List<ProductByQuantity>();
                foreach (var productFile in root.GetFiles())
                {
                    var product = JsonConvert.DeserializeObject<ProductByQuantity>(File.ReadAllText(productFile.FullName), options) ?? new ProductByQuantity();
                    _products.Add(product);
                }
                return _products;
            }
        }

        public List<ProductByWeight> WeightProducts
        {
            get
            {
                var root = new DirectoryInfo(_weightRoot);
                var _products = new List<ProductByWeight>();
                foreach (var productFile in root.GetFiles())
                {
                    var product = JsonConvert.DeserializeObject<ProductByWeight>(File.ReadAllText(productFile.FullName), options) ?? new ProductByWeight();
                    _products.Add(product);
                }
                return _products;
            }
        }

        public List<Product> Products
        {
            get
            {
                var returnList = new List<Product>();

                WeightProducts.ForEach(returnList.Add);
                QuantityProducts.ForEach(returnList.Add);

                return returnList;
            }
        }

        public Dictionary<string, List<Product>> Carts
        {
            get
            {
                Dictionary<string, List<Product>> _carts = new Dictionary<string, List<Product>>();
                var root = new DirectoryInfo(_cartRoot);
                // each cart name has its own directory in _cartRoot
                foreach (var cart in root.GetDirectories())
                {
                    var cartName = cart.Name;
                    var cartProducts = new List<Product>();
                    // get each product in the cart directory
                    foreach (var productFile in cart.GetFiles())
                    {
                        var product = JsonConvert.DeserializeObject<Product>(File.ReadAllText(productFile.FullName), options) ?? new Product();
                        cartProducts.Add(product);
                    }
                    _carts.Add(cartName, cartProducts);
                }

                return _carts;
            }
        }

        public List<ProductByQuantity> testQList = new List<ProductByQuantity>
        {
            new ProductByQuantity { Id=3, Name="Cheese Pizza", Description="Slice of cheese pizza", Quantity=999, Price=1.00m},
        };

        public List<ProductByWeight> testWList = new List<ProductByWeight>
        {
            new ProductByWeight { Id=0, Name="Apple", Description="Bag of apples", Weight=420, Price=1.00m},
            new ProductByWeight { Id=1, Name="Bananas", Description="Bag of bananas", Weight=69, Price=2.00m},
            new ProductByWeight { Id=2, Name="Carrots", Description="Bag of carrots", Weight=25, Price=1.50m},
        };

        public List<ProductByWeight> testWeightProducts = new List<ProductByWeight>
        {
            new ProductByWeight { Id=0, Name="Apple", Description="Bag of apples", Weight=1, Price=1.00m},
            new ProductByWeight { Id=1, Name="Bananas", Description="Bag of bananas", Weight=2, Price=2.00m},
            new ProductByWeight { Id=2, Name="Carrots", Description="Bag of carrots", Weight=3, Price=1.50m},
        };

        public List<ProductByQuantity> testQuantityProducts = new List<ProductByQuantity>
        {
            new ProductByQuantity { Id=3, Name="Cheese Pizza", Description="Slice of cheese pizza", Quantity=1, Price=1.00m},
            new ProductByQuantity { Id=4, Name="Pepperoni Pizza", Description="Slice of pepperoni pizza", Quantity=3, Price=2.50m},
            new ProductByQuantity { Id=5, Name="Bagels", Description="Pack of bagels", Quantity=5, Price=1.99m},

        };
        
        public int NextId
        {
            get
            {
                if (!Products.Any())
                {
                    return 1;
                }

                return Products.Select(t => t.Id).Max() + 1;
            }

        }
    }
}
