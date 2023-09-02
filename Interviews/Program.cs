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

using System.Security.Cryptography.X509Certificates;
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

    public double ReturnBook(string bookName) 
    {
        Book bookToReturn = BorrowedBooks.FirstOrDefault(x => x.Name == bookName);
        var result =Library.Instance.ReinstateBook(bookToReturn);
        BorrowedBooks.Remove(bookToReturn);
        return result;
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
    private List<Client> clients = new List<Client>();

    private void AddClients()
    {
        Client client = new Client("Alex");
        clients.Add(client);
        client = new Client("Bogdan");
        clients.Add(client);
        client = new Client("Tudor");
        clients.Add(client);
    }
    public void DisplayPov()
    {
        Console.Clear();
        Console.WriteLine("Biblioteca - Sistem de Management");
        Console.WriteLine("1. POV Client");
        Console.WriteLine("2. POV Biblioteca");
        Console.WriteLine("3. Iesire");
    }

    #region ClientJourney

    public void ClientJourney()
    {
        Console.Clear();
        Console.WriteLine("1. Alex");
        Console.WriteLine("2. Bogdan");
        Console.WriteLine("3. Tudor");
        Console.WriteLine("4. Inapoi");
        Console.Write("Alegeti client: ");
        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                Client client1 = clients.FirstOrDefault(x => x.Name == "Alex");
                DisplayClientOptions(client1);
                break;
            case "2":
                Client client2 = clients.FirstOrDefault(x => x.Name == "Bogdan");
                DisplayClientOptions(client2);
                break;
            case "3":
                Client client3 = clients.FirstOrDefault(x => x.Name == "Tudor");
                DisplayClientOptions(client3); 
                break;
            case "4":
                
                break;
            default:
                Console.WriteLine("Optiune invalida. Reincercati.");
                break;
        }
    }

    public void DisplayClientOptions(Client client)
    {
        Console.Clear();
        Console.WriteLine($"Salut {client.Name}, ce doresti sa faci astazi?");
        Console.WriteLine("1. Imprumuta carte");
        Console.WriteLine("2. Returneaza carte");
        Console.WriteLine("3. Verifica carti imprumutate");
        Console.WriteLine("4. Inapoi");
        Console.Write("Selectati o optiune: ");
        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                ClientBorrow(client);
                break;
            case "2":
                ClientReturn(client);
                break;
            case "3":
                CheckClientInventory(client);
                break;
            case "4":
                ClientJourney();
                break;
            default:
                Console.WriteLine("Optiune invalida. Reincercati.");
                break;
        }
    }

    public void CheckClientInventory(Client client)
    {
        Console.Clear();
        Console.WriteLine($"Inventar:");
        int counter = 0;

        foreach (var book in client.BorrowedBooks)
        {
            counter++;
            var daysSinceBorrow = DateTime.UtcNow - book.BorrowingDate;
            Console.WriteLine($"{counter}. {book.Name}, imprumutata acum {daysSinceBorrow.Value.Days} zile");
        }
        Console.WriteLine("Apasati enter pentru a merge inapoi");
        Console.ReadLine();
        DisplayClientOptions(client);
    }

    public void ClientBorrow(Client client)
    {
        Console.Clear();
        Console.WriteLine($"Alegeti o carte");
        int counter = 0;
        var distinctBooks = Library.Instance.Books.DistinctBy(book => book.Name);

        foreach (var book in distinctBooks)
        {
            counter++;
            Console.WriteLine($"{counter}. {book.Name}");
        }
        if(counter == 0)
        {
            Console.WriteLine("Nu avem carti");
            Thread.Sleep(3000);
            DisplayClientOptions(client);
        }
        Console.WriteLine("Selectati o carte (nume complet): ");
        string choice = Console.ReadLine();

        client.BorrowBook(choice);

        Console.WriteLine($"Cartea {choice} a fost imprumutata de {client.Name}");
        Console.WriteLine("Apasati enter pentru a merge inapoi");
        Console.ReadLine();
        DisplayClientOptions(client);
    }

    public void ClientReturn(Client client)
    {
        Console.Clear();
        Console.WriteLine($"Alegeti o carte de returnat");
        int counter = 0;

        foreach (var book in client.BorrowedBooks)
        {
            counter++;
            Console.WriteLine($"{counter}. {book.Name}");
        }
        Console.WriteLine("Selectati o carte (nume complet): ");
        string choice = Console.ReadLine();

        var price = client.ReturnBook(choice);

        Console.WriteLine($"Cartea {choice} a fost returnata de {client.Name}. Trebuie platit {price}");
        Console.WriteLine("Apasati enter pentru a merge inapoi");
        Console.ReadLine();
        DisplayClientOptions(client);
    }
    #endregion
    #region LibraryJourney
    public void LibraryJourney()
    {
        Console.Clear();
        Console.WriteLine("1. Adauga carte noua");
        Console.WriteLine("2. Sterge o carte");
        Console.WriteLine("3. Verifica disponibilitate carte");
        Console.WriteLine("4. Vezi toate cartile");
        Console.WriteLine("5. Inapoi");
        Console.WriteLine("Alege o optiune:");
        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                AddBook();
                break;
            case "2":
                DeleteBook();
                break;
            case "3":
                CheckBookAvailability();
                break;
            case "4":
                GetBooks();
                break;
            case "5":
                break;
            default:
                Console.WriteLine("Optiune invalida. Reincercati.");
                break;
        }

    }

    public void AddBook()
    {
        Console.Clear();
        Console.WriteLine("Vom avea nevoe de un ISBN, un nume si un pret de inchiriere.");
        Console.WriteLine("ISBN:");
        string isbn = Console.ReadLine();
        Console.WriteLine("Nume:");
        string name = Console.ReadLine();
        Console.WriteLine("Pret:");
        double price = double.Parse(Console.ReadLine());

        Book newBook = new Book(isbn, name, price);
        Library.Instance.AddBook(newBook);
        Console.WriteLine($"Cartea {newBook.Name} a fost adaugata");
        Console.WriteLine("Apasati enter pentru a merge inapoi");
        Console.ReadLine();
        LibraryJourney();
    }

    public void DeleteBook()
    {
        Console.Clear();
        Console.WriteLine($"Alegeti o carte");
        int counter = 0;
        var distinctBooks = Library.Instance.Books.DistinctBy(book => book.Name);

        foreach (var book in distinctBooks)
        {
            counter++;
            Console.WriteLine($"{counter}. {book.Name}");
        }
        if(counter == 0)
        {
            Console.WriteLine("Nu avem carti");
            Console.WriteLine("Apasati enter pentru a merge inapoi");
            Console.ReadLine();
            LibraryJourney();
        }
        Console.WriteLine("Selectati o carte pentru a o sterge (nume complet): ");
        string choice = Console.ReadLine();

        Book bookToDelete = distinctBooks.FirstOrDefault(x => x.Name == choice);
        Library.Instance.RemoveBook(bookToDelete);
        Console.WriteLine($"Cartea {choice} a fost stearsa");
        Console.WriteLine("Apasati enter pentru a merge inapoi");
        Console.ReadLine();
        LibraryJourney();
    }

    public void CheckBookAvailability()
    {
        Console.Clear();
        Console.WriteLine($"Alegeti o carte");
        int counter = 0;
        var distinctBooks = Library.Instance.Books.DistinctBy(book => book.Name);

        foreach (var book in distinctBooks)
        {
            counter++;
            Console.WriteLine($"{counter}. {book.Name}");
        }
        if (counter == 0)
        {
            Console.WriteLine("Nu avem carti");
            Thread.Sleep(3000);
            LibraryJourney();
        }
        Console.WriteLine("Selectati carte (nume complet): ");
        string choice = Console.ReadLine();

        int result = Library.Instance.CheckBookAvailability(choice);
        Console.WriteLine($"Avem {result} carti {choice} in stoc");
        Console.WriteLine("Apasati enter pentru a merge inapoi");
        Console.ReadLine();
        LibraryJourney();
    }

    public void GetBooks()
    {
        Console.Clear();
        int counter = 0;
        var distinctBooks = Library.Instance.Books.DistinctBy(book => book.Name);

        foreach (var book in distinctBooks)
        {
            counter++;
            Console.WriteLine($"{counter}. {book.Name}");
        }
        if (counter == 0)
        {
            Console.WriteLine("Nu avem carti");
            Thread.Sleep(3000);
            LibraryJourney();
        }

        Console.WriteLine("Apasati enter pentru a merge inapoi");
        Console.ReadLine();
        LibraryJourney();
    }

    #endregion

    public void RunMenu()
    {
        AddClients();
        while(true)
        {
            DisplayPov();
            Console.Write("Selectati o optiune: ");
            string choice = Console.ReadLine();

            switch(choice)
            {
                case "1":
                    ClientJourney();
                    break;
                case "2":
                    LibraryJourney();
                    break;
                case "3":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Optiune invalida. Reincercati.");
                    break;
            }
        }
    }
}


class Program
{
    static void Main(string[] args)
    {
        Menu menu = new Menu();

        menu.RunMenu();
    }
}