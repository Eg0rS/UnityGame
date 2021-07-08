using System;
using System.Xml;
using Adept.Logger;
using AgkCommons.Configurations;
using AgkCommons.Resources;
using DronDonDon.Core.Configurations;
using IoC.Attribute;
using UnityEngine;

namespace DronDonDon.Core.Filter
{
    public class ConfigLoadFilter : IAppFilter
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<ConfigLoadFilter>();
        private const string LOAD_CONFIG_PATH = "Configs/LocalConfig@embeded";
        [Inject]
        private ResourceService _resourceService;
        private GameObject _bootstrapObject;

        public void Run(AppFilterChain chain)
        {
            _resourceService.LoadResource<TextAsset>(LOAD_CONFIG_PATH).Then(ConfigLoaded).Then(chain.Next);
        }

        private void ConfigLoaded(TextAsset localConfigData)
        {
            XmlDocument localConfigXml = new XmlDocument();
            try {
                localConfigXml.LoadXml(localConfigData.text);
            } catch (XmlException e) {
                throw new Exception("LocalConfig xml parse error: " + e.Message);
            }

           
            Configuration configuration = new Configuration();
            configuration.LoadXml(localConfigXml);
            Config.Init(configuration);
            ConfigureLogger(localConfigXml);
            _logger.Info("Logger configurated");
        }

        private void ConfigureLogger(XmlDocument xml)
        {
            if (!LoggerConfigurator.Configure(xml, LoggerType.NLOG)) {
                Debug.LogWarning("logger config empty");
            }
        }
    }
}