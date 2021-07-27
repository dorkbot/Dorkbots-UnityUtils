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
namespace Dorkbots.DorkbotsCommands
{
    public abstract class CommandsCommander : Command, ICommands, ICommand, ICommandCallback
    {
        public int length { get { return commands.length; } }

        protected ICommands commands;

        public CommandsCommander()
        {

        }

        protected void InitCommands(ICommands commands)
        {
            this.commands = commands;
            commands.AddCallback(this);
        }

        // INTERFACE
        public ICommand AddCommand(ICommand command, int position = -1)
        {
            return commands.AddCommand(command, position);
        }

        // CALLBACK FROM COMMANDS
        public virtual void CommandCompleted(ICommand command)
        {
            Complete();
        }

        public void DisposeOfCommand(ICommand command)
        {
            commands.DisposeOfCommand(command);
        }

        public ICommand GetCommandFromName(string name)
        {
            return commands.GetCommandFromName(name);
        }

        public ICommand GetCommandFromPostion(uint position)
        {
            return commands.GetCommandFromPostion(position);
        }

        public int GetCommandsPosition(ICommand command)
        {
            return commands.GetCommandsPosition(command);
        }

        public void RemoveCommand(ICommand command)
        {
            commands.RemoveCommand(command);
        }

        //OVERRIDES
        protected override void ExecuteVirtual()
        {
            commands.Execute();
        }

        protected override void UpdateVirtual()
        {
            commands.Update();
        }

        protected override void ResetVirtual()
        {
            commands.Reset();
        }

        protected override void InitVirtual()
        {
            commands.Init();
        }

        protected override void StopVirtual(bool complete = false)
        {
            commands.Stop(complete);
        }

        protected override void DisposeVirtual()
        {
            commands.Dispose();
        }
    }
}