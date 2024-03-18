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
                new Project ( "Project1",null, "description 1") ,
                new Project  ( "Project2" ,null, "description 2"),
                new Project ( "Project3",null, "description 3"),
                new Project ( "Project4",1, "description 4"),
                new Project("Project5", 2, "description 5")
            );

            context.Permissions.AddRange(
                new Permission ("Dodavanje novog korisnika"),
                new Permission ("Dodavanje korisnika na projekat"),
                new Permission ("Uklanjanje korisnika sa projekta"),
                new Permission ( "Kreiranje projekta" ),
                new Permission ( "Brisanje projekta" ),
                new Permission("Azuriranje projekta"), 
                new Permission("Pregledanje projekta"), 
                new Permission (  "Kreiranje zadatka" ),
                new Permission ("Brisanje zadatka" ),
                new Permission("Azuriranje zadatka"), 
                new Permission ( "Dodavanje uloga korisnicima" ),
                new Permission ("Dodavanje zadataka korisnicima" )
            );


            context.Users.AddRange(

             new Models.User("user1@example.com", "password1", "John", "Doe", null),
             new Models.User("user2@example.com", "password2", "Jane", "Doe", null),
             new Models.User("user3@example.com", "password3", "Mark", "Smith", 1),
             new Models.User("user4@example.com", "password3", "Jack", "Smith", 3),
             new Models.User("user5@example.com", "password3", "Sarah", "Smith", 2),
             new Models.User("user6@example.com", "password3", "Marry", "Smith", 4)
             );

            context.Tasks.AddRange(
                new Models.Task("Task 1 Description", DateTime.Today.AddDays(1), 1, "Open", "Low", 1, 1),
                new Models.Task("Task 2 Description", DateTime.Today.AddDays(2), 2, "Open", "Low", 1, 1),
                new Models.Task("Task 3 Description", DateTime.Today.AddDays(6), 3, "Open", "Low", 1, 1),
                new Models.Task("Task 4 Description", DateTime.Today.AddDays(5), 2, "Open", "Low", 1, 1),
                new Models.Task("Task 5 Description", DateTime.Today.AddDays(4), 4, "Open", "Low", 1, 1),
                new Models.Task("Task 6 Description", DateTime.Today.AddDays(1), 1, "Open", "Low", 1, 1)
                );


            context.Categories.AddRange(
                new Category("Category 1"),
                new Category("Category 2"),
                new Category("Category 3")
            );





            context.SaveChanges();

            Console.WriteLine("Sample data added successfully.");
        }

        public static void addData2(AppDatabaseContext context)
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
                new Project("Project1", null, "description 1"),
                new Project("Project2", null, "description 2"),
                new Project("Project3", 1, "description 3"),
                new Project("Project4", 3, "description 4"),
                new Project("Project5", 4, "description 5")
            );

            context.Permissions.AddRange(
                new Permission("Dodavanje novog korisnika"),
                new Permission("Dodavanje korisnika na projekat"),
                new Permission("Uklanjanje korisnika sa projekta"),
                new Permission("Kreiranje projekta"),
                new Permission("Brisanje projekta"),
                new Permission("Azuriranje projekta"),
                new Permission("Pregledanje projekta"),
                new Permission("Kreiranje zadatka"),
                new Permission("Brisanje zadatka"),
                new Permission("Azuriranje zadatka"),
                new Permission("Dodavanje uloga korisnicima"),
                new Permission("Dodavanje zadataka korisnicima")
            );


            context.Users.AddRange(

             new Models.User("user1@example.com", "password1", "John", "Doe", null),
             new Models.User("user2@example.com", "password2", "Jane", "Doe", 1),
             new Models.User("user3@example.com", "password3", "Mark", "Smith", null),
             new Models.User("user4@example.com", "password3", "Jack", "Smith", 3),
             new Models.User("user5@example.com", "password3", "Sarah", "Smith", null),
             new Models.User("user6@example.com", "password3", "Marry", "Smith", 5)
             );

            context.Tasks.AddRange(
                new Models.Task("Task 1 Description", DateTime.Today.AddDays(1), 3, "Open", "Low", 1, 1),
                new Models.Task("Task 2 Description", DateTime.Today.AddDays(2), 2, "Open", "Low", 1, 1),
                new Models.Task("Task 3 Description", DateTime.Today.AddDays(6), 5, "Open", "Low", 1, 1),
                new Models.Task("Task 4 Description", DateTime.Today.AddDays(5), 6, "Open", "Low", 1, 1),
                new Models.Task("Task 5 Description", DateTime.Today.AddDays(4), 1, "Open", "Low", 1, 1),
                new Models.Task("Task 6 Description", DateTime.Today.AddDays(2), 1, "Open", "Low", 1, 1)
                );


            context.Categories.AddRange(
                new Category("Category 1"),
                new Category("Category 2"),
                new Category("Category 3")
            );





            context.SaveChanges();

            Console.WriteLine("Sample data added successfully.");
        }

    }
}
