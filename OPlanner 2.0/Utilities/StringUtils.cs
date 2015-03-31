using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PlannerNameSpace
{
    public class StringUtils
    {
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if the given strings match, ignoring case and leading and trailing 
        /// whitespace, and treating a null string as equivalent to an empty string.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static bool StringsMatch(string a, string b)
        {
            if (String.IsNullOrWhiteSpace(a) && string.IsNullOrWhiteSpace(b))
                return true;

            if (a == null || b == null)
                return false;

            return String.Compare(a, b, StringComparison.CurrentCultureIgnoreCase) == 0;
        }

        public static string GetExpressionName<T>(Expression<Func<T>> expression)
        {
            MemberExpression body = expression.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("The body must be a member expression");
            }

            return body.Member.Name;
        }

        public static string GetPropertyName<T, TReturn>(Expression<Func<T, TReturn>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            return body.Member.Name;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Given a string potentially consisting of substrings separated by the given
        /// character, returns the substring at the given index.  If no string is found at
        /// that index, null will be returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static string GetSubstring(string str, int idx, char separator)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            string[] substrings = str.Split(separator);
            if (substrings.Length <= idx)
            {
                return null;
            }

            return substrings[idx];
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Given a string potentially consisting of substrings separated by the given 
        /// character, this routine replaces the current value at the given index with the
        /// given new value.  If a negative value is given for idx, the value will instead
        /// be appended appropriately to the string.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static string SetSubstring(string str, string value, int idx, char separator)
        {
            if (idx < 0)
            {
                return AddSubstring(str, value, separator);
            }

            string[] substrings = string.IsNullOrWhiteSpace(str) ? null : str.Split(separator);
            int incomingCount = substrings == null ? 0 : substrings.Length;

            int totalFields = incomingCount;
            if (idx >= totalFields)
            {
                totalFields = idx + 1;
            }

            StringBuilder sb = new StringBuilder();
            int i = 0;
            while (i < totalFields)
            {
                if (i == idx)
                {
                    sb.Append(value);
                }
                else
                {
                    if (i < incomingCount)
                    {
                        sb.Append(substrings[i]);
                    }
                }

                i++;
                if (i < totalFields)
                {
                    sb.Append(separator);
                }

            }

            return sb.ToString();
        }

        public static string AddSubstring(string str, string value, char separator)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return value;
            }

            StringBuilder sb = new StringBuilder(str);
            sb.Append(separator);
            sb.Append(value);
            return sb.ToString();
        }

        public static string ClearSubstring(string str, int idx, char separator)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            if (idx < 0)
            {
                return str;
            }

            string[] substrings = string.IsNullOrWhiteSpace(str) ? null : str.Split(separator);
            int totalFields = substrings.Length;
            if (idx >= totalFields)
            {
                return str;
            }

            if (totalFields == 1)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (string substring in substrings)
            {
                if (idx != i)
                {
                    sb.Append(substring);
                    sb.Append(separator);
                }

                i++;
            }

            string fullstring = sb.ToString();
            fullstring = fullstring.TrimEnd('^');
            return fullstring;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Given a string potentially consisting of substrings separated by the given 
        /// character, this routine returns the index of the substring that matches the
        /// given value.  If no match is found, a negative value is returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static int FindSubstring(string str, string valueToFind, char separator)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return -1;
            }

            int idx = 0;
            string[] substrings = str.Split(separator);
            foreach (string substring in substrings)
            {
                if (StringsMatch(substring, valueToFind))
                {
                    return idx;
                }

                idx++;
            }

            return -1;
        }

    }
}
