using ChallongeApiClient;
using PlayerData;
using System;
using System.IO;
using System.Windows;

namespace SmashTracker
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
    {
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			m_playerDatabase = ApplicationDataUtility.LoadPlayerData(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
			m_challongeClient = new ChallongeClient();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);

			ApplicationDataUtility.SavePlayerData(m_playerDatabase);
		}

		private PlayerDatabase m_playerDatabase;
		private ChallongeClient m_challongeClient;
	}
}
