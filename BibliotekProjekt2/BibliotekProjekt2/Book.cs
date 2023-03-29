using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BibliotekProjekt2
{
    class Book
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
        public static bool LoanBook(List<Book> myBooks, Book chosenBook) // Används för att låna en bok
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
        public static bool ReturnBook(Book chosenBook) // Används för att lämna tillbaka en bok
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
        public static bool ReserveBook(List<Book> myBooks, Book chosenBook) // Används för att reservera en bok
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
        public static List<Book> EditBook(List<Book> books) // Används för att ändra information för en bok
        {
            try
            {
                // Väljer en bok som man sedan ska ändra på
                Console.WriteLine("Välj en bok:");
                Book chosenBook = ChooseBook(books);
                if (chosenBook == null) { return books; }
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
        public static List<Book> CreateBook(List<Book> books) // Används för att skapa en bok
        {
            try
            {
                Console.WriteLine("Skriv in titel:");
                string titleInput = Console.ReadLine();
                Console.WriteLine("Skriv in författare:");
                string authorInput = Console.ReadLine();
                Console.WriteLine("Skriv in året:"); // HÄR
                int yearInput = int.Parse(Console.ReadLine());
                Console.WriteLine("Skriv in ett lösenord:");
                string genreInput = Console.ReadLine();
                Console.WriteLine("Skriv in ett lösenord:");
                string ISBNInput = Console.ReadLine();
                bool validISBN = true;
                foreach (Book aBook in books)
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
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
            }
            return books;
        }
        public static Book ChooseBook(List<Book> books) // Används för att välja en bok ur en lista
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
        public static List<Book> SearchBooks(List<Book> books, string input) // Används för att söka på böcker
        {
            // Skapar en lista med de böcker som innehåller en "input"
            List<Book> searchedBooks = new List<Book>();
            foreach (Book aBook in books)
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
        public static string ViewBook(Book aBook) // Används för att skriva ut information om en bok
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
        public static void ListBooks(List<Book> books) // Används för att lista alla böcker i en lista
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