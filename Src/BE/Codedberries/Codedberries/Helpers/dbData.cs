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
                new Project ("Data Analytics Dashboard for Small Businesses", "This project involves developing a comprehensive data analytics dashboard tailored for small businesses.", DateTime.Today.AddDays(3)),
                new Project ("Mobile App Development for Fitness Tracker", "This project focuses on developing a mobile application for tracking fitness activities.", DateTime.Today.AddDays(5)),
                new Project ("Marketing Campaign for New Product Launch", "In this project, a comprehensive marketing campaign will be strategized and executed to promote the launch of a new product.", DateTime.Today.AddDays(4)),
                new Project ("Infrastructure Upgrade for Local Library", "This project entails upgrading the infrastructure of a local library to modernize its facilities and improve operational efficiency.", DateTime.Today.AddDays(2)),
                new Project ("E-commerce Platform Development for Fashion Boutique", "This project aims to develop a bespoke e-commerce platform tailored for a fashion boutique's online presence.", DateTime.Today.AddDays(4))
            );

            context.Users.AddRange(

             new Models.User("petar.simic@gmail.com", "password1", "Petar", "Simic", 1),
             new Models.User("aleksa.ilic@gmail.com", "password2", "Aleksa", "Ilic", null),
             new Models.User("zoran.gajic@gmail.com", "password3", "Zoran", "Gajic", null),
             new Models.User("lazar.milojevic@gmail.com", "password3", "Lazar", "Milojevic", 3),
             new Models.User("ana.dacic@gmail.com", "password3", "Ana", "Dacic", 2),
             new Models.User("mina.markovic@gmail.com", "password3", "Mina", "Markovic", 4)
             );

            context.Tasks.AddRange(
                new Models.Task("Market Research", "Conduct market research to analyze competitors' platforms", DateTime.Today.AddDays(1), DateTime.Today, 1, 1, 2, 1, 1, 1),
                new Models.Task("Design Wireframes", "Create wireframes and mockups for the user interface ", DateTime.Today.AddDays(2), DateTime.Today, 2, 2, 2, 1, 1, 2),
                new Models.Task("Develop Frontend", "Develop frontend components using HTML, CSS, and JavaScript", DateTime.Today.AddDays(6), DateTime.Today, 3, 3, 1, 1, 1, 1),
                new Models.Task("Implement Backend Functionality", "Develop backend functionality to support user authentication", DateTime.Today.AddDays(5), DateTime.Today, 2, 1, 3, 1, 1, 2),
                new Models.Task("Optimize SEO", "Implement search engine optimization (SEO) best practices to improve visibility", DateTime.Today.AddDays(4), DateTime.Today, 4, 2, 3, 1, 1, 1),
                new Models.Task("Execute marketing campaign", "Execute the marketing campaign across chosen channels", DateTime.Today.AddDays(1), DateTime.Today, 1, 3, 2, 1, 1, 2)
                );


            context.Categories.AddRange(
                new Category("Development", 1),
                new Category("Marketing", 2),
                new Category("Design", 3),
                new Category("Finance", 5),
                new Category("Research", 4)
                );

            context.Statuses.AddRange(
                new Status("New", 1),
                new Status("In Progress", 1),
                new Status("Done", 1),
                new Status("New", 2),
                new Status("In Progress", 2),
                new Status("Done", 2),
                new Status("New", 3),
                new Status("In Progress", 3),
                new Status("Done", 3),
                new Status("New", 4),
                new Status("In Progress", 4),
                new Status("Done", 4),
                new Status("New", 5),
                new Status("In Progress", 5),
                new Status("Done", 5)
            );

            context.Priorities.AddRange(
                new Priority("High Priority", 3),
                new Priority("Medium Priority", 5),
                new Priority("Low Priority", 6)
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

            context.SaveChanges();

            context.Projects.AddRange(
                new Project("Data Analytics Dashboard for Small Businesses", "This project involves developing a comprehensive data analytics dashboard tailored for small businesses.", DateTime.Today.AddDays(5)),
                new Project("Mobile App Development for Fitness Tracker", "This project focuses on developing a mobile application for tracking fitness activities.", DateTime.Today.AddDays(6)),
                new Project("Marketing Campaign for New Product Launch", "In this project, a comprehensive marketing campaign will be strategized and executed to promote the launch of a new product.", DateTime.Today.AddDays(4)),
                new Project("Infrastructure Upgrade for Local Library", "This project entails upgrading the infrastructure of a local library to modernize its facilities and improve operational efficiency.", DateTime.Today.AddDays(3)),
                new Project("E-commerce Platform Development for Fashion Boutique", "This project aims to develop a bespoke e-commerce platform tailored for a fashion boutique's online presence.", DateTime.Today.AddDays(4))
            );

            context.SaveChanges();

            context.Users.AddRange(

             new Models.User("petar.simic@gmail.com", "password1", "Petar", "Simic", null),
             new Models.User("aleksa.ilic@gmail.com", "password2", "Aleksa", "Ilic", 1),
             new Models.User("zoran.gajic@gmail.com", "password3", "Zoran", "Gajic", null),
             new Models.User("lazar.milojevic@gmail.com", "password3", "Lazar", "Milojevic", 3),
             new Models.User("ana.dacic@gmail.com", "password3", "Ana", "Dacic", null),
             new Models.User("mina.markovic@gmail.com", "password3", "Mina", "Markovic", 5)
             );

            context.SaveChanges();

            context.Categories.AddRange(
                new Category("Development",1),
                new Category("Marketing",2),
                new Category("Design",3),
                new Category("Finance",4),
                new Category("Research",5)
            );

            context.SaveChanges();

            context.Statuses.AddRange(
                new Status("New", 1),
                new Status("In Progress", 1),
                new Status("Done", 1),
                new Status("New", 2),
                new Status("In Progress", 2),
                new Status("Done", 2),
                new Status("New", 3),
                new Status("In Progress", 3),
                new Status("Done", 3),
                new Status("New", 4),
                new Status("In Progress", 4),
                new Status("Done", 4),
                new Status("New", 5),
                new Status("In Progress", 5),
                new Status("Done", 5)
            );

            context.SaveChanges();

            context.Priorities.AddRange(
                new Priority("High Priority", 9),
                new Priority("Medium Priority", 6),
                new Priority("Low Priority", 3)
            );

            context.SaveChanges();

            context.Tasks.AddRange(
                new Models.Task("Market Research", "Conduct market research to analyze competitors' platforms", DateTime.Today.AddDays(1), DateTime.Today, 3, 4, 1, 1, 2, 2),
                new Models.Task("Design Wireframes", "Create wireframes and mockups for the user interface ", DateTime.Today.AddDays(2), DateTime.Today, 2, 2, 2, 1, 1, 1),
                new Models.Task("Develop Frontend", "Develop frontend components using HTML, CSS, and JavaScript", DateTime.Today.AddDays(6), DateTime.Today, 5, 4, 3, 1, 2, 2),
                new Models.Task("Implement Backend Functionality", "Develop backend functionality to support user authentication", DateTime.Today.AddDays(5), DateTime.Today, 6, 2, 2, 1, 1, 1),
                new Models.Task("Optimize SEO", "Implement search engine optimization (SEO) best practices to improve visibility", DateTime.Today.AddDays(4), DateTime.Today, 1, 8, 1, 1, 3, 3),
                new Models.Task("Execute marketing campaign", "Execute the marketing campaign across chosen channels", DateTime.Today.AddDays(2), DateTime.Today, 1, 2, 2, 1, 1, 1)
            );

            context.SaveChanges();

            Console.WriteLine("Sample data added successfully.");
        }

    }
}
