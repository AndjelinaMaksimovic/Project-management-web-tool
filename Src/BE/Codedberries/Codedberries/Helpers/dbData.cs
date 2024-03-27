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
                new Project ("Project1", "description 1", DateTime.Today.AddDays(3)),
                new Project ("Project2", "description 2", DateTime.Today.AddDays(5)),
                new Project ("Project3", "description 3", DateTime.Today.AddDays(4)),
                new Project ("Project4", "description 4", DateTime.Today.AddDays(2)),
                new Project ("Project5", "description 5", DateTime.Today.AddDays(4))
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
                new Models.Task("Task 1", "Task 1 Description", DateTime.Today.AddDays(1), 1, 1, 2, 1, 1, 1),
                new Models.Task("Task 2", "Task 2 Description", DateTime.Today.AddDays(2), 2, 2, 2, 1, 1, 2),
                new Models.Task("Task 3", "Task 3 Description", DateTime.Today.AddDays(6), 3, 3, 1, 1, 1, 1),
                new Models.Task("Task 4", "Task 4 Description", DateTime.Today.AddDays(5), 2, 1, 3, 1, 1, 2),
                new Models.Task("Task 5", "Task 5 Description", DateTime.Today.AddDays(4), 4, 2, 3, 1, 1, 1),
                new Models.Task("Task 6", "Task 6 Description", DateTime.Today.AddDays(1), 1, 3, 2, 1, 1, 2)
                );


            context.Categories.AddRange(
                new Category("Category 1"),
                new Category("Category 2"),
                new Category("Category 3")
            );

            context.Statuses.AddRange(
                new Status("Status 1"),
                new Status("Status 2"),
                new Status("Status 3")
            );

            context.Priorities.AddRange(
                new Priority("Priority 1",3),
                new Priority("Priority 2", 5),
                new Priority("Priority 3", 6)
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
                new Project("Project1", "description 1", DateTime.Today.AddDays(5)),
                new Project("Project2", "description 2", DateTime.Today.AddDays(6)),
                new Project("Project3", "description 3", DateTime.Today.AddDays(4)),
                new Project("Project4", "description 4", DateTime.Today.AddDays(3)),
                new Project("Project5", "description 5", DateTime.Today.AddDays(4))
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
                new Models.Task("Task 1", "Task 1 Description", DateTime.Today.AddDays(1), 3, 2, 1, 1, 1, 2),
                new Models.Task("Task 2", "Task 2 Description", DateTime.Today.AddDays(2), 2, 3, 1, 1, 1, 1),
                new Models.Task("Task 3", "Task 3 Description", DateTime.Today.AddDays(6), 5, 2, 3, 1, 1, 2),
                new Models.Task("Task 4", "Task 4 Description", DateTime.Today.AddDays(5), 6, 3, 2, 1, 1, 1),
                new Models.Task("Task 5", "Task 5 Description", DateTime.Today.AddDays(4), 1, 1, 1, 1, 1, 2),
                new Models.Task("Task 6", "Task 6 Description", DateTime.Today.AddDays(2), 1, 1, 3, 1, 1, 1)
                );


            context.Categories.AddRange(
                new Category("Category 1"),
                new Category("Category 2"),
                new Category("Category 3")
            );

             context.Statuses.AddRange(
                new Status("Status 1"),
                new Status("Status 2"),
                new Status("Status 3")
            );

             context.Priorities.AddRange(
                new Priority("Priority 1", 3),
                new Priority("Priority 2", 5),
                new Priority("Priority 3", 6)
            );





            context.SaveChanges();

            Console.WriteLine("Sample data added successfully.");
        }

    }
}
