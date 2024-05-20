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
                new Role("Super user")
                {
                    CanAddNewUser = true,
                    CanAddUserToProject = true,
                    CanRemoveUserFromProject = true,
                    CanCreateProject = true,
                    CanDeleteProject = true,
                    CanEditProject = true,
                    CanViewProject = true,
                    CanAddTaskToUser = true,
                    CanCreateTask = true,
                    CanRemoveTask = true,
                    CanEditTask = true
                },
                new Role("Project owner")
                {
                    CanAddUserToProject = true,
                    CanRemoveUserFromProject = true,
                    CanCreateProject = true,
                    CanDeleteProject = true,
                    CanEditProject = true,
                    CanViewProject = true,
                    CanAddTaskToUser = true,
                    CanCreateTask = true,
                    CanRemoveTask = true,
                    CanEditTask = true
                },
                new Role("Project manager") 
                {
                    CanAddUserToProject = true,
                    CanRemoveUserFromProject = true,
                    CanEditProject = true,
                    CanViewProject = true,
                    CanAddTaskToUser = true,
                    CanCreateTask = true,
                    CanRemoveTask = true,
                    CanEditTask = true
                },
                new Role("Employee")
                {
                    CanEditProject = true,
                    CanViewProject = true,
                    CanEditTask = true
                },
                new Role("Viewer")
                {
                    CanViewProject = true
                }
            );
            
            context.SaveChanges();

            context.Projects.AddRange(
                new Project ("Data Analytics Dashboard for Small Businesses", "This project involves developing a comprehensive data analytics dashboard tailored for small businesses.", DateTime.Today.AddDays(3)),
                new Project ("Mobile App Development for Fitness Tracker", "This project focuses on developing a mobile application for tracking fitness activities.", DateTime.Today.AddDays(5)),
                new Project ("Marketing Campaign for New Product Launch", "In this project, a comprehensive marketing campaign will be strategized and executed to promote the launch of a new product.", DateTime.Today.AddDays(4)),
                new Project ("Infrastructure Upgrade for Local Library", "This project entails upgrading the infrastructure of a local library to modernize its facilities and improve operational efficiency.", DateTime.Today.AddDays(2)),
                new Project ("E-commerce Platform Development for Fashion Boutique", "This project aims to develop a bespoke e-commerce platform tailored for a fashion boutique's online presence.", DateTime.Today.AddDays(4))
            );
            
            context.SaveChanges();

            context.Users.AddRange(

             new Models.User("petar.simic@gmail.com", "password1", "Petar", "Simic", 1, "1.jpg"),
             new Models.User("aleksa.ilic@gmail.com", "password2", "Aleksa", "Ilic", null, "2.jpg"),
             new Models.User("zoran.gajic@gmail.com", "password3", "Zoran", "Gajic", null, "3.jpg"),
             new Models.User("lazar.milojevic@gmail.com", "password3", "Lazar", "Milojevic", 3, "4.jpg"),
             new Models.User("ana.dacic@gmail.com", "password3", "Ana", "Dacic", 2, "5.jpg"),
             new Models.User("mina.markovic@gmail.com", "password3", "Mina", "Markovic", 4, "6.jpg")
             );
            
            context.SaveChanges();

            context.Categories.AddRange(
                new Category("Development", 1),
                new Category("Marketing", 2),
                new Category("Design", 3),
                new Category("Finance", 5),
                new Category("Research", 4)
                );
            
            context.SaveChanges();


            context.Statuses.AddRange(
                new Status("New", 1, 1),
                new Status("In Progress", 1, 2),
                new Status("Done", 1, 3),
                new Status("New", 2, 1),
                new Status("In Progress", 2, 2),
                new Status("Done", 2, 3),
                new Status("New", 3, 1),
                new Status("In Progress", 3, 2),
                new Status("Done", 3, 3),
                new Status("New", 4, 1),
                new Status("In Progress", 4, 2),
                new Status("Done", 4, 3),
                new Status("New", 5, 1),
                new Status("In Progress", 5, 2),
                new Status("Done", 5, 3)
            );
            
            context.SaveChanges();

            context.Priorities.AddRange(
                new Priority("High", 3),
                new Priority("Medium", 5),
                new Priority("Low", 6)
            );
            
            context.SaveChanges();

            context.Tasks.AddRange(
                new Models.Task("Market Research", "Conduct market research to analyze competitors' platforms", DateTime.Today.AddDays(1), DateTime.Today, 1, 1, 2, 1, 1, 1),
                new Models.Task("Design Wireframes", "Create wireframes and mockups for the user interface ", DateTime.Today.AddDays(2), DateTime.Today, 2, 2, 2, 1, 1, 2),
                new Models.Task("Develop Frontend", "Develop frontend components using HTML, CSS, and JavaScript", DateTime.Today.AddDays(6), DateTime.Today, 3, 3, 1, 1, 1, 1),
                new Models.Task("Implement Backend Functionality", "Develop backend functionality to support user authentication", DateTime.Today.AddDays(5), DateTime.Today, 2, 1, 3, 1, 1, 2),
                new Models.Task("Optimize SEO", "Implement search engine optimization (SEO) best practices to improve visibility", DateTime.Today.AddDays(4), DateTime.Today, 4, 2, 3, 1, 1, 1),
                new Models.Task("Execute marketing campaign", "Execute the marketing campaign across chosen channels", DateTime.Today.AddDays(1), DateTime.Today, 1, 3, 2, 1, 2, 2)
                );

            context.UserProjects.AddRange(
                new UserProject { UserId = 1, ProjectId = 1, RoleId = 1 },
                new UserProject { UserId = 2, ProjectId = 1, RoleId = 2 },
                new UserProject { UserId = 3, ProjectId = 2, RoleId = 3 },
                new UserProject { UserId = 4, ProjectId = 2, RoleId = 4 },
                new UserProject { UserId = 5, ProjectId = 3, RoleId = 2 },
                new UserProject { UserId = 6, ProjectId = 3, RoleId = 5 }
            );

            context.TypesOfTaskDependency.AddRange(
                new TypeOfTaskDependency("start to start"),
                new TypeOfTaskDependency("start to end"),
                new TypeOfTaskDependency("end to start"),
                new TypeOfTaskDependency("end to end")
                );


            context.SaveChanges();

            Console.WriteLine("Sample data added successfully.");
        }

        public static void addData2(AppDatabaseContext context)
        {

            // Add sample roles
            context.Roles.AddRange(
                 new Role("Super user")
                 {
                     CanAddNewUser = true,
                     CanAddUserToProject = true,
                     CanRemoveUserFromProject = true,
                     CanCreateProject = true,
                     CanDeleteProject = true,
                     CanEditProject = true,
                     CanViewProject = true,
                     CanAddTaskToUser = true,
                     CanCreateTask = true,
                     CanRemoveTask = true,
                     CanEditTask = true
                 },
                new Role("Project owner")
                {
                    CanAddUserToProject = true,
                    CanRemoveUserFromProject = true,
                    CanCreateProject = true,
                    CanDeleteProject = true,
                    CanEditProject = true,
                    CanViewProject = true,
                    CanAddTaskToUser = true,
                    CanCreateTask = true,
                    CanRemoveTask = true,
                    CanEditTask = true
                },
                new Role("Project manager")
                {
                    CanAddUserToProject = true,
                    CanRemoveUserFromProject = true,
                    CanEditProject = true,
                    CanViewProject = true,
                    CanAddTaskToUser = true,
                    CanCreateTask = true,
                    CanRemoveTask = true,
                    CanEditTask = true
                },
                new Role("Employee")
                {
                    CanEditProject = true,
                    CanViewProject = true,
                    CanEditTask = true
                },
                new Role("Viewer")
                {
                    CanViewProject = true
                }
            );

            context.SaveChanges();

            context.Projects.AddRange(
                new Project("Data Analytics Dashboard for Small Businesses", "This project involves developing a comprehensive data analytics dashboard tailored for small businesses.", DateTime.Today.AddDays(5)),
                new Project("Mobile App Development for Fitness Tracker", "This project focuses on developing a mobile application for tracking fitness activities.", DateTime.Today.AddDays(6)),
                new Project("Marketing Campaign for New Product Launch", "In this project, a comprehensive marketing campaign will be strategized and executed to promote the launch of a new product.", DateTime.Today.AddDays(4)),
                new Project("Infrastructure Upgrade for Local Library", "This project entails upgrading the infrastructure of a local library to modernize its facilities and improve operational efficiency.", DateTime.Today.AddDays(3)),
                new Project("E-commerce Platform Development for Fashion Boutique", "This project aims to develop a bespoke e-commerce platform tailored for a fashion boutique's online presence.", DateTime.Today.AddDays(4)),
                new Project("Sustainability Initiative Implementation for a Corporate Office", "This project involves implementing various sustainability measures within a corporate office environment", DateTime.Today.AddDays(5)),
                new Project("Community Outreach Program for a Nonprofit Organization", "This project focuses on designing and executing a community outreach program for a nonprofit organization", DateTime.Today.AddDays(5)),
                new Project("Website Redesign for a Hospitality Business", "This project involves redesigning the website for a hospitality business, such as a hotel or restaurant", DateTime.Today.AddDays(5)),
                new Project("Supply Chain Optimization for a Manufacturing Company", "This project aims to optimize the supply chain processes of a manufacturing company, including inventory management, and supplier relationship management", DateTime.Today.AddDays(5)),
                new Project("Employee Training Program Development for a Tech Startup", "This project involves developing a comprehensive employee training program for a tech startup", DateTime.Today.AddDays(5))
            );

            context.SaveChanges();

            context.Users.AddRange(

             new Models.User("petar.simic@gmail.com", "password1", "Petar", "Simic", 1, "1.jpg"),
             new Models.User("aleksa.ilic@gmail.com", "password2", "Aleksa", "Ilic", 1, "2.jpg"),
             new Models.User("zoran.gajic@gmail.com", "password3", "Zoran", "Gajic", 2, "3.jpg"),
             new Models.User("lazar.milojevic@gmail.com", "password3", "Lazar", "Milojevic", 3, "4.jpg"),
             new Models.User("ana.dacic@gmail.com", "password3", "Ana", "Dacic", 4, "5.jpg"),
             new Models.User("mina.markovic@gmail.com", "password3", "Mina", "Markovic", 5, "6.jpg"),
             new Models.User("nikola.vukic@gmail.com", "password3", "Nikola", "Vukic", 2, "6.jpg"),
             new Models.User("jovan.jovanovic@gmail.com", "password3", "Jovan", "Jovanovic", 3, "6.jpg"),
             new Models.User("milos.markovic@gmail.com", "password3", "Milos", "Markovic", 4, "6.jpg"),
             new Models.User("pavle.arsic@gmail.com", "password3", "Pavle", "Arsic", 3, "6.jpg"),
             new Models.User("filip.radovic@gmail.com", "password3", "Filip", "Radovic", 2, "6.jpg"),
             new Models.User("marija.ilic@gmail.com", "password3", "Marija", "Ilic", 2, "6.jpg"),
             new Models.User("aleksandra.simic@gmail.com", "password3", "Aleksandra", "Simic", 4, "6.jpg"),
             new Models.User("nina.petrovic@gmail.com", "password3", "Nina", "Petrovic", 3, "6.jpg"),
             new Models.User("marko.ivanovic@gmail.com", "password3", "Marko", "Ivanovic", 1, "6.jpg"),
             new Models.User("milica.popovic@gmail.com", "password3", "Milica", "Popovic", 3, "6.jpg"),
             new Models.User("nikola.Stojadinovic@gmail.com", "password3", "Nikola", "Stojadinovic", 4, "6.jpg"),
             new Models.User("jovana.djordjevic@gmail.com", "password3", "Jovana", "Djordjevic", 2, "6.jpg"),
             new Models.User("stefan.nikolic@gmail.com", "password3", "Stefan", "Nikolic", 3, "6.jpg"),
             new Models.User("teodora.stankovic@gmail.com", "password3", "Teodora", "Stankovic", 4, "6.jpg")
             );

            context.SaveChanges();

            context.Categories.AddRange(
                new Category("Development",1),
                new Category("Design",1),
                new Category("Design",2),
                new Category("Marketing", 3),
                new Category("Finance",4),
                new Category("Research",5),
                new Category("Research", 1),
                new Category("Marketing", 1),
                new Category("Finance", 1),
                new Category("Development", 2),
                new Category("Marketing", 2),
                new Category("Design", 3),
                new Category("Development", 3),
                new Category("Research", 4),
                new Category("Development", 4),
                new Category("Development", 5),
                new Category("Design", 6),
                new Category("Marketing", 6),
                new Category("Development", 7),
                new Category("Design", 7),
                new Category("Finance", 8),
                new Category("Research", 8),
                new Category("Design", 9),
                new Category("Development", 9),
                new Category("Marketing", 10),
                new Category("Finance", 10)


            );

            context.SaveChanges();

            context.Statuses.AddRange(
                new Status("New", 1, 1),
                new Status("In Progress", 1, 2),
                new Status("Done", 1, 3),
                new Status("New", 2, 1),
                new Status("In Progress", 2, 2),
                new Status("Done", 2, 3),
                new Status("New", 3, 1),
                new Status("In Progress", 3, 2),
                new Status("Done", 3, 3),
                new Status("New", 4, 1),
                new Status("In Progress", 4, 2),
                new Status("Done", 4, 3),
                new Status("New", 5, 1),
                new Status("In Progress", 5, 2),
                new Status("Done", 5, 3),
                new Status("New", 6, 1),
                new Status("In Progress", 6, 2),
                new Status("Done", 6, 3),
                new Status("New", 7, 1),
                new Status("In Progress", 7, 2),
                new Status("Done", 7, 3),
                new Status("New", 8, 1),
                new Status("In Progress", 8, 2),
                new Status("Done", 8, 3),
                new Status("New", 9, 1),
                new Status("In Progress", 9, 2),
                new Status("Done", 9, 3),
                new Status("New", 10, 1),
                new Status("In Progress", 10, 2),
                new Status("Done", 10, 3)
            );

            context.SaveChanges();

            context.Priorities.AddRange(
                new Priority("High", 9),
                new Priority("Medium", 6),
                new Priority("Low", 3)
            );

            context.SaveChanges();

            context.Tasks.AddRange(
                new Models.Task("Market Research", "Conduct market research to analyze competitors' platforms.", DateTime.Today.AddDays(1), DateTime.Today, 5, 7, 1, 1, 4, 3), 
                new Models.Task("Design Wireframes", "Create wireframes and mockups for the user interface.", DateTime.Today.AddDays(2), DateTime.Today, 2, 4, 2, 1, 3, 2),
                new Models.Task("Develop Frontend", "Develop frontend components using HTML, CSS, and JavaScript.", DateTime.Today.AddDays(6), DateTime.Today, 5, 1, 3, 1, 1, 1),
                new Models.Task("Implement Backend Functionality", "Develop backend functionality to support user authentication.", DateTime.Today.AddDays(5), DateTime.Today, 6, 2, 2, 1, 1, 1),
                new Models.Task("Optimize SEO", "Implement search engine optimization (SEO) best practices to improve visibility.", DateTime.Today.AddDays(4), DateTime.Today, 1, 5, 1, 1, 3, 2),
                new Models.Task("Execute marketing campaign", "Execute the marketing campaign across chosen channels.", DateTime.Today.AddDays(2), DateTime.Today, 1, 8, 2, 1, 5, 3)
            );

            context.UserProjects.AddRange(
                new UserProject { UserId = 1, ProjectId = 1, RoleId = 1 },
                new UserProject { UserId = 5, ProjectId = 1, RoleId = 1 },
                new UserProject { UserId = 4, ProjectId = 1, RoleId = 3 },
                new UserProject { UserId = 2, ProjectId = 1, RoleId = 2 },
                new UserProject { UserId = 3, ProjectId = 1, RoleId = 3 },
                new UserProject { UserId = 4, ProjectId = 2, RoleId = 4 },
                new UserProject { UserId = 5, ProjectId = 2, RoleId = 2 },
                new UserProject { UserId = 1, ProjectId = 2, RoleId = 1 },
                new UserProject { UserId = 7, ProjectId = 2, RoleId = 3 },
                new UserProject { UserId = 8, ProjectId = 2, RoleId = 2 },
                new UserProject { UserId = 9, ProjectId = 3, RoleId = 3 },
                new UserProject { UserId = 7, ProjectId = 3, RoleId = 4 },
                new UserProject { UserId = 2, ProjectId = 3, RoleId = 2 },
                new UserProject { UserId = 6, ProjectId = 3, RoleId = 1 },
                new UserProject { UserId = 10, ProjectId = 3, RoleId = 4 },
                new UserProject { UserId = 3, ProjectId = 4, RoleId = 2 },
                new UserProject { UserId = 6, ProjectId = 4, RoleId = 3 },
                new UserProject { UserId = 1, ProjectId = 4, RoleId = 2 },
                new UserProject { UserId = 2, ProjectId = 4, RoleId = 4 },
                new UserProject { UserId = 15, ProjectId = 4, RoleId = 1 },
                new UserProject { UserId = 11, ProjectId = 5, RoleId = 2 },
                new UserProject { UserId = 12, ProjectId = 5, RoleId = 3 },
                new UserProject { UserId = 4, ProjectId = 5, RoleId = 2 },
                new UserProject { UserId = 15, ProjectId = 5, RoleId = 4 },
                new UserProject { UserId = 18, ProjectId = 5, RoleId = 1 },
                new UserProject { UserId = 11, ProjectId = 6, RoleId = 2 },
                new UserProject { UserId = 8, ProjectId = 6, RoleId = 3 },
                new UserProject { UserId = 13, ProjectId = 6, RoleId = 2 },
                new UserProject { UserId = 14, ProjectId = 6, RoleId = 4 },
                new UserProject { UserId = 4, ProjectId = 6, RoleId = 1 },
                new UserProject { UserId = 17, ProjectId = 7, RoleId = 2 },
                new UserProject { UserId = 16, ProjectId = 7, RoleId = 3 },
                new UserProject { UserId = 5, ProjectId = 7, RoleId = 2 },
                new UserProject { UserId = 7, ProjectId = 7, RoleId = 4 },
                new UserProject { UserId = 9, ProjectId = 7, RoleId = 1 },
                new UserProject { UserId = 17, ProjectId = 8, RoleId = 2 },
                new UserProject { UserId = 15, ProjectId = 8, RoleId = 3 },
                new UserProject { UserId = 9, ProjectId = 8, RoleId = 2 },
                new UserProject { UserId = 3, ProjectId = 8, RoleId = 4 },
                new UserProject { UserId = 13, ProjectId = 8, RoleId = 1 },
                new UserProject { UserId = 18, ProjectId = 9, RoleId = 2 },
                new UserProject { UserId = 16, ProjectId = 9, RoleId = 3 },
                new UserProject { UserId = 12, ProjectId = 9, RoleId = 2 },
                new UserProject { UserId = 7, ProjectId = 9, RoleId = 4 },
                new UserProject { UserId = 10, ProjectId = 9, RoleId = 1 },
                new UserProject { UserId = 10, ProjectId = 10, RoleId = 2 },
                new UserProject { UserId = 1, ProjectId = 10, RoleId = 3 },
                new UserProject { UserId = 7, ProjectId = 10, RoleId = 2 },
                new UserProject { UserId = 13, ProjectId = 10, RoleId = 4 },
                new UserProject { UserId = 16, ProjectId = 10, RoleId = 1 }
            );

            context.TypesOfTaskDependency.AddRange(
                new TypeOfTaskDependency("start to start"),
                new TypeOfTaskDependency("start to end"),
                new TypeOfTaskDependency("end to start"),
                new TypeOfTaskDependency("end to end")
                );

            context.SaveChanges();

            Console.WriteLine("Sample data added successfully.");
        }

    }
}
