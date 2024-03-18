using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VIPER.Helpers
{
    public static class StringHelper
    {

        public static object GetPropValue( this object src, string propName)
        {
            return src.GetType().GetProperties()
                 .Single(pi => pi.Name == propName)
                 .GetValue(src, null);
        }

        public static dynamic DictionaryToObject(IDictionary<String, Object> dictionary)
        {
            var expandoObj = new ExpandoObject();
            var expandoObjCollection = (ICollection<KeyValuePair<String, Object>>)expandoObj;

            foreach (var keyValuePair in dictionary)
            {
                expandoObjCollection.Add(keyValuePair);
            }
            dynamic eoDynamic = expandoObj;
            return eoDynamic;
        }

        public static T DictionaryToObject<T>( this IDictionary<String, Object> dictionary) where T : class
        {
            return DictionaryToObject(dictionary) as T;
        }

        public static List<string> subScriptCodes = new List<string>() { "\u2080", "\u2081", "\u2082", "\u2083", "\u2084", "\u2085", "\u2086", "\u2087", "\u2088", "\u2089" };

        public static string GetSubScript(Double index)
        {
            var indexString = index.ToString();
            var subScriptBuilder = new StringBuilder();

            for (var  i=0;i< indexString.Length;i++)
            {
                var subIndex = (int)Char.GetNumericValue(indexString[i]);

                subScriptBuilder.Append(subScriptCodes[subIndex]);
            }
            return subScriptBuilder.ToString();
        }
        public static string GetUpScript(String condition)
        {
            for (var i = 0; i < subScriptCodes.Count; i++)
            {
                condition = condition.Replace(subScriptCodes[i],i.ToString());
            }
           return condition;
      }
}
}
