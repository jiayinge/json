using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NativeJsonParser
{
    public static class JsonParser
    {
        public static List<string> ParseArray(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            str = str.Trim();
            int length = str.Length;
            if (length < 3 || str[0] != '[' || str[length - 1] != ']')
            {
                return null;
            }

            var array = new List<string>();

            length -= 2;
            str = str.Substring(1, length);
            var stack = new Stack<char>();
            int index = 0;
            int startIndex = 0;
            bool isInQuotation = false;
            bool justAfterEscape = false;
            while (index < length)
            {
                var ch = str[index];
                bool isCurEscape = false;

                if (isInQuotation)
                {
                    switch (ch)
                    {
                        case '\"':
                            if (!justAfterEscape)
                            {
                                isInQuotation = false;
                            }

                            break;
                        case '\\':
                            if (!justAfterEscape)
                            {
                                isCurEscape = true;
                            }

                            break;
                        case '\'':
                            break;
                        default:
                            if (justAfterEscape)
                            {
                                return null;
                            }

                            break;
                    }
                }
                else
                {
                    var top = '\0';
                    if (stack.Count > 0)
                    {
                        top = stack.Peek();
                    }

                    switch (ch)
                    {
                        case '\"':
                            isInQuotation = true;
                            break;
                        case '[':
                            stack.Push(ch);
                            break;
                        case ']':
                            if (top != '[')
                            {
                                return null;
                            }

                            stack.Pop();
                            break;
                        case '{':
                            stack.Push(ch);
                            break;
                        case '}':
                            if (top != '{')
                            {
                                return null;
                            }

                            stack.Pop();
                            break;
                        case ',':
                            if (stack.Count == 0)
                            {
                                if (index == startIndex)
                                {
                                    return null;
                                }

                                var newItem = str.Substring(startIndex, index - startIndex).Trim();
                                if (string.IsNullOrWhiteSpace(newItem))
                                {
                                    return null;
                                }

                                array.Add(newItem);
                                startIndex = index + 1;
                            }

                            break;
                        default:
                            break;
                    }
                }

                justAfterEscape = isCurEscape;
                index++;
            }

            if (stack.Count != 0)
            {
                return null;
            }

            if (index == startIndex)
            {
                return null;
            }

            var lastNewItem = str.Substring(startIndex, index - startIndex).Trim();
            if (string.IsNullOrWhiteSpace(lastNewItem))
            {
                return null;
            }

            array.Add(lastNewItem);

            return array;
        }

        public static Dictionary<string, string> ParseDictionary(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            str = str.Trim();
            int length = str.Length;
            if (length < 3 || str[0] != '{' || str[length - 1] != '}')
            {
                return null;
            }

            var dict = new Dictionary<string, string>();
            length -= 2;
            str = str.Substring(1, length);
            var stack = new Stack<char>();
            int index = 0;
            int startIndex = 0;
            string key = null;
            bool isKeyFound = false;
            bool isInValue = false;
            bool isInQuotation = false;
            bool justAfterEscape = false;

            while (index < length)
            {
                var ch = str[index];
                bool isCurEscape = false;

                if (isInQuotation)
                {
                    switch (ch)
                    {
                        case '\"':
                            if (!justAfterEscape)
                            {
                                if (!isKeyFound)
                                {
                                    var substr = str.Substring(startIndex, index + 1 - startIndex).Trim();
                                    if (!TryParseQuotedString(substr, out key))
                                    {
                                        return null;
                                    }

                                    if (dict.ContainsKey(key))
                                    {
                                        return null;
                                    }

                                    isKeyFound = true;
                                }

                                isInQuotation = false;
                            }

                            break;
                        case '\\':
                            if (!justAfterEscape)
                            {
                                isCurEscape = true;
                            }

                            break;
                        case '\'':
                            break;
                        default:
                            if (justAfterEscape)
                            {
                                return null;
                            }

                            break;
                    }
                }
                else
                {
                    var top = '\0';
                    if (stack.Count > 0)
                    {
                        top = stack.Peek();
                    }

                    switch (ch)
                    {
                        case '\"':
                            if (isKeyFound && !isInValue)
                            {
                                return null;
                            }

                            isInQuotation = true;
                            break;
                        case '[':
                            if (!isInValue)
                            {
                                return null;
                            }

                            stack.Push(ch);
                            break;
                        case ']':
                            if (!isInValue)
                            {
                                return null;
                            }

                            if (top != '[')
                            {
                                return null;
                            }

                            stack.Pop();
                            break;
                        case '{':
                            if (!isInValue)
                            {
                                return null;
                            }

                            stack.Push(ch);
                            break;
                        case '}':
                            if (!isInValue)
                            {
                                return null;
                            }

                            if (top != '{')
                            {
                                return null;
                            }

                            stack.Pop();
                            break;
                        case ':':
                            if (!isKeyFound)
                            {
                                return null;
                            }

                            if (!isInValue)
                            {
                                isInValue = true;
                                startIndex = index + 1;
                            }
                            else if (stack.Count == 0)
                            {
                                return null;
                            }

                            break;
                        case ',':
                            if (!isInValue)
                            {
                                return null;
                            }

                            if (stack.Count == 0)
                            {
                                if (index == startIndex)
                                {
                                    return null;
                                }

                                var value = str.Substring(startIndex, index - startIndex).Trim();
                                if (string.IsNullOrWhiteSpace(value))
                                {
                                    return null;
                                }

                                dict.Add(key, value);
                                startIndex = index + 1;
                                isKeyFound = false;
                                isInValue = false;
                            }

                            break;
                        default:
                            break;
                    }
                }

                justAfterEscape = isCurEscape;
                index++;
            }

            if (!isInValue)
            {
                return null;
            }

            if (stack.Count != 0)
            {
                return null;
            }

            if (index == startIndex)
            {
                return null;
            }

            var lastValue = str.Substring(startIndex, index - startIndex).Trim();
            if (string.IsNullOrWhiteSpace(lastValue))
            {
                return null;
            }

            dict.Add(key, lastValue);

            return dict;
        }

        private static bool TryParseQuotedString(string quoted, out string str)
        {
            str = null;
            if (quoted == null)
            {
                return false;
            }

            quoted = quoted.Trim();
            if (string.IsNullOrWhiteSpace(quoted))
            {
                return false;
            }

            var length = quoted.Length;
            if (length < 2)
            {
                return false;
            }

            if (quoted[0] != '\"' || quoted[length - 1] != '\"')
            {
                return false;
            }

            if (length == 2)
            {
                str = "";
                return true;
            }

            length -= 2;
            quoted = quoted.Substring(1, length);
            str = quoted.Replace("\\\\", "\\").Replace("\\\"", "\"").Replace("\\\'", "\'");
            return true;
        }
    }
}
