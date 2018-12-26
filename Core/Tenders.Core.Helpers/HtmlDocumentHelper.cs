using HtmlAgilityPack;
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
                result = HttpUtility.HtmlDecode(d.GetElementbyId(id).GetAttributeValue("value", string.Empty));
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
                result = HttpUtility.HtmlDecode(d.GetElementbyId(id).InnerText.Trim());
            }
            catch (System.Exception ex)
            {
                if (!ignoreErrors)
                    throw new NodeNotFoundException($"В документе не найден элемент с id={id}", ex);
            }

            return result;
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
            var result = string.Empty;
            try
            {
                var node = d.DocumentNode.SelectSingleNode(xPath);
                result = node.GetAttributeValue("value", string.Empty);
                if (deepSearch && string.IsNullOrEmpty(result))
                    result = node.InnerText;
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
