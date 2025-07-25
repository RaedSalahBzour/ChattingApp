﻿using ChattingAppAPI.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ChattingAppAPI.Data;

public class Seed
{
    public static async Task SeedUsers(UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager)
    {
        if (await userManager.Users.AnyAsync()) return;
        var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);
        if (users == null) return;
        var roles = new List<AppRole>
        {
            new(){Name="member"},
            new(){Name="admin"},
            new(){Name="moderator"}
        };
        foreach (var role in roles)
        {
            await roleManager.CreateAsync(role);
        }
        foreach (var user in users)
        {
            user.Photos.First().IsApproved = true;
            user.UserName = user.UserName.ToLower();
            await userManager.CreateAsync(user, "RaedRaed!1!");
            await userManager.AddToRoleAsync(user, "member");
        }
        var admin = new AppUser
        {
            UserName = "admin",
            KnownAs = "admin",

        };
        await userManager.CreateAsync(admin, "RaedRaed!1!");
        await userManager.AddToRolesAsync(admin, ["member", "admin", "moderator"]);

    }

}
