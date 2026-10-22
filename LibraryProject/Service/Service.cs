using Dapper;
using LibraryProject.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Numerics;
using System.Text;

namespace LibraryProject.Service
{
    public class ServiceL
    {
        private string _connectionString = @"Server=VIVOBOOK16\MSSQLSERVER_2022;Database=LibraryProject;Integrated Security=True; TrustServerCertificate=True;";

        public ServiceL(){
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
        }
        public ServiceL(string connectionString)
        {
            _connectionString = connectionString;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
        }
        public IEnumerable<Book> SearchBooks(string searchTerm)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT * FROM Books 
                           WHERE title LIKE @Search 
                              OR author LIKE @Search 
                              OR genre LIKE @Search";

                return db.Query<Book>(sql, new { Search = $"%{searchTerm}%" });
            }
        }
        public void AddBook(Book book)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Books (title, author, publisher, genre, pages_count, publish_year, cost_price, retail_price, stock_quantity, parent_book_id) 
                           VALUES (@title, @author, @publisher, @genre, @pages_count, @publish_year, @cost_price, @retail_price, @stock_quantity, @parent_book_id)";

                db.Execute(sql, book);
            }
        }
        public bool DeleteBook(int bookId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM Books WHERE book_id = @bookId";
                int rowsAffected = db.Execute(sql, new { bookId });
                return rowsAffected > 0;
            }
        }

        public bool UpdateBook(Book book)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE Books 
                           SET title = @title, author = @author, publisher = @publisher, genre = @genre, 
                               pages_count = @pages_count, publish_year = @publish_year, cost_price = @cost_price, 
                               retail_price = @retail_price, stock_quantity = @stock_quantity, parent_book_id = @parent_book_id
                           WHERE book_id = @book_id";

                int rowsAffected = db.Execute(sql, book);
                return rowsAffected > 0;
            }
        }

        public bool SellBook(int bookId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = "UPDATE Books SET stock_quantity = stock_quantity - 1 WHERE book_id = @bookId AND stock_quantity > 0";
                return db.Execute(sql, new { bookId }) > 0;
            }
        }

        public bool WriteOffBook(int bookId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = "UPDATE Books SET stock_quantity = stock_quantity - 1 WHERE book_id = @bookId AND stock_quantity > 0";
                return db.Execute(sql, new { bookId }) > 0;
            }
        }

        public void AddBookToPromotion(string promoName, decimal discount, int bookId, DateTime start, DateTime end)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Promotions (name, discount_percent, book_id, start_date, end_date) 
                           VALUES (@promoName, @discount, @bookId, @start, @end)";

                db.Execute(sql, new { promoName, discount, bookId, start, end });
            }
        }

        public bool ReserveBook(int bookId, string customerName)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string updateSql = "UPDATE Books SET stock_quantity = stock_quantity - 1 WHERE book_id = @bookId AND stock_quantity > 0";
                int updated = db.Execute(updateSql, new { bookId });

                if (updated > 0)
                {
                    string insertSql = "INSERT INTO Reservations (book_id, customer_name, quantity, status) VALUES (@bookId, @customerName, 1, 'active')";
                    db.Execute(insertSql, new { bookId, customerName });
                    return true;
                }

                return false;
            }
        }






        public IEnumerable<Book> SearchBooks(string title, string author, string genre)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT * FROM Books 
                       WHERE (@Title IS NULL OR title LIKE @Title)
                         AND (@Author IS NULL OR author LIKE @Author)
                         AND (@Genre IS NULL OR genre LIKE @Genre)";

                return db.Query<Book>(sql, new
                {
                    Title = string.IsNullOrEmpty(title) ? null : $"%{title}%",
                    Author = string.IsNullOrEmpty(author) ? null : $"%{author}%",
                    Genre = string.IsNullOrEmpty(genre) ? null : $"%{genre}%"
                });
            }
        }

        public IEnumerable<Book> GetNewBooks()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = "SELECT TOP 10 * FROM Books ORDER BY publish_year DESC, book_id DESC";
                return db.Query<Book>(sql);
            }
        }

        private string GetDateFilter(string period)
        {
            return period.ToLower() switch
            {
                "day" => "DATEADD(day, -1, GETDATE())",
                "week" => "DATEADD(week, -1, GETDATE())",
                "month" => "DATEADD(month, -1, GETDATE())",
                "year" => "DATEADD(year, -1, GETDATE())",
                _ => "DATEADD(month, -1, GETDATE())" 
            };
        }

        public IEnumerable<dynamic> GetTopBooks(string period)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string dateCondition = GetDateFilter(period);

                string sql = $@"SELECT TOP 5 b.book_id, b.title, b.author, COUNT(r.reservation_id) AS total_reserved
                        FROM Reservations r
                        JOIN Books b ON r.book_id = b.book_id
                        -- Тут фільтр по даті (якщо у вашій таблиці буде колонка дати, наразі групуємо за актуальними)
                        GROUP BY b.book_id, b.title, b.author
                        ORDER BY total_reserved DESC";

                return db.Query(sql);
            }
        }

        public IEnumerable<dynamic> GetTopAuthors(string period)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT TOP 3 b.author, COUNT(r.reservation_id) AS popularity_score
                       FROM Reservations r
                       JOIN Books b ON r.book_id = b.book_id
                       GROUP BY b.author
                       ORDER BY popularity_score DESC";

                return db.Query(sql);
            }
        }

        public IEnumerable<dynamic> GetTopGenres(string period)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT TOP 3 b.genre, COUNT(r.reservation_id) AS popularity_score
                       FROM Reservations r
                       JOIN Books b ON r.book_id = b.book_id
                       GROUP BY b.genre
                       ORDER BY popularity_score DESC";

                return db.Query(sql);
            }
        }
    }
}