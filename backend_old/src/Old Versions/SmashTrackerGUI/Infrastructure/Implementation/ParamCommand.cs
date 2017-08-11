using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SmashTrackerGUI.Infrastructure
{
	public class ParamCommand : ICommand
	{
		private Action<object> execute;
		private bool canExecute;

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public ParamCommand(Action<object> execute, bool canExecute)
		{
			this.execute = execute;
			this.canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			return canExecute;
		}

		public void Execute(object parameter)
		{
			this.execute(parameter);
		}
	}
}
