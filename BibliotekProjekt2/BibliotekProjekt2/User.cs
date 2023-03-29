using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BibliotekProjekt2
{
    class User
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
        public static List<User> Login(List<User> users) // Används för att logga in på en användare
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
        public static List<User> Logout(List<User> users, User aUser) // Används för att logga ut från en användare
        {
            aUser.CurrentUser = false;
            Console.WriteLine("Utloggningen lyckades.");
            return users;
        }
        public static List<User> CreateUser(List<User> users) // Används för att skapa en ny användare
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
        public static List<User> ManageUser(List<User> users, User aUser) // Används för att hantera sin egen användare
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
        public static List<User> EditUser(List<User> users) // Används för att ändra information för en användare
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
                                foreach (User checkUser in users)
                                {
                                    if (newPersonalNumber == checkUser.PersonalNumber)
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
    }
}