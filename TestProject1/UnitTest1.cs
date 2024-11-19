using System.Collections.Generic;
using System.Linq;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject1
{
    [TestClass]
    public class LibraryServiceTests
    {
        private LibraryService libraryService;

        [TestInitialize]
        public void SetUp()
        {
            libraryService = new LibraryService(isTestMode: true);
        }

        [TestMethod]
        public void AddBook_ShouldAddBookToCollection()
        {
            // Arrange
            var newBook = new Book { Title = "New Book", Author = "New Author", ISBN = "394573985" };

            // Act
            libraryService.AddBook(newBook);

            // Assert
            var books = libraryService.ReadBooks();
            Assert.IsTrue(books.Exists(b => b.Title == "New Book" && b.Author == "New Author"), "Book was not added to the collection.");
        }

        [TestMethod]
        public void EditBook_ShouldUpdateBookDetails()
        {
            // Arrange
            var book = new Book { Title = "Original Title", Author = "Original Author", ISBN = "1111" };
            libraryService.AddBook(book);

            var updatedBook = new Book { Id = book.Id, Title = "Updated Title", Author = "Updated Author", ISBN = "222" };

            // Act
            libraryService.EditBook(updatedBook);

            // Assert
            var books = libraryService.ReadBooks();
            var editedBook = books.Find(b => b.Id == book.Id);
            Assert.IsNotNull(editedBook, "Edited book not found.");
            Assert.AreEqual("Updated Title", editedBook.Title, "Book title was not updated.");
            Assert.AreEqual("Updated Author", editedBook.Author, "Book author was not updated.");
            Assert.AreEqual("222", editedBook.ISBN, "Book ISBN was not updated.");
        }

        [TestMethod]
        public void DeleteBook_ShouldRemoveBookFromCollection()
        {
            // Arrange
            var book = new Book { Title = "To Delete", Author = "Author", ISBN = "3333333333" };
            libraryService.AddBook(book);

            // Act
            libraryService.DeleteBook(book.Id);

            // Assert
            var books = libraryService.ReadBooks();
            Assert.IsFalse(books.Exists(b => b.Id == book.Id), "Book was not deleted.");
        }

        [TestMethod]
        public void AddUser_ShouldAddUserToCollection()
        {
            // Arrange
            var newUser = new User { Name = "New User", Email = "newuser@example.com" };

            // Act
            libraryService.AddUser(newUser);

            // Assert
            var users = libraryService.ReadUsers();
            Assert.IsTrue(users.Exists(u => u.Name == "New User" && u.Email == "newuser@example.com"), "User was not added to the collection.");
        }

        [TestMethod]
        public void EditUser_ShouldUpdateUserDetails()
        {
            // Arrange
            var user = new User { Name = "Original User", Email = "original@example.com" };
            libraryService.AddUser(user);

            var updatedUser = new User { Id = user.Id, Name = "Updated User", Email = "updated@example.com" };

            // Act
            libraryService.EditUser(updatedUser);

            // Assert
            var users = libraryService.ReadUsers();
            var editedUser = users.Find(u => u.Id == user.Id);
            Assert.IsNotNull(editedUser, "Edited user not found.");
            Assert.AreEqual("Updated User", editedUser.Name, "User name was not updated.");
            Assert.AreEqual("updated@example.com", editedUser.Email, "User email was not updated.");
        }

        [TestMethod]
        public void DeleteUser_ShouldRemoveUserFromCollection()
        {
            // Arrange
            var user = new User { Name = "User to Delete", Email = "delete@example.com" };
            libraryService.AddUser(user);

            // Act
            libraryService.DeleteUser(user.Id);

            // Assert
            var users = libraryService.ReadUsers();
            Assert.IsFalse(users.Exists(u => u.Id == user.Id), "User was not deleted.");
        }

        [TestMethod]
        public void BorrowBook_ShouldAddBookToUserBorrowedBooks()
        {
            // Arrange
            var userId = 1;
            var book = new Book { Title = "Borrowed Book", Author = "Author", ISBN = "4444444444" };
            libraryService.AddBook(book);

            // Act
            var result = libraryService.BorrowBook(userId, book.Id);

            // Assert
            Assert.IsTrue(result, "Book was not successfully borrowed.");
            var borrowedBooks = libraryService.GetBorrowedBooks();
            Assert.IsTrue(borrowedBooks[userId].Exists(b => b.Id == book.Id), "Book not found in user's borrowed list.");
        }
    }
}
