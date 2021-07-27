/*
* Author: Dayvid jones
* http://www.dayvid.com
* Copyright (c) Superhero Robot 2018
* http://www.superherorobot.com
* Managed by Dorkbots
* http://www.dorkbots.com/
* Version: 1
* 
* Licence Agreement
*
* You may distribute and modify this class freely, provided that you leave this header intact,
* and add appropriate headers to indicate your changes. Credit is appreciated in applications
* that use this code, but is not required.
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
using System;
using System.Collections.Generic;

namespace Dorkbots.DorkbotsCommands
{
    public abstract class Commands : Command, ICommands
    {
        protected List<ICommand> commands = new List<ICommand>();
        protected uint currentCommandIndex = 0;
        protected uint startIndex = 0;

        public int length { get { return commands.Count; } }

        public Commands() : base() { }

        // ABSTRACT METHODS (must be overridden in a subclass)
        protected abstract void ExecuteCommand();
        protected abstract void UpdateCommand();
        public abstract void CommandCompleted(ICommand command);

        public ICommand AddCommand(ICommand command, int position = -1)
        {
            if (commands.IndexOf(command) > -1)
            {
                throw new Exception("This Command instance has already been added!!!!!!");
            }

            if (position > -1)
            {
                if (position >= commands.Count) position = commands.Count;
                commands.Insert(position, command);
            }
            else
            {
                commands.Add(command);
            }

            command.AddCallback(this);

            return command;
        }

        public ICommand GetCommandFromPostion(uint position)
        {
            return commands[(int)position];
        }

        public ICommand GetCommandFromName(string name)
        {
            ICommand returnCommand = null;
            ICommand currentCommand = null;

            for (int i = 0; i < commands.Count; i++)
            {
                currentCommand = commands[i];
                if (currentCommand.name == name)
                {
                    if (returnCommand != null)
                    {
                        throw new Exception("More than one command shares this name!!!!!!");
                    }
                    else
                    {
                        returnCommand = currentCommand;
                    }
                }
            }

            return returnCommand;
        }

        public int GetCommandsPosition(ICommand command)
        {
            int postion = -1;

            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i] == command)
                {
                    postion = i;
                }
            }

            return postion;
        }

        public void RemoveCommand(ICommand command)
        {
            int index = commands.IndexOf(command);
            if (index > -1)
            {
                commands.RemoveAt(index);
                command.RemoveCallback(this);
            }
            else
            {
                throw new Exception("This ICommands does not contain the passed ICommand!!!!!!");
            }
        }

        public void DisposeOfCommand(ICommand command)
        {
            RemoveCommand(command);
            command.Dispose();
        }

        protected override void ExecuteVirtual()
        {
            currentCommandIndex = 0;
            startIndex = 0;

            if (commands.Count > 0)
            {
                ExecuteCommand();
            }
            else
            {
                CommandsCompleted();
            }
        }

        protected override void UpdateVirtual()
        {
            UpdateCommand();
        }

        sealed override protected void ResetVirtual()
        {
            currentCommandIndex = 0;
            startIndex = 0;
            for (int i = 0; i < commands.Count; i++)
            {
                commands[i].Reset();
            }
        }

        protected override void InitVirtual()
        {
            base.InitVirtual();
        }

        protected virtual void CommandsCompleted()
        {
            this.Complete();
        }

        protected override void StopVirtual(bool complete = false)
        {
            for (int i = 0; i < commands.Count; i++)
            {
                commands[i].Stop(complete);
            }
        }

        protected override void DisposeVirtual()
        {
            for (int i = 0; i < commands.Count; i++)
            {
                commands[i].Dispose();
            }

            commands.Clear();
        }
    }
}