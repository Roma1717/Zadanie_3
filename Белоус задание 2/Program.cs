using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace SportStoreConsoleApp
{
    enum OrderStatus { Pending, Paid, Shipped, Delivered, Cancelled }
    
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

    class Order
    {
        public int Id { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal Total { get; set; }
        public OrderStatus Status { get; set; }
    }

    class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Position { get; set; } = "";
        public decimal Salary { get; set; }
    }
class Person
    {
        public int Id { get; set; }
        public string LastName { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string MiddleName { get; set; } = "";
        public DateTime BirthDate { get; set; }
        public string Passport { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";

        public Person() { }

        public Person(string lastName, string firstName, string middleName,
                      DateTime birthDate, string passport, string address, string phone)
        {
            LastName = lastName;
            FirstName = firstName;
            MiddleName = middleName;
            BirthDate = birthDate;
            Passport = passport;
            Address = address;
            Phone = phone;
        }

        public static bool AddPerson(SqlConnection conn, Person person)
        {
            string query = @"INSERT INTO Person 
                        (LastName, FirstName, MiddleName, BirthDate, Passport, Address, Phone)
                         VALUES (@lastName, @firstName, @middleName, @birthDate, @passport, @address, @phone)";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@lastName", person.LastName);
                cmd.Parameters.AddWithValue("@firstName", person.FirstName);
                cmd.Parameters.AddWithValue("@middleName", string.IsNullOrEmpty(person.MiddleName) ? (object)DBNull.Value : person.MiddleName);
                cmd.Parameters.AddWithValue("@birthDate", person.BirthDate);
                cmd.Parameters.AddWithValue("@passport", person.Passport);
                cmd.Parameters.AddWithValue("@address", string.IsNullOrEmpty(person.Address) ? (object)DBNull.Value : person.Address);
                cmd.Parameters.AddWithValue("@phone", string.IsNullOrEmpty(person.Phone) ? (object)DBNull.Value : person.Phone);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool UpdatePerson(SqlConnection conn, int id, Person person)
        {
            string query = @"UPDATE Person SET 
                        LastName=@lastName, FirstName=@firstName, MiddleName=@middleName,
                        BirthDate=@birthDate, Passport=@passport, Address=@address, Phone=@phone
                        WHERE Id=@id";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@lastName", person.LastName);
                cmd.Parameters.AddWithValue("@firstName", person.FirstName);
                cmd.Parameters.AddWithValue("@middleName", string.IsNullOrEmpty(person.MiddleName) ? (object)DBNull.Value : person.MiddleName);
                cmd.Parameters.AddWithValue("@birthDate", person.BirthDate);
                cmd.Parameters.AddWithValue("@passport", person.Passport);
                cmd.Parameters.AddWithValue("@address", string.IsNullOrEmpty(person.Address) ? (object)DBNull.Value : person.Address);
                cmd.Parameters.AddWithValue("@phone", string.IsNullOrEmpty(person.Phone) ? (object)DBNull.Value : person.Phone);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool DeletePerson(SqlConnection conn, int id)
        {
            string query = "DELETE FROM Person WHERE Id=@id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static List<Person> GetAllPersons(SqlConnection conn)
        {
            List<Person> list = new List<Person>();
            string query = "SELECT * FROM Person";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Person
                        {
                            Id = (int)reader["Id"],
                            LastName = reader["LastName"].ToString(),
                            FirstName = reader["FirstName"].ToString(),
                            MiddleName = reader["MiddleName"].ToString(),
                            BirthDate = (DateTime)reader["BirthDate"],
                            Passport = reader["Passport"].ToString(),
                            Address = reader["Address"].ToString(),
                            Phone = reader["Phone"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public override string ToString()
        {
            return $"{Id}. {LastName} {FirstName} {MiddleName} | Birth: {BirthDate:yyyy-MM-dd} | Passport: {Passport} | Phone: {Phone} | Address: {Address}";
        }
    }

class Position
    {
        private int _id;
        private string _name = "";
        private string _description = "";
        private decimal _salary;

        public int Id
        {
            get { return _id; }
            set
            {
                if (value < 0) throw new ArgumentException("Id не может быть отрицательным.");
                _id = value;
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value ?? ""; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value ?? ""; }
        }

        public decimal Salary
        {
            get { return _salary; }
            set
            {
                if (value < 0) throw new ArgumentException("Зарплата не может быть отрицательной.");
                _salary = value;
            }
        }

        public Position() { }

        public Position(string name, string description, decimal salary)
        {
            Name = name;
            Description = description;
            Salary = salary;
        }

        public static bool AddPosition(SqlConnection conn, Position position)
        {
            string query = @"INSERT INTO Должность (Название, Описание, Зарплата)
                         VALUES (@name, @description, @salary)";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@name", position.Name);
                cmd.Parameters.AddWithValue("@description", string.IsNullOrEmpty(position.Description) ? (object)DBNull.Value : position.Description);
                cmd.Parameters.AddWithValue("@salary", position.Salary);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool UpdatePosition(SqlConnection conn, int id, Position position)
        {
            string query = @"UPDATE Должность SET 
                         Название=@name, Описание=@description, Зарплата=@salary
                         WHERE Id=@id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", position.Name);
                cmd.Parameters.AddWithValue("@description", string.IsNullOrEmpty(position.Description) ? (object)DBNull.Value : position.Description);
                cmd.Parameters.AddWithValue("@salary", position.Salary);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool DeletePosition(SqlConnection conn, int id)
        {
            string query = "DELETE FROM Должность WHERE Id=@id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static List<Position> GetAllPositions(SqlConnection conn)
        {
            List<Position> list = new List<Position>();
            string query = "SELECT * FROM Должность";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Position
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Название"].ToString(),
                            Description = reader["Описание"].ToString(),
                            Salary = (decimal)reader["Зарплата"]
                        });
                    }
                }
            }
            return list;
        }

        public override string ToString()
        {
            return $"{Id}. {Name} | {Description} | Зарплата: {Salary:0.00}";
        }
    }

class Bank
    {
        private int _id;
        private string _name = "";
        private string _address = "";
        private string _phone = "";

        public int Id
        {
            get { return _id; }
            set
            {
                if (value < 0) throw new ArgumentException("Id не может быть отрицательным.");
                _id = value;
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value ?? ""; }
        }

        public string Address
        {
            get { return _address; }
            set { _address = value ?? ""; }
        }

        public string Phone
        {
            get { return _phone; }
            set { _phone = value ?? ""; }
        }

        public Bank() { }

        public Bank(string name, string address, string phone)
        {
            Name = name;
            Address = address;
            Phone = phone;
        }

        public static bool AddBank(SqlConnection conn, Bank bank)
        {
            string query = @"INSERT INTO Банк (Название, Адрес, Телефон)
                         VALUES (@name, @address, @phone)";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@name", bank.Name);
                cmd.Parameters.AddWithValue("@address", string.IsNullOrEmpty(bank.Address) ? (object)DBNull.Value : bank.Address);
                cmd.Parameters.AddWithValue("@phone", string.IsNullOrEmpty(bank.Phone) ? (object)DBNull.Value : bank.Phone);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool UpdateBank(SqlConnection conn, int id, Bank bank)
        {
            string query = @"UPDATE Банк SET 
                         Название=@name, Адрес=@address, Телефон=@phone
                         WHERE Id=@id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", bank.Name);
                cmd.Parameters.AddWithValue("@address", string.IsNullOrEmpty(bank.Address) ? (object)DBNull.Value : bank.Address);
                cmd.Parameters.AddWithValue("@phone", string.IsNullOrEmpty(bank.Phone) ? (object)DBNull.Value : bank.Phone);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool DeleteBank(SqlConnection conn, int id)
        {
            string query = "DELETE FROM Банк WHERE Id=@id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static List<Bank> GetAllBanks(SqlConnection conn)
        {
            List<Bank> list = new List<Bank>();
            string query = "SELECT * FROM Банк";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Bank
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Название"].ToString(),
                            Address = reader["Адрес"].ToString(),
                            Phone = reader["Телефон"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public override string ToString()
        {
            return $"{Id}. {Name} | Адрес: {Address} | Телефон: {Phone}";
        }
    }

class Card
    {
        private int _id;
        private string _cardNumber = "";
        private int _employeeId;
        private int _bankId;
        private DateTime _expiryDate;

        
        public int Id
        {
            get { return _id; }
            set
            {
                if (value < 0) throw new ArgumentException("Id не может быть отрицательным.");
                _id = value;
            }
        }

        public string CardNumber
        {
            get { return _cardNumber; }
            set { _cardNumber = value ?? ""; }
        }

        public int EmployeeId
        {
            get { return _employeeId; }
            set
            {
                if (value < 0) throw new ArgumentException("EmployeeId не может быть отрицательным.");
                _employeeId = value;
            }
        }

        public int BankId
        {
            get { return _bankId; }
            set
            {
                if (value < 0) throw new ArgumentException("BankId не может быть отрицательным.");
                _bankId = value;
            }
        }

        public DateTime ExpiryDate
        {
            get { return _expiryDate; }
            set { _expiryDate = value; }
        }

        public Card() { }

        public Card(string cardNumber, int employeeId, int bankId, DateTime expiryDate)
        {
            CardNumber = cardNumber;
            EmployeeId = employeeId;
            BankId = bankId;
            ExpiryDate = expiryDate;
        }

        public static bool AddCard(SqlConnection conn, Card card)
        {
            string query = @"INSERT INTO Карта (CardNumber, EmployeeId, BankId, ExpiryDate)
                         VALUES (@cardNumber, @employeeId, @bankId, @expiryDate)";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@cardNumber", card.CardNumber);
                cmd.Parameters.AddWithValue("@employeeId", card.EmployeeId);
                cmd.Parameters.AddWithValue("@bankId", card.BankId);
                cmd.Parameters.AddWithValue("@expiryDate", card.ExpiryDate);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool UpdateCard(SqlConnection conn, int id, Card card)
        {
            string query = @"UPDATE Карта SET 
                         CardNumber=@cardNumber, EmployeeId=@employeeId, BankId=@bankId, ExpiryDate=@expiryDate
                         WHERE Id=@id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@cardNumber", card.CardNumber);
                cmd.Parameters.AddWithValue("@employeeId", card.EmployeeId);
                cmd.Parameters.AddWithValue("@bankId", card.BankId);
                cmd.Parameters.AddWithValue("@expiryDate", card.ExpiryDate);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool DeleteCard(SqlConnection conn, int id)
        {
            string query = "DELETE FROM Карта WHERE Id=@id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static List<Card> GetAllCards(SqlConnection conn)
        {
            List<Card> list = new List<Card>();
            string query = "SELECT * FROM Карта";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Card
                        {
                            Id = (int)reader["Id"],
                            CardNumber = reader["CardNumber"].ToString(),
                            EmployeeId = (int)reader["EmployeeId"],
                            BankId = (int)reader["BankId"],
                            ExpiryDate = (DateTime)reader["ExpiryDate"]
                        });
                    }
                }
            }
            return list;
        }

        public override string ToString()
        {
            return $"{Id}. Номер карты: {CardNumber} | Сотрудник ID: {EmployeeId} | Банк ID: {BankId} | Действительна до: {ExpiryDate:yyyy-MM-dd}";
        }
    }


class EducationLevel
    {
        private int _id;
        private string _name = "";
        private string _description = "";

        
        public int Id
        {
            get { return _id; }
            set
            {
                if (value < 0) throw new ArgumentException("Id не может быть отрицательным.");
                _id = value;
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value ?? ""; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value ?? ""; }
        }

        public EducationLevel() { }

        public EducationLevel(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public static bool AddEducationLevel(SqlConnection conn, EducationLevel level)
        {
            string query = @"INSERT INTO УровеньОбразования (Название, Описание)
                         VALUES (@name, @description)";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@name", level.Name);
                cmd.Parameters.AddWithValue("@description", string.IsNullOrEmpty(level.Description) ? (object)DBNull.Value : level.Description);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool UpdateEducationLevel(SqlConnection conn, int id, EducationLevel level)
        {
            string query = @"UPDATE УровеньОбразования SET 
                         Название=@name, Описание=@description
                         WHERE Id=@id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", level.Name);
                cmd.Parameters.AddWithValue("@description", string.IsNullOrEmpty(level.Description) ? (object)DBNull.Value : level.Description);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool DeleteEducationLevel(SqlConnection conn, int id)
        {
            string query = "DELETE FROM УровеньОбразования WHERE Id=@id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static List<EducationLevel> GetAllEducationLevels(SqlConnection conn)
        {
            List<EducationLevel> list = new List<EducationLevel>();
            string query = "SELECT * FROM УровеньОбразования";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new EducationLevel
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Название"].ToString(),
                            Description = reader["Описание"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public override string ToString()
        {
            return $"{Id}. {Name} | Описание: {Description}";
        }
    }

class Specialty
    {
        private int _id;
        private string _name = "";
        private string _description = "";

        public int Id
        {
            get { return _id; }
            set
            {
                if (value < 0) throw new ArgumentException("Id не может быть отрицательным.");
                _id = value;
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value ?? ""; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value ?? ""; }
        }

        public Specialty() { }

        public Specialty(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public static bool AddSpecialty(SqlConnection conn, Specialty specialty)
        {
            string query = @"INSERT INTO Специальность (Название, Описание)
                         VALUES (@name, @description)";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@name", specialty.Name);
                cmd.Parameters.AddWithValue("@description", string.IsNullOrEmpty(specialty.Description) ? (object)DBNull.Value : specialty.Description);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool UpdateSpecialty(SqlConnection conn, int id, Specialty specialty)
        {
            string query = @"UPDATE Специальность SET 
                         Название=@name, Описание=@description
                         WHERE Id=@id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", specialty.Name);
                cmd.Parameters.AddWithValue("@description", string.IsNullOrEmpty(specialty.Description) ? (object)DBNull.Value : specialty.Description);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool DeleteSpecialty(SqlConnection conn, int id)
        {
            string query = "DELETE FROM Специальность WHERE Id=@id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static List<Specialty> GetAllSpecialties(SqlConnection conn)
        {
            List<Specialty> list = new List<Specialty>();
            string query = "SELECT * FROM Специальность";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Specialty
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Название"].ToString(),
                            Description = reader["Описание"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public override string ToString()
        {
            return $"{Id}. {Name} | Описание: {Description}";
        }
    }

class Qualification
    {
        private int _id;
        private string _name = "";
        private string _description = "";

        public int Id
        {
            get { return _id; }
            set
            {
                if (value < 0) throw new ArgumentException("Id не может быть отрицательным.");
                _id = value;
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value ?? ""; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value ?? ""; }
        }

        public Qualification() { }

        public Qualification(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public static bool AddQualification(SqlConnection conn, Qualification qualification)
        {
            string query = @"INSERT INTO Квалификация (Название, Описание)
                         VALUES (@name, @description)";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@name", qualification.Name);
                cmd.Parameters.AddWithValue("@description", string.IsNullOrEmpty(qualification.Description) ? (object)DBNull.Value : qualification.Description);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool UpdateQualification(SqlConnection conn, int id, Qualification qualification)
        {
            string query = @"UPDATE Квалификация SET 
                         Название=@name, Описание=@description
                         WHERE Id=@id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", qualification.Name);
                cmd.Parameters.AddWithValue("@description", string.IsNullOrEmpty(qualification.Description) ? (object)DBNull.Value : qualification.Description);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool DeleteQualification(SqlConnection conn, int id)
        {
            string query = "DELETE FROM Квалификация WHERE Id=@id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static List<Qualification> GetAllQualifications(SqlConnection conn)
        {
            List<Qualification> list = new List<Qualification>();
            string query = "SELECT * FROM Квалификация";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Qualification
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Название"].ToString(),
                            Description = reader["Описание"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public override string ToString()
        {
            return $"{Id}. {Name} | Описание: {Description}";
        }
    }

class EducationalInstitution
    {
        private int _id;
        private string _name = "";
        private string _address = "";
        private string _phone = "";

        public int Id
        {
            get { return _id; }
            set
            {
                if (value < 0) throw new ArgumentException("Id не может быть отрицательным.");
                _id = value;
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value ?? ""; }
        }

        public string Address
        {
            get { return _address; }
            set { _address = value ?? ""; }
        }

        public string Phone
        {
            get { return _phone; }
            set { _phone = value ?? ""; }
        }

        public EducationalInstitution() { }

        public EducationalInstitution(string name, string address, string phone)
        {
            Name = name;
            Address = address;
            Phone = phone;
        }

        public static bool AddInstitution(SqlConnection conn, EducationalInstitution institution)
        {
            string query = @"INSERT INTO УчебноеЗаведение (Название, Адрес, Телефон)
                         VALUES (@name, @address, @phone)";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@name", institution.Name);
                cmd.Parameters.AddWithValue("@address", string.IsNullOrEmpty(institution.Address) ? (object)DBNull.Value : institution.Address);
                cmd.Parameters.AddWithValue("@phone", string.IsNullOrEmpty(institution.Phone) ? (object)DBNull.Value : institution.Phone);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool UpdateInstitution(SqlConnection conn, int id, EducationalInstitution institution)
        {
            string query = @"UPDATE УчебноеЗаведение SET 
                         Название=@name, Адрес=@address, Телефон=@phone
                         WHERE Id=@id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", institution.Name);
                cmd.Parameters.AddWithValue("@address", string.IsNullOrEmpty(institution.Address) ? (object)DBNull.Value : institution.Address);
                cmd.Parameters.AddWithValue("@phone", string.IsNullOrEmpty(institution.Phone) ? (object)DBNull.Value : institution.Phone);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool DeleteInstitution(SqlConnection conn, int id)
        {
            string query = "DELETE FROM УчебноеЗаведение WHERE Id=@id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static List<EducationalInstitution> GetAllInstitutions(SqlConnection conn)
        {
            List<EducationalInstitution> list = new List<EducationalInstitution>();
            string query = "SELECT * FROM УчебноеЗаведение";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new EducationalInstitution
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Название"].ToString(),
                            Address = reader["Адрес"].ToString(),
                            Phone = reader["Телефон"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public override string ToString()
        {
            return $"{Id}. {Name} | Адрес: {Address} | Телефон: {Phone}";
        }
    }

class Education
    {
        private int _id;
        private int _personId;
        private int _educationLevelId;
        private int _specialtyId;
        private int _qualificationId;
        private int _institutionId;
        private int _graduationYear;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int PersonId
        {
            get { return _personId; }
            set { _personId = value; }
        }

        public int EducationLevelId
        {
            get { return _educationLevelId; }
            set { _educationLevelId = value; }
        }

        public int SpecialtyId
        {
            get { return _specialtyId; }
            set { _specialtyId = value; }
        }

        public int QualificationId
        {
            get { return _qualificationId; }
            set { _qualificationId = value; }
        }

        public int InstitutionId
        {
            get { return _institutionId; }
            set { _institutionId = value; }
        }

        public int GraduationYear
        {
            get { return _graduationYear; }
            set { _graduationYear = value; }
        }

        public Education() { }

        public Education(int personId, int educationLevelId, int specialtyId, int qualificationId, int institutionId, int graduationYear)
        {
            PersonId = personId;
            EducationLevelId = educationLevelId;
            SpecialtyId = specialtyId;
            QualificationId = qualificationId;
            InstitutionId = institutionId;
            GraduationYear = graduationYear;
        }

        
        public static bool AddEducation(SqlConnection conn, Education edu)
        {
            string query = @"INSERT INTO Образование 
                         (IdPerson, IdEducationLevel, IdSpecialty, IdQualification, IdInstitution, ГодОкончания)
                         VALUES (@personId, @levelId, @specialtyId, @qualificationId, @institutionId, @year)";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@personId", edu.PersonId);
                cmd.Parameters.AddWithValue("@levelId", edu.EducationLevelId);
                cmd.Parameters.AddWithValue("@specialtyId", edu.SpecialtyId);
                cmd.Parameters.AddWithValue("@qualificationId", edu.QualificationId);
                cmd.Parameters.AddWithValue("@institutionId", edu.InstitutionId);
                cmd.Parameters.AddWithValue("@year", edu.GraduationYear);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool UpdateEducation(SqlConnection conn, int id, Education edu)
        {
            string query = @"UPDATE Образование SET 
                         IdPerson=@personId, IdEducationLevel=@levelId, IdSpecialty=@specialtyId, 
                         IdQualification=@qualificationId, IdInstitution=@institutionId, ГодОкончания=@year
                         WHERE Id=@id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@personId", edu.PersonId);
                cmd.Parameters.AddWithValue("@levelId", edu.EducationLevelId);
                cmd.Parameters.AddWithValue("@specialtyId", edu.SpecialtyId);
                cmd.Parameters.AddWithValue("@qualificationId", edu.QualificationId);
                cmd.Parameters.AddWithValue("@institutionId", edu.InstitutionId);
                cmd.Parameters.AddWithValue("@year", edu.GraduationYear);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool DeleteEducation(SqlConnection conn, int id)
        {
            string query = "DELETE FROM Образование WHERE Id=@id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static List<Education> GetAllEducations(SqlConnection conn)
        {
            List<Education> list = new List<Education>();
            string query = "SELECT * FROM Образование";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Education
                        {
                            Id = (int)reader["Id"],
                            PersonId = (int)reader["IdPerson"],
                            EducationLevelId = (int)reader["IdEducationLevel"],
                            SpecialtyId = (int)reader["IdSpecialty"],
                            QualificationId = (int)reader["IdQualification"],
                            InstitutionId = (int)reader["IdInstitution"],
                            GraduationYear = (int)reader["ГодОкончания"]
                        });
                    }
                }
            }
            return list;
        }

        public override string ToString()
        {
            return $"ID: {Id}, PersonID: {PersonId}, LevelID: {EducationLevelId}, SpecialtyID: {SpecialtyId}, QualificationID: {QualificationId}, InstitutionID: {InstitutionId}, Год окончания: {GraduationYear}";
        }
    }

class DocumentType
    {
        private int _id;
        private string _name = "";
        private string _description = "";

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value ?? ""; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value ?? ""; }
        }

        public DocumentType() { }

        public DocumentType(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public static bool AddDocumentType(SqlConnection conn, DocumentType docType)
        {
            string query = @"INSERT INTO ТипДокумента (Название, Описание)
                         VALUES (@name, @description)";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@name", docType.Name);
                cmd.Parameters.AddWithValue("@description", string.IsNullOrEmpty(docType.Description) ? (object)DBNull.Value : docType.Description);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool UpdateDocumentType(SqlConnection conn, int id, DocumentType docType)
        {
            string query = @"UPDATE ТипДокумента SET Название=@name, Описание=@description
                         WHERE Id=@id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", docType.Name);
                cmd.Parameters.AddWithValue("@description", string.IsNullOrEmpty(docType.Description) ? (object)DBNull.Value : docType.Description);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool DeleteDocumentType(SqlConnection conn, int id)
        {
            string query = "DELETE FROM ТипДокумента WHERE Id=@id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static List<DocumentType> GetAllDocumentTypes(SqlConnection conn)
        {
            List<DocumentType> list = new List<DocumentType>();
            string query = "SELECT * FROM ТипДокумента";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new DocumentType
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Название"].ToString(),
                            Description = reader["Описание"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public override string ToString()
        {
            return $"{Id}. {Name} | Описание: {Description}";
        }
    }

class Organization
    {
        private int _id;
        private string _name = "";
        private string _address = "";
        private string _phone = "";

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value ?? ""; }
        }

        public string Address
        {
            get { return _address; }
            set { _address = value ?? ""; }
        }

        public string Phone
        {
            get { return _phone; }
            set { _phone = value ?? ""; }
        }

        public Organization() { }

        
        public Organization(string name, string address, string phone)
        {
            Name = name;
            Address = address;
            Phone = phone;
        }

        public static bool AddOrganization(SqlConnection conn, Organization org)
        {
            string query = @"INSERT INTO Организация (Название, Адрес, Телефон)
                         VALUES (@name, @address, @phone)";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@name", org.Name);
                cmd.Parameters.AddWithValue("@address", string.IsNullOrEmpty(org.Address) ? (object)DBNull.Value : org.Address);
                cmd.Parameters.AddWithValue("@phone", string.IsNullOrEmpty(org.Phone) ? (object)DBNull.Value : org.Phone);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool UpdateOrganization(SqlConnection conn, int id, Organization org)
        {
            string query = @"UPDATE Организация SET Название=@name, Адрес=@address, Телефон=@phone
                         WHERE Id=@id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", org.Name);
                cmd.Parameters.AddWithValue("@address", string.IsNullOrEmpty(org.Address) ? (object)DBNull.Value : org.Address);
                cmd.Parameters.AddWithValue("@phone", string.IsNullOrEmpty(org.Phone) ? (object)DBNull.Value : org.Phone);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool DeleteOrganization(SqlConnection conn, int id)
        {
            string query = "DELETE FROM Организация WHERE Id=@id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static List<Organization> GetAllOrganizations(SqlConnection conn)
        {
            List<Organization> list = new List<Organization>();
            string query = "SELECT * FROM Организация";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Organization
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Название"].ToString(),
                            Address = reader["Адрес"].ToString(),
                            Phone = reader["Телефон"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public override string ToString()
        {
            return $"{Id}. {Name} | Адрес: {Address} | Телефон: {Phone}";
        }
    }

    class Program
    {
        static List<Product> products = new List<Product>();
        static List<CartItem> cart = new List<CartItem>();
        static List<Order> orders = new List<Order>();
        static List<Employee> employees = new List<Employee>();

        static string connectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=Пользователи;Trusted_Connection=True;";

        static void Main()
        {

        
        Console.OutputEncoding = System.Text.Encoding.UTF8;
            LoadSampleData();

            if (!Login()) return;

            while (true)
            {
                Console.WriteLine("\nМеню:");
                Console.WriteLine("1. Каталог товаров");
                Console.WriteLine("2. Добавить в корзину");
                Console.WriteLine("3. Просмотреть корзину");
                Console.WriteLine("4. Оформить заказ");
                Console.WriteLine("5. Управление заказами");
                Console.WriteLine("6. Управление сотрудниками");
                Console.WriteLine("7. Отчёт по продажам");
                Console.WriteLine("8. Управление физ.лицами");
                Console.WriteLine("9. Управление должностями");
                Console.WriteLine("10. Управление бaнками");
                Console.WriteLine("11. Управление картой");
                Console.WriteLine("12. Управление уровнем образования");
                Console.WriteLine("13. Управление специальностями");
                Console.WriteLine("14. Управление квалификациями");
                Console.WriteLine("15. Управление заведением");
                Console.WriteLine("16. Управление образованием");
                Console.WriteLine("17. Управление документами");
                Console.WriteLine("18. Управление организациями");
                Console.WriteLine("0. Выход");

                int choice = ReadInt("Выберите пункт меню: ");
                Console.WriteLine();

                switch (choice)
                {
                    case 1: ShowCatalog(); break;
                    case 2: AddToCart(); break;
                    case 3: ShowCart(); break;
                    case 4: PlaceOrder(); break;
                    case 5: ManageOrders(); break;
                    case 6: ManageEmployees(); break;
                    case 7: ShowSalesReport(); break;
                    case 8: ManagePersons(); break;
                    case 9: ManagePositions(); break;
                    case 10: ManageBanks(); break;
                    case 11: ManageCards(); break;
                    case 12: ManageEducationLevels(); break;
                    case 13: ManageSpecialties(); break;
                    case 14: ManageQualifications(); break;
                    case 15: ManageInstitutions(); break;
                    case 16: ManageEducations(); break;
                    case 17: ManageDocumentTypes(); break;
                    case 18: ManageOrganizations(); break;
                    case 0: return;
                    default: ConsoleWrite("❌ Неверный выбор."); break;
                }
            }
        }

        static bool Login()
        {
            Console.Write("Введите логин: ");
            string login = Console.ReadLine();

            Console.Write("Введите пароль: ");
            string password = Console.ReadLine();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                const string userQuery =
                    "SELECT КодПользователя, КодСекретногоВопроса, Ответ " +
                    "FROM Пользователь WHERE Логин = @login AND Пароль = @password";

                using (var cmd = new SqlCommand(userQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.Parameters.AddWithValue("@password", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            return Fail("Неверный логин или пароль.");

                        int questionId = (int)reader["КодСекретногоВопроса"];
                        string correctAnswer = reader["Ответ"].ToString() ?? "";
                        reader.Close();

                        string question = GetQuestionText(conn, questionId);
                        Console.WriteLine("Секретный вопрос: " + question);

                        Console.Write("Введите ответ: ");
                        string answer = Console.ReadLine();

                        if (string.Equals(answer.Trim(), correctAnswer.Trim(), StringComparison.OrdinalIgnoreCase))
                            return Success("Авторизация успешна!");
                        else
                            return Fail("Неверный ответ на секретный вопрос.");
                    }
                }
            }
        }

        static string GetQuestionText(SqlConnection conn, int id)
        {
            using (var qCmd = new SqlCommand(
                "SELECT ТекстВопроса FROM СекретныйВопрос WHERE КодСекретногоВопроса = @id", conn))
            {
                qCmd.Parameters.AddWithValue("@id", id);
                object result = qCmd.ExecuteScalar();
                return result != null ? result.ToString() : "Неизвестный вопрос";
            }
        }

        static bool Fail(string msg)
        {
            Console.WriteLine("❌ " + msg);
            return false;
        }

        static bool Success(string msg)
        {
            Console.WriteLine("✅ " + msg);
            return true;
        }
        static void ManageOrganizations()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                Console.WriteLine("1. Показать все организации");
                Console.WriteLine("2. Добавить организацию");
                Console.WriteLine("3. Изменить организацию");
                Console.WriteLine("4. Удалить организацию");

                int choice = ReadInt("Выберите действие: ");

                if (choice == 1)
                {
                    List<Organization> orgs = Organization.GetAllOrganizations(conn);
                    foreach (var org in orgs) Console.WriteLine(org);
                }
                else if (choice == 2)
                {
                    string name = ReadString("Название организации: ");
                    string address = ReadString("Адрес: ");
                    string phone = ReadString("Телефон: ");

                    if (Organization.AddOrganization(conn, new Organization(name, address, phone)))
                        Console.WriteLine("✅ Организация добавлена");
                }
                else if (choice == 3)
                {
                    int id = ReadInt("Введите ID организации для изменения: ");
                    string name = ReadString("Название организации: ");
                    string address = ReadString("Адрес: ");
                    string phone = ReadString("Телефон: ");

                    if (Organization.UpdateOrganization(conn, id, new Organization(name, address, phone)))
                        Console.WriteLine("✅ Организация обновлена");
                }
                else if (choice == 4)
                {
                    int id = ReadInt("Введите ID организации для удаления: ");
                    if (Organization.DeleteOrganization(conn, id))
                        Console.WriteLine("✅ Организация удалена");
                }
            }
        }

        static void ManageDocumentTypes()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                Console.WriteLine("1. Показать все типы документов");
                Console.WriteLine("2. Добавить тип документа");
                Console.WriteLine("3. Изменить тип документа");
                Console.WriteLine("4. Удалить тип документа");

                int choice = ReadInt("Выберите действие: ");

                if (choice == 1)
                {
                    List<DocumentType> docTypes = DocumentType.GetAllDocumentTypes(conn);
                    foreach (var dt in docTypes) Console.WriteLine(dt);
                }
                else if (choice == 2)
                {
                    string name = ReadString("Название типа документа: ");
                    string description = ReadString("Описание: ");

                    if (DocumentType.AddDocumentType(conn, new DocumentType(name, description)))
                        Console.WriteLine("✅ Тип документа добавлен");
                }
                else if (choice == 3)
                {
                    int id = ReadInt("Введите ID типа документа для изменения: ");
                    string name = ReadString("Название типа документа: ");
                    string description = ReadString("Описание: ");

                    if (DocumentType.UpdateDocumentType(conn, id, new DocumentType(name, description)))
                        Console.WriteLine("✅ Тип документа обновлён");
                }
                else if (choice == 4)
                {
                    int id = ReadInt("Введите ID типа документа для удаления: ");
                    if (DocumentType.DeleteDocumentType(conn, id))
                        Console.WriteLine("✅ Тип документа удалён");
                }
            }
        }

        static void ManageEducations()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                Console.WriteLine("1. Показать все образования");
                Console.WriteLine("2. Добавить образование");
                Console.WriteLine("3. Изменить образование");
                Console.WriteLine("4. Удалить образование");

                int choice = ReadInt("Выберите действие: ");

                if (choice == 1)
                {
                    List<Education> educations = Education.GetAllEducations(conn);
                    foreach (var edu in educations) Console.WriteLine(edu);
                }
                else if (choice == 2)
                {
                    int personId = ReadInt("ID сотрудника: ");
                    int levelId = ReadInt("ID уровня образования: ");
                    int specialtyId = ReadInt("ID специальности: ");
                    int qualificationId = ReadInt("ID квалификации: ");
                    int institutionId = ReadInt("ID учебного заведения: ");
                    int year = ReadInt("Год окончания: ");

                    if (Education.AddEducation(conn, new Education(personId, levelId, specialtyId, qualificationId, institutionId, year)))
                        Console.WriteLine("✅ Образование добавлено");
                }
                else if (choice == 3)
                {
                    int id = ReadInt("Введите ID образования для изменения: ");
                    int personId = ReadInt("ID сотрудника: ");
                    int levelId = ReadInt("ID уровня образования: ");
                    int specialtyId = ReadInt("ID специальности: ");
                    int qualificationId = ReadInt("ID квалификации: ");
                    int institutionId = ReadInt("ID учебного заведения: ");
                    int year = ReadInt("Год окончания: ");

                    if (Education.UpdateEducation(conn, id, new Education(personId, levelId, specialtyId, qualificationId, institutionId, year)))
                        Console.WriteLine("✅ Образование обновлено");
                }
                else if (choice == 4)
                {
                    int id = ReadInt("Введите ID образования для удаления: ");
                    if (Education.DeleteEducation(conn, id))
                        Console.WriteLine("✅ Образование удалено");
                }
            }
        }

        static void ManageInstitutions()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                Console.WriteLine("1. Показать все учебные заведения");
                Console.WriteLine("2. Добавить учебное заведение");
                Console.WriteLine("3. Изменить учебное заведение");
                Console.WriteLine("4. Удалить учебное заведение");

                int choice = ReadInt("Выберите действие: ");

                if (choice == 1)
                {
                    List<EducationalInstitution> institutions = EducationalInstitution.GetAllInstitutions(conn);
                    foreach (var inst in institutions) Console.WriteLine(inst);
                }
                else if (choice == 2)
                {
                    string name = ReadString("Название учебного заведения: ");
                    string address = ReadString("Адрес: ");
                    string phone = ReadString("Телефон: ");

                    if (EducationalInstitution.AddInstitution(conn, new EducationalInstitution(name, address, phone)))
                        Console.WriteLine("✅ Учебное заведение добавлено");
                }
                else if (choice == 3)
                {
                    int id = ReadInt("Введите ID учебного заведения для изменения: ");
                    string name = ReadString("Название учебного заведения: ");
                    string address = ReadString("Адрес: ");
                    string phone = ReadString("Телефон: ");

                    if (EducationalInstitution.UpdateInstitution(conn, id, new EducationalInstitution(name, address, phone)))
                        Console.WriteLine("✅ Учебное заведение обновлено");
                }
                else if (choice == 4)
                {
                    int id = ReadInt("Введите ID учебного заведения для удаления: ");
                    if (EducationalInstitution.DeleteInstitution(conn, id))
                        Console.WriteLine("✅ Учебное заведение удалено");
                }
            }
        }

        static void ManageQualifications()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                Console.WriteLine("1. Показать все квалификации");
                Console.WriteLine("2. Добавить квалификацию");
                Console.WriteLine("3. Изменить квалификацию");
                Console.WriteLine("4. Удалить квалификацию");

                int choice = ReadInt("Выберите действие: ");

                if (choice == 1)
                {
                    List<Qualification> qualifications = Qualification.GetAllQualifications(conn);
                    foreach (var q in qualifications) Console.WriteLine(q);
                }
                else if (choice == 2)
                {
                    string name = ReadString("Название квалификации: ");
                    string description = ReadString("Описание: ");

                    if (Qualification.AddQualification(conn, new Qualification(name, description)))
                        Console.WriteLine("✅ Квалификация добавлена");
                }
                else if (choice == 3)
                {
                    int id = ReadInt("Введите ID квалификации для изменения: ");
                    string name = ReadString("Название квалификации: ");
                    string description = ReadString("Описание: ");

                    if (Qualification.UpdateQualification(conn, id, new Qualification(name, description)))
                        Console.WriteLine("✅ Квалификация обновлена");
                }
                else if (choice == 4)
                {
                    int id = ReadInt("Введите ID квалификации для удаления: ");
                    if (Qualification.DeleteQualification(conn, id))
                        Console.WriteLine("✅ Квалификация удалена");
                }
            }
        }

        static void ManageSpecialties()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                Console.WriteLine("1. Показать все специальности");
                Console.WriteLine("2. Добавить специальность");
                Console.WriteLine("3. Изменить специальность");
                Console.WriteLine("4. Удалить специальность");

                int choice = ReadInt("Выберите действие: ");

                if (choice == 1)
                {
                    List<Specialty> specialties = Specialty.GetAllSpecialties(conn);
                    foreach (var s in specialties) Console.WriteLine(s);
                }
                else if (choice == 2)
                {
                    string name = ReadString("Название специальности: ");
                    string description = ReadString("Описание: ");

                    if (Specialty.AddSpecialty(conn, new Specialty(name, description)))
                        Console.WriteLine("✅ Специальность добавлена");
                }
                else if (choice == 3)
                {
                    int id = ReadInt("Введите ID специальности для изменения: ");
                    string name = ReadString("Название специальности: ");
                    string description = ReadString("Описание: ");

                    if (Specialty.UpdateSpecialty(conn, id, new Specialty(name, description)))
                        Console.WriteLine("✅ Специальность обновлена");
                }
                else if (choice == 4)
                {
                    int id = ReadInt("Введите ID специальности для удаления: ");
                    if (Specialty.DeleteSpecialty(conn, id))
                        Console.WriteLine("✅ Специальность удалена");
                }
            }
        }

        static void ManageEducationLevels()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                Console.WriteLine("1. Показать все уровни образования");
                Console.WriteLine("2. Добавить уровень образования");
                Console.WriteLine("3. Изменить уровень образования");
                Console.WriteLine("4. Удалить уровень образования");

                int choice = ReadInt("Выберите действие: ");

                if (choice == 1)
                {
                    List<EducationLevel> levels = EducationLevel.GetAllEducationLevels(conn);
                    foreach (var l in levels) Console.WriteLine(l);
                }
                else if (choice == 2)
                {
                    string name = ReadString("Название уровня образования: ");
                    string description = ReadString("Описание: ");

                    if (EducationLevel.AddEducationLevel(conn, new EducationLevel(name, description)))
                        Console.WriteLine("✅ Уровень образования добавлен");
                }
                else if (choice == 3)
                {
                    int id = ReadInt("Введите ID уровня образования для изменения: ");
                    string name = ReadString("Название уровня образования: ");
                    string description = ReadString("Описание: ");

                    if (EducationLevel.UpdateEducationLevel(conn, id, new EducationLevel(name, description)))
                        Console.WriteLine("✅ Уровень образования обновлён");
                }
                else if (choice == 4)
                {
                    int id = ReadInt("Введите ID уровня образования для удаления: ");
                    if (EducationLevel.DeleteEducationLevel(conn, id))
                        Console.WriteLine("✅ Уровень образования удалён");
                }
            }
        }

        static void ManageCards()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                Console.WriteLine("1. Показать все карты");
                Console.WriteLine("2. Добавить карту");
                Console.WriteLine("3. Изменить карту");
                Console.WriteLine("4. Удалить карту");

                int choice = ReadInt("Выберите действие: ");

                if (choice == 1)
                {
                    List<Card> cards = Card.GetAllCards(conn);
                    foreach (var c in cards) Console.WriteLine(c);
                }
                else if (choice == 2)
                {
                    string cardNumber = ReadString("Номер карты: ");
                    int employeeId = ReadInt("ID сотрудника: ");
                    int bankId = ReadInt("ID банка: ");
                    DateTime expiryDate = DateTime.Parse(ReadString("Срок действия (yyyy-MM-dd): "));

                    if (Card.AddCard(conn, new Card(cardNumber, employeeId, bankId, expiryDate)))
                        Console.WriteLine("✅ Карта добавлена");
                }
                else if (choice == 3)
                {
                    int id = ReadInt("Введите ID карты для изменения: ");
                    string cardNumber = ReadString("Номер карты: ");
                    int employeeId = ReadInt("ID сотрудника: ");
                    int bankId = ReadInt("ID банка: ");
                    DateTime expiryDate = DateTime.Parse(ReadString("Срок действия (yyyy-MM-dd): "));

                    if (Card.UpdateCard(conn, id, new Card(cardNumber, employeeId, bankId, expiryDate)))
                        Console.WriteLine("✅ Карта обновлена");
                }
                else if (choice == 4)
                {
                    int id = ReadInt("Введите ID карты для удаления: ");
                    if (Card.DeleteCard(conn, id))
                        Console.WriteLine("✅ Карта удалена");
                }
            }
        }

        static void ManageBanks()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                Console.WriteLine("1. Показать все банки");
                Console.WriteLine("2. Добавить банк");
                Console.WriteLine("3. Изменить банк");
                Console.WriteLine("4. Удалить банк");

                int choice = ReadInt("Выберите действие: ");

                if (choice == 1)
                {
                    List<Bank> banks = Bank.GetAllBanks(conn);
                    foreach (var b in banks) Console.WriteLine(b);
                }
                else if (choice == 2)
                {
                    string name = ReadString("Название банка: ");
                    string address = ReadString("Адрес: ");
                    string phone = ReadString("Телефон: ");

                    if (Bank.AddBank(conn, new Bank(name, address, phone)))
                        Console.WriteLine("✅ Банк добавлен");
                }
                else if (choice == 3)
                {
                    int id = ReadInt("Введите ID банка для изменения: ");
                    string name = ReadString("Название банка: ");
                    string address = ReadString("Адрес: ");
                    string phone = ReadString("Телефон: ");

                    if (Bank.UpdateBank(conn, id, new Bank(name, address, phone)))
                        Console.WriteLine("✅ Банк обновлён");
                }
                else if (choice == 4)
                {
                    int id = ReadInt("Введите ID банка для удаления: ");
                    if (Bank.DeleteBank(conn, id))
                        Console.WriteLine("✅ Банк удалён");
                }
            }
        }

        static void ManagePositions()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                Console.WriteLine("1. Показать все должности");
                Console.WriteLine("2. Добавить должность");
                Console.WriteLine("3. Изменить должность");
                Console.WriteLine("4. Удалить должность");

                int choice = ReadInt("Выберите действие: ");

                if (choice == 1)
                {
                    List<Position> positions = Position.GetAllPositions(conn);
                    foreach (var pos in positions) Console.WriteLine(pos);
                }
                else if (choice == 2)
                {
                    string name = ReadString("Название должности: ");
                    string description = ReadString("Описание: ");
                    decimal salary = ReadDecimal("Зарплата: ");

                    if (Position.AddPosition(conn, new Position(name, description, salary)))
                        Console.WriteLine("✅ Должность добавлена");
                }
                else if (choice == 3)
                {
                    int id = ReadInt("Введите ID должности для изменения: ");
                    string name = ReadString("Название должности: ");
                    string description = ReadString("Описание: ");
                    decimal salary = ReadDecimal("Зарплата: ");

                    if (Position.UpdatePosition(conn, id, new Position(name, description, salary)))
                        Console.WriteLine("✅ Должность обновлена");
                }
                else if (choice == 4)
                {
                    int id = ReadInt("Введите ID должности для удаления: ");
                    if (Position.DeletePosition(conn, id))
                        Console.WriteLine("✅ Должность удалена");
                }
            }
        }

        static void ManagePersons()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                Console.WriteLine("1. Show all persons\n2. Add person\n3. Update person\n4. Delete person");
                int choice = ReadInt("Select: ");

                if (choice == 1)
                {
                    List<Person> persons = Person.GetAllPersons(conn);
                    foreach (var p in persons) Console.WriteLine(p);
                }
                else if (choice == 2)
                {
                    string lastName = ReadString("Last name: ");
                    string firstName = ReadString("First name: ");
                    string middleName = ReadString("Middle name: ");
                    DateTime birthDate = DateTime.Parse(ReadString("Birth date (yyyy-MM-dd): "));
                    string passport = ReadString("Passport: ");
                    string address = ReadString("Address: ");
                    string phone = ReadString("Phone: ");

                    if (Person.AddPerson(conn, new Person(lastName, firstName, middleName, birthDate, passport, address, phone)))
                        Console.WriteLine("✅ Person added");
                }
                else if (choice == 3)
                {
                    int id = ReadInt("Enter ID to update: ");
                    string lastName = ReadString("Last name: ");
                    string firstName = ReadString("First name: ");
                    string middleName = ReadString("Middle name: ");
                    DateTime birthDate = DateTime.Parse(ReadString("Birth date (yyyy-MM-dd): "));
                    string passport = ReadString("Passport: ");
                    string address = ReadString("Address: ");
                    string phone = ReadString("Phone: ");

                    if (Person.UpdatePerson(conn, id, new Person(lastName, firstName, middleName, birthDate, passport, address, phone)))
                        Console.WriteLine("✅ Person updated");
                }
                else if (choice == 4)
                {
                    int id = ReadInt("Enter ID to delete: ");
                    if (Person.DeletePerson(conn, id))
                        Console.WriteLine("✅ Person deleted");
                }
            }
        }
        static void ShowCatalog()
        {
            Console.WriteLine("Каталог товаров:");
            foreach (var p in products.OrderBy(p => p.Id))
                Console.WriteLine($"{p.Id}. {p.Name} | Категория: {p.Category} | Цена: {p.Price:0.00} | Остаток: {p.Stock}");
        }

        static void AddToCart()
        {
            ShowCatalog();
            int id = ReadInt("Введите ID товара: ");
            Product product = products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                ConsoleWrite("❌ Товар не найден.");
                return;
            }

            int qty = ReadInt("Введите количество: ");
            if (qty <= 0)
            {
                ConsoleWrite("⚠ Количество должно быть положительным.");
                return;
            }

            if (qty > product.Stock)
            {
                ConsoleWrite($"⚠ Недостаточно на складе (доступно {product.Stock}).");
                return;
            }

            CartItem existing = cart.FirstOrDefault(ci => ci.ProductId == id);
            if (existing != null)
                existing.Quantity += qty;
            else
                cart.Add(new CartItem { ProductId = id, Quantity = qty });

            ConsoleWrite($"✅ Добавлено {qty} шт. «{product.Name}» в корзину.");
        }

        static void ShowCart()
        {
            if (!cart.Any())
            {
                ConsoleWrite("Корзина пуста.");
                return;
            }

            Console.WriteLine("Содержимое корзины:");
            foreach (var item in cart)
            {
                Product product = products.First(p => p.Id == item.ProductId);
                Console.WriteLine($"{product.Name} — {item.Quantity} шт. x {product.Price:0.00} = {(item.Quantity * product.Price):0.00}");
            }

            Console.WriteLine("Итого: " +
                cart.Sum(ci => products.First(p => p.Id == ci.ProductId).Price * ci.Quantity).ToString("0.00"));
        }

        static void PlaceOrder()
        {
            if (!cart.Any())
            {
                ConsoleWrite("Корзина пуста.");
                return;
            }

            decimal total = cart.Sum(ci => products.First(p => p.Id == ci.ProductId).Price * ci.Quantity);
            int id = orders.Count + 1;

            orders.Add(new Order
            {
                Id = id,
                Items = new List<CartItem>(cart),
                Total = total,
                Status = OrderStatus.Pending
            });

            foreach (var ci in cart)
            {
                Product p = products.First(x => x.Id == ci.ProductId);
                p.Stock -= ci.Quantity;
            }

            cart.Clear();
            ConsoleWrite($"Заказ оформлен. Номер заказа: {id}, сумма: {total:0.00}");
        }

        static void ManageOrders()
        {
            Console.WriteLine("Заказы:");
            foreach (var o in orders)
                Console.WriteLine($"Заказ {o.Id}: {o.Items.Count} товаров, сумма {o.Total:0.00}, статус {o.Status}");

            int id = ReadInt("Введите ID заказа для изменения статуса (0 - отмена): ");
            if (id == 0) return;

            Order order = orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                ConsoleWrite("❌ Заказ не найден.");
                return;
            }

            Console.WriteLine("1. Оплачен\n2. Отправлен\n3. Доставлен\n4. Отменён");
            int status = ReadInt("Выберите статус: ");

            if (status >= 1 && status <= 4)
            {
                order.Status = (OrderStatus)status;
                ConsoleWrite("✅ Статус обновлён: " + order.Status);
            }
            else
            {
                ConsoleWrite("⚠ Неверный статус.");
            }
        }

        static void ManageEmployees()
        {
            Console.WriteLine("1. Показать сотрудников");
            Console.WriteLine("2. Добавить сотрудника");
            Console.WriteLine("3. Удалить сотрудника");

            int choice = ReadInt("Выберите действие: ");

            if (choice == 1) ShowEmployees();
            else if (choice == 2) AddEmployee();
            else if (choice == 3) DeleteEmployee();
            else ConsoleWrite("Неверный выбор.");
        }

        static void ShowEmployees()
        {
            if (!employees.Any())
            {
                ConsoleWrite("Список сотрудников пуст.");
                return;
            }

            foreach (var e in employees)
                Console.WriteLine($"{e.Id}. {e.Name} | Должность: {e.Position} | Зарплата: {e.Salary:0.00}");
        }

        static void AddEmployee()
        {
            string name = ReadString("Имя сотрудника: ");
            string position = ReadString("Должность: ");
            decimal salary = ReadDecimal("Зарплата: ");
            int id = employees.Count + 1;

            employees.Add(new Employee
            {
                Id = id,
                Name = name,
                Position = position,
                Salary = salary
            });

            ConsoleWrite("✅ Сотрудник добавлен.");
        }

        static void DeleteEmployee()
        {
            int id = ReadInt("Введите ID сотрудника: ");
            Employee emp = employees.FirstOrDefault(e => e.Id == id);

            if (emp == null)
            {
                ConsoleWrite("❌ Сотрудник не найден.");
                return;
            }

            employees.Remove(emp);
            ConsoleWrite("✅ Сотрудник удалён.");
        }

        static void ShowSalesReport()
        {
            var sold = orders.Where(o =>
                o.Status == OrderStatus.Paid ||
                o.Status == OrderStatus.Shipped ||
                o.Status == OrderStatus.Delivered).ToList();

            Console.WriteLine("📊 Отчёт по продажам:");
            Console.WriteLine("Продано заказов: " + sold.Count);
            Console.WriteLine("Выручка: " + sold.Sum(o => o.Total).ToString("0.00"));
        }

        static void LoadSampleData()
        {
            products.AddRange(new[]
            {
                new Product { Id = 1, Name = "Футбольный мяч", Category = "Футбол", Price = 1200, Stock = 10 },
                new Product { Id = 2, Name = "Бутсы Nike", Category = "Футбол", Price = 3200, Stock = 5 },
                new Product { Id = 3, Name = "Перчатки Adidas", Category = "Бокс", Price = 2100, Stock = 7 }
            });

            employees.AddRange(new[]
            {
                new Employee { Id = 1, Name = "Иванов И.И.", Position = "Менеджер", Salary = 45000 },
                new Employee { Id = 2, Name = "Петров П.П.", Position = "Кладовщик", Salary = 40000 }
            });
        }

        static int ReadInt(string prompt)
        {
            Console.Write(prompt);
            int val;
            while (!int.TryParse(Console.ReadLine(), out val))
                Console.Write("Введите корректное число: ");
            return val;
        }

        static string ReadString(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        static decimal ReadDecimal(string prompt)
        {
            Console.Write(prompt);
            decimal val;
            while (!decimal.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out val))
                Console.Write("Введите корректное число: ");
            return val;
        }

        static void ConsoleWrite(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
