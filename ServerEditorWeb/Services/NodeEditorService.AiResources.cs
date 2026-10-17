using System.Text.Json;
using ServerEditorWeb.Models;

namespace ServerEditorWeb.Services;

public partial class NodeEditorService
{
    public (bool success, string message) ImportAiResources(ProjectNode project, string json)
    {
        if (ActiveProject != project || !_openProjects.Contains(project))
            return (false, "The target project changed or was closed. Generate resources again for the current project.");
        if (project.IsLocked)
            return (false, "Unlock the project before adding AI resources.");

        var occupied = new List<string>();
        void Collect(TreeNode node)
        {
            if (node is ProjectNode other && other != project) return;
            occupied.Add(node.Name);
            if (node is VariableGroupNode)
            {
                foreach (var root in node.Children.OfType<FolderNode>())
                    occupied.AddRange(root.Children.Select(child => child.Name));
                return;
            }
            foreach (var child in node.Children) Collect(child);
        }
        Collect(project);
        var current = project.Model;
        occupied.AddRange(current.Scripts.Select(item => item.Name));
        occupied.AddRange(current.PlcPrograms.Select(item => item.Name));
        occupied.AddRange(current.Screens.Select(item => item.Name));
        occupied.AddRange(current.Recipes.Select(item => item.Name));
        occupied.AddRange(current.Schedulers.Select(item => item.Name));
        occupied.AddRange(current.Reports.Select(item => item.Name));
        occupied.AddRange(current.CalculatedVariables.Select(item => item.Name));
        occupied.AddRange(current.Assets.Select(item => item.Name));
        occupied.AddRange(current.BatchSequences.Select(item => item.Name));
        occupied.AddRange(current.Events.Select(item => item.Name));
        occupied.AddRange(current.AutomationRules.SelectMany(item => new[] { item.Name, item.Id }));
        occupied.AddRange(current.AiAgents.SelectMany(item => new[] { item.Name, item.Id }));
        occupied.AddRange(current.McpConnections.SelectMany(item => new[] { item.Name, item.Id }));
        occupied.AddRange(current.AliasMaps.Select(item => item.Name));
        occupied.AddRange(current.Users.Select(item => item.Username));
        occupied.AddRange(current.UserGroups.Select(item => item.Name));
        occupied.AddRange(current.Folder.Folders.Select(item => item.Name));
        occupied.AddRange(current.Strings.Select(item => item.Key));
        occupied.AddRange(current.Images.Select(item => item.Id));
        occupied.AddRange(current.Cameras.Select(item => item.CameraId));
        occupied.AddRange(current.UserSymbolGroups.SelectMany(item => new[] { item.Id, item.Name }));
        var importNamespace = "AI_Task";
        for (var suffix = 2; occupied.Any(name => name.Equals(importNamespace, StringComparison.OrdinalIgnoreCase)
            || name.StartsWith(importNamespace + "_", StringComparison.OrdinalIgnoreCase)
            || name.StartsWith(importNamespace + ".", StringComparison.OrdinalIgnoreCase)); suffix++)
            importNamespace = $"AI_Task_{suffix}";

        SharedModels.NodeModel additions;
        try
        {
            additions = AiResourceBundle.Parse(json, importNamespace);
        }
        catch (JsonException ex)
        {
            return (false, $"Cannot add resources: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            return (false, $"Invalid resource shape: {ex.Message}");
        }
        catch (ArgumentException ex)
        {
            return (false, $"Invalid resource JSON: {ex.Message}");
        }

        var staging = new ProjectNode(additions, "");
        BuildProjectChildren(staging);
        var attachments = new List<(TreeNode parent, TreeNode child)>();
        foreach (var group in staging.Children)
        {
            if (group is VariableGroupNode)
            {
                if (additions.Folder.Variables.Count == 0 && additions.Folder.Folders.Count == 0) continue;
                var parent = project.Children.OfType<VariableGroupNode>().Single().Children.OfType<FolderNode>().Single();
                attachments.Add((parent, group.Children.Single()));
            }
            else if (group.Children.Count > 0)
            {
                var parent = project.Children.Single(node => node.GetType() == group.GetType());
                attachments.AddRange(group.Children.Select(child => (parent, child)));
            }
        }
        var model = project.Model;
        var count = attachments.Count + additions.Strings.Count + additions.Images.Count + additions.Cameras.Count + additions.UserSymbolGroups.Count;
        if (count == 0) return (false, "The bundle contains no new resources.");

        if (additions.Folder.Variables.Count > 0 || additions.Folder.Folders.Count > 0)
            model.Folder.Folders.Add(additions.Folder);
        model.Scripts.AddRange(additions.Scripts);
        model.PlcPrograms.AddRange(additions.PlcPrograms);
        model.Screens.AddRange(additions.Screens);
        model.Recipes.AddRange(additions.Recipes);
        model.Schedulers.AddRange(additions.Schedulers);
        model.Reports.AddRange(additions.Reports);
        model.CalculatedVariables.AddRange(additions.CalculatedVariables);
        model.Assets.AddRange(additions.Assets);
        model.BatchSequences.AddRange(additions.BatchSequences);
        model.Events.AddRange(additions.Events);
        model.AutomationRules.AddRange(additions.AutomationRules);
        model.Users.AddRange(additions.Users);
        model.UserGroups.AddRange(additions.UserGroups);
        model.Strings.AddRange(additions.Strings);
        model.Images.AddRange(additions.Images);
        model.Cameras.AddRange(additions.Cameras);
        model.AliasMaps.AddRange(additions.AliasMaps);
        model.UserSymbolGroups.AddRange(additions.UserSymbolGroups);
        model.AiAgents.AddRange(additions.AiAgents);
        model.McpConnections.AddRange(additions.McpConnections);
        foreach (var (parent, child) in attachments)
        {
            child.Parent = parent;
            parent.Children.Add(child);
            for (TreeNode? ancestor = parent; ancestor != null; ancestor = ancestor.Parent)
                ancestor.IsExpanded = true;
        }
        if (attachments.Count > 0) SetSingleSelection(attachments[0].child);
        HasUnsavedChanges = true;
        if (additions.UserSymbolGroups.Count > 0) NotifyUserSymbolGroupsChanged();
        NotifyStateChanged();
        return (true, $"Added resources in '{importNamespace}'. Existing resources and settings were preserved. Review new executable resources before enabling them. Refresh from Tree to update the full JSON view when needed.");
    }
}
