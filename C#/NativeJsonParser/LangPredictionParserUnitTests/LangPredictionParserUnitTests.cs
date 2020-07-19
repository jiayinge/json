using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LangPredictionParser;

namespace LangPredictionParserUnitTests
{
    [TestClass]
    public class LangPredictionParserUnitTests
    {
        [TestMethod]
        public void TestTryParserLangPredictionResposne()
        {
            {
                var response = "";
                Assert.IsFalse(LangPredictionParser.LangPredictionParser.TryParserLangPredictionResposne(response, out string language));
                Assert.AreEqual("no_predict", language, false);
            }

            {
                var response = "[{\"predictions\": {\"en\": 0.777042, \"nl\": 0.067003, \"de\": 0.028444}, \"text\": \"how rich is\"}, {\"predictions\": {\"zh-Hans\": 0.999764, \"zh-Hant\": 0.000235, \"ko\": 1e-06}, \"text\": \"比尔盖茨\"}]";
                Assert.IsFalse(LangPredictionParser.LangPredictionParser.TryParserLangPredictionResposne(response, out string language));
                Assert.AreEqual("multi_lingual", language, false);
            }

            {
                var response = "[{\"predictions\": {\"en\": 0.894583, \"nl\": 0.030771, \"de\": 0.024074}, \"text\": \"how rich is bill gates\"}]";
                Assert.IsTrue(LangPredictionParser.LangPredictionParser.TryParserLangPredictionResposne(response, out string language));
                Assert.AreEqual("en", language, false);
            }

            {
                var response = "[{\"predictions\": {\"zh-Hans\": 0.87783, \"ja\": 0.075544, \"ko\": 0.026439}, \"text\": \"自理\"}, {\"predictions\": {\"en\": 0.215268, \"ht\": 0.210945, \"xh\": 0.146735}, \"text\": \"\b\"}]";
                Assert.IsFalse(LangPredictionParser.LangPredictionParser.TryParserLangPredictionResposne(response, out string language));
                Assert.AreEqual("multi_lingual", language, false);
            }

            {
                var response = "[{\"predictions\": {\"en\": 0.512721, \"fil\": 0.126065, \"ms\": 0.060791}, \"text\": \"'utf-8'\"}]";
                Assert.IsTrue(LangPredictionParser.LangPredictionParser.TryParserLangPredictionResposne(response, out string language));
                Assert.AreEqual("en", language, false);
            }

            {
                var response = "[{\"predictions\": {\"zh-Hans\": 0.999768, \"zh-Hant\": 0.000232}, \"text\": \"2017 年版《江西省房屋建筑和市政基础设 施工程施工招标投标示范格式文本\"}]";
                Assert.IsTrue(LangPredictionParser.LangPredictionParser.TryParserLangPredictionResposne(response, out string language));
                Assert.AreEqual("zh-Hans", language, false);
            }

            {
                var response = "[{\"predictions\": {\"zh-Hans\": 0.599768, \"zh-Hant\": 0.400232}, \"text\": \"2017 年版《江西省房屋建筑和市政基础设 施工程施工招标投标示范格式文本\"}]";
                Assert.IsFalse(LangPredictionParser.LangPredictionParser.TryParserLangPredictionResposne(response, out string language));
                Assert.AreEqual("no_prominent", language, false);
            }
        }
    }
}
