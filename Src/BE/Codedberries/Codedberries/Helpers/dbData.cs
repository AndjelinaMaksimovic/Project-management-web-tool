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
                new Category("Development",1),//1
                new Category("Design",1),//2
                new Category("Design",2),//3
                new Category("Marketing", 3),//4
                new Category("Finance",4),//5
                new Category("Research",5),//6
                new Category("Research", 1),//7
                new Category("Marketing", 1),//8
                new Category("Finance", 1),//9
                new Category("Development", 2),//10
                new Category("Marketing", 2),//11
                new Category("Design", 3),//12
                new Category("Development", 3),//13
                new Category("Research", 4),//14
                new Category("Development", 4),//15
                new Category("Development", 5),//16
                new Category("Design", 6),//17
                new Category("Marketing", 6),//18
                new Category("Development", 7),//19
                new Category("Design", 7),//20
                new Category("Finance", 8),//21
                new Category("Research", 8),//22
                new Category("Design", 9),//23
                new Category("Development", 9),//24
                new Category("Marketing", 10),//25
                new Category("Finance", 10) //26


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
                new Models.Task("Research popular data visualization tools", "Research popular data visualization tools suitable for small businesses.", DateTime.Today.AddDays(10), DateTime.Today, 1, 1, 1, 2, 1),
                new Models.Task("Develop a list of key performance indicators", "Develop a list of key performance indicators relevant to small businesses for inclusion in the dashboard.", DateTime.Today.AddDays(11), DateTime.Today.AddDays(2),  2, 1, 1, 1, 1), //dependency1
                new Models.Task("Design wireframes", "Design wireframes for the dashboard interface", DateTime.Today.AddDays(8), DateTime.Today,  2, 1, 1, 8, 1),
                new Models.Task("Create a database", "Create a database schema to efficiently store data", DateTime.Today.AddDays(6), DateTime.Today,  2, 1, 1, 8, 1),
                new Models.Task("Implement data extraction scripts", "Implement data extraction scripts to gather relevant data", DateTime.Today.AddDays(16), DateTime.Today.AddDays(3),  3, 2, 5, 2, 1),
                new Models.Task("Develop algorithms", "Develop algorithms for data processing and aggregation", DateTime.Today.AddDays(5), DateTime.Today,  2, 2, 5, 1, 1),
                new Models.Task("Design data visualisation components", "Design and implement data visualization components using selected tools", DateTime.Today.AddDays(5), DateTime.Today,  3, 2, 6, 2, 1),
                new Models.Task("Conduct usability testing", "Conduct usability testing with small business owners", DateTime.Today.AddDays(9), DateTime.Today,  3, 3, 5, 2, 1),
                new Models.Task("Optimize dashboard", "Optimize dashboard performance and scalability to ensure smooth operation", DateTime.Today.AddDays(7), DateTime.Today,  3, 3, 3, 8, 1),
                new Models.Task("Document installation instructions", "Add document installation instructions and user guides", DateTime.Today.AddDays(21), DateTime.Today.AddDays(6),  3, 3, 3, 8, 1),
               
               new Models.Task("Define the target audience", "Define the target audience and conduct market research", DateTime.Today.AddDays(8), DateTime.Today,  4, 3, 3, 3, 2),
               new Models.Task("Develop user personas", "Create user personas representing different types of users", DateTime.Today.AddDays(10), DateTime.Today,  4, 2, 3, 10, 2),
               new Models.Task("Design wireframes and mockups", "Create wireframes and mockups for the fitness tracker app", DateTime.Today.AddDays(12), DateTime.Today,  5, 1, 2, 11, 2),
               new Models.Task("Choose development technologies", "Select appropriate technologies for mobile app development", DateTime.Today.AddDays(14), DateTime.Today.AddDays(3),  6, 2, 2, 3, 2),
               new Models.Task("Implement user authentication", "Develop user authentication and authorization features", DateTime.Today.AddDays(16), DateTime.Today.AddDays(5),  4, 1, 3, 3, 2),
               new Models.Task("Integrate with wearable devices", "Integrate app with wearable devices for real-time data collection", DateTime.Today.AddDays(18), DateTime.Today,  5, 2, 3, 10, 2),
               new Models.Task("Develop activity tracking algorithms", "Design and implement algorithms for activity tracking", DateTime.Today.AddDays(20), DateTime.Today.AddDays(3),  6, 2, 2, 3, 2),
               new Models.Task("Design personalized workout plans", "Create personalized workout plans based on user profiles", DateTime.Today.AddDays(22), DateTime.Today,  6, 3, 2, 11, 2),
               new Models.Task("Implement social sharing features", "Add social sharing capabilities to the app", DateTime.Today.AddDays(24), DateTime.Today.AddDays(9),  5, 1, 3, 10, 2),
               new Models.Task("Conduct thorough testing", "Perform functional, usability, and performance testing", DateTime.Today.AddDays(26), DateTime.Today.AddDays(11),  6, 1, 3, 11, 2),

               new Models.Task("Conduct market research", "Gather data and insights on target market and competitors", DateTime.Today.AddDays(8), DateTime.Today,  7, 3, 2, 4, 3),
               new Models.Task("Develop marketing personas", "Create detailed personas representing target customers", DateTime.Today.AddDays(10), DateTime.Today,  9, 2, 2, 13, 3),
               new Models.Task("Design marketing materials", "Create visually appealing materials for print and digital channels", DateTime.Today.AddDays(12), DateTime.Today.AddDays(3),  8, 1, 3, 12, 3),
               new Models.Task("Define marketing channels", "Identify and prioritize marketing channels for reaching target audience", DateTime.Today.AddDays(14), DateTime.Today,  9, 2, 2, 12, 3),
               new Models.Task("Plan social media strategy", "Develop a comprehensive strategy for engaging with customers on social media platforms", DateTime.Today.AddDays(16), DateTime.Today.AddDays(5),  7, 3, 3, 4, 3),
               new Models.Task("Create content calendar", "Outline a schedule for publishing content across various marketing channels", DateTime.Today.AddDays(18), DateTime.Today.AddDays(6),  8, 1, 2, 13, 3),
               new Models.Task("Launch email marketing campaign", "Execute targeted email campaigns to generate interest and drive sales", DateTime.Today.AddDays(20), DateTime.Today,  9, 1, 3, 4, 3),
               new Models.Task("Coordinate influencer partnerships", "Collaborate with influencers to amplify brand reach and credibility", DateTime.Today.AddDays(22), DateTime.Today.AddDays(10),  7, 2, 2, 12, 3),
               new Models.Task("Monitor campaign performance", "Track key metrics and analyze campaign effectiveness in real-time", DateTime.Today.AddDays(24), DateTime.Today.AddDays(10),  8, 3, 3, 13, 3),
               new Models.Task("Gather customer feedback", "Collect feedback from customers to iterate and optimize marketing strategies", DateTime.Today.AddDays(26), DateTime.Today.AddDays(14),  7, 1, 2, 4, 3),

               new Models.Task("Assess current infrastructure", "Conduct a thorough evaluation of the library's existing facilities and technology", DateTime.Today.AddDays(8), DateTime.Today.AddDays(1),  10, 3, 2, 5, 4),
               new Models.Task("Identify upgrade requirements", "Determine the specific needs and priorities for infrastructure upgrades", DateTime.Today.AddDays(10), DateTime.Today,  11, 2, 3, 14, 4),
               new Models.Task("Plan budget allocation", "Develop a budget plan for funding the infrastructure upgrade project", DateTime.Today.AddDays(12), DateTime.Today,  12, 1, 2, 15, 4),
               new Models.Task("Research modern library technologies", "Explore innovative technologies and solutions for library upgrades", DateTime.Today.AddDays(14), DateTime.Today,  10, 2, 2, 14, 4),
               new Models.Task("Upgrade computer systems", "Replace outdated computers with modern, efficient systems", DateTime.Today.AddDays(16), DateTime.Today,  11, 3, 3, 5, 4),
               new Models.Task("Install high-speed internet", "Upgrade internet infrastructure to provide faster and more reliable connectivity", DateTime.Today.AddDays(18), DateTime.Today.AddDays(5),  12, 2, 2, 15, 4),
               new Models.Task("Enhance digital catalog system", "Improve the library's digital catalog for easier access and navigation", DateTime.Today.AddDays(20), DateTime.Today.AddDays(6),  10, 1, 3, 14, 4),
               new Models.Task("Upgrade lighting and HVAC systems", "Replace outdated lighting and HVAC systems with energy-efficient alternatives", DateTime.Today.AddDays(22), DateTime.Today.AddDays(7),  11, 2, 2, 5, 4),
               new Models.Task("Train staff on new technology", "Provide training sessions to library staff on using and maintaining upgraded technology", DateTime.Today.AddDays(24), DateTime.Today.AddDays(16),  12, 3, 3, 15, 4),
               new Models.Task("Coordinate facility renovations", "Oversee renovations to accommodate new infrastructure upgrades", DateTime.Today.AddDays(26), DateTime.Today.AddDays(17),  10, 1, 2, 5, 4),

               new Models.Task("Define platform requirements", "Gather and document functional and non-functional requirements for the e-commerce platform", DateTime.Today.AddDays(8), DateTime.Today,  13, 3, 2, 6, 5),
               new Models.Task("Research e-commerce platforms", "Evaluate existing e-commerce platforms to determine the best fit for the fashion boutique", DateTime.Today.AddDays(10), DateTime.Today.AddDays(3),  14, 2, 3, 16, 5),
               new Models.Task("Design user interface", "Create mockups and prototypes for the e-commerce platform's user interface", DateTime.Today.AddDays(12), DateTime.Today.AddDays(3),  15, 1, 2, 6, 5),
               new Models.Task("Develop product catalog", "Build a comprehensive catalog of fashion items to be sold on the platform", DateTime.Today.AddDays(14), DateTime.Today,  13, 2, 2, 16, 5),
               new Models.Task("Implement shopping cart functionality", "Enable users to add and manage items in their shopping cart", DateTime.Today.AddDays(16), DateTime.Today.AddDays(8),  14, 3, 3, 6, 5),
               new Models.Task("Integrate payment gateway", "Integrate a secure payment gateway for processing online transactions", DateTime.Today.AddDays(18), DateTime.Today.AddDays(9),  15, 2, 2, 16, 5),
               new Models.Task("Enable user account management", "Implement user registration, login, and profile management features", DateTime.Today.AddDays(20), DateTime.Today,  13, 1, 3, 6, 5),
               new Models.Task("Optimize for mobile devices", "Ensure the e-commerce platform is fully responsive and optimized for mobile use", DateTime.Today.AddDays(22), DateTime.Today.AddDays(10),  14, 2, 2, 6, 5),
               new Models.Task("Set up order fulfillment process", "Establish procedures for processing and shipping customer orders efficiently", DateTime.Today.AddDays(24), DateTime.Today.AddDays(11),  15, 3, 3, 16, 5),
               new Models.Task("Test and deploy", "Conduct thorough testing and deploy the e-commerce platform to production environment", DateTime.Today.AddDays(26), DateTime.Today.AddDays(12),  13, 1, 2, 6, 5),

               new Models.Task("Conduct sustainability audit", "Assess current practices and identify areas for improvement in sustainability efforts", DateTime.Today.AddDays(8), DateTime.Today,  16, 3, 2, 17, 6),
               new Models.Task("Formulate sustainability goals", "Define specific, measurable goals for the corporate office's sustainability initiative", DateTime.Today.AddDays(10), DateTime.Today,  17, 2, 3, 17, 6),
               new Models.Task("Develop green procurement policy", "Create guidelines for purchasing environmentally friendly products and services", DateTime.Today.AddDays(12), DateTime.Today.AddDays(4),  18, 1, 2, 18, 6),
               new Models.Task("Implement energy efficiency measures", "Upgrade lighting, HVAC systems, and appliances to reduce energy consumption", DateTime.Today.AddDays(14), DateTime.Today.AddDays(5),  16, 2, 2, 17, 6),
               new Models.Task("Launch waste reduction campaign", "Promote recycling and waste reduction practices among employees", DateTime.Today.AddDays(16), DateTime.Today.AddDays(7),  17, 3, 3, 18, 6),
               new Models.Task("Establish green transportation options", "Encourage carpooling, biking, and public transportation for commuting to the office", DateTime.Today.AddDays(18), DateTime.Today.AddDays(10),  18, 2, 2, 17, 6),
               new Models.Task("Educate employees on sustainability", "Provide training sessions and resources to raise awareness about sustainability practices", DateTime.Today.AddDays(20), DateTime.Today.AddDays(11),  16, 1, 3, 18, 6),
               new Models.Task("Monitor water usage", "Install water meters and track water consumption to identify opportunities for conservation", DateTime.Today.AddDays(22), DateTime.Today,  17, 2, 2, 17, 6),
               new Models.Task("Collaborate with suppliers", "Work with suppliers to source sustainable materials and reduce environmental impact", DateTime.Today.AddDays(24), DateTime.Today.AddDays(14),  18, 3, 3, 18, 6),
               new Models.Task("Measure and report progress", "Collect data and prepare reports to track progress towards sustainability goals", DateTime.Today.AddDays(26), DateTime.Today.AddDays(16),  16, 1, 2, 17, 6),

               new Models.Task("Identify target communities", "Research and identify communities in need of support and outreach", DateTime.Today.AddDays(8), DateTime.Today,  19, 3, 2, 19, 7),
               new Models.Task("Develop outreach strategy", "Create a comprehensive strategy for engaging with target communities and addressing their needs", DateTime.Today.AddDays(10), DateTime.Today.AddDays(2),  20, 2, 3, 19, 7),
               new Models.Task("Recruit volunteers", "Attract and onboard volunteers to support outreach initiatives and events", DateTime.Today.AddDays(12), DateTime.Today.AddDays(4),  19, 1, 2, 20, 7),
               new Models.Task("Organize community events", "Plan and execute events to foster community engagement and provide resources", DateTime.Today.AddDays(14), DateTime.Today.AddDays(6),  20, 2, 2, 19, 7),
               new Models.Task("Establish partnerships", "Collaborate with local organizations and businesses to maximize impact and resources", DateTime.Today.AddDays(16), DateTime.Today.AddDays(5),  19, 3, 3, 20, 7),
               new Models.Task("Provide educational workshops", "Offer workshops on relevant topics to empower community members with knowledge and skills", DateTime.Today.AddDays(18), DateTime.Today.AddDays(8),  20, 2, 2, 19, 7),
               new Models.Task("Distribute resources", "Deliver essential resources such as food, clothing, and educational materials to those in need", DateTime.Today.AddDays(20), DateTime.Today.AddDays(7),  19, 1, 3, 20, 7),
               new Models.Task("Conduct outreach campaigns", "Launch targeted campaigns to raise awareness and participation in community programs", DateTime.Today.AddDays(22), DateTime.Today.AddDays(10),  20, 2, 2, 19, 7),
               new Models.Task("Evaluate program impact", "Gather feedback and data to assess the effectiveness of outreach efforts and make improvements", DateTime.Today.AddDays(24), DateTime.Today.AddDays(13),  19, 3, 3, 20, 7),
               new Models.Task("Document success stories", "Capture and share stories of individuals positively impacted by the outreach program", DateTime.Today.AddDays(26), DateTime.Today.AddDays(15),  20, 1, 2, 19, 7),

               new Models.Task("Conduct website audit", "Evaluate the current website's design, functionality, and performance", DateTime.Today.AddDays(8), DateTime.Today.AddDays(1),  22, 3, 2, 21, 8),
               new Models.Task("Define target audience", "Identify the primary audience and their preferences for the website redesign", DateTime.Today.AddDays(10), DateTime.Today.AddDays(2),  23, 2, 3, 21, 8),
               new Models.Task("Research industry trends", "Explore design and technology trends in the hospitality industry for inspiration", DateTime.Today.AddDays(12), DateTime.Today,  24, 1, 2, 22, 8),
               new Models.Task("Develop user personas", "Create personas representing different types of website visitors and their needs", DateTime.Today.AddDays(14), DateTime.Today.AddDays(4),  22, 2, 2, 21, 8),
               new Models.Task("Design wireframes", "Create wireframes to visualize the layout and structure of the new website", DateTime.Today.AddDays(16), DateTime.Today.AddDays(5),  23, 3, 3, 22, 8),
               new Models.Task("Choose color scheme and typography", "Select colors and fonts that reflect the brand identity and appeal to the target audience", DateTime.Today.AddDays(18), DateTime.Today.AddDays(7),  24, 2, 2, 21, 8),
               new Models.Task("Develop website content", "Write engaging and informative content for the new website pages", DateTime.Today.AddDays(20), DateTime.Today.AddDays(9),  22, 3, 3, 22, 8),
               new Models.Task("Implement responsive design", "Ensure the website is fully responsive and optimized for all devices and screen sizes", DateTime.Today.AddDays(22), DateTime.Today.AddDays(11),  23, 2, 2, 21, 8),
               new Models.Task("Conduct user testing", "Gather feedback from real users to identify usability issues and make improvements", DateTime.Today.AddDays(24), DateTime.Today.AddDays(10),  24, 1, 3, 22, 8),
               new Models.Task("Launch website", "Deploy the redesigned website to production and make it live for visitors", DateTime.Today.AddDays(26), DateTime.Today.AddDays(14),  22, 1, 2, 21, 8),

               new Models.Task("Analyze current website performance", "Evaluate website traffic, user engagement, and conversion rates using analytics tools", DateTime.Today.AddDays(8), DateTime.Today,  25, 3, 2, 23, 9),
               new Models.Task("Conduct competitor analysis", "Research and analyze competitor websites to identify strengths, weaknesses, and opportunities", DateTime.Today.AddDays(10), DateTime.Today,  26, 2, 3, 24, 9),
               new Models.Task("Define website redesign goals", "Establish clear objectives and KPIs for the website redesign project", DateTime.Today.AddDays(12), DateTime.Today,  27, 1, 2, 23, 9),
               new Models.Task("Gather stakeholder input", "Collect feedback and requirements from key stakeholders including management, staff, and customers", DateTime.Today.AddDays(14), DateTime.Today.AddDays(3),  25, 2, 2, 24, 9),
               new Models.Task("Create mood boards and style tiles", "Develop visual concepts to guide the design direction of the new website", DateTime.Today.AddDays(16), DateTime.Today,  26, 3, 3, 23, 9),
               new Models.Task("Design user experience (UX) wireframes", "Create wireframes to outline the user flow and layout of key website pages", DateTime.Today.AddDays(18), DateTime.Today.AddDays(8),  27, 2, 2, 24, 9),
               new Models.Task("Develop website content strategy", "Plan the structure and messaging of website content to align with business goals and target audience", DateTime.Today.AddDays(20), DateTime.Today.AddDays(11),  25, 1, 3, 23, 9),
               new Models.Task("Build responsive website design", "Implement responsive design techniques to ensure optimal viewing experience across devices", DateTime.Today.AddDays(22), DateTime.Today.AddDays(13),  26, 2, 2, 24, 9),
               new Models.Task("Conduct usability testing", "Recruit participants to test the website prototype and gather feedback on usability and functionality", DateTime.Today.AddDays(24), DateTime.Today.AddDays(15),  27, 3, 3, 23, 9),
               new Models.Task("Launch redesigned website", "Deploy the finalized website to production environment and announce its launch to stakeholders and customers", DateTime.Today.AddDays(26), DateTime.Today.AddDays(16),  25, 1, 2, 24, 9),

               new Models.Task("Conduct training needs assessment", "Evaluate skill gaps and training requirements among employees", DateTime.Today.AddDays(8), DateTime.Today,  28, 3, 2, 25, 10),
               new Models.Task("Define training objectives", "Establish clear learning objectives and goals for the training program", DateTime.Today.AddDays(10), DateTime.Today,  29, 2, 3, 25, 10),
               new Models.Task("Identify training topics", "Determine the specific subjects and skills to be covered in the training program", DateTime.Today.AddDays(12), DateTime.Today.AddDays(2),  30, 1, 2, 26, 10),
               new Models.Task("Develop training curriculum", "Design a structured curriculum outlining the sequence and content of training modules", DateTime.Today.AddDays(14), DateTime.Today.AddDays(4),  28, 3, 3, 25, 10),
               new Models.Task("Create training materials", "Produce engaging and informative training materials such as presentations, guides, and videos", DateTime.Today.AddDays(16), DateTime.Today.AddDays(3),  29, 2, 2, 26, 10),
               new Models.Task("Select training delivery methods", "Choose the most effective delivery methods, such as in-person workshops, online courses, or blended learning", DateTime.Today.AddDays(18), DateTime.Today.AddDays(6),  30, 1, 3, 25, 10),
               new Models.Task("Schedule training sessions", "Plan the timing and logistics of training sessions to accommodate employee schedules", DateTime.Today.AddDays(20), DateTime.Today.AddDays(7),  28, 3, 2, 26, 10),
               new Models.Task("Recruit external trainers", "Hire expert trainers or consultants to deliver specialized training sessions as needed", DateTime.Today.AddDays(22), DateTime.Today.AddDays(8),  29, 2, 2, 25, 10),
               new Models.Task("Train internal trainers", "Provide training for internal staff members who will serve as trainers or facilitators for the program", DateTime.Today.AddDays(24), DateTime.Today.AddDays(10),  30, 1, 3, 26, 10),
               new Models.Task("Evaluate training effectiveness", "Collect feedback and assess the impact of the training program on employee performance and skills development", DateTime.Today.AddDays(26), DateTime.Today.AddDays(16),  28, 1, 2, 25, 10)
            );
            context.SaveChanges();

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

            context.TaskUsers.AddRange(
                new TaskUser { TaskId = 1, UserId = 1 },
                new TaskUser { TaskId = 2, UserId = 2 },
                new TaskUser { TaskId = 3, UserId = 4 },
                new TaskUser { TaskId = 4, UserId = 5 },
                new TaskUser { TaskId = 5, UserId = 5 },
                new TaskUser { TaskId = 6, UserId = 3 },
                new TaskUser { TaskId = 7, UserId = 1 },
                new TaskUser { TaskId = 8, UserId = 3 },
                new TaskUser { TaskId = 9, UserId = 4 },
                new TaskUser { TaskId = 10, UserId = 4 },
                
                new TaskUser { TaskId = 11, UserId = 1 },
                new TaskUser { TaskId = 12, UserId = 4 },
                new TaskUser { TaskId = 13, UserId = 1 },
                new TaskUser { TaskId = 14, UserId = 7 },
                new TaskUser { TaskId = 15, UserId = 5 },
                new TaskUser { TaskId = 16, UserId = 8 },
                new TaskUser { TaskId = 17, UserId = 4 },
                new TaskUser { TaskId = 18, UserId = 1 },
                new TaskUser { TaskId = 19, UserId = 7 },
                new TaskUser { TaskId = 20, UserId = 8 },
                
                new TaskUser { TaskId = 21, UserId = 2 },
                new TaskUser { TaskId = 22, UserId = 10 },
                new TaskUser { TaskId = 23, UserId = 6},
                new TaskUser { TaskId = 24, UserId = 7 },
                new TaskUser { TaskId = 25, UserId = 9 },
                new TaskUser { TaskId = 26, UserId = 2 },
                new TaskUser { TaskId = 27, UserId = 7 },
                new TaskUser { TaskId = 28, UserId = 10 },
                new TaskUser { TaskId = 29, UserId = 6},
                new TaskUser { TaskId = 30, UserId = 9 },
                
                new TaskUser { TaskId = 31, UserId = 1 },
                new TaskUser { TaskId = 32, UserId = 6 },
                new TaskUser { TaskId = 33, UserId = 2 },
                new TaskUser { TaskId = 34, UserId = 3 },
                new TaskUser { TaskId = 35, UserId = 6 },
                new TaskUser { TaskId = 36, UserId = 1 },
                new TaskUser { TaskId = 37, UserId = 2 },
                new TaskUser { TaskId = 38, UserId = 3 },
                new TaskUser { TaskId = 39, UserId = 6 },
                new TaskUser { TaskId = 40, UserId = 15 },
                
                new TaskUser { TaskId = 41, UserId = 4 },
                new TaskUser { TaskId = 42, UserId = 11 },
                new TaskUser { TaskId = 43, UserId = 12 },
                new TaskUser { TaskId = 44, UserId = 15 },
                new TaskUser { TaskId = 45, UserId = 18 },
                new TaskUser { TaskId = 46, UserId = 4 },
                new TaskUser { TaskId = 47, UserId = 11 },
                new TaskUser { TaskId = 48, UserId = 12 },
                new TaskUser { TaskId = 49, UserId = 15 },
                new TaskUser { TaskId = 50, UserId = 18 },
                
                new TaskUser { TaskId = 51, UserId = 8},
                new TaskUser { TaskId = 52, UserId = 11 },
                new TaskUser { TaskId = 53, UserId = 13 },
                new TaskUser { TaskId = 54, UserId = 14 },
                new TaskUser { TaskId = 55, UserId = 4 },
                new TaskUser { TaskId = 56, UserId = 8 },
                new TaskUser { TaskId = 57, UserId = 11 },
                new TaskUser { TaskId = 58, UserId = 13 },
                new TaskUser { TaskId = 59, UserId = 14 },
                new TaskUser { TaskId = 60, UserId = 4 },
                
                new TaskUser { TaskId = 61, UserId = 5 },
                new TaskUser { TaskId = 62, UserId = 7 },
                new TaskUser { TaskId = 63, UserId = 9 },
                new TaskUser { TaskId = 64, UserId = 17 },
                new TaskUser { TaskId = 65, UserId = 16 },
                new TaskUser { TaskId = 66, UserId = 5},
                new TaskUser { TaskId = 67, UserId = 7 },
                new TaskUser { TaskId = 68, UserId = 9 },
                new TaskUser { TaskId = 69, UserId = 17 },
                new TaskUser { TaskId = 70, UserId = 16 },
                
                new TaskUser { TaskId = 71, UserId = 3 },
                new TaskUser { TaskId = 72, UserId = 9 },
                new TaskUser { TaskId = 73, UserId = 13 },
                new TaskUser { TaskId = 74, UserId = 15 },
                new TaskUser { TaskId = 75, UserId = 17 },
                new TaskUser { TaskId = 76, UserId = 3 },
                new TaskUser { TaskId = 77, UserId = 9 },
                new TaskUser { TaskId = 78, UserId = 13 },
                new TaskUser { TaskId = 79, UserId = 15 },
                new TaskUser { TaskId = 80, UserId = 17 },
                
                new TaskUser { TaskId = 81, UserId = 7 },
                new TaskUser { TaskId = 82, UserId = 10 },
                new TaskUser { TaskId = 83, UserId = 12 },
                new TaskUser { TaskId = 84, UserId = 16 },
                new TaskUser { TaskId = 85, UserId = 18 },
                new TaskUser { TaskId = 86, UserId = 7 },
                new TaskUser { TaskId = 87, UserId = 10 },
                new TaskUser { TaskId = 88, UserId = 12 },
                new TaskUser { TaskId = 89, UserId = 16 },
                new TaskUser { TaskId = 90, UserId = 18 },
                
                new TaskUser { TaskId = 91, UserId = 1 },
                new TaskUser { TaskId = 92, UserId = 7 },
                new TaskUser { TaskId = 93, UserId = 10 },
                new TaskUser { TaskId = 94, UserId = 13 },
                new TaskUser { TaskId = 95, UserId = 16 },
                new TaskUser { TaskId = 96, UserId = 1 },
                new TaskUser { TaskId = 97, UserId = 7 },
                new TaskUser { TaskId = 98, UserId = 10 },
                new TaskUser { TaskId = 99, UserId = 13 },
                new TaskUser { TaskId = 100, UserId = 16 } 
                );
           
            context.SaveChanges();

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
