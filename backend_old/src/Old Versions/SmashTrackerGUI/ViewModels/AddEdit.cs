using SmashTracker.Utility;
using SmashTrackerGUI.Infrastructure;
using SmashTrackerGUI.Models;
using SmashTrackerGUI.Models.TrueSkill;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SmashTrackerGUI.ViewModels
{
	public class AddEdit : BaseViewModel
	{
		public AddEdit(Player player = null)
		{
			if (player == null)
			{
				IsEdit = false;
				EditPlayer = new Player();
			}
			else
			{
				IsEdit = true;
				EditPlayer = player;
				OriginalCharacters = player.Characters.ToReadOnlyCollection();
			}

			db = new PlayerDatabase();
			IsAddingChar = false;
		}

		private Player m_EditPlayer;
		public Player EditPlayer
		{
			get
			{
				return m_EditPlayer;
			}
			set
			{
				m_EditPlayer = value;
				RaisePropertyChanged();
			}
		}

		private bool m_IsEdit;
		public bool IsEdit
		{
			get
			{
				return m_IsEdit;
			}
			set
			{
				m_IsEdit = value;
				RaisePropertyChanged();
			}
		}

		private bool m_IsAddingChar;
		public bool IsAddingChar
		{
			get
			{
				return m_IsAddingChar;
			}
			set
			{
				m_IsAddingChar = value;
				RaisePropertyChanged();
			}
		}

		private Character m_AddCharacter;
		public Character AddCharacter
		{
			get
			{
				return m_AddCharacter;
			}
			set
			{
				m_AddCharacter = value;
				RaisePropertyChanged();
			}
		}

		// This enables the ability to track what characters have been removed. 
		private ReadOnlyCollection<Character> m_OriginalCharacters;
		public ReadOnlyCollection<Character> OriginalCharacters
		{
			get
			{
				return m_OriginalCharacters;
			}
			set
			{
				m_OriginalCharacters = value;
				RaisePropertyChanged();
			}
		}

		public RelayCommand AddCharacterCommand
		{
			get
			{
				return new RelayCommand(AddChar, true);
			}
		}

		public RelayCommand Save
		{
			get
			{
				return new RelayCommand(SavePlayer, true);
			}
		}

		private PlayerDatabase db;

		// Member functions
		private void AddChar()
		{
			if (!IsAddingChar)
			{
				IsAddingChar = !IsAddingChar;
				return;
			}

			EditPlayer.Characters.Add(AddCharacter);
		}

		private void SavePlayer()
		{
			switch(IsEdit)
			{
				case true:
					SaveEdit();
					break;
				case false:
					SaveNew();
					break;
			}

			((MainWindow)Application.Current.MainWindow.DataContext).ToPlayerView.Execute(true);
		}

		private void SaveEdit()
		{
			// Create character change list
			ReadOnlyCollection<Character> AddedCharacters = EditPlayer.Characters.Where(c => OriginalCharacters.Any(x => x == c)).ToReadOnlyCollection();
			ReadOnlyCollection<Character> RemovedCharacters = OriginalCharacters.Where(c => !EditPlayer.Characters.Contains(c)).ToReadOnlyCollection();

			// Update player info
			db.UpdatePlayer(EditPlayer);

			// Add / Remove characters
			foreach (Character character in AddedCharacters)
				db.AddCharacter(EditPlayer.Id, character);

		}

		private void SaveNew()
		{
			Player player = db.AddPlayer(EditPlayer.Name, TrueSkillSettings.DefaultRating(), EditPlayer.Tag);

			// Since we know the player is new, we can just add all the characters in the player's character list.
			foreach (Character character in EditPlayer.Characters)
				db.AddCharacter(player.Id, character);
		}
	}
}
