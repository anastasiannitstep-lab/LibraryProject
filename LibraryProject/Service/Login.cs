using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Xml.Linq;

namespace LibraryProject.Service
{
    internal class Login
    {
        private string _connectionString = @"Server=VIVOBOOK16\MSSQLSERVER_2022;Database=LibraryProject;Integrated Security=True; TrustServerCertificate=True;";
        public Login(string connectionString = "")
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                _connectionString = connectionString;
            }
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
        }
        public bool ReturnTrueORFalse(string name, string password) {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                string sqlQuery = "SELECT * FROM Users WHERE username = @Name AND password_hash = @Password";

                var user = db.QueryFirstOrDefault<dynamic>(sqlQuery, new { Name = name, Password = password });

                if (user != null)
                {
                    Console.WriteLine("Вас допущено до секретних знань Бібліотеки");
                    return true;
                }
                else
                {
                    Console.WriteLine("Або ім'я чи пароль неправильні, або ви не є учасником Бібліотеки");
                    return false;
                }
            }
        }
    }
}
