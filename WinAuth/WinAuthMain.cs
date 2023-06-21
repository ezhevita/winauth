/*
 * Copyright (C) 2013 Colin Mackie.
 * This software is distributed under the terms of the GNU General Public License.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using NLog;
using NLog.Config;
using NLog.Targets;
using WinAuth.Resources;

namespace WinAuth
{
	/// <summary>
	/// Class that launches the main form
	/// </summary>
	static class WinAuthMain
	{
    /// <summary>
    /// Name of this application used for %USEPATH%\[name] folder
    /// </summary>
    public const string APPLICATION_NAME = "WinAuth";

    /// <summary>
    /// Window title for this application
    /// </summary>
    public const string APPLICATION_TITLE = "WinAuth";

    /// <summary>
    /// Winuath email address used as sender to backup emails
    /// </summary>
    public const string WINAUTHBACKUP_EMAIL = "winauth@gmail.com";

    /// <summary>
    /// URL to get latest information
    /// </summary>
#if BETA
		public const string WINAUTH_UPDATE_URL = "https://raw.githubusercontent.com/winauth/winauth/master/docs/current-beta-version.xml";
#else
    public const string WINAUTH_UPDATE_URL = "https://raw.githubusercontent.com/winauth/winauth/master/docs/current-version.xml";
#endif

		/// <summary>
		/// Set of inbuilt icons and authenticator types
		/// </summary>
		public static List<(string, string)> AUTHENTICATOR_ICONS = new List<(string, string)>
		{
			("WinAuth", "WinAuthIcon.png"),

			("+Google", "GoogleIcon.png"),
			("Authenticator", "GoogleAuthenticatorIcon.png"),
			("Google", "GoogleIcon.png"),
			("Chrome", "ChromeIcon.png"),
			("Google (Blue)", "Google2Icon.png"),
			("GMail", "GMailIcon.png"),

			("+Games", "BattleNetAuthenticatorIcon.png"),
			("Battle.Net", "BattleNetAuthenticatorIcon.png"),
			("World of Warcraft", "WarcraftIcon.png"),
			("Diablo III", "DiabloIcon.png"),
			("s8", string.Empty),
			("Steam", "SteamAuthenticatorIcon.png"),
			("Steam (Circle)", "SteamIcon.png"),
			("s1", string.Empty),
			("EA", "EAIcon.png"),
			("EA (White)", "EA2Icon.png"),
			("EA (Black)", "EA3Icon.png"),
			("s2", string.Empty),
			("Origin", "OriginIcon.png"),
			("s3", string.Empty),
			("ArenaNet", "ArenaNetIcon.png"),
			("Guild Wars 2", "GuildWarsAuthenticatorIcon.png"),
			("s4", string.Empty),
			("Trion", "TrionAuthenticatorIcon.png"),
			("Glyph", "GlyphIcon.png"),
			("ArcheAge", "ArcheAgeIcon.png"),
			("Rift", "RiftIcon.png"),
			("Defiance", "DefianceIcon.png"),
			("s5", string.Empty),
			("WildStar", "WildstarIcon.png"),
			("s6", string.Empty),
			("Firefall", "FirefallIcon.png"),
			("s7", string.Empty),
			("RuneScape", "RuneScapeIcon.png"),
			("s9", string.Empty),
			("SWTOR", "Swtor.png"),
			("SWTOR (Empire)", "SwtorEmpire.png"),
			("SWTOR (Republic)", "SwtorRepublic.png"),

			("+Software", "MicrosoftAuthenticatorIcon.png"),
			("Microsoft", "MicrosoftAuthenticatorIcon.png"),
			("Windows 8", "Windows8Icon.png"),
			("Windows 7", "Windows7Icon.png"),
			("Windows Phone", "WindowsPhoneIcon.png"),
			("s3", string.Empty),
			("Android", "AndroidIcon.png"),
			("s4", string.Empty),
			("Apple", "AppleIcon.png"),
			("Apple (Black)", "AppleWhiteIcon.png"),
			("Apple (Color)", "AppleColorIcon.png"),
			("Mac", "MacIcon.png"),
			("s5", string.Empty),
			("BitBucket", "BitBucketIcon.png"),
			("DigitalOcean", "DigitalOceanIcon.png"),
			("Dreamhost", "DreamhostIcon.png"),
			("DropBox", "DropboxIcon.png"),
			("DropBox (White)", "DropboxWhiteIcon.png"),
			("Evernote", "EvernoteIcon.png"),
			("Git", "GitIcon.png"),
			("GitHub", "GitHubIcon.png"),
			("GitHub (White)", "GitHub2Icon.png"),
			("GitLab", "GitLabIcon.png"),
			("GitLab (Fox)", "GitLabFox2Icon.png"),
			("IFTTT", "IFTTTIcon.png"),
			("Itch.io", "ItchIcon.png"),
			("KickStarter", "KickStarterIcon.png"),
			("LastPass", "LastPassIcon.png"),
			("Name.com", "NameIcon.png"),
			("Teamviewer", "TeamviewerIcon.png"),
			("s7", string.Empty),
			("Amazon", "AmazonIcon.png"),
			("Amazon AWS", "AmazonAWSIcon.png"),
			("s8", string.Empty),
			("PayPal", "PayPalIcon.png"),

			("+Crypto", "BitcoinIcon.png"),
			("Bitcoin", "BitcoinIcon.png"),
			("Bitcoin Gold", "BitcoinGoldIcon.png"),
			("Bitcoin Euro", "BitcoinEuroIcon.png"),
			("Litecoin", "LitecoinIcon.png"),
			("Dogecoin", "DogeIcon.png"),

			("+Social", "FacebookIcon.png"),
			("eBay", "eBayIcon.png"),
			("Facebook", "FacebookIcon.png"),
			("Flickr", "FlickrIcon.png"),
			("Instagram", "InstagramIcon.png"),
			("LinkedIn", "LinkedinIcon.png"),
			("Tumblr", "TumblrIcon.png"),
			("Tumblr (Flat)", "Tumblr2Icon.png"),
			("Twitter", "TwitterIcon.png"),
			("Wordpress", "WordpressIcon.png"),
			("Wordpress (B&W)", "WordpressWhiteIcon.png"),
			("Yahoo", "YahooIcon.png"),
			("Okta", "OktaVerifyAuthenticatorIcon.png")
		};

		public static List<RegisteredAuthenticator> REGISTERED_AUTHENTICATORS = new List<RegisteredAuthenticator>
		{
			new RegisteredAuthenticator {Name="Authenticator", AuthenticatorType=RegisteredAuthenticator.AuthenticatorTypes.RFC6238_TIME, Icon="WinAuthIcon.png"},
			null,
			new RegisteredAuthenticator {Name="Google", AuthenticatorType=RegisteredAuthenticator.AuthenticatorTypes.Google, Icon="GoogleIcon.png"},
			new RegisteredAuthenticator {Name="Microsoft", AuthenticatorType=RegisteredAuthenticator.AuthenticatorTypes.Microsoft, Icon="MicrosoftAuthenticatorIcon.png"},
			new RegisteredAuthenticator {Name="Battle.Net", AuthenticatorType=RegisteredAuthenticator.AuthenticatorTypes.BattleNet, Icon="BattleNetAuthenticatorIcon.png"},
			new RegisteredAuthenticator {Name="Guild Wars 2", AuthenticatorType=RegisteredAuthenticator.AuthenticatorTypes.GuildWars, Icon="GuildWarsAuthenticatorIcon.png"},
			new RegisteredAuthenticator {Name="Glyph / Trion", AuthenticatorType=RegisteredAuthenticator.AuthenticatorTypes.Trion, Icon="GlyphIcon.png"},
			new RegisteredAuthenticator {Name="Steam", AuthenticatorType=RegisteredAuthenticator.AuthenticatorTypes.Steam, Icon="SteamAuthenticatorIcon.png"},
			new RegisteredAuthenticator {Name="Okta Verify", AuthenticatorType=RegisteredAuthenticator.AuthenticatorTypes.OktaVerify, Icon="OktaVerifyAuthenticatorIcon.png"}
		};

		public static ResourceManager StringResources = new ResourceManager(typeof(strings).FullName, typeof(strings).Assembly);

		public static ILogger Logger;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			try
			{
				// configure Logger
				var config = new LoggingConfiguration();
				//
				var fileTarget = new FileTarget();
				string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), APPLICATION_NAME);
				if (Directory.Exists(dir) == false)
				{
					dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				}
				fileTarget.FileName = Path.Combine(dir, "winauth.log");
				fileTarget.Layout = @"${longdate} ${assembly-version} ${logger} ${message} ${exception:format=tostring}";
				fileTarget.DeleteOldFileOnStartup = false;
				fileTarget.AutoFlush = true;
				config.AddTarget("file", fileTarget);
				//
				var rule = new LoggingRule("*", LogLevel.Error, fileTarget);
				config.LoggingRules.Add(rule);
				//
				LogManager.Configuration = config;
				Logger = LogManager.GetLogger(APPLICATION_NAME);

				using (var instance = new SingleGlobalInstance(2000))
				{
					if (!Debugger.IsAttached)
					{
						AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
						Application.ThreadException += OnThreadException;
						Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

						try
						{
							main();
						}
						catch (Exception ex)
						{
							LogException(ex);
							throw;
						}
					}
					else
					{
						main();
					}
				}
			}
			catch (TimeoutException)
			{
				// find the window or notify window
				foreach (var process in Process.GetProcesses())
				{
					if (process.ProcessName == APPLICATION_NAME)
					{
						process.Refresh();

						var hwnd = process.MainWindowHandle;
						if (hwnd == 0)
						{
							hwnd = WinAPI.FindWindow(null, APPLICATION_TITLE);
						}

						// send it the open message
						WinAPI.SendMessage(hwnd, WinAPI.WM_USER + 1, 0, 0);
						return;
					}
				}

				// fallback
				MessageBox.Show(string.Format(strings.AlreadyRunning, APPLICATION_NAME), APPLICATION_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		static void OnThreadException(object sender, ThreadExceptionEventArgs e)
		{
			LogException(e.Exception);
		}

		static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			LogException(e.ExceptionObject as Exception);
		}

		public static void LogException(Exception ex, bool silently = false)
		{
			// add catch for unknown application exceptions to try and get closer to bug
			//StringBuilder capture = new StringBuilder(DateTime.Now.ToString("u") + " ");
			//try
			//{
			//	Exception e = ex;
			//	capture.Append(e.Message).Append(Environment.NewLine);
			//	while (e != null)
			//	{
			//		capture.Append(new StackTrace(e, true).ToString()).Append(Environment.NewLine);
			//		e = e.InnerException;
			//	}
			//	//
			//	LogMessage(capture.ToString());
			//}
			//catch (Exception) { }

			try
			{
				Logger.Error(ex);

				if (silently == false)
				{
					ExceptionForm report = new ExceptionForm();
					report.ErrorException = ex;
					report.TopMost = true;
					if (_form != null && _form.Config != null)
					{
						report.Config = _form.Config;
					}
					if (report.ShowDialog() == DialogResult.Cancel)
					{
						Process.GetCurrentProcess().Kill();
					}
				}
			}
			catch (Exception) { }
		}

		/// <summary>
		/// Log a message into the winauth.log file
		/// </summary>
		/// <param name="msg">messagae to be logged</param>
		public static void LogMessage(string msg)
		{
			if (string.IsNullOrEmpty(msg))
			{
				return;
			}

			Logger.Info(msg);
		}

		private static WinAuthForm _form;

		private static void main()
		{
			// Fix #226: set to use TLS1.2
			try
			{
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			}
			catch (Exception)
			{
				// not 4.5 installed - we could prompt, but not for now
			}

			// Issue #53: set a default culture
			if (Thread.CurrentThread.CurrentCulture == null || Thread.CurrentThread.CurrentUICulture == null)
			{
				CultureInfo ci = new CultureInfo("en"); // or en-US, en-GB
				Thread.CurrentThread.CurrentCulture = ci;
				Thread.CurrentThread.CurrentUICulture = ci;
			}

			strings.Culture = Thread.CurrentThread.CurrentUICulture;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			_form = new WinAuthForm();
			Application.Run(_form);
		}
  }
}
