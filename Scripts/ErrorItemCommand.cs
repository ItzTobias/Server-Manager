﻿using System;
using System.Windows.Input;

namespace Server_Manager.Scripts
{
    public class ErrorItemCommand : ICommand
    {
#pragma warning disable 0067
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            clickCommand?.Invoke();
        }

        private readonly Action clickCommand;

        public ErrorItemCommand(Action clickCommand)
        {
            this.clickCommand = clickCommand;
        }
    }
}
