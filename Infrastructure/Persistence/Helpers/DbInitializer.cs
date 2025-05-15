namespace Infrastructure.Persistence.Helpers;

using DbContexts;
using Domain.Entities;


public static class DbInitializer {

    public static async Task SeedAsync(AppDbContext appDbContext)
    {
        if (!appDbContext.Users.Any() && !appDbContext.Posts.Any()){
            var newUser = new User
            {
                FullName = "bardiya",
                ProfilePictureUrl = "https://img-b.udemycdn.com/user/200_H/16004620_10db_5.jpg"
            };

            appDbContext.Users.Add(newUser);
            await appDbContext.SaveChangesAsync();

            var newPost = new Post
            {
                Content = "This is going to be our first post which is being loaded from the database and it has been created using our test user.",
                ImageUrl = "",
                NrOfReports = 0,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                UserId = newUser.Id
            };

            var newPostWithImage = new Post
            {
                Content = "This is going to be our first post which is being loaded from the database and it has been created using our test user. This post has an image",
                ImageUrl = "https://unsplash.com/photos/foggy-mountain-summit-1Z2niiBPg5A",
                NrOfReports = 0,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                UserId = newUser.Id
            };

            await appDbContext.Posts.AddRangeAsync(newPost, newPostWithImage);
            await appDbContext.SaveChangesAsync();
        }
    }

}
