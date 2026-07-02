// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

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

    // ─── External (OAuth) Login Tests ────────────────────────

    private static ExternalAuthConfig CreateExternalAuthConfig(string defaultGroup = "Viewers") => new()
    {
        Enabled = true,
        DefaultGroup = defaultGroup,
        Providers = new()
        {
            new ExternalAuthProvider { Name = "Google", Enabled = true, ClientId = "id", ClientSecret = "secret" }
        }
    };

    [Fact]
    public void LoginExternal_MappedUser_Succeeds()
    {
        var auth = new RuntimeAuthService();
        var users = CreateUsers();
        var groups = CreateGroups();
        // "admin" is in the user list
        var (success, _) = auth.LoginExternal("admin", "Admin User", "Google", users, groups, CreateExternalAuthConfig());

        Assert.True(success);
        Assert.True(auth.IsAuthenticated);
        Assert.Equal("admin", auth.Username);
        Assert.Equal("Admins", auth.Group);
    }

    [Fact]
    public void LoginExternal_UnmappedUser_UsesDefaultGroup()
    {
        var auth = new RuntimeAuthService();
        var users = CreateUsers();
        var groups = CreateGroups();
        var (success, _) = auth.LoginExternal("newuser@example.com", "New User", "Google", users, groups, CreateExternalAuthConfig("Viewers"));

        Assert.True(success);
        Assert.True(auth.IsAuthenticated);
        Assert.Equal("newuser@example.com", auth.Username);
        Assert.Equal("Viewers", auth.Group);
        Assert.Equal("Read", auth.AccessLevel);
    }

    [Fact]
    public void LoginExternal_UnmappedUser_NoDefaultGroup_Denied()
    {
        var auth = new RuntimeAuthService();
        var users = CreateUsers();
        var groups = CreateGroups();
        var (success, message) = auth.LoginExternal("unknown@example.com", "Unknown", "Google", users, groups, CreateExternalAuthConfig(""));

        Assert.False(success);
        Assert.False(auth.IsAuthenticated);
        Assert.Contains("no default group", message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void LoginExternal_DomainRestriction_Allowed()
    {
        var auth = new RuntimeAuthService();
        var cfg = CreateExternalAuthConfig();
        cfg.Providers[0].AllowedDomains = "example.com, other.com";
        var (success, _) = auth.LoginExternal("user@example.com", "User", "Google", CreateUsers(), CreateGroups(), cfg);

        Assert.True(success);
    }

    [Fact]
    public void LoginExternal_DomainRestriction_Denied()
    {
        var auth = new RuntimeAuthService();
        var cfg = CreateExternalAuthConfig();
        cfg.Providers[0].AllowedDomains = "mycompany.com";
        var (success, message) = auth.LoginExternal("user@external.com", "User", "Google", CreateUsers(), CreateGroups(), cfg);

        Assert.False(success);
        Assert.Contains("not allowed", message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void LoginExternal_EmptyEmail_Fails()
    {
        var auth = new RuntimeAuthService();
        var (success, _) = auth.LoginExternal("", "No Email", "Google", CreateUsers(), CreateGroups(), CreateExternalAuthConfig());

        Assert.False(success);
        Assert.False(auth.IsAuthenticated);
    }

    [Fact]
    public void LoginExternal_GroupWithoutRuntimeAccess_Denied()
    {
        var auth = new RuntimeAuthService();
        var cfg = CreateExternalAuthConfig("NoRuntime");
        var (success, message) = auth.LoginExternal("random@test.com", "Random", "Google", CreateUsers(), CreateGroups(), cfg);

        Assert.False(success);
        Assert.Contains("runtime viewer access", message, StringComparison.OrdinalIgnoreCase);
    }
}
