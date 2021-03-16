using SmashTracker.Utility;
using SmashTrackerGUI.Infrastructure;
using SmashTrackerGUI.Models;
using SmashTrackerGUI.Models.TrueSkill;
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
			PlayerList = db.GetAllPlayers();
			SelectedSort = SortType.Default;
			SelectedSortString = "Default";
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

		private string m_SelectedSortString;
		public string SelectedSortString
		{
			get
			{
				return m_SelectedSortString;
			}
			set
			{
				m_SelectedSortString = value;
				RaisePropertyChanged();
				SelectedSort = (SortType) Enum.Parse(typeof(SortType), m_SelectedSortString);
			}
		}

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
		public RelayCommand Remove
		{
			get
			{
				return new RelayCommand(RemovePlayer, true);
			}
		}

		// Private functions
		private void RemovePlayer()
		{
			db.RemovePlayer(SelectedPlayer);
			PlayerList.Remove(SelectedPlayer);
		}

		private void SortPlayers()
		{
			switch(SelectedSort)
			{
				case SortType.Name:
					PlayerList = PlayerList.OrderBy(p => p.Name).ToObservableCollection();
					break;
				case SortType.Rating:
					PlayerList = PlayerList.OrderBy(p => p.Rating).ToObservableCollection();
					break;
				case SortType.Tag:
					PlayerList = PlayerList.OrderBy(p => p.Tag).ToObservableCollection();
					break;
			}

			RaisePropertyChanged("PlayerList");
		}

		// Sort Values
		public enum SortType
		{
			Default = 0,
			Name,
			Tag,
			Rating
		}
	}
}
