using System;

namespace PlannerNameSpace
{
    public class EnumUtils
    {
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the string representation of the given enumeration value - if the enum
        /// value has underscore chars in it, they will be replaced by spaces.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static string EnumToString<T>(T enumVal)
        {
            string enumText = enumVal.ToString();
            enumText = enumText.Replace("aa", "<");
            enumText = enumText.Replace("zz", ">");
            enumText = enumText.Replace('_', ' ');
            return enumText;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the enum value represented by the given string, for the specified enum
        /// type, with any spaces in the string replaced by underscores.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static T StringToEnum<T>(string enumText)
        {
            if (enumText == null)
            {
                return default(T);
            }

            try
            {
                enumText = enumText.Replace("<", "aa");
                enumText = enumText.Replace(">", "zz");
                enumText = enumText.Replace(' ', '_');
                T enumVal = (T)Enum.Parse(typeof(T), enumText, true);
                return enumVal;
            }

            catch
            {
                return default(T);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns an array of strings representing all the values in the specified enum
        /// type.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static AsyncObservableCollection<string> GetEnumValues<T>(bool excludeWildcardValues = false)
        {
            Array enumValues = typeof(T).GetEnumValues();
            AsyncObservableCollection<string> values = new AsyncObservableCollection<string>();
            foreach (T enumVal in enumValues)
            {
                string enumText = EnumToString<T>(enumVal);
                if (!excludeWildcardValues || (!enumText.StartsWith("<") && !enumText.StartsWith("aa")))
                {
                    values.Add(enumText);
                }
            }

            return values;
        }

    }
}
