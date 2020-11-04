using System;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CallSqlFunction
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "host=localhost;db=northwind;uid=bulskov;pwd=henrik";
            
            UseAdo(connectionString);
            UseEntityFramework(connectionString);
            UseAdoViaEntityFramework(connectionString);


        }

        private static void UseEntityFramework(string connectionString)
        {
            Console.WriteLine("Entity Framework");
            var ctx = new NorthwindContex(connectionString);

            //var result = ctx.SearchResults.FromSqlRaw("select * from search({0})", "%ab%");
            var result = ctx.SearchResults.FromSqlInterpolated($"select * from search({"%ab%"})");

            foreach (var searchResult in result)
            {
                Console.WriteLine($"{searchResult.Id}, {searchResult.Name}");
            }

        }

        private static void UseAdoViaEntityFramework(string connectionString)
        {
            Console.WriteLine("ADO from Entity Framework");
            var ctx = new NorthwindContex(connectionString);
            var connection = (NpgsqlConnection) ctx.Database.GetDbConnection();
            connection.Open();

            var cmd = new NpgsqlCommand("select * from search('%ab%')", connection);

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"{reader.GetInt32(0)}, {reader.GetString(1)}");
            }

        }

        private static void UseAdo(string connectionString)
        {
            Console.WriteLine("Plain ADO");
            var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            var cmd = new NpgsqlCommand("select * from search('%ab%')", connection);

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"{reader.GetInt32(0)}, {reader.GetString(1)}");
            }
        }
    }
}
