using System.Text.Json;
using SharedModels;

namespace ServerEditorWeb.Models;

/// <summary>Carries a tree node click with modifier key state.</summary>
public record TreeNodeClickArgs(TreeNode Node, bool CtrlKey, bool ShiftKey);

public abstract class TreeNode
{
    public string Name { get; set; } = "";
    public bool IsSelected { get; set; }
    public bool IsExpanded { get; set; }
    public List<TreeNode> Children { get; set; } = new();
    public TreeNode? Parent { get; set; }
    public abstract string TypeName { get; }
    public abstract string Icon { get; }
}

public class FolderNode : TreeNode
{
    public override string TypeName => "Folder";
    public override string Icon => "📁";
    public Folder Folder { get; }

    public FolderNode(Folder folder)
    {
        Folder = folder;
        Name = folder.Name;
    }

    public void SyncName() => Folder.Name = Name;
}

public class VariableGroupNode : TreeNode
{
    public override string TypeName => "VariableGroup";
    public override string Icon => "🗄️";

    public VariableGroupNode()
    {
        Name = "Variables";
    }
}

public class VariableNode : TreeNode
{
    public override string TypeName => "Variable";
    public override string Icon => "🏷️";
    public Variable Variable { get; }

    public static readonly string[] AvailableTypes = ["Double", "Int32", "Boolean", "String", "DateTime", "Float", "Int16", "UInt16", "UInt32"];

    private string _driverSettingsJson = "{}";

    public VariableNode(Variable variable)
    {
        Variable = variable;
        Name = variable.Name;

        if (variable.DriverConfigs is { Count: > 0 })
        {
            _driverSettingsJson = JsonSerializer.Serialize(variable.DriverConfigs, new JsonSerializerOptions { WriteIndented = true });
        }
    }

    public void SyncName() => Variable.Name = Name;

    public string DriverSettingsJson
    {
        get => _driverSettingsJson;
        set
        {
            _driverSettingsJson = value;
            try
            {
                if (string.IsNullOrWhiteSpace(value) || value.Trim() == "{}")
                {
                    Variable.DriverConfigs = null;
                }
                else
                {
                    Variable.DriverConfigs = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(value);
                }
            }
            catch
            {
                // Invalid JSON — keep model as-is
            }
        }
    }
}

public class ScriptGroupNode : TreeNode
{
    public override string TypeName => "ScriptGroup";
    public override string Icon => "📜";

    public ScriptGroupNode()
    {
        Name = "Scripts";
    }
}

public class ScriptNode : TreeNode
{
    public override string TypeName => "Script";
    public override string Icon => "⚡";
    public ScriptConfig Script { get; }

    public ScriptNode(ScriptConfig script)
    {
        Script = script;
        Name = script.Name;
    }

    public void SyncName() => Script.Name = Name;
}

public class ScreenGroupNode : TreeNode
{
    public override string TypeName => "ScreenGroup";
    public override string Icon => "🖥️";

    public ScreenGroupNode()
    {
        Name = "Screens";
    }
}

public class ScreenNode : TreeNode
{
    public override string TypeName => "Screen";
    public override string Icon => "🖼️";
    public ScreenConfig Screen { get; }

    public ScreenNode(ScreenConfig screen)
    {
        Screen = screen;
        Name = screen.Name;
    }

    public void SyncName() => Screen.Name = Name;
}

public class UserGroupListNode : TreeNode
{
    public override string TypeName => "UserGroupList";
    public override string Icon => "👥";

    public UserGroupListNode()
    {
        Name = "Users & Groups";
    }
}

public class UserGroupNode : TreeNode
{
    public override string TypeName => "UserGroup";
    public override string Icon => "🛡️";
    public UserGroupConfig UserGroup { get; }

    public UserGroupNode(UserGroupConfig group)
    {
        UserGroup = group;
        Name = group.Name;
    }

    public void SyncName() => UserGroup.Name = Name;
}

public class UserNode : TreeNode
{
    public override string TypeName => "User";
    public override string Icon => "👤";
    public UserConfig User { get; }

    public UserNode(UserConfig user)
    {
        User = user;
        Name = user.Username;
    }

    public void SyncName() => User.Username = Name;
}

public class PlcGroupNode : TreeNode
{
    public override string TypeName => "PlcGroup";
    public override string Icon => "🔧";

    public PlcGroupNode()
    {
        Name = "PLC Programs";
    }
}

public class PlcProgramNode : TreeNode
{
    public override string TypeName => "PlcProgram";
    public override string Icon => "⚙️";
    public PlcProgramConfig PlcProgram { get; }

    public PlcProgramNode(PlcProgramConfig plc)
    {
        PlcProgram = plc;
        Name = plc.Name;
    }

    public void SyncName() => PlcProgram.Name = Name;
}

public class RecipeGroupNode : TreeNode
{
    public override string TypeName => "RecipeGroup";
    public override string Icon => "🍳";

    public RecipeGroupNode()
    {
        Name = "Recipes";
    }
}

public class RecipeNode : TreeNode
{
    public override string TypeName => "Recipe";
    public override string Icon => "📦";
    public RecipeConfig Recipe { get; }

    public RecipeNode(RecipeConfig recipe)
    {
        Recipe = recipe;
        Name = recipe.Name;
    }

    public void SyncName() => Recipe.Name = Name;
}
