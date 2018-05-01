using SmashTrackerGUI.Infrastructure;
using SmashTrackerGUI.Models;
using System;
using System.Collections.ObjectModel;

namespace SmashTrackerGUI.ViewModels
{
	public class PlayerDB : NotifyChange
	{
		public PlayerDB()
		{
			db = new PlayerDatabase();
			m_PlayerList = db.GetAllPlayers();
		}

		// Member Variables
		private PlayerDatabase db;

		private ObservableCollection<Player> m_PlayerList;
		public ObservableCollection<Player> PlayerList
		{
			get
			{
				return m_PlayerList;
			}
			set
			{
				m_PlayerList = value;
				RaisePropertyChanged();
			}
		}

		private Player m_SelectedPlayer;
		public Player SelectedPlayer
		{
			get
			{
				return m_SelectedPlayer;
			}
			set
			{
				m_SelectedPlayer = value;
				RaisePropertyChanged();
			}
		}

		// Relay Command Variables
		public RelayCommand Add
		{
			get
			{
				return new RelayCommand(AddPlayer, true);
			}
		}

		public RelayCommand Remove
		{
			get
			{
				return new RelayCommand(RemovePlayer, true);
			}
		}

		// Private functions
		private void AddPlayer()
		{

		}

		private void RemovePlayer()
		{
			PlayerList.Remove(SelectedPlayer);
			// TODO: Add Remove Player function to PlayerDatabase
			// db.RemovePlayer(SelectedPlayer);
		}

		private void EditPlayer()
		{

		}
	}
}
