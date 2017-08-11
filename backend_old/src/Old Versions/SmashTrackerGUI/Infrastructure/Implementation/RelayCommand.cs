using System;
using System.Windows.Input;

namespace SmashTrackerGUI.Infrastructure
{
	public class RelayCommand : ICommand
	{
		private Action localAction;
		private bool m_localCanExecute;
		public RelayCommand(Action action, bool canExecute)
		{
			localAction = action;
			m_localCanExecute = canExecute;
		}

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return m_localCanExecute;
		}

		public void Execute(object parameter)
		{
			localAction();
		}
	}
}
