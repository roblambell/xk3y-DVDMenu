using System;
using System.Xml.XPath;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace xk3yDVDMenu
{
internal static class ThemeManager
{
    public static string Evaluate(string expression)
    {
        try
        {
            //Already a number
            Int32.Parse(expression);
            return expression;
        }
        catch (Exception)
        {
        }
        try
        {
            string evaluate = new XPathDocument
                (new StringReader("<r/>")).CreateNavigator().Evaluate
                (String.Format("number({0})", new
                                                  Regex(@"([\+\-\*])")
                                                  .Replace(expression, " ${1} ")
                                                  .Replace("/", " div ")
                                                  .Replace("%", " mod "))).ToString();
            Int32.Parse(evaluate);
            return evaluate;
        }
        catch
        {
            return expression;
        }
    }


   static public string ReplaceVals(string strInput, Dictionary<string, object> Values)
    {
        var regex = new Regex(@"\{[^\}]*\}");
        MatchCollection matches = regex.Matches(strInput);
        foreach (Match result in matches)
        {
            string aggregate = Values.Keys.Aggregate(
                result.Value, (current, v) => current.Replace("$" + v, Values[v].ToString())
                );
            strInput = strInput.Replace(result.Value, ThemeManager.Evaluate(aggregate.Replace("{", "").Replace("}", "")));
        }
        return strInput;
    }

}
}