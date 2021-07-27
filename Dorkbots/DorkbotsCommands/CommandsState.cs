﻿/*
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
using Signals;

namespace Dorkbots.DorkbotsCommands
{
    public abstract class CommandsState : ICommandsState
    {
        public Signal<ICommandsState> commandsCompletedSignal { get; private set; }
        public object data { get; protected set; }
        public bool running { get; private set; }

        protected ICommands rootCommands = new SerialCommands();

        public CommandsState() 
        {
            
        }

        public void Init()
        {
            commandsCompletedSignal = new Signal<ICommandsState>();

            rootCommands.AddCallback(this);

            SetupCommands();
        }

        public void Start()
        {
            StartAbstract();
            rootCommands.Execute();

            running = true;
        }

        protected virtual void StartAbstract(){}

        public void Update()
        {
            if (running)
            {
                UpdateAbstract();
                rootCommands.Update();
            }
        }

        protected virtual void UpdateAbstract() { }

        public void Reset()
        {
            rootCommands.Reset();
            ResetVirtual();
        }

        protected virtual void ResetVirtual() { }


        public void CommandCompleted(ICommand command)
        {
            running = false;
            CommandCompletedVirtual();
            commandsCompletedSignal.Dispatch(this);
        }

        protected virtual void CommandCompletedVirtual(){ }

        protected abstract void SetupCommands();

        public void Stop()
        {
            running = false;

            rootCommands.Stop();

            StopVirtual();
        }

        protected virtual void StopVirtual() { }

        public void Dispose()
        {
            DisposeVirtual();

            if (commandsCompletedSignal != null)
            {
                commandsCompletedSignal.Dispose();
                commandsCompletedSignal = null;
                rootCommands.Dispose();
                rootCommands = null;
            } 
        }

        protected virtual void DisposeVirtual() { }
    }
}