using SmashTrackerGUI.Infrastructure;
using SmashTrackerGUI.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace SmashTrackerGUI.ViewModels
{
	public class PlayerDB : BaseViewModel
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

		private bool m_IsEditing;
		public bool IsEditing
		{
			get
			{
				return m_IsEditing;
			}
			set
			{
				m_IsEditing = value;
				RaisePropertyChanged();
			}
		}

		public ReadOnlyCollection<string> SortTypes => Array.AsReadOnly<string>(Enum.GetNames(typeof(SortType)));

		private SortType m_SelectedSort;
		public SortType SelectedSort
		{
			get
			{
				return m_SelectedSort;
			}
			set
			{
				m_SelectedSort = value;
				RaisePropertyChanged();
				SortPlayers();
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
			db.RemovePlayer(SelectedPlayer);
			PlayerList.Remove(SelectedPlayer);
		}

		private void EditPlayer()
		{

		}

		private void SortPlayers()
		{
			switch(SelectedSort)
			{
				case SortType.Name:
					PlayerList.OrderByDescending(p => p.Name);
					break;
				case SortType.Rating:
					PlayerList.OrderByDescending(p => p.Rating);
					break;
				case SortType.Tag:
					PlayerList.OrderByDescending(p => p.Tag);
					break;
			}

			RaisePropertyChanged("PlayerList");
		}

		// Sort Values
		public enum SortType
		{
			Name = 0,
			Tag,
			Rating
		}
	}
}
