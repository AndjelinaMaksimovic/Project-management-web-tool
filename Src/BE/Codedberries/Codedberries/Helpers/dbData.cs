using System.Runtime.Intrinsics.X86;
using Codedberries.Models;

namespace Codedberries.Helpers
{
    public class dbData
    {
        public static void addData1(AppDatabaseContext context)
        {

            // Add sample roles
            context.Roles.AddRange(
                new Role("Super user"),
                new Role("Project owner"),
                new Role("Project manager"),
                new Role("Employee"),
                new Role("Viewer")
            );

            context.Projects.AddRange(
                new Project ( "Project1",null) ,
                new Project  ( "Project2" ,null),
                new Project ( "Project3",null ),
                new Project ( "Project4",1 )
            );

            context.Permissions.AddRange(
                new Permission ( "Kreiranje projekta" ),
                new Permission ( "Brisanje projekta" ),
                new Permission (  " Kreiranje zadatka" ),
                new Permission ("Brisanje zadatka" ),
                new Permission ("Pregledanje projekta" ),
                new Permission ("Brisanje projekta" ),
                new Permission ( "Dodavanje uloga korisnicima" ),
                new Permission ("Dodavanje zadataka korisnicima" )
            );








            context.SaveChanges();

            Console.WriteLine("Sample data added successfully.");
        }

    }
}
