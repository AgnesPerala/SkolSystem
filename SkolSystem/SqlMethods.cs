using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data.Common;
using SkolSystem.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore;

namespace SkolSystem;

public class SqlMethods
{
    private string connectionString;
    public SqlMethods()
    {
        // connection string till min lokala databas
        connectionString = "Data Source=DESKTOP-UKFB1CL\\MSSQLSERVER01;Initial Catalog=SkolSystem;Integrated Security=True;TrustServerCertificate=True;";
    }

    public void GetPersonal()
    {
        string query = "SELECT * FROM Personal"; // query kod som hämtar all personal 

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read()) 
                    {
                        // skriver ut den hämtade informationen
                        string text = $"ID: {reader["PersonalID"]}, yrke: {reader["Befattning"]}, Namn: {reader["PFörnamn"]} {reader["PEfternamn"]}";
                        Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                        Console.WriteLine(text);
                    }
                }
            }

            connection.Close();
        }
    }
    public void GetLatestGrades()
    {
        // query som hämtar betyg från senaste månaden
        string query = "SELECT Kurs.KursNamn, Betyg.Datum, student.SFörnamn, Student.SEfternamn , Betyg.SlutBetyg, Personal.PFörnamn, Personal.PEfternamn FROM (((Betyg INNER JOIN Personal on Betyg.PersonalID=Personal.PersonalID) INNER JOIN Student on Betyg.StudentID=Student.StudentID) INNER JOIN Kurs on betyg.KursID=Kurs.KursID) WHERE Datum >= DATEADD(MONTH, -1, GETDATE());";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string text = $"{reader["SFörnamn"]} {reader["SEfternamn"]} fick betyget {reader["SlutBetyg"]} i kursen {reader["KursNamn"]} av läraren: {reader["PFörnamn"]} {reader["PEfternamn"]} den {((DateTime)reader["Datum"]).ToString("yy-MM-dd")}";
                        Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                        Console.WriteLine(text);
                    }
                }
            }

            connection.Close();
        }
    }
    public void GetCourseStatistic()
    {
        // query som visar kurs statistic 
        string query = "SELECT Kurs.KursNamn, AVG(Betyg.SlutBetyg) AS Snittbetyg, MAX(Betyg.SlutBetyg) AS HogstaBetyg, MIN(Betyg.SlutBetyg) AS LagstaBetyg FROM Kurs JOIN Betyg ON Kurs.KursID = Betyg.KursID GROUP BY Kurs.KursNamn;";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(query, connection)) // läser av queryn 
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string text = $"kurs: {reader["KursNamn"]}, snittbetyg: {reader["Snittbetyg"]}, högsta betyg: {reader["HogstaBetyg"]}, lägsta betyg: {reader["LagstaBetyg"]}\t";
                        Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                        Console.WriteLine(text);
                    }
                }
            }

            connection.Close();
        }
    }
    public void AddNewStudent()
    {
        // query som hämtar alla table från klassen
        string query = "SELECT * FROM Klass;";
        List<string> allKlassNamn = new List<string>();
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    Console.WriteLine("De klasser som finns är: ");
                    while (reader.Read())
                    {
                        // skriver ut vilka klasser som finns i klass tablet 
                        Console.Write($"{reader["KlassNamn"]}, ");
                        allKlassNamn.Add(reader["KlassNamn"].ToString()); // och sparar det i en lista 
                    }
                }
            }

            connection.Close();
        }

        bool klassExists = false;
        string valdKlass = "";
        while (!klassExists) // loop för att programmet inte ska kracha om användaer skriver in fel
        {
            Console.Write("välj id på klassen: ");
            valdKlass = Console.ReadLine();

            // kollar om användarens svar stämmer överens om de som finns i listan
            if (allKlassNamn.Contains(valdKlass))
            {
                klassExists = true;
            }
        }

        int klassId = GetKlassIdFromKlassNamn(valdKlass);

        // användaren skriver in namn på nya eleven
        Console.Write("skriv in förnam på eleven: ");
        string fornamn = Console.ReadLine();

        Console.Write("skriv in efternamn: ");
        string efternamn = Console.ReadLine();

        // query som lägger till den nya studenten
        query = $"INSERT INTO Student(SFörnamn, SEfternamn, KlassID) VALUES ('{fornamn}','{efternamn}',{klassId});";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0) // kollar om den läggs till annrs skrivs felmedelande ut 
                {
                    Console.WriteLine("Eleven är nu tillagd");
                    Console.WriteLine($" namn: {fornamn} {efternamn}, klass: {valdKlass}");
                }
                else
                {
                    Console.WriteLine("Något gick fel. Eleven kunde inte läggas till.");
                }
            }

            connection.Close();
        }
    }

    private int GetKlassIdFromKlassNamn(string valdKlass)
    {
        // query som hämtar klassnamn med klass id 
        string query = $"SELECT Klass.KlassID FROM Klass WHERE KlassNamn = '{valdKlass}';";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // returnerar klassid 
                        return int.Parse(reader["KlassID"].ToString());
                    }
                }
            }
            connection.Close();
        }
        throw new Exception("klassen fanns inte som du valt");
    }
}
