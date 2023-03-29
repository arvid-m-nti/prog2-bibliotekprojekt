using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static BibliotekProjekt2.Book;
using static BibliotekProjekt2.User;

namespace BibliotekProjekt2
{
    class Program
    {
        // Public static strings för pathway för usersFile och booksFile, behövs ändras om man använder på annan enhet
        public static string bookFilePath = "C:/Users/arvid.maxvall/source/repos/BibliotekProjekt2/booksFile.txt";
        public static string userFilePath = "C:/Users/arvid.maxvall/source/repos/BibliotekProjekt2/usersFile.txt";
        static void Main(string[] args)
        {
            List<User> users = new List<User>(); // Lista för användare
            List<Book> books = new List<Book>(); // Lista för böcker
            // Läser booksFile.txt och separerar saker med ',' och lägger till dem i books
            string[] bookLines = File.ReadAllLines(bookFilePath);
            foreach (var line in bookLines)
            {
                string[] entries = line.Split(',');
                Book newBook = new Book() { Title = entries[0], Author = entries[1], Year = int.Parse(entries[2]), Genre = entries[3], ISBN = entries[4], Available = bool.Parse(entries[5]), ReservedBy = new List<User>() };
                for (int i = 6; i < entries.Length; i++)
                {
                    foreach (User aUser in users)
                    {
                        if (entries[i] == aUser.PersonalNumber)
                        {
                            newBook.ReservedBy.Add(aUser);
                        }
                    }
                }
                books.Add(newBook);
            }
            // Läser usersFile.txt och separerar saker med ',' och lägger till dem i users
            string[] userLines = File.ReadAllLines(userFilePath);
            foreach (var line in userLines)
            {
                string[] entries = line.Split(',');
                User newUser = new User() { FirstName = entries[0], LastName = entries[1], PersonalNumber = entries[2], Password = entries[3], CurrentUser = bool.Parse(entries[4]), Librarian = bool.Parse(entries[5]), MyBooks = new List<Book>() };
                for (int i = 6; i < entries.Length; i++) // entries[6] och uppåt är ISBN för böcker i MyBooks, de läggs till i listan
                {
                    foreach (Book aBook in books)
                    {
                        if (entries[i] == aBook.ISBN)
                        {
                            newUser.MyBooks.Add(aBook);
                        }
                    }
                }
                users.Add(newUser);
            }
            Menu(users, books); // Efter alla värden har satts så öppnas Menu funktionen
        }
        static void Menu(List<User> users, List<Book> books) // Funktion för menyn, andra funktioner kallas härifrån
        {
            Console.Clear();
            foreach (Book aBook in books)
            {
                bool isOwned = false;
                foreach (User aUser in users)
                {
                    foreach (Book thisBook in aUser.MyBooks) // Om en bok ägs av någon så är isOwned true
                    {
                        if (aBook == thisBook)
                        {
                            isOwned = true;
                        }
                    }
                }
                if (isOwned) // Om isOwned == true så är Available false
                {
                    aBook.Available = false;
                }
                else // Om isOwned == false så är Available true
                {
                    aBook.Available = true;
                }
            }

            // Använder streamwriter för att spara användares och böckers värden i textfilerna
            using (StreamWriter userWriter = new StreamWriter(userFilePath))
            {
                foreach (User aUser in users)
                {
                    string userWriting = $"{aUser}";
                    foreach (Book aBook in aUser.MyBooks)
                    {
                        userWriting += $",{aBook.ISBN}"; // ISBN här representerar en bok som användaren har
                    }
                    userWriter.WriteLine(userWriting);
                }
            }
            using (StreamWriter bookWriter = new StreamWriter(bookFilePath))
            {
                foreach (Book aBook in books)
                {
                    string bookWriting = $"{aBook}";
                    foreach (User aUser in aBook.ReservedBy)
                    {
                        bookWriting += $",{aUser.PersonalNumber}";
                    }
                    bookWriter.WriteLine(bookWriting);
                }
            }

            User currentUser = new User();
            // currentBooks är böcker som användaren har, reservedBooks är böcker som användaren har reserverat
            bool loggedIn = false;
            // foreach funktion för att se om man är inloggad på någon användare, currentUser blir den som är inloggad
            foreach (User aUser in users)
            {
                if (aUser.CurrentUser)
                {
                    currentUser = aUser;
                    loggedIn = true;
                }
            }
            foreach (Book aBook in books)
            {
                if (aBook.Available)
                {
                    foreach (User aUser in users)
                    {
                        // Om boken är reserverad av en användare så läggs boken till i den användarens MyBooks
                        if (aBook.ReservedBy.Count >= 1)
                        {
                            if (aBook.ReservedBy[0].PersonalNumber == aUser.PersonalNumber)
                            {
                                aBook.Available = false;
                                aBook.ReservedBy.Remove(aUser);
                                aUser.MyBooks.Add(aBook);
                            }
                        }
                    }
                }
            }
            if (loggedIn)
            {
                // Bibliotekarie och medlem har olika menyer
                if (currentUser.Librarian)
                {
                    // Bibliotekarie kan redigera böcker och användare, kan även lägga till böcker
                    Console.WriteLine($"Inloggad som bibliotekarie {currentUser.FirstName} {currentUser.LastName}");
                    Console.WriteLine("(a) Mitt konto");
                    Console.WriteLine("(b) Alla böcker");
                    Console.WriteLine("(c) Användare");
                    Console.WriteLine("(d) Lägg till bok");
                    Console.WriteLine("(e) Sök böcker");
                    string menuInput = Console.ReadLine();
                    switch (menuInput.ToLower())
                    {
                        case "a":
                            Console.Clear();
                            users = ManageUser(users, currentUser); // Kör ManageUser funktionen
                            break;
                        case "b":
                            Console.Clear();
                            books = EditBook(books); // Kör EditBook funktionen
                            break;
                        case "c":
                            Console.Clear();
                            users = EditUser(users); // Kör EditUser funktionen
                            break;
                        case "d":
                            Console.Clear();
                            books = CreateBook(books); // Kör CreateBook funktionen
                            break;
                        case "e":
                            Console.Clear();
                            // Söker på en bok som man sedan kör EditBook funktionen med
                            string bookSearch = Console.ReadLine();
                            List<Book> searchedBooks = SearchBooks(books, bookSearch);
                            EditBook(searchedBooks);
                            break;
                    }
                }
                else // Om man inte är bibliotekare är man medlem
                {
                    // Medlem kan låna och lämna tillbaka böcker
                    Console.WriteLine($"Inloggad som medlem {currentUser.FirstName} {currentUser.LastName}");
                    Console.WriteLine("(a) Mitt konto");
                    Console.WriteLine("(b) Mina böcker");
                    Console.WriteLine("(c) Alla böcker");
                    Console.WriteLine("(d) Sök böcker");

                    string menuInput = Console.ReadLine();
                    switch (menuInput.ToLower())
                    {
                        case "a":
                            Console.Clear();
                            users = ManageUser(users, currentUser); // Kör ManageUser funktionen
                            break;
                        case "b":
                            Console.Clear();
                            Console.WriteLine($"{currentUser.FirstName}s böcker:");
                            Book myChosenBook = ChooseBook(currentUser.MyBooks); // Väljer en bok med ChooseBook funktionen
                            if (myChosenBook == null) { break; } // Om ChooseBook misslyckas breakas switch satsen
                            Console.WriteLine(ViewBook(myChosenBook));  // Skriver ut information om valda boken
                            Console.WriteLine($"(a) Lämna tillbaka bok");
                            Console.WriteLine($"(b) Tillbaka");
                            string myBooksInput = Console.ReadLine();
                            switch (myBooksInput.ToLower())
                            {
                                case "a":
                                    // Kör ReturnBook funktionen, om den lyckas så tas boken bort från MyBooks
                                    if (ReturnBook(myChosenBook))
                                    {
                                        myChosenBook.Available = true;
                                        currentUser.MyBooks.Remove(myChosenBook);
                                    }
                                    break;
                                case "b":
                                    Menu(users, books);
                                    break;
                            }
                            break;
                        case "c":
                            Console.Clear();
                            Book chosenBook = ChooseBook(books); // Väljer en bok med ChooseBook funktionen
                            if (chosenBook == null) { break; } // Om ChooseBook misslyckas breakas switch satsen
                            Console.WriteLine(ViewBook(chosenBook)); // Skriver ut information om valda boken
                            if (chosenBook.Available)
                            {
                                Console.WriteLine($"(a) Låna bok");
                                Console.WriteLine($"(b) Tillbaka");
                                string bookInput = Console.ReadLine();
                                switch (bookInput.ToLower())
                                {
                                    case "a":
                                        // Kör LoanBook funktionen, om den lyckas så läggs boken till i MyBooks
                                        if (LoanBook(currentUser.MyBooks, chosenBook))
                                        {
                                            chosenBook.Available = false;
                                            currentUser.MyBooks.Add(chosenBook);
                                        }
                                        break;
                                    case "b":
                                        Menu(users, books);
                                        break;
                                }
                            }
                            else
                            {
                                Console.WriteLine($"(a) Reservera bok");
                                Console.WriteLine($"(b) Tillbaka");
                                string bookInput = Console.ReadLine();
                                switch (bookInput.ToLower())
                                {
                                    case "a":
                                        // Kör ReserveBook funktionen, om den lyckas så läggs användaren till i ReservedBy
                                        if (ReserveBook(currentUser.MyBooks, chosenBook))
                                        {
                                            chosenBook.ReservedBy.Add(currentUser);
                                        }
                                        break;
                                    case "b":
                                        Menu(users, books);
                                        break;
                                }
                            }
                            break;
                        case "d":
                            Console.Clear();
                            string bookSearch = Console.ReadLine();
                            List<Book> searchedBooks = SearchBooks(books, bookSearch); // Lista med det som innehåller bookSearch
                            Book searchedChosenBook = ChooseBook(searchedBooks); // Väljer en bok från nya listan med ChooseBook
                            if (searchedChosenBook == null) { break; } // Om ChooseBook misslyckas breakas switch satsen
                            Console.WriteLine(ViewBook(searchedChosenBook)); // Skriver ut information om boken
                            Console.WriteLine($"(a) Låna bok");
                            Console.WriteLine($"(b) Tillbaka");
                            string searchedBookInput = Console.ReadLine();
                            switch (searchedBookInput.ToLower())
                            {
                                case "a":
                                    // Kör LoanBook funktionen, om den lyckas läggs boken till i MyBooks
                                    if (LoanBook(currentUser.MyBooks, searchedChosenBook))
                                    {
                                        searchedChosenBook.Available = false;
                                        currentUser.MyBooks.Add(searchedChosenBook);
                                    }
                                    break;
                                case "b":
                                    Menu(users, books);
                                    break;
                            }
                            break;
                    }
                }
            }
            else // Om man inte är inloggad:
            {
                Console.WriteLine("(a) Logga in");
                Console.WriteLine("(b) Skapa konto");
                Console.WriteLine("(c) Böcker");
                Console.WriteLine("(d) Sök böcker");
                string menuInput = Console.ReadLine();
                switch (menuInput.ToLower())
                {
                    case "a":
                        Console.Clear();
                        users = Login(users); // Kör Login funktionen
                        break;
                    case "b":
                        Console.Clear();
                        users = CreateUser(users); // Kör CreateUser funktionen
                        break;
                    case "c":
                        Console.Clear();
                        ListBooks(books); // Kör ListBooks funktionen
                        break;
                    case "d":
                        Console.Clear();
                        string bookSearch = Console.ReadLine();
                        ListBooks(SearchBooks(books, bookSearch)); // Kör ListBooks funktionen med böcker som innehåller bookSearch
                        break;
                }
            }
            Console.ReadKey();
            Menu(users, books);
        }
    }
}