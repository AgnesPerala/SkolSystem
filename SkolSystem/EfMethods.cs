using SkolSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkolSystem;

public class EfMethods
{
    public void GetStudents()
    {
        using (var context = new MyDbContext())
        {
            // hämtar lista av table student 
            List<Student> allStudents = context.Student.ToList();

            // hämtar lista av table klass 
            List<Klass> allKlasser = context.Klass.ToList();

            // skriver ut alla studenter med foreach 
            foreach (Student student in allStudents)
            {
                Console.WriteLine($"id: {student.StudentID}, namn: {student.SFörnamn} {student.SEfternamn}, klass: {student.Klass.KlassNamn}");
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            }
        }
    }
    public void GetStudentsFromClass()
    {
        using (var context = new MyDbContext())
        {
            // hämtar lista med alla studenter
            List<Student> allStudents = context.Student.ToList();
            // hämtar lista med alla klasser
            List<Klass> allKlasser = context.Klass.ToList();

            // metod som ber användaren skriva in en klass
            int valdKlassId = GetValdKlassID(allKlasser);

            foreach (Student student in allStudents)
            {
                // kollar om klassID stämmer överens med den användaren valt och skriver då ut de som går i den klassen
                if (student.KlassID == valdKlassId)
                {
                    Console.WriteLine($"id: {student.StudentID}, namn: {student.SFörnamn} {student.SEfternamn}, klass: {student.Klass.KlassNamn}");
                    Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                }
            }
        }
    }

    private int GetValdKlassID(List<Klass> allKlasser)
    {
        Console.WriteLine("de klasser som finns är: ");
        // skriver ut klasserna 
        foreach (Klass klass in allKlasser)
        {
            Console.Write($"{klass.KlassNamn}, ");
        }
        string valdKlass = "";
        bool klassExists = false;
        int valdKlassId = 0;
        while (!klassExists)
        {
            Console.Write("välj en av klasserna: ");
            valdKlass = Console.ReadLine();

            klassExists = allKlasser.Any(k => k.KlassNamn == valdKlass);
            if (klassExists) // if stats om det användaren skriver in stämmer överens med de klasser som finns 
            {

                Klass hittadKlass = allKlasser.FirstOrDefault(k => k.KlassNamn == valdKlass);

                valdKlassId = hittadKlass.KlassID;
            }
        }

        return valdKlassId;
    }

    public void AddNewEmlpoyee()
    {
        // användaren skriver in 
        Console.Write("skriv in förnam på anstälda: ");
        string fornamn = Console.ReadLine();

        Console.Write("skriv in efternamn: ");
        string efternamn = Console.ReadLine();

        Console.Write("vilken roll har den anstälda: ");
        string befattning = Console.ReadLine();

        // skapar objekt av personal 
        Personal personal = new Personal()
        {
            Befattning = befattning,
            PFörnamn = fornamn,
            PEfternamn = efternamn

        };
        try s
        {
            using (var context = new MyDbContext())
            {
                // lägger till den nya anställda 
                context.Personal.Add(personal);
                context.SaveChanges(); // sparar den i data basen 
                Console.WriteLine($"namn: {fornamn} {efternamn}, befattning: {befattning}");
            }
        }
        catch
        {
            Console.WriteLine("misslyckades med att lägga till den anställda");
        }

    }
}
