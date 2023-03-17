using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BibliotekProjekt2
{
    class Program
    {
        public class User
        {
            // Strings för förnamn, efternamn, personnummer & lösenord
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string PersonalNumber { get; set; }
            public string Password { get; set; }
            public bool CurrentUser { get; set; } // Boolean för att se om man är inloggad på den specifika användaren
            public bool Librarian { get; set; } // Boolean för att se om användaren är bibliotekarie
            public List<Book> MyBooks { get; set; } // Lista för de böcker som användaren har
            public override string ToString() // Sätter in klassens värden i en string
            {
                return $"{FirstName},{LastName},{PersonalNumber},{Password},{CurrentUser},{Librarian}";
            }
        }
        public class Book
        {
            // String för titel, författare, genre & ISBN, samt en integer för året boken släpptes
            public string Title { get; set; }
            public string Author { get; set; }
            public int Year { get; set; }
            public string Genre { get; set; }
            public string ISBN { get; set; }
            public bool Available { get; set; } // Boolean för om boken ägs av någon
            public List<User> ReservedBy { get; set; } // Lista för de användare som reserverat boken
            public override string ToString() // Sätter in klassens värden i en string
            {
                return $"{Title},{Author},{Year},{Genre},{ISBN},{Available}";
            }
        }
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
                for(int i = 6; i < entries.Length; i++)
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
                    foreach(User aUser in aBook.ReservedBy)
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
            foreach(Book aBook in books)
            {
                if (aBook.Available)
                {
                    foreach(User aUser in users)
                    {
                        // Om boken är reserverad av en användare så läggs boken till i den användarens MyBooks
                        if(aBook.ReservedBy.Count >= 1)
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
        static List<User> Login(List<User> users) // Används för att logga in på en användare
        {
            Console.WriteLine("Skriv in ditt personnummer:");
            string personalNumberInput = Console.ReadLine();
            Console.WriteLine("Skriv in ditt lösenord:");
            string passwordInput = Console.ReadLine();

            bool loginSuccess = false;
            foreach (User aUser in users)
            {
                // Efter man skrivit in personnummer och lösenord kollar den om det stämmer, om det gör det så loggas man in
                if (personalNumberInput == aUser.PersonalNumber && passwordInput == aUser.Password)
                {
                    aUser.CurrentUser = true;
                    loginSuccess = true;
                    Console.WriteLine("Inloggningen lyckades.");
                    Console.WriteLine($"Hej {aUser.FirstName}.");
                }
            }
            if (loginSuccess == false)
            {
                Console.WriteLine("Inloggningen misslyckades.");
            }
            return users;
        }
        static List<User> Logout(List<User> users, User aUser) // Används för att logga ut från en användare
        {
            aUser.CurrentUser = false;
            Console.WriteLine("Utloggningen lyckades.");
            return users;
        }
        static List<User> CreateUser(List<User> users) // Används för att skapa en ny användare
        {
            Console.WriteLine("Skriv in ditt förnamn:");
            string firstNameInput = Console.ReadLine();
            Console.WriteLine("Skriv in ditt efternamn:");
            string lastNameInput = Console.ReadLine();
            Console.WriteLine("Skriv in ditt personnummer:");
            string personalNumberInput = Console.ReadLine();
            Console.WriteLine("Skriv in ett lösenord:");
            string passwordInput = Console.ReadLine();
            if (personalNumberInput.Length == 12)
            {
                // När man skrivit in alla nödvändiga värden kollar den om personnummret är giltig, isåfall skapas den nya användaren
                users.Add(new User() { FirstName = firstNameInput, LastName = lastNameInput, PersonalNumber = personalNumberInput, Password = passwordInput, CurrentUser = false, Librarian = false, MyBooks = new List<Book>() });
                Console.WriteLine("Ditt konto har skapats.");
            }
            else
            {
                Console.WriteLine("Ogiltigt personnummer.");
            }
            return users;
        }
        static List<User> ManageUser(List<User> users, User aUser) // Används för att hantera sin egen användare
        {
            Console.WriteLine($"Förnamn: {aUser.FirstName}");
            Console.WriteLine($"Efternamn: {aUser.LastName}");
            Console.WriteLine($"Personnummer: {aUser.PersonalNumber}");
            Console.WriteLine($"Lösenord: {aUser.Password}");
            Console.WriteLine($"(a) Tillbaka");
            Console.WriteLine($"(b) Byt lösenord");
            Console.WriteLine($"(c) Logga ut");
            string accountInput = Console.ReadLine();
            switch (accountInput.ToLower())
            {
                case "a":
                    break;
                case "b":
                    Console.WriteLine("Skriv in nytt lösenord:");
                    string newPassword = Console.ReadLine();
                    if (newPassword == aUser.Password)
                    {
                        Console.WriteLine("Fel: Samma lösenord.");
                    }
                    else
                    {
                        aUser.Password = newPassword;
                        Console.WriteLine("Lösenordet har nu bytts.");
                    }
                    break;
                case "c":
                    Logout(users, aUser);
                    break;
            }
            return users;
        }
        static List<User> EditUser(List<User> users) // Används för att ändra information för en användare
        {
            Console.WriteLine("Användare:");
            // Sorterar användare efter efternamn
            var sort = from aUser in users
                       orderby aUser.LastName
                       select aUser;
            int userNumber1 = 1;
            foreach (var aUser in sort) // Skriver ut alla användare
            {
                Console.WriteLine($"{userNumber1}. {aUser.LastName}, {aUser.FirstName}");
                userNumber1++;
            }
            try
            {
                Console.WriteLine("Välj en användare:");
                int chosenUser = int.Parse(Console.ReadLine());
                int userNumber2 = 1;
                bool chosenUserSuccess = false;
                foreach (var aUser in sort)
                {
                    if (chosenUser == userNumber2) // Om man lyckas välja en användare skrivs information om den upp
                    {
                        chosenUserSuccess = true;
                        Console.Clear();
                        Console.WriteLine($"(a) Förnamn: {aUser.FirstName}");
                        Console.WriteLine($"(b) Efternamn: {aUser.LastName}");
                        Console.WriteLine($"(c) Personnummer: {aUser.PersonalNumber}");
                        Console.WriteLine($"(d) Lösenord: {aUser.Password}");
                        Console.WriteLine("(e) Ta bort användare");
                        Console.WriteLine("(f) Tillbaka");
                        Console.WriteLine("Välj vad du vill ändra:");
                        string editInput = Console.ReadLine();
                        switch (editInput.ToLower()) // Man ändrar värden för förnamn, efternamn, personnummer och lösenord, man kan också radera användaren
                        {
                            case "a":
                                Console.WriteLine("Skriv in nytt förnamn:");
                                string newFirstName = Console.ReadLine();
                                if (newFirstName == aUser.FirstName)
                                {
                                    Console.WriteLine("Kan inte byta till samma värde.");
                                }
                                else
                                {
                                    Console.WriteLine($"Vill du byta förnamnet från {aUser.FirstName} till {newFirstName}? (y/n)");
                                    string confirmChange = Console.ReadLine();
                                    if (confirmChange.ToLower() == "y")
                                    {
                                        Console.WriteLine($"Du har nu ändrat förnamnet från {aUser.FirstName} till {newFirstName}.");
                                        aUser.FirstName = newFirstName;
                                    }
                                }
                                break;
                            case "b":
                                Console.WriteLine("Skriv in nytt efternamn:");
                                string newLastName = Console.ReadLine();
                                if (newLastName == aUser.LastName)
                                {
                                    Console.WriteLine("Kan inte byta till samma värde.");
                                }
                                else
                                {
                                    Console.WriteLine($"Vill du byta efternamnet från {aUser.LastName} till {newLastName}? (y/n)");
                                    string confirmChange = Console.ReadLine();
                                    if (confirmChange.ToLower() == "y")
                                    {
                                        Console.WriteLine($"Du har nu ändrat förnamnet från {aUser.LastName} till {newLastName}.");
                                        aUser.FirstName = newLastName;
                                    }
                                }
                                break;
                            case "c":
                                Console.WriteLine("Skriv in nytt personnummer:");
                                string newPersonalNumber = Console.ReadLine();
                                bool validPersonalNumber = true;
                                foreach(User checkUser in users)
                                {
                                    if(newPersonalNumber == checkUser.PersonalNumber)
                                    {
                                        Console.WriteLine("Personnumret är inte giltigt.");
                                        validPersonalNumber = false;
                                        break;
                                    }
                                }
                                if (validPersonalNumber)
                                {
                                    Console.WriteLine($"Vill du byta personnumret från {aUser.PersonalNumber} till {newPersonalNumber}? (y/n)");
                                    string confirmChange = Console.ReadLine();
                                    if (confirmChange.ToLower() == "y")
                                    {
                                        Console.WriteLine($"Du har nu ändrat personnumret från {aUser.PersonalNumber} till {newPersonalNumber}.");
                                        aUser.PersonalNumber = newPersonalNumber;
                                    }
                                }
                                break;
                            case "d":
                                Console.WriteLine("Skriv in nytt lösenord:");
                                string newPassword = Console.ReadLine();
                                if (newPassword == aUser.Password)
                                {
                                    Console.WriteLine("Kan inte byta till samma värde.");
                                }
                                else
                                {
                                    Console.WriteLine($"Vill du byta lösenordet från {aUser.Password} till {newPassword}? (y/n)");
                                    string confirmChange = Console.ReadLine();
                                    if (confirmChange.ToLower() == "y")
                                    {
                                        Console.WriteLine($"Du har nu ändrat lösenordet från {aUser.Password} till {newPassword}.");
                                        aUser.Password = newPassword;
                                    }
                                }
                                break;
                            case "e":
                                Console.WriteLine($"Är du säker att du vill ta bort {aUser.FirstName} {aUser.LastName} från biblioteket? (y/n)");
                                string confirmRemoval = Console.ReadLine();
                                if (confirmRemoval.ToLower() == "y")
                                {
                                    Console.WriteLine($"Du har nu tagit bort {aUser.FirstName} {aUser.LastName} från biblioteket.");
                                    users.Remove(aUser);
                                }
                                break;
                            case "f":
                                // Gör inget
                                break;
                        }
                        if (chosenUserSuccess == false)
                        {
                            Console.WriteLine("Användaren du har valt finns inte.");
                        }
                    }
                    userNumber2++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return users;
        }
        static bool LoanBook(List<Book> myBooks, Book chosenBook) // Används för att låna en bok
        {
            bool chosenBookSuccess = true;
            foreach (Book yourBook in myBooks)
            {
                if (chosenBook.ISBN == yourBook.ISBN) // Om boken finns i myBooks så kan man inte låna den igen
                {
                    chosenBookSuccess = false;
                    Console.WriteLine($"Du har redan {chosenBook.Title} ({chosenBook.Year}) av {chosenBook.Author}.");
                    break;
                }
            }
            if (chosenBookSuccess)
            {
                if (chosenBook.Available) // Om boken inte ägs av någon annan så returnas True
                {
                    Console.WriteLine($"Vill låna {chosenBook.Title} ({chosenBook.Year}) av {chosenBook.Author}? (y/n)");
                    string confirmLoan = Console.ReadLine();
                    if (confirmLoan.ToLower() == "y")
                    {
                        return true;
                    }
                }
                else
                {
                    Console.WriteLine("Denna bok är inte tillgänglig.");

                }
            }
            return false;
        }
        static bool ReturnBook(Book chosenBook) // Används för att lämna tillbaka en bok
        {
            // Säkerställer att du vill lämna tillbaka boken, sedan returnas true
            Console.WriteLine($"Vill lämna tillbaka {chosenBook.Title} ({chosenBook.Year}) av {chosenBook.Author}? (y/n)");
            string confirmReturn = Console.ReadLine();
            if (confirmReturn.ToLower() == "y")
            {
                return true;
            }
            return false;
        }
        static bool ReserveBook(List<Book> myBooks, Book chosenBook) // Används för att reservera en bok
        {
            bool chosenBookSuccess = true;
            foreach (Book yourBook in myBooks)
            {
                if (chosenBook.ISBN == yourBook.ISBN) // Om man redan har boken kan man inte reservera
                {
                    chosenBookSuccess = false;
                    Console.WriteLine($"Du har redan {chosenBook.Title} ({chosenBook.Year}) av {chosenBook.Author}.");
                    break;
                }
            }
            if (chosenBookSuccess)
            {
                Console.WriteLine($"Vill du reservera {chosenBook.Title} ({chosenBook.Year}) av {chosenBook.Author}? (y/n)");
                string confirmReservation = Console.ReadLine();
                if (confirmReservation.ToLower() == "y")
                {
                    return true; // Returnar true om man har valt en giltig bok
                }
            }
            return false;
        }
        static List<Book> EditBook(List<Book> books) // Används för att ändra information för en bok
        {
            try
            {
                // Väljer en bok som man sedan ska ändra på
                Console.WriteLine("Välj en bok:");
                Book chosenBook = ChooseBook(books);
                Console.Clear();
                Console.WriteLine($"(a) Titel: {chosenBook.Title}");
                Console.WriteLine($"(b) Författare: {chosenBook.Author}");
                Console.WriteLine($"(c) År: {chosenBook.Year}");
                Console.WriteLine($"(d) Genre: {chosenBook.Genre}");
                Console.WriteLine($"(e) ISBN: {chosenBook.ISBN}");
                Console.WriteLine("(f) Ta bort bok");
                Console.WriteLine("(g) Tillbaka");
                Console.WriteLine("Välj vad du vill ändra:");
                string editInput = Console.ReadLine();
                switch (editInput.ToLower()) // Här kan man välja vad man ska ändra, sedan ändrar man
                {
                    case "a":
                        Console.WriteLine("Skriv in ny titel:");
                        string newTitle = Console.ReadLine();
                        if (newTitle == chosenBook.Title)
                        {
                            Console.WriteLine("Kan inte byta till samma värde.");
                        }
                        else
                        {
                            Console.WriteLine($"Vill dubyta titeln från {chosenBook.Title} till {newTitle}? (y/n)");
                            string confirmChange = Console.ReadLine();
                            if (confirmChange.ToLower() == "y")
                            {
                                Console.WriteLine($"Du har nu ändrat titeln från {chosenBook.Title} till {newTitle}.");
                                chosenBook.Title = newTitle;
                            }
                        }
                        break;
                    case "b":
                        Console.WriteLine("Skriv in ny författare:");
                        string newAuthor = Console.ReadLine();
                        if (newAuthor == chosenBook.Author)
                        {
                            Console.WriteLine("Kan inte byta till samma värde.");
                        }
                        else
                        {
                            Console.WriteLine($"Vill du byta författaren från {chosenBook.Author} till {newAuthor}? (y/n)");
                            string confirmChange = Console.ReadLine();
                            if (confirmChange.ToLower() == "y")
                            {
                                Console.WriteLine($"Du har nu ändrat titeln från {chosenBook.Author} till {newAuthor}.");
                                chosenBook.Author = newAuthor;
                            }
                        }
                        break;
                    case "c":
                        Console.WriteLine("Skriv in nytt år:");
                        int newYear = int.Parse(Console.ReadLine());
                        if (newYear == chosenBook.Year)
                        {
                            Console.WriteLine("Kan inte byta till samma värde.");
                        }
                        else
                        {
                            Console.WriteLine($"Vill du byta året från {chosenBook.Year} till {newYear}? (y/n)");
                            string confirmChange = Console.ReadLine();
                            if (confirmChange.ToLower() == "y")
                            {
                                Console.WriteLine($"Du har nu ändrat titeln från {chosenBook.Year} till {newYear}.");
                                chosenBook.Year = newYear;
                            }
                        }
                        break;
                    case "d":
                        Console.WriteLine("Skriv in ny genre:");
                        string newGenre = Console.ReadLine();
                        if (newGenre == chosenBook.Genre)
                        {
                            Console.WriteLine("Kan inte byta till samma värde.");
                        }
                        else
                        {
                            Console.WriteLine($"Vill du byta genren från {chosenBook.Genre} till {newGenre}? (y/n)");
                            string confirmChange = Console.ReadLine();
                            if (confirmChange.ToLower() == "y")
                            {
                                Console.WriteLine($"Du har nu ändrat titeln från {chosenBook.Genre} till {newGenre}.");
                                chosenBook.Genre = newGenre;
                            }
                        }
                        break;
                    case "e":
                        Console.WriteLine("Skriv in ny ISBN:");
                        string newISBN = Console.ReadLine();
                        bool availableISBN = true;
                        foreach (Book checkBook in books)
                        {
                            if (chosenBook.ISBN == checkBook.ISBN)
                            {
                                Console.WriteLine("Kan inte byta till ett redan existerande ISBN.");
                                availableISBN = false;
                                break;
                            }
                        }
                        if (availableISBN)
                        {
                            Console.WriteLine($"Vill du byta titeln från {chosenBook.ISBN} till {newISBN}? (y/n)");
                            string confirmChange = Console.ReadLine();
                            if (confirmChange.ToLower() == "y")
                            {
                                Console.WriteLine($"Du har nu ändrat titeln från {chosenBook.ISBN} till {newISBN}.");
                                chosenBook.ISBN = newISBN;
                            }
                        }
                        break;
                    case "f":
                        Console.WriteLine($"Är du säker att du vill ta bort {chosenBook.Title} ({chosenBook.Year}) av {chosenBook.Author} från biblioteket? (y/n)");
                        string confirmRemoval = Console.ReadLine();
                        if (confirmRemoval.ToLower() == "y")
                        {
                            Console.WriteLine($"Du har nu tagit bort {chosenBook.Title} ({chosenBook.Year}) av {chosenBook.Author} från biblioteket.");
                            books.Remove(chosenBook);
                        }
                        break;
                    case "g":
                        // Gör inget
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return books;
        }
        static List<Book> CreateBook(List<Book> books) // Används för att skapa en bok
        {
            try
            {
                Console.WriteLine("Skriv in ditt förnamn:");
                string titleInput = Console.ReadLine();
                Console.WriteLine("Skriv in ditt efternamn:");
                string authorInput = Console.ReadLine();
                Console.WriteLine("Skriv in ditt personnummer:");
                int yearInput = int.Parse(Console.ReadLine());
                Console.WriteLine("Skriv in ett lösenord:");
                string genreInput = Console.ReadLine();
                Console.WriteLine("Skriv in ett lösenord:");
                string ISBNInput = Console.ReadLine();
                bool validISBN = true;
                foreach(Book aBook in books)
                {
                    // Om ISBN är ogiltigt så är validISBN false
                    if (ISBNInput.Length != 13 || ISBNInput == aBook.ISBN)
                    {
                        Console.WriteLine("Ogiltigt ISBN.");
                        validISBN = false;
                    }
                }
                if (validISBN) // Giltigt ISBN skapar boken
                {
                    books.Add(new Book() { Title = titleInput, Author = authorInput, Year = yearInput, Genre = genreInput, ISBN = ISBNInput, Available = true, ReservedBy = new List<User>() });
                    Console.WriteLine("Ny bok har skapats.");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
            }
            return books;
        }
        static Book ChooseBook(List<Book> books) // Används för att välja en bok ur en lista
        {
            // Listar böcker och sorterar dem
            ListBooks(books);
            var sort = from aBook in books
                       orderby aBook.Author
                       select aBook;
            try
            {
                // Om man valt bok så returnas boken
                int chosenBook = int.Parse(Console.ReadLine());
                int bookNumber = 1;
                bool chosenBookSuccess = false;
                foreach (var aBook in sort)
                {
                    if (chosenBook == bookNumber)
                    {
                        chosenBookSuccess = true;
                        return aBook;
                    }
                    bookNumber++;
                }
                if (chosenBookSuccess == false)
                {
                    Console.WriteLine("Boken du har valt är inte tillgänglig.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return null;
        }
        static List<Book> SearchBooks(List<Book> books, string input) // Används för att söka på böcker
        {
            // Skapar en lista med de böcker som innehåller en "input"
            List<Book> searchedBooks = new List<Book>();
            foreach(Book aBook in books)
            {
                bool contains = false;
                if (aBook.Title.ToLower().Contains(input.ToLower()))
                {
                    contains = true;
                }
                else if (aBook.Author.ToLower().Contains(input.ToLower()))
                {
                    contains = true;
                }
                else if (aBook.Genre.ToLower().Contains(input.ToLower()))
                {
                    contains = true;
                }
                else if (aBook.ISBN.ToLower().Contains(input.ToLower()))
                {
                    contains = true;
                }

                if (contains)
                {
                    searchedBooks.Add(aBook);
                }
            }
            return searchedBooks;
        }
        static string ViewBook(Book aBook) // Används för att skriva ut information om en bok
        {
            // Returnar en string med information om boken
            Console.Clear();
            if (aBook == null) // Om boken inte finns så returnas en tom string
            {
                return "";
            }
            return $"Titel: {aBook.Title}\n" +
                $"Författare: {aBook.Author}\n" +
                $"År: {aBook.Year}\n" +
                $"Genre: {aBook.Genre}\n" +
                $"ISBN: {aBook.ISBN}\n" +
                $"Tillgänglig: {aBook.Available}";
        }
        static void ListBooks(List<Book> books) // Används för att lista alla böcker i en lista
        {
            // Sorterar och skriver ut böcker i en lista
            var sort = from aBook in books
                       orderby aBook.Author
                       select aBook;
            int bookNumber = 1;
            foreach (var aBook in sort)
            {
                Console.WriteLine($"{bookNumber}. {aBook.Title} ({aBook.Year}) av {aBook.Author}");
                bookNumber++;
            }
        }
    }
}