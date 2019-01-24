using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Tenders.Core.Helpers
{
    public static class HtmlDocumentHelper
    {
        public static string GetValueById(this HtmlDocument d, string id, bool ignoreErrors = false)
        {
            var result = string.Empty;
            try
            {
                var text = d?.GetElementbyId(id)?.GetAttributeValue("value", string.Empty);
                if (text == null)
                    return null;

                result = HttpUtility.HtmlDecode(text);
            }
            catch (System.Exception ex)
            {
                if (!ignoreErrors)
                    throw new NodeNotFoundException($"В документе не найден элемент с id={id}", ex);
            }

            return result;
        }

        public static string GetTextById(this HtmlDocument d, string id, bool ignoreErrors = false)
        {
            var result = string.Empty;
            try
            {
                var text = d?.GetElementbyId(id)?.InnerText?.Trim();
                if (text == null)
                    return null;

                result = HttpUtility.HtmlDecode(text);
            }
            catch (System.Exception ex)
            {
                if (!ignoreErrors)
                    throw new NodeNotFoundException($"В документе не найден элемент с id={id}", ex);
            }

            return result;
        }

        public static string[] GetValuesByRegexp(this HtmlDocument d, string pattern, bool ignoreErrors = false)
        {
            IEnumerable<string> result = new List<string>();
            try
            {
                result = Regex.Match(d.Text, pattern)?.Groups?.Select(c => c.Value)?.Skip(1);
            }
            catch (System.Exception ex)
            {
                if (!ignoreErrors)
                    throw new NodeNotFoundException($"В документе не найден элемент по шаблону {pattern}", ex);
            }

            return result.ToArray();
        }

        public static void SetValueById(this HtmlDocument d, string id, string value, bool ignoreErrors = false)
        {
            try
            {
                d.GetElementbyId(id).SetAttributeValue("value", value);
            }
            catch (System.Exception ex)
            {
                if (!ignoreErrors)
                    throw new NodeNotFoundException($"Не удалось установить значение {value} для элемента {id}", ex);
            }
        }

        public static string GetValueByXPath(this HtmlDocument d, string xPath, bool ignoreErrors = false, bool deepSearch = false)
        {
            return GetValueByXPath(d?.DocumentNode, xPath, ignoreErrors, deepSearch);
        }

        public static string GetValueByXPath(this HtmlNode node, string xPath, bool ignoreErrors = false, bool deepSearch = false)
        {
            var result = string.Empty;
            try
            {
                var targetNode = node?.SelectSingleNode(xPath);
                if (targetNode == null)
                {
                    return null;
                }

                result = targetNode.GetAttributeValue("value", string.Empty);
                if (deepSearch && string.IsNullOrEmpty(result))
                    result = targetNode.InnerText;
            }
            catch (System.Exception ex)
            {
                if (!ignoreErrors)
                    throw new NodeNotFoundException($"В документе не найден элемент с xpath={xPath}", ex);
            }

            return HttpUtility.HtmlDecode(result);
        }
    }
}
