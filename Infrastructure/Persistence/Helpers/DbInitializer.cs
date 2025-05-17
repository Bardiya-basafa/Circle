namespace Infrastructure.Persistence.Helpers;

using Constants;
using DbContexts;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;


public static class DbInitializer {

    public static async Task SeedUserAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        // roles 
        if (!roleManager.Roles.Any()){
            foreach (var roleName in AppRoles.AllRoles){
                if (!await roleManager.RoleExistsAsync(roleName)){
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        // users with roles 
        // normal user 
        if (!userManager.Users.Any(u => !string.IsNullOrEmpty(u.Email))){
            var password = "password1234$";

            var newUser = new User()
            {
                UserName = "bardiya",
                Email = "bardiya@gmail.com",
                EmailConfirmed = true,
                FullName = "bardiya basafa",
                ProfilePictureUrl = "https://img-b.udemycdn.com/user/200_H/16004620_10db_5.jpg"
            };

            var result = await userManager.CreateAsync(newUser, password);

            if (result.Succeeded){
                await userManager.AddToRoleAsync(newUser, AppRoles.User);
            }

            var newUserAdmin = new User()
            {
                UserName = "bardiyaAdmin",
                Email = "bardiyaAdmin@gmail.com",
                EmailConfirmed = true,
                FullName = "bardiya Admin",
                ProfilePictureUrl = "https://img-b.udemycdn.com/user/200_H/16004620_10db_5.jpg"
            };

            var resultAdmin = await userManager.CreateAsync(newUserAdmin, password);

            if (result.Succeeded){
                await userManager.AddToRoleAsync(newUserAdmin, AppRoles.User);
            }
        }
    }

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
