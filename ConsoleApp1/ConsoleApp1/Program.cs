using System;
using System.Data.SqlClient;

class Program
{
    static string connectionString =
        "Server=localhost;Database=LibraryDB;Trusted_Connection=True;";

    static void Main()
    {
        CreateTableIfNotExists();
        InsertSampleData();
        SearchByAuthor();
    }

    static void CreateTableIfNotExists()
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Books' AND xtype='U')
            CREATE TABLE Books (
                Id INT PRIMARY KEY IDENTITY(1,1),
                Title NVARCHAR(100) NOT NULL,
                Author NVARCHAR(100) NOT NULL,
                Year INT NOT NULL
            )";

            SqlCommand command = new SqlCommand(query, connection);
            command.ExecuteNonQuery();
        }
    }

    static void InsertSampleData()
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = @"
            IF NOT EXISTS (SELECT * FROM Books)
            BEGIN
                INSERT INTO Books (Title, Author, Year)
                VALUES 
                ('1984', 'George Orwell', 1949),
                ('The Hobbit', 'J.R.R. Tolkien', 1937),
                ('Clean Code', 'Robert C. Martin', 2008)
            END";

            SqlCommand command = new SqlCommand(query, connection);
            command.ExecuteNonQuery();
        }
    }

    static void SearchByAuthor()
    {
        Console.Write("Введіть автора для пошуку: ");
        string author = Console.ReadLine();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT * FROM Books WHERE Author LIKE @Author";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Author", "%" + author + "%");

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["Id"]}");
                Console.WriteLine($"Назва: {reader["Title"]}");
                Console.WriteLine($"Автор: {reader["Author"]}");
                Console.WriteLine($"Рік: {reader["Year"]}");
                Console.WriteLine("-----------------------");
            }

            reader.Close();
        }
    }
}
