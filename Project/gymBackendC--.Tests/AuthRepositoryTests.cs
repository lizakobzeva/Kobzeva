using gymBackendC__.WebApi.Auth;
using gymBackendC__.WebApi.Models.Entities;

using Microsoft.EntityFrameworkCore;

namespace gymBackendC__.Tests;

public class AuthRepositoryTests
{
    private static List<Users> GetUsersFakes() => [
        new()
        {
            Id = 1,
            Username = "test",
            Email = "test@test.com",
            Password = "test_test",
            Role = Roles.User
        },
        new()
        {
            Id = 2,
            Username = "test2",
            Email = "test2@test.com",
            Password = "test2_test2",
            Role = Roles.Manager
        },
        new()
        {
            Id = 3,
            Username = "test3",
            Email = "test3@test.com",
            Password = "test3_test3",
            Role = Roles.Admin
        },
    ];
    
    [Fact]
    public async Task TestCorrectGetByUsernameAsync()
    {
        // -- Arrange
        var (appDbContext, authRepository) = DatabaseMock.GetAuthRepository();
        List<Users> usersFakes = GetUsersFakes();
        await appDbContext.Users.AddRangeAsync(usersFakes);
        await appDbContext.SaveChangesAsync();
        
        // -- Act
        Users selectedUser = await authRepository.GetByUsernameAsync("test");
        
        // -- Assert
        // check if record found
        Assert.NotNull(selectedUser);
        // check if correct data selected
        Assert.Equal(selectedUser, usersFakes.Where(x => x.Username == "test").ToList()[0]);
    }
    
    [Fact]
    public async Task TestIncorrectGetByUsernameAsync()
    {
        // -- Arrange
        var (appDbContext, authRepository) = DatabaseMock.GetAuthRepository();
        List<Users> usersFakes = GetUsersFakes();
        await appDbContext.Users.AddRangeAsync(usersFakes);
        await appDbContext.SaveChangesAsync();
        
        // -- Act
        Users selectedUser = await authRepository.GetByUsernameAsync("test_idk");
        
        // -- Assert
        // check if record not found
        Assert.Null(selectedUser);
    }
    
    [Fact]
    public async Task TestCorrectGetByIdsAsync()
    {
        // -- Arrange
        var (appDbContext, authRepository) = DatabaseMock.GetAuthRepository();
        List<Users> usersFakes = GetUsersFakes();
        await appDbContext.Users.AddRangeAsync(usersFakes);
        await appDbContext.SaveChangesAsync();
        
        // -- Act
        List<int> selectedIds = [1, 3, 4];
        List<Users> selectedUsers = await authRepository.GetByIdsAsync(selectedIds);
        List<Users> selectedUserFakes = usersFakes.Where(x => selectedIds.Contains(x.Id)).ToList();
        
        // -- Assert
        // check if method worked
        Assert.NotNull(selectedUsers);
        // check if correct records count selected
        Assert.Equal(2, selectedUsers.Count());
        // check if correct data selected
        Assert.True(selectedUsers.ToHashSet().SetEquals(selectedUserFakes));
    }

    [Fact]
    public async Task TestIncorrectGetByIdsAsync()
    {
        // -- Arrange
        var (appDbContext, authRepository) = DatabaseMock.GetAuthRepository();
        List<Users> usersFakes = GetUsersFakes();
        await appDbContext.Users.AddRangeAsync(usersFakes);
        await appDbContext.SaveChangesAsync();
        
        // -- Act
        List<Users> selectedUsers = await authRepository.GetByIdsAsync(new List<int>() {5, 7});
            
        // -- Assert
        // check if method worked
        Assert.NotNull(selectedUsers);
        // check if correct records count selected
        Assert.Empty(selectedUsers);
    }
    
    [Fact]
    public async Task TestCreateAsync()
    {
        // -- Arrange
        var (appDbContext, authRepository) = DatabaseMock.GetAuthRepository();
        
        // -- Act
        Users user = new Users()
        {
            Username = "test",
            Email = "test@test.com",
            Password = "test_test",
            Role = Roles.User
        };
        await authRepository.CreateAsync(user);
        Users createdUser = await appDbContext.Users.SingleOrDefaultAsync(u => u.Username == "test");
        
        // -- Assert
        // check if record exists
        Assert.NotNull(createdUser);
        // check if one record wrote
        Assert.Equal(1, appDbContext.Users.Count());
        // check if record has correct data
        Assert.Equal("test", createdUser!.Username);
        Assert.Equal("test@test.com", createdUser!.Email);
        Assert.Equal("test_test", createdUser!.Password);
        Assert.Equal(Roles.User, createdUser!.Role);
    }
}