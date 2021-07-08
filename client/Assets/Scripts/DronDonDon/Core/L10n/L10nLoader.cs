using System.Collections.Generic;
using AgkCommons.Configurations;
using AgkCommons.Extension;
using AgkCommons.L10n.Model;
using AgkCommons.L10n.Service;
using AgkCommons.Resources;
using DronDonDon.Core.Configurations;
using IoC.Attribute;
using IoC.Extension;
using RSG;
using UnityEngine;

namespace DronDonDon.Core.L10n
{
    // ReSharper disable once InconsistentNaming
    public class L10nLoader : MonoBehaviour
    {
        [Inject]
        private TranslationRepository _translationRepository;
        [Inject]
        private ResourceService _resourceService;

        private const string LANG_CONFIGURATIONS_TYPE = "Translations/lang{0}{1}@embeded";
        
        private void Awake()
        {
            this.Inject();
        }

        public IPromise LoadEmbededLangs(params string[] filenames)
        {
            List<IPromise> proms = new List<IPromise>();
            for (int i = 0; i < filenames.Length; i++) {
                foreach (Lang lang in ConfigLanguages) {
                    proms.Add(_resourceService.LoadConfiguration(string.Format(LANG_CONFIGURATIONS_TYPE, filenames[i], lang.GetName().Capitalize()))
                                              .Then(conf => OnLangConfigurationLoaded(conf, lang)));
                } 
            }
            return Promise.All(proms);
        }
        

        private List<Lang> ConfigLanguages
        {
            get
            {
                List<Lang> result = new List<Lang>();
                foreach (string language in Config.Languages) {
                    result.Add(LangTypeExtensions.ValueOf(language));
                }
                return result;
            }
        }

        private void OnLangConfigurationLoaded(Configuration loadedObject, Lang lang)
        {
            List<Configuration> langs = loadedObject.GetList<Configuration>("lang");
            foreach (Configuration langConfiguration in langs) {
                string configurationLang = langConfiguration.GetString("name");
                if (configurationLang != lang.ToString().ToLowerInvariant()) {
                    continue;
                }
                LoadLang(langConfiguration, lang);
            }
        }

        private void LoadLang(Configuration langConfiguration, Lang lang)
        {
            List<Translation> translations = new List<Translation>();

            foreach (Configuration configuration in langConfiguration.GetList<Configuration>("message")) {
                string translationId = configuration.GetString("key");
                if (string.IsNullOrEmpty(translationId)) { // пропускаем если некоррестный ключ
                    continue;
                }
                string translation = configuration.GetString("");
                Translation translationInfo = new Translation(lang, translationId, new[] {translation});
                translations.Add(translationInfo);
            }
            foreach (Configuration configuration in langConfiguration.GetList<Configuration>("gender")) {
                string translationId = configuration.GetString("key");
                if (string.IsNullOrEmpty(translationId)) { // пропускаем если некоррестный ключ
                    continue;
                }
                string[] forms = configuration.GetList<string>("form")
                                              .ToArray();
                Translation translationInfo = new Translation(lang, translationId, forms);
                translations.Add(translationInfo);
            }
            foreach (Configuration configuration in langConfiguration.GetList<Configuration>("plural")) {
                string translationId = configuration.GetString("key");
                if (string.IsNullOrEmpty(translationId)) { // пропускаем если некоррестный ключ
                    continue;
                }
                string[] forms = configuration.GetList<string>("form")
                                              .ToArray();
                Translation translationInfo = new Translation(lang, translationId, forms);
                translations.Add(translationInfo);
            }
            _translationRepository.AddTranslation(lang, translations);
        }
    }
}