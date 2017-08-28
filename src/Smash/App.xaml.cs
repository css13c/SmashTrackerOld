using ChallongeApiClient;
using PlayerData;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
			if (File.Exists(playerFilepath))
				m_playerDatabase = ApplicationDataUtility.LoadPlayerData(playerFilepath);
			else
				m_playerDatabase = ApplicationDataUtility.LoadPlayerData();

			base.OnStartup(e);
		}

		protected override void OnExit(ExitEventArgs e)
		{
			ApplicationDataUtility.SavePlayerData(m_playerDatabase);

			base.OnExit(e);
		}

		public PlayerDatabase m_playerDatabase;

		private const string playerFilepath = @"E:\Code\SmashTracker\SmashData.xml";
	}
}
