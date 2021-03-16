using SmashTrackerGUI.Infrastructure;
using SmashTrackerGUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmashTrackerGUI.ViewModels
{
    public class MainWindow : BaseViewModel
    {
		public MainWindow()
		{
			// Default to the main page
			CurrentViewModel = new MainMenu();
		}

		private BaseViewModel m_CurrentViewModel;
		public BaseViewModel CurrentViewModel
		{
			get
			{
				return m_CurrentViewModel;
			}
			set
			{
				m_CurrentViewModel = value;
				RaisePropertyChanged();
			}
		}

		public RelayCommand ToPlayerView
		{
			get
			{
				return new RelayCommand(ToPlayer, true);
			}
		}

		public RelayCommand ToTournamentView
		{
			get
			{
				return new RelayCommand(ToTournament, true);
			}
		}

		public RelayCommand ToMainView
		{
			get
			{
				return new RelayCommand(ToMain, true);
			}
		}

		public RelayCommand ToAddView
		{
			get
			{
				return new RelayCommand(ToAdd, true);
			}
		}

		public ParamCommand ToEditView
		{
			get
			{
				return new ParamCommand(ToEdit, true);
			}
		}

		// Member functions
		private void ToPlayer()
		{
			CurrentViewModel = new PlayerDB();
		}

		private void ToTournament()
		{
			CurrentViewModel = new Tournaments();
		}

		private void ToMain()
		{
			CurrentViewModel = new MainMenu();
		}

		private void ToAdd()
		{
			CurrentViewModel = new AddEdit(null);
		}

		private void ToEdit(object parameter)
		{
			Player player = parameter as Player;
			if (player != null)
				CurrentViewModel = new AddEdit(player);
		}
	}
}
