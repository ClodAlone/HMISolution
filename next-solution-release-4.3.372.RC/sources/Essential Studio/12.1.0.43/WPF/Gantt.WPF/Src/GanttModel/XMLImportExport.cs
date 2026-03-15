#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Syncfusion.ProjIO;
using System.Collections;

namespace Syncfusion.Windows.Controls.Gantt
{
    /// <summary>
    /// A class that helps to Export/Import current project in Gantt
    /// </summary>
    public class XMLImportExport
    {
        #region constructor

        public XMLImportExport()
        {

        }

        #endregion

        #region Import From XML

        /// <summary>
        /// Imports from XML.
        /// </summary>
        /// <param name="FilePath">The file path.</param>
        /// <returns></returns>
        public static ObservableCollection<TaskDetails> ImportFromXML(string FilePath)
        {
            ObservableCollection<TaskDetails> ImportedSource = null;
            Dictionary<Task, List<ProjIO.Resource>> ResourceAssignment = null;

            if (String.IsNullOrEmpty(FilePath))
                return null;

            // Importing the xml
            Project project = ProjectReader.Open(FilePath);

            if (project == null)
                return null;

            // Getting the resource assignment from the imported xml
            if (project.Assignments != null && project.Assignments.Count > 0 && project.Resources != null && project.Resources.Count > 0)
                ResourceAssignment = GenerateResourceAssignment(project.Assignments);

            List<Task> porjectTasks = project.RootTask.Children;

            // Creating Task collection source to populate it in Gantt
            ImportedSource = CreateGanttTasks(porjectTasks, ResourceAssignment);

            return ImportedSource;
        }


        /// <summary>
        /// Creates the gantt tasks.
        /// </summary>
        /// <param name="taskList">The task list.</param>
        /// <param name="resourcesAssignment">The resources assignment.</param>
        /// <returns></returns>
        private static ObservableCollection<TaskDetails> CreateGanttTasks(List<Task> taskList, Dictionary<Task, List<ProjIO.Resource>> resourcesAssignment)
        {
            ObservableCollection<TaskDetails> ganttTaskCollection = new ObservableCollection<TaskDetails>();

            // Iterating through the task to create corresponding Gantt tasks
            foreach (Task task in taskList)
            {
                if (task == null)
                    continue;

                TaskDetails ganttTask = new TaskDetails();
                
                ganttTask.TaskId = task.ID;
                ganttTask.TaskName = task.Name;
                ganttTask.StartDate = task.Start;
                ganttTask.FinishDate = task.Finish;
               // ganttTask.Duration = task.Duration;
                ganttTask.Progress = task.PercentComplete;
                ganttTask.Predecessor = GetPredecessor(task.PredecessorLink);

                List<ProjIO.Resource> resurceList = new List<ProjIO.Resource>();

                // Creating resource from assignments
                if (resourcesAssignment != null && resourcesAssignment.TryGetValue(task, out resurceList))
                {
                    foreach (ProjIO.Resource res in resurceList)
                    {
                        if (res == null)
                            continue;

                        ganttTask.Resources.Add(new Resource() { ID = res.ID, Name = res.Name });
                    }
                }

                ganttTaskCollection.Add(ganttTask);

                // creating hierarchy
                if (task.Children != null && task.Children.Count > 0)
                {
                    var result = CreateGanttTasks(task.Children,resourcesAssignment);
                    foreach (TaskDetails child in result)
                    {
                        ganttTask.Child.Add(child);
                    }
                }
            }
            return ganttTaskCollection;
        }

        /// <summary>
        /// Gets the predecessor.
        /// </summary>
        /// <param name="predecessorLink">The predecessor link.</param>
        /// <returns></returns>
        private static ObservableCollection<Predecessor> GetPredecessor(List<TaskLink> predecessorLink)
        {
            ObservableCollection<Predecessor> GanttPredecessors = new ObservableCollection<Predecessor>();

            // Creating predecessors from the predecessor link
            foreach (TaskLink tlink in predecessorLink)
            {
                Predecessor predecessor = new Predecessor();
                predecessor.GanttTaskIndex = tlink.Predecessor.ID;

                // creating predecessor link type
                switch (tlink.Type)
                {
                    case TaskLinkType.FinishToFinish:
                        predecessor.GanttTaskRelationship = GanttTaskRelationship.FinishToFinish;
                        break;
                    case TaskLinkType.FinishToStart:
                        predecessor.GanttTaskRelationship = GanttTaskRelationship.FinishToStart;
                        break;
                    case TaskLinkType.StartToFinish:
                        predecessor.GanttTaskRelationship = GanttTaskRelationship.StartToFinish;
                        break;
                    case TaskLinkType.StartToStart:
                        predecessor.GanttTaskRelationship = GanttTaskRelationship.StartToStart;
                        break;
                }
                GanttPredecessors.Add(predecessor);
            }
            return GanttPredecessors;
        }

        /// <summary>
        /// Generates the resource assignment.
        /// </summary>
        /// <param name="assignments">The assignments.</param>
        /// <returns></returns>
        private static Dictionary<Task, List<ProjIO.Resource>> GenerateResourceAssignment(List<Assignment> assignments)
        {
            Dictionary<Task, List<ProjIO.Resource>> assignedResources = new Dictionary<Task, List<ProjIO.Resource>>();

            // To get the assignment based on the task, this will help us to create resoure on iterating the task itself
            foreach (Assignment assign in assignments)
            {
                if (!assignedResources.Keys.Contains(assign.Task))
                {
                    assignedResources.Add(assign.Task, new List<ProjIO.Resource>());
                    assignedResources[assign.Task].Add(assign.Resource);
                }
                else
                {
                    if (!assignedResources[assign.Task].Contains(assign.Resource))
                    {
                        assignedResources[assign.Task].Add(assign.Resource);
                    }
                }
            }
            return assignedResources;
        }

        #endregion

        #region Export To XML

        /// <summary>
        /// Exports to XML.
        /// </summary>
        /// <param name="taskDetailsCollection">The task details collection.</param>
        /// <param name="FilePath">The file path.</param>
        public static void ExportToXML(IEnumerable taskDetailsCollection, string FilePath)
        {
            // Initializing the variables to create objects for exporting.
            Project project = new Project();
            Dictionary<TaskDetails, Task> likendTasks = new Dictionary<TaskDetails, Task>();
            Dictionary<TaskDetails, Task> linearTasks = new Dictionary<TaskDetails, Task>();
            List<Assignment> assignedTasks = new List<Assignment>();

            // Creating the poject tasks and getting the input for assignment and predecessor
            project.RootTask.Children = CreateProjectTasks(taskDetailsCollection, likendTasks, assignedTasks ,linearTasks);

            project.CalculateTaskIDs();

            // Creating predecessors
            if (likendTasks.Count > 0)
                UpdateLinkedTasks(likendTasks, linearTasks);

            // Creating resource assignment
            if (assignedTasks.Count > 0)
            {
                project.Assignments.AddRange(assignedTasks);

                project.Resources = assignedTasks.Select((a) => a.Resource).ToList();
            }

            // Calculating UIDs of tasks and resources
            project.CalculateResourceIDs();

            // Exporting the current project to xml
            project.Save(FilePath);
        }

        /// <summary>
        /// Creates the project tasks.
        /// </summary>
        /// <param name="taskDetailsCollection">The task details collection.</param>
        /// <param name="likendTasks">The likend tasks.</param>
        /// <param name="assignedTasks">The assigned tasks.</param>
        /// <param name="linearTasks">The linear tasks.</param>
        /// <returns></returns>
        private static List<Task> CreateProjectTasks(IEnumerable taskDetailsCollection, Dictionary<TaskDetails, Task> likendTasks, List<Assignment> assignedTasks, Dictionary<TaskDetails, Task> linearTasks)
        {
            List<Task> projectTaskList = new List<Task>();

            foreach (TaskDetails ganttTask in taskDetailsCollection)
            {
                if (ganttTask == null)
                    continue;

                Task projTask = new Task();

                projTask.ID = ganttTask.TaskId;
                projTask.Name = ganttTask.TaskName;
                projTask.Start = ganttTask.StartDate;
               // projTask.Duration = ganttTask.Duration;
                projTask.Finish = ganttTask.FinishDate;
                projTask.PercentComplete = (int)ganttTask.Progress;
                projTask.ConstraintType = TaskConstraintType.StartNoEarlierThan;
                projTask.ConstraintDate = ganttTask.StartDate;

                // Adding predecessor task
                if (ganttTask.Predecessor != null && ganttTask.Predecessor.Count > 0)
                    likendTasks.Add(ganttTask, projTask);

                // Creating hierarchy
                if (ganttTask.Child != null && ganttTask.Child.Count > 0)
                    projTask.Children = CreateProjectTasks(ganttTask.Child,likendTasks, assignedTasks,linearTasks);

                // Creating resource assignment
                if (ganttTask.Resources != null && ganttTask.Resources.Count > 0)
                {
                    foreach(Resource res in ganttTask.Resources)
                    {
                        Assignment assignment = new Assignment();
                        assignment.Task = projTask;
                        assignment.Resource = new ProjIO.Resource { Name = res.Name, ID = res.ID };
                        assignedTasks.Add(assignment);
                    }
                }
               
                linearTasks.Add(ganttTask, projTask);

                // Adding the created task to project
                projectTaskList.Add(projTask);
            }

            return projectTaskList;
        }

        /// <summary>
        /// Updates the linked tasks.
        /// </summary>
        /// <param name="likendTasks">The likend tasks.</param>
        /// <param name="linearTasks">The linear tasks.</param>
        private static void UpdateLinkedTasks(Dictionary<TaskDetails, Task> likendTasks, Dictionary<TaskDetails, Task> linearTasks)
        {
            // Creating predecessor Link from gantt predecessors
            foreach (TaskDetails task in likendTasks.Keys)
            {
                foreach (Predecessor pre in task.Predecessor)
                {
                   // TaskLink taskLink = new TaskLink();
                    var result = linearTasks.Values.Where((t) => t.ID == pre.GanttTaskIndex);

                    if (result == null || result.Count() <=0 || result.First() == null)
                        continue;

                    Task projTask = result.First();

                  //  taskLink.PredecessorUID = projTask.UID;
                  //  taskLink.Predecessor = projTask;
                  //  taskLink.Successor = linearTasks[task];
                  //  taskLink.LagFormat = DelayFormat.Days;

                    TaskLinkType tLinkType = TaskLinkType.FinishToStart;

                    ///// Getting predecessor link type
                    switch (pre.GanttTaskRelationship)
                    {
                        case GanttTaskRelationship.FinishToFinish:
                            tLinkType = TaskLinkType.FinishToFinish;
                            break;
                        case GanttTaskRelationship.FinishToStart:
                            tLinkType = TaskLinkType.FinishToStart;
                            break;
                        case GanttTaskRelationship.StartToFinish:
                            tLinkType = TaskLinkType.StartToFinish;
                            break;
                        case GanttTaskRelationship.StartToStart:
                            tLinkType = TaskLinkType.StartToStart;
                            break;
                    }

                    TaskLink taskLink = new TaskLink(projTask, linearTasks[task], tLinkType);

                   // linearTasks[task].PredecessorLink.Add(taskLink);
                }
            }
        }
        #endregion
    }
}
