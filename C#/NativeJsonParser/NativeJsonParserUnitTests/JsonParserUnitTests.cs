using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NativeJsonParser;

namespace NativeJsonParserUnitTests
{
    [TestClass]
    public class JsonParserUnitTests
    {
        [TestMethod]
        public void TestTryParseArray()
        {
            {
                Assert.IsFalse(JsonParser.TryParseArray(null, out List<string> array));
            }

            {
                string json = "[{\"predictions\": {\"en\": 0.777042, \"nl\": 0.067003, \"de\": 0.028444}, \"text\": \"how rich is\"}, {\"predictions\": {\"zh-Hans\": 0.999764, \"zh-Hant\": 0.000235, \"ko\": 1e-06}, \"text\": \"比尔盖茨\"}]";
                Assert.IsTrue(JsonParser.TryParseArray(json, out List<string> array));
                Assert.AreEqual(2, array.Count);
                string item0 = "{\"predictions\": {\"en\": 0.777042, \"nl\": 0.067003, \"de\": 0.028444}, \"text\": \"how rich is\"}";
                string item1 = "{\"predictions\": {\"zh-Hans\": 0.999764, \"zh-Hant\": 0.000235, \"ko\": 1e-06}, \"text\": \"比尔盖茨\"}";
                Assert.AreEqual(item0, array[0], false);
                Assert.AreEqual(item1, array[1], false);
            }

            {
                string json = "[{\"predictions\": {\"en\": 0.777042, \"nl\": 0.067003, \"de\": 0.028444}, \"text\": \"how rich is\"}, {\"predictions\": {\"[zh-Hans\": 0.999764, \"\\\"zh-Hant\\\"\": 0.000235, \"ko\": 1e-06}, \"text\": \"比尔盖茨\"}]";
                Assert.IsTrue(JsonParser.TryParseArray(json, out List<string> array));
                Assert.AreEqual(2, array.Count);
                string item0 = "{\"predictions\": {\"en\": 0.777042, \"nl\": 0.067003, \"de\": 0.028444}, \"text\": \"how rich is\"}";
                string item1 = "{\"predictions\": {\"[zh-Hans\": 0.999764, \"\\\"zh-Hant\\\"\": 0.000235, \"ko\": 1e-06}, \"text\": \"比尔盖茨\"}";
                Assert.AreEqual(item0, array[0], false);
                Assert.AreEqual(item1, array[1], false);
            }

            {
                string json = "[{\"predictions\": {\"en\": 0.777042, \"nl\": 0.067003, \"de\": 0.028444}, \"text\": \"how rich is\"}, {\"predictions\": {\"[zh-Hans]\": 0.999764, \"]zh-Hant[\": 0.000235, \"ko\": 1e-06}, \"text\": \"比尔盖茨\"}]";
                Assert.IsTrue(JsonParser.TryParseArray(json, out List<string> array));
                Assert.AreEqual(2, array.Count);
                string item0 = "{\"predictions\": {\"en\": 0.777042, \"nl\": 0.067003, \"de\": 0.028444}, \"text\": \"how rich is\"}";
                string item1 = "{\"predictions\": {\"[zh-Hans]\": 0.999764, \"]zh-Hant[\": 0.000235, \"ko\": 1e-06}, \"text\": \"比尔盖茨\"}";
                Assert.AreEqual(item0, array[0], false);
                Assert.AreEqual(item1, array[1], false);
            }

            {
                string json = "[{\"predictions\": {\"en\": 0.777042, \"nl\": 0.067003, \"de\": 0.028444}, \"text\": \"how rich is\"}, {\"predictions\": {\"zh-{Hans]\": 0.999764, \"zh}-Hant[\": 0.000235, \"ko\": 1e-06}, \"text\": \"比尔盖茨\"}]";
                Assert.IsTrue(JsonParser.TryParseArray(json, out List<string> array));
                Assert.AreEqual(2, array.Count);
                string item0 = "{\"predictions\": {\"en\": 0.777042, \"nl\": 0.067003, \"de\": 0.028444}, \"text\": \"how rich is\"}";
                string item1 = "{\"predictions\": {\"zh-{Hans]\": 0.999764, \"zh}-Hant[\": 0.000235, \"ko\": 1e-06}, \"text\": \"比尔盖茨\"}";
                Assert.AreEqual(item0, array[0], false);
                Assert.AreEqual(item1, array[1], false);
            }

            {
                string json = " [ { \"predictions\": {\"en\": 0.777042, \"nl\": 0.067003, \"de\": 0.028444}, \"text\": \"how rich is\" }  ,   { \"predictions\": {\"zh-Hans\": 0.999764, \"zh-Hant\": 0.000235, \"ko\": 1e-06}, \"text\": \"比尔盖茨\" } ] ";
                Assert.IsTrue(JsonParser.TryParseArray(json, out List<string> array));
                Assert.AreEqual(2, array.Count);
                string item0 = "{ \"predictions\": {\"en\": 0.777042, \"nl\": 0.067003, \"de\": 0.028444}, \"text\": \"how rich is\" }";
                string item1 = "{ \"predictions\": {\"zh-Hans\": 0.999764, \"zh-Hant\": 0.000235, \"ko\": 1e-06}, \"text\": \"比尔盖茨\" }";
                Assert.AreEqual(item0, array[0], false);
                Assert.AreEqual(item1, array[1], false);
            }

            {
                string json = "\t[\r{ \"predictions\": {\"en\": 0.777042, \"nl\": 0.067003, \"de\": 0.028444}, \"text\": \"how rich is\" }\n ,\t  { \"predictions\": {\"zh-Hans\": 0.999764, \"zh-Hant\": 0.000235, \"ko\": 1e-06}, \"text\": \"比尔盖茨\" }\n]\t";
                Assert.IsTrue(JsonParser.TryParseArray(json, out List<string> array));
                Assert.AreEqual(2, array.Count);
                string item0 = "{ \"predictions\": {\"en\": 0.777042, \"nl\": 0.067003, \"de\": 0.028444}, \"text\": \"how rich is\" }";
                string item1 = "{ \"predictions\": {\"zh-Hans\": 0.999764, \"zh-Hant\": 0.000235, \"ko\": 1e-06}, \"text\": \"比尔盖茨\" }";
                Assert.AreEqual(item0, array[0], false);
                Assert.AreEqual(item1, array[1], false);
            }

            {
                string json = "\t[\r{ \"predictions\": {\"en\": 0.777042, \"nl\": 0.067003, \"de\": 0.028444}, \"text\": \"how rich is\" }\n , \n]\t";
                Assert.IsFalse(JsonParser.TryParseArray(json, out List<string> array));
            }

            {
                string json = "\t[\r{ \"predictions\": {\"en\": 0.777042, \"nl\": 0.067003, \"de\": 0.028444}, \"text\": \"how rich is\" }\n  \n]\t";
                Assert.IsTrue(JsonParser.TryParseArray(json, out List<string> array));
                Assert.AreEqual(1, array.Count);
                string item0 = "{ \"predictions\": {\"en\": 0.777042, \"nl\": 0.067003, \"de\": 0.028444}, \"text\": \"how rich is\" }";
                Assert.AreEqual(item0, array[0], false);
            }

            {
                string json = "[\"a\",\"b\",{},[]]";
                Assert.IsTrue(JsonParser.TryParseArray(json, out List<string> array));
                Assert.AreEqual(4, array.Count);
                string item0 = "\"a\"";
                string item1 = "\"b\"";
                string item2 = "{}";
                string item3 = "[]";
                Assert.AreEqual(item0, array[0], false);
                Assert.AreEqual(item1, array[1], false);
                Assert.AreEqual(item2, array[2], false);
                Assert.AreEqual(item3, array[3], false);
            }
        }

        [TestMethod]
        public void TestTryParseDictionary()
        {
            {
                Assert.IsFalse(JsonParser.TryParseDictionary("", out Dictionary<string, string> dict));
            }

            {
                string json = "{\"predictions\": {\"en\": 0.777042, \"nl\": 0.067003, \"de\": 0.028444}, \"text\": \"how rich is\"}";
                Assert.IsTrue(JsonParser.TryParseDictionary(json, out Dictionary<string, string> dict));
                var key0 = "predictions";
                var value0 = "{\"en\": 0.777042, \"nl\": 0.067003, \"de\": 0.028444}";
                var key1 = "text";
                var value1 = "\"how rich is\"";
                Assert.AreEqual(2, dict.Count);
                Assert.IsTrue(dict.ContainsKey(key0));
                Assert.AreEqual(value0, dict[key0], false);
                Assert.IsTrue(dict.ContainsKey(key1));
                Assert.AreEqual(value1, dict[key1], false);
            }

            {
                string json = "{\"pr[edictions\": {\"e]n\": 0.777042, \"nl\": 0.067003, \"de\": 0.028444}, \"text{\": \"how rich i}s\"}";
                Assert.IsTrue(JsonParser.TryParseDictionary(json, out Dictionary<string, string> dict));
                var key0 = "pr[edictions";
                var value0 = "{\"e]n\": 0.777042, \"nl\": 0.067003, \"de\": 0.028444}";
                var key1 = "text{";
                var value1 = "\"how rich i}s\"";
                Assert.AreEqual(2, dict.Count);
                Assert.IsTrue(dict.ContainsKey(key0));
                Assert.AreEqual(value0, dict[key0], false);
                Assert.IsTrue(dict.ContainsKey(key1));
                Assert.AreEqual(value1, dict[key1], false);
            }

            {
                string json = "   {\"predictions\": {\"en\": 0.777042, \"nl\": 0.067003, \"de\": 0.028444}  , \"text\": \"how rich is\"}   ";
                Assert.IsTrue(JsonParser.TryParseDictionary(json, out Dictionary<string, string> dict));
                var key0 = "predictions";
                var value0 = "{\"en\": 0.777042, \"nl\": 0.067003, \"de\": 0.028444}";
                var key1 = "text";
                var value1 = "\"how rich is\"";
                Assert.AreEqual(2, dict.Count);
                Assert.IsTrue(dict.ContainsKey(key0));
                Assert.AreEqual(value0, dict[key0], false);
                Assert.IsTrue(dict.ContainsKey(key1));
                Assert.AreEqual(value1, dict[key1], false);
            }
        }
    }
}
