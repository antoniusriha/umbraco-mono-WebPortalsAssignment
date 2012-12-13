using System;
using System.Collections.Specialized;
using System.Xml;
using System.Web;
using System.Web.Caching;

using umbraco.BusinessLogic;

namespace umbraco.Test
{
	public class SetUpUtilities
	{
		public SetUpUtilities () {}

		private const string _umbracoDbDSN = "server=127.0.0.1;database=mycms;user id=www-data;datalayer=MySql";
		private const string _umbracoConfigFile = "/home/kol3/Development/umbraco/test/m57j75-umbraco-mono-9dda8e8/umbraco/presentation/config/umbracoSettings.config";
		private const string _dynamicBase = "/tmp/kol3-temp-aspnet-0";
		public static NameValueCollection GetAppSettings()
		{
			NameValueCollection appSettings = new NameValueCollection();

			//add application settings
			appSettings.Add("umbracoDbDSN", _umbracoDbDSN);

			return appSettings;
		}

		public static void AddUmbracoConfigFileToHttpCache()
		{
			XmlDocument temp = new XmlDocument();
			XmlTextReader settingsReader = new XmlTextReader(_umbracoConfigFile);

			temp.Load(settingsReader);
			HttpRuntime.Cache.Insert("umbracoSettingsFile", temp,
										new CacheDependency(_umbracoConfigFile));
		}

		public static void RemoveUmbracoConfigFileFromHttpCache()
		{
			HttpRuntime.Cache.Remove("umbracoSettingsFile");
		}

		public static void InitConfigurationManager()
		{
			ConfigurationManagerService.ConfigManager = new ConfigurationManagerTest(SetUpUtilities.GetAppSettings());
		}

		public static void InitAppDomainDynamicBase()
		{
			AppDomain.CurrentDomain.SetDynamicBase(_dynamicBase);
			//AppDomain.CurrentDomain.SetupInformation.DynamicBase = "/tmp/kol3-temp-aspnet-0";
		}

	}
}

