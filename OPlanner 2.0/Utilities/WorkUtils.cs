using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace PlannerNameSpace
{
    public class WorkUtils
    {
        public const int WorkHoursPerDay = 8;
        public static Dictionary<string, DateTime> Holidays;

        static WorkUtils()
        {
            CompileHolidays();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// GetNetWorkingDays
        /// 
        /// </summary>
        //------------------------------------------------------------------------------------
        public static int GetNetWorkingDays(DateTime startDate, DateTime endDate)
        {
            return GetNetWorkingDays(startDate, endDate, new AsyncObservableCollection<OffTimeItem>());
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// GetNetWorkingDays
        /// 
        /// </summary>
        //------------------------------------------------------------------------------------
        public static int GetNetWorkingDays(DateTime startDate, DateTime endDate, AsyncObservableCollection<OffTimeItem> offTimeItems)
        {
            int netWorkingDays = 0;
            DateTime date = startDate;
            while (date <= endDate)
            {
                if (IsWorkDay(date))
                {
                    bool isOffTimeDate = false;
                    if (offTimeItems.Count > 0)
                    {
                        foreach (OffTimeItem offTimeItem in offTimeItems)
                        {
                            if (date >= offTimeItem.StartDate && date <= offTimeItem.EndDate)
                            {
                                isOffTimeDate = true;
                                break;
                            }
                        }
                    }

                    if (!isOffTimeDate)
                    {
                        netWorkingDays++;
                    }
                }

                date = date.AddDays(1);
            }

            return netWorkingDays;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// GetNetWorkingDays
        /// 
        /// </summary>
        //------------------------------------------------------------------------------------
        public static int GetTotalDays(DateTime startDate, DateTime endDate)
        {
            int totalDays = 0;
            DateTime date = startDate;
            while (date <= endDate)
            {
                totalDays++;
                date = date.AddDays(1);
            }

            return totalDays;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// GetNetWorkingDays
        /// 
        /// </summary>
        //------------------------------------------------------------------------------------
        public static int GetNetOffDays(DateTime startDate, DateTime endDate, AsyncObservableCollection<OffTimeItem> offTimeItems)
        {
            int netOffDays = 0;
            DateTime date = startDate;
            while (date <= endDate)
            {
                if (IsWorkDay(date))
                {
                    bool isOffTimeDate = false;

                    if (offTimeItems.Count > 0)
                    {
                        foreach (OffTimeItem offTimeItem in offTimeItems)
                        {
                            if (date >= offTimeItem.StartDate && date <= offTimeItem.EndDate)
                            {
                                isOffTimeDate = true;
                                break;
                            }
                        }
                    }

                    if (isOffTimeDate)
                    {
                        netOffDays++;
                    }
                }

                date = date.AddDays(1);
            }

            return netOffDays;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// GetNetWorkingHours - Returns the total number of working hours available between
        /// the two given days, taking into account weekends and company holidays.
        /// 
        /// </summary>
        //------------------------------------------------------------------------------------
        public static int GetNetWorkingHours(DateTime startDate, DateTime endDate)
        {
            return GetNetWorkingHours(startDate, endDate, new AsyncObservableCollection<OffTimeItem>());
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// GetNetWorkingHours - Returns the total number of working hours available between
        /// the two given days, taking into account weekends and company holidays.
        /// 
        /// </summary>
        //------------------------------------------------------------------------------------
        public static int GetNetWorkingHours(DateTime startDate, DateTime endDate, AsyncObservableCollection<OffTimeItem> offTimeItems)
        {
            int netWorkingDays = GetNetWorkingDays(startDate, endDate, offTimeItems);
            return netWorkingDays * WorkHoursPerDay;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Starting with the given date, returns the next date going forward in time that is
        /// a working day (if the given date *is* a working day, then that same date will be
        /// returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static DateTime GetNextWorkingDay(DateTime fromDate)
        {
            DateTime date = fromDate;
            for (; ; )
            {
                if (IsWorkDay(date))
                {
                    return date;
                }

                date = date.AddDays(1);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Starting with the given date, returns the next date going forward in time that is
        /// a working day (if the given date *is* a working day, then that same date will be
        /// returned.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static DateTime GetNextWorkingDay(DateTime fromDate, AsyncObservableCollection<OffTimeItem> offTimeItems)
        {
            DateTime date = fromDate;
            for (; ; )
            {
                if (IsWorkDay(date, offTimeItems))
                {
                    return date;
                }

                date = date.AddDays(1);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Worker function that sets the landing date for the given backlog item, based on
        /// the given starting date, and the given estimate of the number of working days to
        /// complete the job.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static DateTime AddWorkingDays(DateTime startingDate, int daysToAdd, AsyncObservableCollection<OffTimeItem> offTimeItems)
        {
            DateTime landingDate = startingDate;
            while (daysToAdd > 0)
            {
                landingDate = landingDate.AddDays(1);
                landingDate = GetNextWorkingDay(landingDate, offTimeItems);
                daysToAdd--;
            }

            return landingDate;
        }


        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if the given date lands on a company work day.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static bool IsWorkDay(DateTime date)
        {
            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday && !IsHoliday(date))
            {
                return true;
            }

            return false;
        }

        public static bool IsWorkDay(DateTime date, AsyncObservableCollection<OffTimeItem> offTimeItems)
        {
            if (IsWorkDay(date))
            {
                if (offTimeItems.Count > 0)
                {
                    foreach (OffTimeItem offTimeItem in offTimeItems)
                    {
                        if (date >= offTimeItem.StartDate && date <= offTimeItem.EndDate)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public static SolidColorBrush GetWorkCapacityFillColor(int workHoursRemaining, int workHoursAvailable)
        {
            const byte MaxRed = 235;
            const byte MaxGreen = 235;

            const double GreenThreshold = 0.70;
            const double YellowThreshold = 1.00;
            const double RedThreshold = 1.10;

            SolidColorBrush fillBrush = new SolidColorBrush();

            double remaining = workHoursRemaining;
            double available = workHoursAvailable;

            double fillPct = available <= 0 ? 2 : remaining / available;

            if (remaining == 0)
            {
                fillBrush.Color = Colors.Green;
            }
            else if (fillPct <= GreenThreshold)
            {
                fillBrush.Color = Color.FromRgb(0, MaxGreen, 0);
            }
            else if (fillPct < YellowThreshold)
            {
                int steps = (int)((YellowThreshold - GreenThreshold) * 100);
                int perStep = MaxRed / steps;
                byte redComponent = (byte)((fillPct - GreenThreshold) * 100 * perStep);
                fillBrush.Color = Color.FromRgb(redComponent, MaxGreen, 0);
            }
            else if (fillPct == YellowThreshold)
            {
                fillBrush.Color = Color.FromRgb(MaxRed, MaxGreen, 0);
            }
            else if (fillPct <= RedThreshold)
            {
                int steps = (int)((RedThreshold - YellowThreshold) * 100);
                int perStep = MaxGreen / steps;
                byte greenComponent = (byte)((RedThreshold - fillPct) * 100 * perStep);
                fillBrush.Color = Color.FromRgb(MaxRed, greenComponent, 0);
            }
            else
            {
                fillBrush.Color = Color.FromRgb(MaxRed, 0, 0);
            }

            return fillBrush;
        }

        public static bool IsHoliday(DateTime date)
        {
            return Holidays.ContainsKey(date.ToShortDateString());
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Compiles the list of MS holidays.
        /// 
        /// </summary>
        //------------------------------------------------------------------------------------
        private static void CompileHolidays()
        {
            if (Holidays == null)
            {
                Holidays = new Dictionary<string, DateTime>();

                // All company holidays for 2013
                AddHoliday(new DateTime(2013, 1, 1));
                AddHoliday(new DateTime(2013, 5, 27));
                AddHoliday(new DateTime(2013, 7, 4));
                AddHoliday(new DateTime(2013, 9, 2));
                AddHoliday(new DateTime(2013, 11, 28));
                AddHoliday(new DateTime(2013, 11, 29));
                AddHoliday(new DateTime(2013, 12, 24));
                AddHoliday(new DateTime(2013, 12, 25));

                // 2014
                AddHoliday(new DateTime(2014, 1, 1));
                AddHoliday(new DateTime(2014, 5, 26));
                AddHoliday(new DateTime(2014, 7, 4));
                AddHoliday(new DateTime(2014, 9, 1));
                AddHoliday(new DateTime(2014, 11, 27));
                AddHoliday(new DateTime(2014, 11, 28));
                AddHoliday(new DateTime(2014, 12, 24));
                AddHoliday(new DateTime(2014, 12, 25));
            }
        }

        private static void AddHoliday(DateTime holiday)
        {
            Holidays.Add(holiday.ToShortDateString(), holiday);
        }


    }
}
