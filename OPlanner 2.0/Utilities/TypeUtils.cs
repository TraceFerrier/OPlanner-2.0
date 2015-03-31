using System;

namespace PlannerNameSpace
{
    public static class TypeUtils
    {
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns an integer representation of the given value.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static int GetIntValue(object value)
        {
            if (value == null)
            {
                return 0;
            }

            if (value.GetType() == typeof(int))
            {
                return (int)value;
            }

            if (value.GetType() == typeof(string))
            {
                int intValue;
                if (Int32.TryParse(value as string, out intValue))
                {
                    return intValue;
                }
            }

            return 0;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns an integer representation of the given value.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static long GetLongValue(object value)
        {
            if (value == null)
            {
                return 0;
            }

            if (value.GetType() == typeof(long))
            {
                return (long)value;
            }

            if (value.GetType() == typeof(string))
            {
                long longValue;
                if (Int64.TryParse(value as string, out longValue))
                {
                    return longValue;
                }
            }

            return 0;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a double representation of the given value.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static double GetDoubleValue(object value)
        {
            if (value == null)
            {
                return 0;
            }

            if (value.GetType() == typeof(double))
            {
                return (double)value;
            }

            if (value.GetType() == typeof(string))
            {
                double doubleValue;
                if (Double.TryParse(value as string, out doubleValue))
                {
                    return doubleValue;
                }
            }

            return 0;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the boolean value of the specified field for this item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static bool GetBoolValue(object value)
        {
            if (value == null)
            {
                return false;
            }

            string strValue = GetStringValue(value);
            if (string.IsNullOrWhiteSpace(strValue))
            {
                return false;
            }

            bool boolValue;
            if (!Boolean.TryParse(strValue, out boolValue))
            {
                return false;
            }

            return boolValue;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a string representation of the given value.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static string GetStringValue(object value)
        {
            if (value == null)
            {
                return null;
            }

            return value.ToString();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a date representation of the given value.  If the value can't be converted
        /// to a valid DateTime object, a default DateTime object will be returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static DateTime GetDateTimeValue(object value)
        {
            DateTime? datetime = GetNullableDateTimeValue(value);
            if (datetime == null)
            {
                return default(DateTime);
            }

            return datetime.Value;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a date representation of the given value.  If the value can't be converted
        /// to a valid DateTime object, null will be returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static Nullable<DateTime> GetNullableDateTimeValue(object value)
        {
            if (value == null)
            {
                return null;
            }

            // If the given value is already a DateTime, we're good to go.
            if (value.GetType() == typeof(DateTime))
            {
                return (DateTime)value;
            }

            if (value.GetType() == typeof(string))
            {
                DateTime dt;
                if (DateTime.TryParse(value as string, out dt))
                {
                    return dt;
                }
            }

            return null;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the given value in local time format.  If the value is already 
        /// local, the format will be unchanged.
        /// </summary>
        /// 
        /// <remarks>
        /// Assumes that if the DateTime kind is unspecified, the current format is
        /// universal.
        /// </remarks>
        //------------------------------------------------------------------------------------
        public static DateTime GetValueAsLocalTime(object value)
        {
            DateTime date = (DateTime)value;
            if (date.Kind == DateTimeKind.Utc || date.Kind == DateTimeKind.Unspecified)
            {
                return DateTime.SpecifyKind(date.ToLocalTime(), DateTimeKind.Local);
            }

            return date;
        }


    }
}
