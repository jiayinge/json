using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using NativeJsonParser;

namespace LangPredictionParser
{
    public static class LangPredictionParser
    {
        private const string NoPrediction = "no_predict";
        private const string InvalidPrediction = "invalid_preidct";
        private const string MultiLingual = "multi_lingual";
        private const string NoLanguage = "no_language";
        private const string NoProminent = "no_prominent";
        private const double ProminentRatio = 1.5D;

        private const string PredictionsKey = "predictions";

        public static string ParserLangPredictionResposne(string response)
        {
            TryParserLangPredictionResposne(response, out string language);
            return language;
        }

        public static bool TryParserLangPredictionResposne(string response, out string language)
        {
            if (string.IsNullOrWhiteSpace(response))
            {
                language = NoPrediction;
                return false;
            }

            if (!JsonParser.TryParseArray(response, out List<string> segmentsArray))
            {
                language = InvalidPrediction;
                return false;
            }

            if (segmentsArray == null || segmentsArray.Count == 0)
            {
                language = InvalidPrediction;
                return false;
            }
            else if (segmentsArray.Count > 1)
            {
                language = MultiLingual;
                return false;
            }

            if (!JsonParser.TryParseDictionary(segmentsArray[0], out Dictionary<string, string> segmentFieldsDict))
            {
                language = InvalidPrediction;
                return false;
            }

            if (!segmentFieldsDict.ContainsKey(PredictionsKey))
            {
                language = InvalidPrediction;
                return false;
            }

            if (!JsonParser.TryParseDictionary(segmentFieldsDict[PredictionsKey], out Dictionary<string, string> languageDict))
            {
                language = InvalidPrediction;
                return false;
            }

            if (languageDict == null || languageDict.Count == 0)
            {
                language = NoLanguage;
                return false;
            }

            language = NoProminent;
            double max = 0D;
            double secondMax = -1D;
            foreach (var entry in languageDict)
            {
                double confidence = 0D;
                try
                {
                    confidence = double.Parse(entry.Value);
                }
                catch (Exception)
                {
                    language = InvalidPrediction;
                    return false;
                }

                if (confidence > max)
                {
                    secondMax = max;
                    max = confidence;
                    language = entry.Key;
                }
                else if (confidence > secondMax)
                {
                    secondMax = confidence;
                }
            }

            if (max < secondMax * ProminentRatio)
            {
                language = NoProminent;
                return false;
            }

            return true;
        }
    }
}
