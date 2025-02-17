using ConsoleApp1.Models;

namespace ConsoleApp1.Data
{
    public class DbInit
    {
        public void Init(ApplicationContext context)
        {
            if (!context.Authors.Any())
            {
                context.Authors.AddRange(new Author[]
                {
                    new Author { Name = "Jess Kidd"},
                    new Author { Name = "Martha McPhee"},
                    new Author { Name = "Megan Miranda"},
                    new Author { Name = "Helen Phillips"},
                    new Author { Name = "Karen Kingsbury"}
                });
                context.SaveChanges();
            }
            if (!context.Categories.Any())
            {
                context.Categories.AddRange
                    (
                    new Category
                    {
                        Name = "Artistic",
                        Description = "The specificity of fiction is revealed by comparing it with art forms that use " +
                    "other material instead of verbal and linguistic material."
                    },
                    new Category
                    {
                        Name = "Adventure",
                        Description = "The genre is close to action movies, but unlike the latter, in adventure films, " +
                    "the emphasis is shifted from brutal violence to wit of the characters, the ability to outsmart, to deceive the villain."
                    }
                    );
                context.SaveChanges();
            }
            if (!context.Books.Any())
            {
                Book theNightShip = new Book
                {
                    Title = "The Night Ship",
                    Description = "Based on a real-life event, an epic historical novel from the award-winning author of Things in Jars.",
                    Price = 70,
                    PublishedOn = new DateTime(2022, 9, 12),
                    Publisher = "Jess Kidd",
                    Category = context.Categories.FirstOrDefault(e => e.Name.Equals("Artistic")),
                    Authors = new List<Author>()
                    {
                        context.Authors.FirstOrDefault(e => e.Name.Equals("Jess Kidd"))
                    }
                };

                Book theOnlySurvivors = new Book
                {
                    Title = "The Only Survivors",
                    Description = "Thrilling mystery about a group of former classmates who reunite to mark the tenth anniversary of a tragic accident—only" +
                    " to have one of the survivors disappear, casting fear and suspicion on the original tragedy.",
                    Price = 59,
                    PublishedOn = new DateTime(2021, 7, 24),
                    Publisher = "Novel Group Company",
                    Category = context.Categories.FirstOrDefault(e => e.Name.Equals("Adventure")),
                    Authors = new List<Author>()
                    {
                        context.Authors.FirstOrDefault(e=>e.Name.Equals("Megan Miranda")),
                        context.Authors.FirstOrDefault(e=>e.Name.Equals("Helen Phillips"))
                    }
                };

                context.Books.AddRange(new Book[]
                {
                   theNightShip,
                   theOnlySurvivors
                });

                context.SaveChanges();
            }
            if (!context.Promotions.Any())
            {
                context.Promotions.AddRange
                    (
                    new Promotion
                    {
                        Name = "Christmas Eve discount!",
                        Percent = 15,
                        Book = context.Books.FirstOrDefault(e => e.Title.Equals("The Night Ship"))
                    },
                    new Promotion
                    {
                        Name = "Christmas Eve discount!",
                        Amount = 25,
                        Book = context.Books.FirstOrDefault(e => e.Title.Equals("The Only Survivors"))
                    }
                    );
                context.SaveChanges();
            }
        }
    }
}
