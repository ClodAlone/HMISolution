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
using System.Threading.Tasks;
using Syncfusion.JavaScript.Models;


namespace Syncfusion.JavaScript
{
    public class CommandsBuilder<T> where T : class
    {
        private Commands<T> commands;
        private Column<T> column = new Column<T>();
        private List<Commands<T>> commandList = new List<Commands<T>>();
        public CommandsBuilder(Column<T> column, List<Commands<T>> command)
        {
            commands = new Commands<T>();
            this.column = column;
            commandList = command;
            this.column.Commands = new List<Commands<T>>();
        }
        public CommandsBuilder<T> Type(UnboundType type)
        {
            commands.Type = type.ToString().ToLower();
            return this;
        }
        public CommandsBuilder<T> Type(String type)
        {
            commands.Type = type;
            return this;
        }
        public CommandsBuilder<T> ButtonOptions(ButtonProperties button)
        {
            commands.ButtonOptions = button;
            return this;
        }
        public void Add()
        {
            this.column.Commands.Add(commands);
            commands = new Commands<T>();
           
        }
        //ButtonOption need to be coded after the tools wrapper created.
    }
}
