
using System.Threading.Channels;
using System.Data.SqlClient;
using SkolSystem.Models;
namespace SkolSystem;

internal class Program
{
    static void Main(string[] args)
    {
        SqlMethods sqlMethods = new SqlMethods(); // objekt för att nå alla sql metoder 
        EfMethods efMethods = new EfMethods(); // objekt för att nå alla entity framework metoder 
        bool exit = false; // loopar tills man stänger av programmet
        while (!exit)
        {
            Console.WriteLine("\n------ interaktionsmeny till databasen ------\n");
            Console.WriteLine("1. hämta personal på skolan");
            Console.WriteLine("2. hämta studenter på skolan");
            Console.WriteLine("3. hämta studenter från en viss klass");
            Console.WriteLine("4. hämta betyg från senaste månaden");
            Console.WriteLine("5. hämta statictic i betyg från kurser");
            Console.WriteLine("6. lägg till en ny elev");
            Console.WriteLine("7. lätt till ny personal");
            Console.WriteLine("0. avsluta programmet\n");

            Console.Write("skriv in en siffra för att välja: ");
            string menu = Console.ReadLine();
            Console.WriteLine("");

            switch (menu) // switch case för att klicka sig vidare till medtoderna för att visa infon 
            {
                case "1":
                    sqlMethods.GetPersonal();
                    break;
                case "2":
                    efMethods.GetStudents();
                    break;
                case "3":
                    efMethods.GetStudentsFromClass();
                    break;
                case "4":
                    sqlMethods.GetLatestGrades();
                    break;
                case "5":
                    sqlMethods.GetCourseStatistic();
                    break;
                case "6":
                    sqlMethods.AddNewStudent();
                    break;
                case "7":
                    efMethods.AddNewEmlpoyee();
                    break;
                case "0":
                    Console.WriteLine("avslutar programmet");
                    exit = true;
                    break;
                default:
                    Console.WriteLine("du måste skriva en siffra mellan 0 och 7"); // körs igen om man skriver in felaktigt 
                    break;
            } 
        }
    }
}
