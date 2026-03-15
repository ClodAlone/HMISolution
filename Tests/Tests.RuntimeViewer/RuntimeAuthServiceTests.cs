using Xunit;
using RuntimeViewer.Shared.Services;
using SharedModels;

namespace Tests.RuntimeViewer;

public class RuntimeAuthServiceTests
{
    private static List<UserConfig> CreateUsers() => new()
    {
        new UserConfig
        {
            Username = "admin",
            PasswordHash = PasswordHasher.Hash("Admin123!"),
            Group = "Admins"
        },
        new UserConfig
        {
            Username = "viewer",
            PasswordHash = PasswordHasher.Hash("View1234!"),
            Group = "Viewers"
        },
        new UserConfig
        {
            Username = "legacy",
            Password = "oldpassword",
            Group = "Admins"
        }
    };

    private static List<UserGroupConfig> CreateGroups() => new()
    {
        new UserGroupConfig { Name = "Admins", AccessLevel = "ReadWrite", CanAccessRuntime = true, CanAccessEditor = true },
        new UserGroupConfig { Name = "Viewers", AccessLevel = "Read", CanAccessRuntime = true },
        new UserGroupConfig { Name = "NoRuntime", AccessLevel = "Read", CanAccessRuntime = false }
    };

    [Fact]
    public void Login_ValidCredentials_Succeeds()
    {
        var auth = new RuntimeAuthService();
        var (success, _) = auth.Login("admin", "Admin123!", CreateUsers(), CreateGroups());

        Assert.True(success);
        Assert.True(auth.IsAuthenticated);
        Assert.Equal("admin", auth.Username);
        Assert.Equal("Admins", auth.Group);
        Assert.Equal("ReadWrite", auth.AccessLevel);
    }

    [Fact]
    public void Login_WrongPassword_Fails()
    {
        var auth = new RuntimeAuthService();
        var (success, message) = auth.Login("admin", "WrongPass!", CreateUsers(), CreateGroups());

        Assert.False(success);
        Assert.False(auth.IsAuthenticated);
        Assert.Contains("Invalid", message);
    }

    [Fact]
    public void Login_UnknownUser_Fails()
    {
        var auth = new RuntimeAuthService();
        var (success, _) = auth.Login("nobody", "pass", CreateUsers(), CreateGroups());

        Assert.False(success);
    }

    [Fact]
    public void Login_EmptyUsername_Fails()
    {
        var auth = new RuntimeAuthService();
        var (success, message) = auth.Login("", "pass", CreateUsers(), CreateGroups());

        Assert.False(success);
        Assert.Contains("required", message);
    }

    [Fact]
    public void Login_LegacyPassword_Succeeds()
    {
        var auth = new RuntimeAuthService();
        var (success, _) = auth.Login("legacy", "oldpassword", CreateUsers(), CreateGroups());

        Assert.True(success);
        Assert.True(auth.IsAuthenticated);
    }

    [Fact]
    public void Login_GroupWithoutRuntimeAccess_Fails()
    {
        var users = new List<UserConfig>
        {
            new() { Username = "restricted", PasswordHash = PasswordHasher.Hash("Pass1234!"), Group = "NoRuntime" }
        };
        var auth = new RuntimeAuthService();
        var (success, message) = auth.Login("restricted", "Pass1234!", users, CreateGroups());

        Assert.False(success);
        Assert.Contains("runtime", message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Login_CaseInsensitiveUsername()
    {
        var auth = new RuntimeAuthService();
        var (success, _) = auth.Login("ADMIN", "Admin123!", CreateUsers(), CreateGroups());

        Assert.True(success);
    }

    [Fact]
    public void Logout_ClearsState()
    {
        var auth = new RuntimeAuthService();
        auth.Login("admin", "Admin123!", CreateUsers(), CreateGroups());
        Assert.True(auth.IsAuthenticated);

        auth.Logout();

        Assert.False(auth.IsAuthenticated);
        Assert.Null(auth.Username);
        Assert.Null(auth.Group);
        Assert.Null(auth.AccessLevel);
    }

    [Fact]
    public void HasWriteAccess_ReadWriteGroup_ReturnsTrue()
    {
        var auth = new RuntimeAuthService();
        auth.Login("admin", "Admin123!", CreateUsers(), CreateGroups());
        Assert.True(auth.HasWriteAccess("some-access"));
    }

    [Fact]
    public void HasWriteAccess_ReadOnlyGroup_ReturnsFalse()
    {
        var auth = new RuntimeAuthService();
        auth.Login("viewer", "View1234!", CreateUsers(), CreateGroups());
        Assert.False(auth.HasWriteAccess("some-access"));
    }

    [Fact]
    public void HasReadAccess_AnyAuthenticatedUser_ReturnsTrue()
    {
        var auth = new RuntimeAuthService();
        auth.Login("viewer", "View1234!", CreateUsers(), CreateGroups());
        Assert.True(auth.HasReadAccess("some-access"));
    }

    [Fact]
    public void HasReadAccess_NotAuthenticated_ReturnsFalse()
    {
        var auth = new RuntimeAuthService();
        Assert.False(auth.HasReadAccess("restricted"));
    }

    [Fact]
    public void HasReadAccess_EmptyRequiredAccess_AlwaysTrue()
    {
        var auth = new RuntimeAuthService();
        Assert.True(auth.HasReadAccess(""));
        Assert.True(auth.HasWriteAccess(""));
    }

    [Fact]
    public void IsLoginRequired_NoUsers_ReturnsFalse()
    {
        var model = new NodeModel { Server = new ServerSettings { EnableRuntimeLogin = true } };
        Assert.False(RuntimeAuthService.IsLoginRequired(model));
    }

    [Fact]
    public void IsLoginRequired_LoginEnabled_WithUsers_ReturnsTrue()
    {
        var model = new NodeModel
        {
            Server = new ServerSettings { EnableRuntimeLogin = true },
            Users = new() { new UserConfig { Username = "u1" } }
        };
        Assert.True(RuntimeAuthService.IsLoginRequired(model));
    }

    [Fact]
    public void IsLoginRequired_LoginDisabled_ReturnsFalse()
    {
        var model = new NodeModel
        {
            Server = new ServerSettings { EnableRuntimeLogin = false },
            Users = new() { new UserConfig { Username = "u1" } }
        };
        Assert.False(RuntimeAuthService.IsLoginRequired(model));
    }

    [Fact]
    public void Login_MustChangePassword_FirstLogin()
    {
        var users = new List<UserConfig>
        {
            new()
            {
                Username = "newuser",
                PasswordHash = PasswordHasher.Hash("Temp123!"),
                Group = "Admins",
                MustChangePasswordOnFirstLogin = true,
                HasLoggedInBefore = false
            }
        };
        var auth = new RuntimeAuthService();
        auth.Login("newuser", "Temp123!", users, CreateGroups());

        Assert.True(auth.IsAuthenticated);
        Assert.True(auth.MustChangePassword);
    }

    [Fact]
    public void StateChanged_FiresOnLogin()
    {
        var auth = new RuntimeAuthService();
        int fired = 0;
        auth.StateChanged += () => fired++;

        auth.Login("admin", "Admin123!", CreateUsers(), CreateGroups());

        Assert.Equal(1, fired);
    }

    [Fact]
    public void StateChanged_FiresOnLogout()
    {
        var auth = new RuntimeAuthService();
        auth.Login("admin", "Admin123!", CreateUsers(), CreateGroups());

        int fired = 0;
        auth.StateChanged += () => fired++;
        auth.Logout();

        Assert.Equal(1, fired);
    }
}
