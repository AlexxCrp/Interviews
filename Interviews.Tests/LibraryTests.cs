using FakeItEasy;
using FluentAssertions;

namespace Interviews.Tests
{
    public class LibraryTests
    {
        [Fact]
        public void Client_BorrowBook_RemovesBookFromLibrary()
        {
            //Arrange
            var client = new Client("Alex");
            var book = new Book("12345", "Dune", 4);
            Library.Instance.AddBook(book);
            //Act
            client.BorrowBook("Dune");
            //Assert
            Library.Instance.Books.Should().NotContain(book);
            client.BorrowedBooks.Should().Contain(book);
        }
        [Fact]
        public void Client_ReturnBook_AddsBookBackToLibrary()
        {
            //Arrange
            var client = new Client("Alex");
            var book = new Book("12345", "Dune", 4);
            client.BorrowedBooks.Add(book);
            //Act
            client.ReturnBook("Dune");
            //Assert
            Library.Instance.Books.Should().Contain(book);
            client.BorrowedBooks.Should().NotContain(book);
        }
        [Fact]
        public void Library_CalculatePrice_ReturnsCorectPriceAfterLongerThan14Days()
        {
            //Arrange
            var initialPrice = 20;
            var borrowingDate = DateTime.Now.AddDays(-15);

            var book = new Book("12345", "Dune", initialPrice);
            book.BorrowingDate = borrowingDate;
            //Act
            var calculatedPrice = Library.Instance.ReinstateBook(book);
            //Assert
            var expectedPrice = initialPrice + initialPrice * 0.01;
            calculatedPrice.Should().Be(expectedPrice);
        }
        
    }
}