using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace SportStoreConsoleApp
{
    enum OrderStatus { Pending, Paid, Shipped, Delivered, Cancelled }

    // product
    class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }

    class CartItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    class OrderItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    class Order
    {
        public int Id { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public decimal Total { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    class Program
    {
        // 🔹 Подключение к SQL Server (замени имя сервера и базы)
        static string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=Пользователи;Trusted_Connection=True;";

        static List<Product> products = new List<Product>();
        static List<CartItem> cart = new List<CartItem>();
        static List<Order> orders = new List<Order>();
        static int nextProductId = 1;
        static int nextOrderId = 1;
        static CultureInfo ci = CultureInfo.InvariantCulture;

        static void Main()
        {
            // 🔐 Авторизация
            if (!AuthorizeUser())
            {
                Console.WriteLine("Авторизация не удалась. Программа завершена.");
                return;
            }

            // 🏪 Основное меню магазина
            SeedProducts();
            while (true)
            {
                Console.WriteLine("\n=== Магазин спортивных товаров ===");
                Console.WriteLine("1. Просмотр каталога");
                Console.WriteLine("2. Поиск и фильтрация товаров");
                Console.WriteLine("3. Добавить товар в корзину");
                Console.WriteLine("4. Просмотр корзины");
                Console.WriteLine("5. Оформить заказ");
                Console.WriteLine("6. Оплатить заказ");
                Console.WriteLine("7. Просмотр заказов");
                Console.WriteLine("8. Учёт поставок (пополнение склада / добавление товара)");
                Console.WriteLine("9. Склад (остатки)");
                Console.WriteLine("10. Отчётность по продажам");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите действие: ");

                var input = Console.ReadLine();
                Console.Clear();
                switch (input)
                {
                    case "1": ShowCatalog(); break;
                    case "2": SearchProducts(); break;
                    case "3": AddToCart(); break;
                    case "4": ShowCart(); break;
                    case "5": CreateOrder(); break;
                    case "6": PayOrder(); break;
                    case "7": ShowOrders(); break;
                    case "8": ReceiveSupply(); break;
                    case "9": ShowStock(); break;
                    case "10": ShowSalesReport(); break;
                    case "0": return;
                    default: Console.WriteLine("Неверный выбор."); break;
                }
            }
        }

        // 🔹 Авторизация пользователя
        static bool AuthorizeUser()
        {
            Console.WriteLine("=== Авторизация ===");
            Console.WriteLine("1. Войти");
            Console.WriteLine("2. Зарегистрироваться");
            Console.Write("Выберите действие: ");
            var choice = Console.ReadLine();

            if (choice == "1") return Login();
            else if (choice == "2") return Register();
            else return false;
        }

        static bool Login()
        {
            Console.Write("Введите логин: ");
            var login = Console.ReadLine();
            Console.Write("Введите пароль: ");
            var password = Console.ReadLine();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT КодПользователя, КодСекретногоВопроса, Ответ FROM Пользователь WHERE Логин = @login AND Пароль = @password";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.Parameters.AddWithValue("@password", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            Console.WriteLine("Неверный логин или пароль.");
                            return false;
                        }

                        int userId = (int)reader["КодПользователя"];
                        int questionId = (int)reader["КодСекретногоВопроса"];
                        string correctAnswer = reader["Ответ"].ToString() ?? "";

                        reader.Close();

                        // Получаем текст вопроса
                        string qTextQuery = "SELECT ТекстВопроса FROM СекретныйВопрос WHERE КодСекретногоВопроса = @id";
                        using (var qCmd = new SqlCommand(qTextQuery, conn))
                        {
                            qCmd.Parameters.AddWithValue("@id", questionId);
                            var question = qCmd.ExecuteScalar()?.ToString();

                            Console.WriteLine($"Секретный вопрос: {question}");
                            Console.Write("Введите ответ: ");
                            var answer = Console.ReadLine();

                            if (answer?.Trim().Equals(correctAnswer.Trim(), StringComparison.OrdinalIgnoreCase) == true)
                            {
                                Console.WriteLine("Авторизация успешна!");
                                return true;
                            }
                            else
                            {
                                Console.WriteLine("Неверный ответ на секретный вопрос.");
                                return false;
                            }
                        }
                    }
                }
            }
        }

        static bool Register()
        {
            Console.Write("Введите логин: ");
            var login = Console.ReadLine();
            Console.Write("Введите пароль: ");
            var password = Console.ReadLine();
            Console.Write("Введите email: ");
            var email = Console.ReadLine();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Выводим список секретных вопросов
                Console.WriteLine("Выберите секретный вопрос:");
                var qCmd = new SqlCommand("SELECT КодСекретногоВопроса, ТекстВопроса FROM СекретныйВопрос", conn);
                using (var reader = qCmd.ExecuteReader())
                {
                    while (reader.Read())
                        Console.WriteLine($"{reader["КодСекретногоВопроса"]}. {reader["ТекстВопроса"]}");
                }

                Console.Write("Введите номер вопроса: ");
                int qId = int.Parse(Console.ReadLine() ?? "1");

                Console.Write("Введите ответ: ");
                var answer = Console.ReadLine();

                string insert = "INSERT INTO Пользователь (Логин, Пароль, ЭлектроннаяПочта, КодСекретногоВопроса, Ответ) " +
                                "VALUES (@login, @password, @email, @qId, @answer)";
                using (var cmd = new SqlCommand(insert, conn))
                {
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@email", email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@qId", qId);
                    cmd.Parameters.AddWithValue("@answer", answer);
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Регистрация прошла успешно! Теперь войдите.");
            }
            return Login();
        }

        static void SeedProducts()
        {
            AddProduct("Беговые кроссовки", "Обувь", 79.99m, 10);
            AddProduct("Футбольный мяч", "Инвентарь", 24.50m, 20);
            AddProduct("Спортивная футболка", "Одежда", 19.99m, 30);
            AddProduct("Гантели 5 кг", "Тренировки", 29.99m, 15);
        }

        static void AddProduct(string name, string category, decimal price, int stock)
        {
            products.Add(new Product
            {
                Id = nextProductId++,
                Name = name,
                Category = category,
                Price = price,
                Stock = stock
            });
        }

        static void ShowCatalog()
        {
            Console.WriteLine("Каталог товаров:");
            foreach (var p in products.OrderBy(p => p.Id))
            {
                Console.WriteLine($"{p.Id}. {p.Name} | Категория: {p.Category} | Цена: {p.Price:0.00} | Остаток: {p.Stock}");
            }
        }

        static void SearchProducts()
        {
            Console.Write("Введите слово/фразу для поиска (или оставьте пустым): ");
            var q = (Console.ReadLine() ?? "").Trim();
            var results = products.AsEnumerable();
            if (!string.IsNullOrEmpty(q))
            {
                results = results.Where(p =>
                    (p.Name != null && p.Name.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (p.Category != null && p.Category.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
                );
            }

            Console.WriteLine("Результаты:");
            foreach (var p in results)
                Console.WriteLine($"{p.Id}. {p.Name} | {p.Category} | {p.Price:0.00} | Остаток: {p.Stock}");
        }

        static void AddToCart()
        {
            ShowCatalog();
            var id = ReadInt("Введите ID товара: ");
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null) { Console.WriteLine("Товар не найден."); return; }

            var qty = ReadInt("Введите количество: ");
            if (qty <= 0) { Console.WriteLine("Количество должно быть положительным."); return; }
            if (qty > product.Stock) { Console.WriteLine($"Недостаточно на складе (доступно {product.Stock})."); return; }

            var existing = cart.FirstOrDefault(ci => ci.ProductId == id);
            if (existing != null) existing.Quantity += qty;
            else cart.Add(new CartItem { ProductId = id, Quantity = qty });

            Console.WriteLine("Товар добавлен в корзину.");
        }

        static void ShowCart()
        {
            if (!cart.Any()) { Console.WriteLine("Корзина пуста."); return; }
            Console.WriteLine("Корзина:");
            decimal total = 0;
            foreach (var c in cart)
            {
                var p = products.FirstOrDefault(x => x.Id == c.ProductId);
                if (p == null) continue;
                var line = p.Price * c.Quantity;
                total += line;
                Console.WriteLine($"{p.Id}. {p.Name} x{c.Quantity} = {line:0.00}");
            }
            Console.WriteLine($"Итого: {total:0.00}");
        }

        static void CreateOrder()
        {
            if (!cart.Any()) { Console.WriteLine("Корзина пуста."); return;}

            foreach (var ci in cart)
            {
                var p = products.FirstOrDefault(x => x.Id == ci.ProductId);
                if (p == null) { Console.WriteLine($"Товар с ID {ci.ProductId} больше не доступен."); return; }
                if (ci.Quantity > p.Stock) { Console.WriteLine($"Недостаточно товара '{p.Name}' (доступно {p.Stock})."); return; }
            }

            var orderItems = new List<OrderItem>();
            decimal total = 0m;
            foreach (var ci in cart)
            {
                var p = products.First(x => x.Id == ci.ProductId);
                p.Stock -= ci.Quantity; 
                var oi = new OrderItem
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    Quantity = ci.Quantity,
                    UnitPrice = p.Price
                };
                orderItems.Add(oi);
                total += oi.UnitPrice * oi.Quantity;
            }

            var order = new Order
            {
                Id = nextOrderId++,
                Items = orderItems,
                Total = total,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.Now
            };
            orders.Add(order);
            cart.Clear();
            Console.WriteLine($"Заказ #{order.Id} создан. Сумма: {order.Total:0.00}. Статус: {order.Status}");
        }

        static void PayOrder()
        {
            ShowOrders();
            var id = ReadInt("Введите ID заказа для оплаты: ");
            var order = orders.FirstOrDefault(o => o.Id == id);
            if (order == null) { Console.WriteLine("Заказ не найден."); return; }
            if (order.Status != OrderStatus.Pending) { Console.WriteLine("Оплатить можно только заказы в статусе Pending."); return; }

            order.Status = OrderStatus.Paid;
            Console.WriteLine($"Заказ #{order.Id} оплачен.");
        }

        static void ShowOrders()
        {
            if (!orders.Any()) { Console.WriteLine("Заказов нет."); return; }
            Console.WriteLine("Заказы:");
            foreach (var o in orders.OrderBy(o => o.Id))
            {
                Console.WriteLine($"#{o.Id} | {o.CreatedAt:yyyy-MM-dd HH:mm} | {o.Total:0.00} | {o.Status}");
                foreach (var it in o.Items)
                    Console.WriteLine($"   - {it.ProductName} x{it.Quantity} = {it.UnitPrice * it.Quantity:0.00}");
            }
        }

        static void ReceiveSupply()
        {
            Console.WriteLine("1 - Пополнить существующий товар");
            Console.WriteLine("2 - Добавить новый товар");
            Console.Write("Выберите: ");
            var choice = Console.ReadLine();
            if (choice == "1")
            {
                ShowCatalog();
                var id = ReadInt("Введите ID товара для пополнения: ");
                var prod = products.FirstOrDefault(p => p.Id == id);
                if (prod == null) { Console.WriteLine("Товар не найден."); return; }
                var qty = ReadInt("Введите количество поступления: ");
                if (qty <= 0) { Console.WriteLine("Количество должно быть положительным."); return; }
                prod.Stock += qty;
                Console.WriteLine($"Остаток товара '{prod.Name}' = {prod.Stock}");
            }
            else if (choice == "2")
            {
                Console.Write("Название: ");
                var name = Console.ReadLine() ?? "Новый товар";
                Console.Write("Категория: ");
                var cat = Console.ReadLine() ?? "Разное";
                var price = ReadDecimal("Цена: ");
                var qty = ReadInt("Количество: ");
                if (qty <= 0) { Console.WriteLine("Количество должно быть положительным."); return; }
                AddProduct(name, cat, price, qty);
                Console.WriteLine("Товар добавлен и поступил на склад.");
            }
            else Console.WriteLine("Неверный выбор.");
        }

        static void ShowStock()
        {
            Console.WriteLine("Склад (остатки):");
            foreach (var p in products.OrderBy(p => p.Id))
                Console.WriteLine($"{p.Id}. {p.Name} | Остаток: {p.Stock}");
        }

        static void ShowSalesReport()
        {
            var soldOrders = orders.Where(o => o.Status == OrderStatus.Paid || o.Status == OrderStatus.Shipped || o.Status == OrderStatus.Delivered);
            var count = soldOrders.Count();
            var revenue = soldOrders.Sum(o => o.Total);
            Console.WriteLine("Отчёт по продажам:");
            Console.WriteLine($"Продано заказов: {count}");
            Console.WriteLine($"Выручка: {revenue:0.00}");
        }

        static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                int v;
                if (int.TryParse(s, out v)) return v;
                Console.WriteLine("Неверный ввод. Введите целое число.");
            }
        }

        static decimal ReadDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                decimal v;
                if (decimal.TryParse(s, NumberStyles.Number, ci, out v)) return v;
                Console.WriteLine("Неверный ввод. Введите число (пример 12.34).");
            }
        }
    }
}
