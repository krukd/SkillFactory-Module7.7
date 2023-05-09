using System.Reflection;

namespace SkillFactory_Module7._7
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello!\n");
            List<Product<ProductInCartInfo>> cart = Shop.AddProductsToCart();
            Console.WriteLine("\nОформляем заказ");
            Console.WriteLine("Введите Ваше имя:");
            Customer Customer = new Customer(Console.ReadLine());
            Delivery delivery;
            int deliveryChoosen = Shop.ChooseDeliverType();
            switch (deliveryChoosen)
            {
                case 1:
                    HomeDelivery homeDelivery = new HomeDelivery();
                    homeDelivery.SetDeliveryAdress();
                    homeDelivery.SetDeliveryTime();
                    homeDelivery.SetDeliveryDate();
                    delivery = homeDelivery;
                    break;
                case 2:
                    PickPointDelivery pickPointDelivery = new PickPointDelivery();
                    pickPointDelivery.SetPickPoint();
                    pickPointDelivery.SetDeliveryDate();
                    delivery = pickPointDelivery;
                    break;
                case 3:
                    ShopDelivery shopDelivery = new ShopDelivery();
                    shopDelivery.SetShopLocation();
                    shopDelivery.SetDeliveryDate();
                    delivery = shopDelivery;
                    break;
                default:
                    throw new ArgumentException("Неверный тип доставки.");
            }
            Order<Delivery, ProductInCartInfo> order = new Order<Delivery, ProductInCartInfo>(delivery, 1, "Заказ", Customer, DateTime.Now, cart);
            Console.WriteLine("\nЗаказ готов");
            Console.WriteLine("\nВаш заказ:");
            order.DisplayOrderInfo();
            Console.ReadKey();

        }
    }




    abstract class Delivery
    {
        private protected string address;
        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        private DateOnly deliveryDate;
        public DateOnly DeliveryDate
        {
            get { return deliveryDate; }
            set { deliveryDate = value; }
        }

        public void SetDeliveryAdress()
        {
            Console.WriteLine("\nВведите адрес доставки:");
            address = Console.ReadLine();
        }

        public void SetDeliveryDate()
        {
            DateOnly date;
            Console.WriteLine("\nВведите дату доставки:");
            bool exit = false;
            string input;
            while (!exit)
            {
                input = Console.ReadLine();
                if (DateOnly.TryParse(input, out date))
                {

                    deliveryDate = date;
                    exit = true;

                }
                else
                {
                    Console.WriteLine("Некорректная дата доставки\n");
                }
            }
        }
    }

    class HomeDelivery : Delivery
    {
        private TimeOnly deliveryTime;
        public TimeOnly DeliveryTime
        {
            get { return deliveryTime; }
            set { deliveryTime = value; }
        }
        public void SetDeliveryTime()
        {
            TimeOnly time;
            Console.WriteLine("\nВведите время доставки:");
            bool exit = false;
            string input;
            while (!exit)
            {
                input = Console.ReadLine();
                if (TimeOnly.TryParse(input, out time))
                {

                    deliveryTime = time;
                    exit = true;

                }
                else
                {
                    Console.WriteLine("Некорректное время доставки\n");
                }
            }
        }
    }

    class PickPointDelivery : Delivery
    {
        private string pickPoint;

        public void SetPickPoint()
        {
            Console.WriteLine("\nУкажите пункт выдачи:");
            pickPoint = Console.ReadLine();
        }
        public void GetPickPoint()
        {
            Console.WriteLine(pickPoint);
        }
    }

    class ShopDelivery : Delivery
    {
        private string shopLocation;
        public void SetShopLocation()
        {
            Console.WriteLine("\nУкажите название и адрес магазина:");
            shopLocation = Console.ReadLine();
        }
        public void GetShopLocation()
        {
            Console.WriteLine(shopLocation);
        }
    }

    class Order<TDelivery, TStruct> where TDelivery : Delivery
    {
        public TDelivery Delivery;

        public int Number { get; }
        public string Description { get; set; }
        public Customer Customer { get; }
        public DateTime Date { get; }
        public decimal TotalAmount { get; }
        public int Count { get; }

        private List<Product<TStruct>> Products;


        public Order()
        {
            Products = new List<Product<TStruct>>();
        }


        public Order(TDelivery delivery, int number, string description, Customer customer, DateTime date, List<Product<TStruct>> products)
        {
            Delivery = delivery;
            Number = number;
            Description = description;
            Customer = customer;
            Date = date;
            Products = products;
            Count = products.Count;
        }


        public void DisplayOrderInfo()
        {
            Console.WriteLine($"Заказ #{Number}");
            Console.WriteLine($"Имя покупателя: {Customer.Name}");
            Console.WriteLine($"Описание: {Description}");
            Console.WriteLine("\nТовары:");
            foreach (var product in Products)
            {
                FieldInfo nameProp = product.Info.GetType().GetField("Name");
                FieldInfo priceProp = product.Info.GetType().GetField("Price");
                FieldInfo amountProp = product.Info.GetType().GetField("Amount");
                if (nameProp != null && nameProp.FieldType == typeof(string)
                    && priceProp != null && priceProp.FieldType == typeof(decimal)
                    && amountProp != null && amountProp.FieldType == typeof(int))
                {
                    Console.WriteLine($" - {nameProp.GetValue(product.Info)}, {priceProp.GetValue(product.Info)} руб. x {amountProp.GetValue(product.Info)} шт.");
                }
            }
            
        }
        public void DisplayAddress()
        {
            Console.WriteLine(Delivery.Address);
        }
        public TStruct GetProductInfo(int index)
        {
            if (index >= 0 && index < Products.Count)
            {
                return Products[index].Info;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

    }

        public enum DeliveryType
        {
            Home,
            PickPoint,
            Shop
        };
        public struct ProductInfo
        {
            public string Name;
            public decimal Price;
        }
        public struct ProductInCartInfo
        {
            public string Name;
            public int Amount;
            public decimal Price;
        }
        public class Product<TStruct>
        {
            public TStruct Info { get; set; }

            public Product(TStruct info)
            {
                Info = info;
            }
            public string GetName()
            {
                FieldInfo nameProp = Info.GetType().GetField("Name");
                if (nameProp == null)
                {
                    throw new ArgumentException("FieldInfo doesn't contain Name");
                }
                return nameProp.GetValue(Info)?.ToString();
            }
            public decimal GetPrice()
            {
                FieldInfo priceProp = Info.GetType().GetField("Price");
                if (priceProp == null)
                {
                    throw new ArgumentException("FieldInfo doesn't contain Price");
                }
                return (decimal)priceProp.GetValue(Info);
            }
            public decimal GetAmount()
            {
                FieldInfo priceProp = Info.GetType().GetField("Amount");
                if (priceProp == null)
                {
                    throw new ArgumentException("FieldInfo doesn't contain Amount");
                }
                return (decimal)priceProp.GetValue(Info);
            }


        }
        static class Shop
        {
            public static int MaxProductsCount = 10;

            static private List<Product<ProductInfo>> productList = new List<Product<ProductInfo>>()
    {
        new Product<ProductInfo>(new ProductInfo { Name = "Апельсины", Price = 900 }),
        new Product<ProductInfo>(new ProductInfo { Name = "Яблоки", Price = 600 }),
        new Product<ProductInfo>(new ProductInfo { Name = "Виноград", Price = 700 }),
    };
            static public List<Product<ProductInfo>> ProductList
            {
                get { return productList; }
            }
            static public List<Product<ProductInfo>> GetProductList()
            {
                return productList;
            }
            static public Product<ProductInfo> GetProduct(int index)
            {
                return productList[index];
            }

            static public void DisplayProductsList()
            {
                Console.WriteLine(String.Format("{0,-5} {1,-20} {2,0}", "Номер", "Название продукта", "Стоимость, руб."));
                int index = 1;
                foreach (Product<ProductInfo> product in productList)
                {
                    Console.WriteLine(String.Format("{0,-5} {1,-20} {2,0}", index, product.Info.Name, product.Info.Price));
                    index++;
                }
            }
            static public List<Product<ProductInCartInfo>> AddProductsToCart()
            {
                List<Product<ProductInCartInfo>> cart = new List<Product<ProductInCartInfo>>();
                Console.WriteLine("Список продуктов, доступных для покупки:\n");
                List<Product<ProductInfo>> products = GetProductList();
                bool exit = false;
                int number;
                string input;
                while (!exit)
                {
                    if (cart.Count <= MaxProductsCount)
                    {
                        Console.WriteLine("Список продуктов:");
                        DisplayProductsList();
                        Console.Write("Выход - команда закончить выбор\n");
                        Console.Write("\nВыберите номер продукта: ");
                        input = Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine("Достигнуто максимальное число продуктов в корзине");
                        input = "ВЫХОД";
                    }
                    if (int.TryParse(input, out number) && number > 0 && number <= products.Count)
                    {
                        Console.Write("Введите количество: ");
                        input = Console.ReadLine();
                        int amount;
                        if (int.TryParse(input, out amount) && amount > 0)
                        {
                            ProductInCartInfo info = new ProductInCartInfo { Name = products[number - 1].GetName(), Price = products[number - 1].GetPrice(), Amount = amount };
                            Product<ProductInCartInfo> product = new Product<ProductInCartInfo>(info);
                            cart.Add(product);
                            Console.WriteLine($"Продукт {products[number - 1].GetName()} ({products[number - 1].GetPrice()} руб.) добавлен в корзину в количестве: {amount} шт.");
                        }
                        else
                        {
                            Console.WriteLine("Некорректное количество продуктов\n");
                        }
                    }
                    else if (input.ToUpper().Contains("ВЫХОД"))
                    {
                        exit = true;
                    }
                    else
                    {
                        Console.WriteLine("Некорректный выбор продуктов");
                    }
                }
                return cart;
            }

            static public int ChooseDeliverType()
            {
                Console.WriteLine("\nДоступны следующие способы доставки:\n");
                int i = 1;
                foreach (DeliveryType item in (DeliveryType[])Enum.GetValues(typeof(DeliveryType)))
                {
                    Console.WriteLine(i + " " + item);
                    i++;
                }
                bool isChosen = false;
                int deliveryChoosen = 0;
                do
                {
                    Console.WriteLine("\nВведите номер способа доставки:");
                    string answer = Console.ReadLine();
                    if (int.TryParse(answer, out int j))
                    {
                        if (j > 0 && j <= Enum.GetNames(typeof(DeliveryType)).Length)
                        {
                            deliveryChoosen = j;
                            isChosen = true;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Неправильный выбор");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Неправильный выбор");
                    }
                } while (!isChosen);
                return deliveryChoosen;
            }
        }
        abstract class Person
        {
            private protected string name;
            public string Name { get { return name; } set { name = value; } }

            private protected string phone;
            public string Phone { get { return phone; } set { phone = value; } }

            private protected string email;
            public string Email { get { return email; } set { email = value; } }
            public Person(string Name) { name = Name; }
            public void DisplayName()
            {
                Console.WriteLine(name);
            }
        }
        class Customer : Person
        {
            public Customer(string name) : base(name) { }
        }

    

}