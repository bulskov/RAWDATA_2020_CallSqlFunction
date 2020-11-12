using System;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CallSqlFunction
{
    /*
         
         Assume that the following function is present in the DB:
         
            CREATE OR REPLACE FUNCTION search (pattern VARCHAR) 
               RETURNS TABLE (
                  p_id INT,
	               p_name VARCHAR
            ) 
            AS $$
            BEGIN
               RETURN QUERY SELECT
                  productid,
	              productname
               FROM
                  products
               WHERE
                  productname LIKE pattern;
            END; $$ 
             
            LANGUAGE 'plpgsql';
         
            You can the use either ADO or Entity Framework to execute the function.

             CREATE OR REPLACE FUNCTION insertcategory(id int, name varchar, description varchar)
              RETURNS void AS $$
            BEGIN
               insert into categories values(id, name, desc);
            END
            $$  
            LANGUAGE plpgsql;
         */

    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "host=localhost;db=northwind;uid=bulskov;pwd=henrik";
            
            UseAdo(connectionString);
            UseEntityFramework(connectionString);
            UseAdoViaEntityFramework(connectionString);
            UseEntityFrameworkToCallFunction(connectionString);


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

        private static void UseEntityFrameworkToCallFunction(string connectionString)
        {
            Console.WriteLine("ADO from Entity Framework");
            var ctx = new NorthwindContex(connectionString);

            int id = 101;
            string name = "testing";
            string description = "testing desc";

            ctx.Database.ExecuteSqlInterpolated($"select insertcategory({id},{name},{description})");

            var category = ctx.Categories.Find(id);

            Console.WriteLine("Newly inserted category:");
            Console.WriteLine($"Id={category.Id}, Name={category.Name}, Description={category.Description}");

            ctx.Categories.Remove(category);

            ctx.SaveChanges();
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
