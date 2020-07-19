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
        private const string InvalidPrediction = "invalid_predict";
        private const string MultiLingual = "multi_lingual";
        private const string NoLanguage = "no_language";
        private const string NoProminent = "no_prominent";
        private const double ProminentRatio = 1.5D;
        private const double ProminentThreshold = 0.15D;

        private const string PredictionsKey = "predictions";

        public static string ParserLangPredictionResposne(string response)
        {
            string language = "";
            if (string.IsNullOrWhiteSpace(response))
            {
                language = NoPrediction;
                return language;
            }

            var segmentsArray = JsonParser.ParseArray(response);
            if (segmentsArray == null)
            {
                language = InvalidPrediction;
                return language;
            }

            if (segmentsArray.Count == 0)
            {
                language = InvalidPrediction;
                return language;
            }
            else if (segmentsArray.Count > 1)
            {
                language = MultiLingual;
                return language;
            }

            var segmentFieldsDict = JsonParser.ParseDictionary(segmentsArray[0]);
            if (segmentFieldsDict == null)
            {
                language = InvalidPrediction;
                return language;
            }

            if (!segmentFieldsDict.ContainsKey(PredictionsKey))
            {
                language = InvalidPrediction;
                return language;
            }

            var languageDict = JsonParser.ParseDictionary(segmentFieldsDict[PredictionsKey]);
            if (languageDict == null)
            {
                language = InvalidPrediction;
                return language;
            }

            if (languageDict.Count == 0)
            {
                language = NoLanguage;
                return language;
            }

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
                    return language;
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

            if (max <= ProminentThreshold || max < secondMax * ProminentRatio)
            {
                language = NoProminent;
                return language;
            }
            else
            {
                return language;
            }
        }
    }
}
