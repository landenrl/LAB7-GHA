using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services
{
    public class LibraryService
    {
        private List<Book> books;
        private List<User> users;
        private Dictionary<int, List<Book>> borrowedBooks;
        private readonly string booksFilePath;
        private readonly string usersFilePath;
        private readonly bool isTestMode;

        public LibraryService(string booksFilePath = "Data/Books.csv", string usersFilePath = "Data/Users.csv", bool isTestMode = false)
        {
            this.booksFilePath = booksFilePath;
            this.usersFilePath = usersFilePath;
            this.isTestMode = isTestMode;

            books = new List<Book>();
            users = new List<User>();
            borrowedBooks = new Dictionary<int, List<Book>>();

            if (!isTestMode)
            {
                books = ReadBooksFromCsv();
                users = ReadUsersFromCsv();
            }
        }

        public List<Book> ReadBooks()
        {
            return books;
        }

        public List<User> ReadUsers()
        {
            return users;
        }

        public void AddBook(Book book)
        {
            book.Id = books.Any() ? books.Max(b => b.Id) + 1 : 1;
            books.Add(book);

            if (!isTestMode)
            {
                WriteBooksToCsv();
            }
        }

        public void EditBook(Book updatedBook)
        {
            var existingBook = books.FirstOrDefault(b => b.Id == updatedBook.Id);
            if (existingBook != null)
            {
                existingBook.Title = updatedBook.Title;
                existingBook.Author = updatedBook.Author;
                existingBook.ISBN = updatedBook.ISBN;

                if (!isTestMode)
                {
                    WriteBooksToCsv();
                }
            }
        }

        public void DeleteBook(int bookId)
        {
            var bookToRemove = books.FirstOrDefault(b => b.Id == bookId);
            if (bookToRemove != null)
            {
                books.Remove(bookToRemove);

                if (!isTestMode)
                {
                    WriteBooksToCsv();
                }
            }
        }

        public void AddUser(User user)
        {
            user.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
            users.Add(user);

            if (!isTestMode)
            {
                WriteUsersToCsv();
            }
        }

        public void EditUser(User updatedUser)
        {
            var existingUser = users.FirstOrDefault(u => u.Id == updatedUser.Id);
            if (existingUser != null)
            {
                existingUser.Name = updatedUser.Name;
                existingUser.Email = updatedUser.Email;

                if (!isTestMode)
                {
                    WriteUsersToCsv();
                }
            }
        }

        public void DeleteUser(int userId)
        {
            var userToRemove = users.FirstOrDefault(u => u.Id == userId);
            if (userToRemove != null)
            {
                users.Remove(userToRemove);

                if (!isTestMode)
                {
                    WriteUsersToCsv();
                }
            }
        }

        public bool BorrowBook(int userId, int bookId)
        {
            var book = books.FirstOrDefault(b => b.Id == bookId);
            if (book == null) return false;

            if (!borrowedBooks.ContainsKey(userId))
            {
                borrowedBooks[userId] = new List<Book>();
            }

            borrowedBooks[userId].Add(book);
            return true;
        }

        public bool ReturnBook(int userId, int bookId)
        {
            if (borrowedBooks.ContainsKey(userId))
            {
                var bookToReturn = borrowedBooks[userId].FirstOrDefault(b => b.Id == bookId);
                if (bookToReturn != null)
                {
                    borrowedBooks[userId].Remove(bookToReturn);

                    if (!borrowedBooks[userId].Any())
                    {
                        borrowedBooks.Remove(userId);
                    }
                    return true;
                }
            }
            return false;
        }

        public Dictionary<int, List<Book>> GetBorrowedBooks()
        {
            return borrowedBooks;
        }

        private List<Book> ReadBooksFromCsv()
        {
            var booksList = new List<Book>();

            try
            {
                foreach (var line in File.ReadLines(booksFilePath))
                {
                    var pattern = @"(?:^|,)(?:""([^""]*)""|([^,]*))";
                    var matches = Regex.Matches(line, pattern);
                    var fields = new List<string>();

                    foreach (Match match in matches)
                    {
                        if (match.Groups[1].Success)
                        {
                            fields.Add(match.Groups[1].Value);
                        }
                        else
                        {
                            fields.Add(match.Groups[2].Value);
                        }
                    }

                    if (fields.Count >= 4)
                    {
                        booksList.Add(new Book
                        {
                            Id = int.Parse(fields[0].Trim()),
                            Title = fields[1].Trim(),
                            Author = fields[2].Trim(),
                            ISBN = fields[3].Trim()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading books: {ex.Message}");
            }

            return booksList;
        }

        private List<User> ReadUsersFromCsv()
        {
            var usersList = new List<User>();

            try
            {
                foreach (var line in File.ReadLines(usersFilePath))
                {
                    var fields = line.Split(',');

                    if (fields.Length >= 3)
                    {
                        usersList.Add(new User
                        {
                            Id = int.Parse(fields[0].Trim()),
                            Name = fields[1].Trim(),
                            Email = fields[2].Trim()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading users: {ex.Message}");
            }
            return usersList;
        }

        private void WriteBooksToCsv()
        {
            try
            {
                using (var writer = new StreamWriter(booksFilePath, false))
                {
                    foreach (var book in books)
                    {
                        writer.WriteLine($"{book.Id},{book.Title},{book.Author},{book.ISBN}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while writing books: {ex.Message}");
            }
        }

        private void WriteUsersToCsv()
        {
            try
            {
                using (var writer = new StreamWriter(usersFilePath, false))
                {
                    foreach (var user in users)
                    {
                        writer.WriteLine($"{user.Id},{user.Name},{user.Email}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while writing users: {ex.Message}");
            }
        }
    }
}
