/*
 
Deci

Biblioteca ->   Carti
                GetCarti
                GetNrExemplare(Carte carte)
                Imprumuta(carte)
                Restituie(carte)
                

Carte ->    Nume
            ISBN
            Pret de inchiriere

Meniu
            


 
 
 */

using static System.Reflection.Metadata.BlobBuilder;

public class Client
{
    public static int Id { get; set; } = 0;
    public string Name { get; set; }
    public List<Book> BorrowedBooks { get; set; } = new List<Book>();

    public Client(string name)
    {
        Id++;
        Name = name;
    }
    public void BorrowBook(string bookName)
    {
        if (Library.Instance.CheckBookAvailability(bookName) > 0)
        {
            BorrowedBooks.Add(Library.Instance.Books.FirstOrDefault(x => x.Name == bookName));
            Library.Instance.BorrowBook(bookName);
        }
    }

    public void ReturnBook(string bookName) 
    {
        Book bookToReturn = BorrowedBooks.FirstOrDefault(x => x.Name == bookName);
        Library.Instance.ReinstateBook(bookToReturn);
        BorrowedBooks.Remove(bookToReturn);
    }
}

public class Book
{
    public string Isbn { get; set; } // conform unui google search isbn-ul este diferit la fiecare exemplar al aceleiasi carti asa ca il vom folosi ca pe un id
    public string Name { get; set; }
    public double Price { get; set; }
    public DateTime? BorrowingDate { get; set; }

    public Book(string isbn, string name, double price)
    {
        Isbn = isbn;
        Name = name;
        Price = price;
        BorrowingDate = null;
    }
}

public class Library
{
    public List<Book> Books { get; set; }
    public List<Book> BorrowedBooks { get; set; }

    private static TimeSpan MaxBorrowTime = TimeSpan.FromDays(14);

    private static readonly Library instance = new Library();

    private Library()
    {
        Books = new List<Book>();
        BorrowedBooks= new List<Book>();
    }

    public static Library Instance
    {
        get
        {
            return instance;
        }
    }

    public void AddBook(Book book)
    {
        Books.Add(book);
    }

    public void RemoveBook(Book book) 
    {
        Books.Remove(book);
    }

    public void BorrowBook(string bookName)
    {
        Book bookToBorrow = Books.FirstOrDefault(x => x.Name == bookName);
       
        Books.Remove(bookToBorrow);

        bookToBorrow.BorrowingDate = DateTime.Now;

        BorrowedBooks.Add(bookToBorrow);

    }

    public double ReinstateBook(Book book)
    {
        BorrowedBooks.Remove(book);

        double price = CalculatePrice(book.Price, book.BorrowingDate);
        book.BorrowingDate = null;

        Books.Add(book);

        return price;
    }

    private double CalculatePrice(double price, DateTime? borrowingDate)
    {
        var borrowTime = DateTime.Now - borrowingDate;
        if (borrowTime > MaxBorrowTime)
        {
            var lateDays = MaxBorrowTime - borrowTime;

            return price + 1 / 100 * price * lateDays.Value.Days;
        }
        else
        {
            return price;
        }
    }

    public int CheckBookAvailability(string bookName)
    {
        return Books.Count(x=> x.Name == bookName);
    }

}

public class Menu
{

}


class Program
{
    static void Main(string[] args)
    {
        Book book1 = new Book("1", "carte1", 100);
        Book book2 = new Book("2", "carte2", 200);
        Book book3 = new Book("3", "carte3", 300);
        Library.Instance.AddBook(book1);
        Library.Instance.AddBook(book2);
        Library.Instance.AddBook(book3);

        var user = new Client("Alex");

        user.BorrowBook("carte1");
        user.BorrowBook("carte2");
        user.ReturnBook("carte2");

        foreach(var book in Library.Instance.Books)
        {
            Console.WriteLine(book.Name);
        }
        foreach (var book in Library.Instance.BorrowedBooks)
        {
            Console.WriteLine(book.Name);
        }


    }
}