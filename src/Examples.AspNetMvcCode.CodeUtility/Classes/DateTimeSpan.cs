using System;

namespace Examples.AspNetMvcCode.CodeUtility
{
    /// <summary>
    /// this class allows to compare DateTime values
    /// returning the difference in years, months, days, etc units.
    /// Especially useful to represent countdowns or expiration
    /// </summary>
    public struct DateTimeSpan : IEquatable<DateTimeSpan>
    {
        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="years"></param>
        /// <param name="months"></param>
        /// <param name="days"></param>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        /// <param name="milliseconds"></param>
        /// <param name="isNegative"></param>
        /// <param name="datePartIsEqual"></param>
        public DateTimeSpan(
            int years
            , int months
            , int days
            , int hours
            , int minutes
            , int seconds
            , int milliseconds
            , bool isNegative
            , bool datePartIsEqual
            )
        {
            this.Years = years;
            this.Months = months;
            this.Days = days;
            this.Hours = hours;
            this.Minutes = minutes;
            this.Seconds = seconds;
            this.Milliseconds = milliseconds;
            this.IsNegative = isNegative;
            this.DatePartIsEqual = datePartIsEqual;
        }

        /// <summary>
        /// Years in instance
        /// </summary>
        public int Years { get; }
        /// <summary>
        /// Months in instance
        /// </summary>
        public int Months { get; }
        /// <summary>
        /// Days in instance
        /// </summary>
        public int Days { get; }
        /// <summary>
        /// Hours in instance
        /// </summary>
        public int Hours { get; }
        /// <summary>
        /// Minutes in instance
        /// </summary>
        public int Minutes { get; }
        /// <summary>
        /// Seconds in instance
        /// </summary>
        public int Seconds { get; }
        /// <summary>
        /// Milliseconds in instance
        /// </summary>
        public int Milliseconds { get; }
        /// <summary>
        /// sign of instance
        /// </summary>
        public bool IsNegative { get; }

        /// <summary>
        /// After being used in comparison between two datetime, returns true if years, months and days were equal
        /// </summary>
        public bool DatePartIsEqual { get; }

        enum Phase { Years, Months, Days, Done }

        /// <summary>
        /// Compares two dates and returns the difference in <see cref="DateTimeSpan"/> interval
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public static DateTimeSpan CompareDates(DateTime date1, DateTime date2)
        {
            bool localIsNegative = false;
            bool datePartIsEqual = false;
            if (date2.Date < date1.Date)
            {
                (date2, date1) = (date1, date2);
                localIsNegative = true;
            }
            if (date1.Day == date2.Day
                && date1.Month == date2.Month
                && date1.Year == date2.Year)
            {
                datePartIsEqual = true;
            }

            DateTime current = date1;
            int years = 0;
            int months = 0;
            int days = 0;

            Phase phase = Phase.Years;
            DateTimeSpan span = new DateTimeSpan();

            while (phase != Phase.Done)
            {
                switch (phase)
                {
                    case Phase.Years:
                        if (current.AddYears(years + 1) > date2)
                        {
                            phase = Phase.Months;
                            current = current.AddYears(years);
                        }
                        else
                        {
                            years++;
                        }
                        break;

                    case Phase.Months:
                        if (current.AddMonths(months + 1) > date2)
                        {
                            phase = Phase.Days;
                            current = current.AddMonths(months);
                        }
                        else
                        {
                            months++;
                        }
                        break;

                    case Phase.Days:
                        if (current.AddDays(days + 1) > date2)
                        {
                            current = current.AddDays(days);
                            TimeSpan timespan = date2 - current;
                            span = new DateTimeSpan(
                                years
                                , months
                                , days
                                , timespan.Hours
                                , timespan.Minutes
                                , timespan.Seconds
                                , timespan.Milliseconds
                                , localIsNegative
                                , datePartIsEqual
                                );
                            phase = Phase.Done;
                        }
                        else
                        {
                            days++;
                        }
                        break;
                }//end switch
            }//end while

            return span;
        }

        /// <summary>
        /// Exact comparison between this instance to another <see cref="DateTimeSpan"/>. 
        /// Uses only date time parts of the instances
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(DateTimeSpan other)
        {
            return Years.Equals(other.Years)
                       && Months.Equals(other.Months)
                       && Days.Equals(other.Days)
                       && Hours.Equals(other.Hours)
                       && Minutes.Equals(other.Minutes)
                       && Seconds.Equals(other.Seconds)
                       && Months.Equals(other.Months)
                       && Milliseconds.Equals(other.Milliseconds);
        }

        /// <summary>
        /// Implementation of <see cref="IEquatable{T}"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (obj is DateTimeSpan other)
            {
                return Equals(other);
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Override for equality comparator
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static bool operator ==(DateTimeSpan obj1, DateTimeSpan obj2)
        {
            return obj1.Equals(obj2);
        }

        /// <summary>
        /// Override for difference comparator
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static bool operator !=(DateTimeSpan obj1, DateTimeSpan obj2)
        {
            return !(obj1.Equals(obj2));
        }

        /// <summary>
        /// override of base object method 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Years.GetHashCode();
                hashCode = (hashCode * 397) ^ Months.GetHashCode();
                hashCode = (hashCode * 397) ^ Days.GetHashCode();
                hashCode = (hashCode * 397) ^ Hours.GetHashCode();
                hashCode = (hashCode * 397) ^ Minutes.GetHashCode();
                hashCode = (hashCode * 397) ^ Seconds.GetHashCode();
                hashCode = (hashCode * 397) ^ Months.GetHashCode();
                hashCode = (hashCode * 397) ^ Milliseconds.GetHashCode();
                return hashCode;
            }
        }
    }
}