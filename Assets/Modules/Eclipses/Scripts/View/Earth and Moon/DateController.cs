using UnityEngine;
using System;

namespace Module.Eclipses
{
    public class DateController : MonoBehaviour
    {
        //172 is the 21th of June(Summer Solstice)
        //it's where the simulation starts in the game
        public static int startDay = 172;

        //will hold the number of day of the Year
        private int dayNumber;

        //the precise number of day of the year
        public static float dayNumber_precise { get; private set; }

        //used to calculate the date
        private int monthNumber = 0;
        private string month;
        private int day = 0;
        [HideInInspector]
        public int year = 2019;

        private bool nextyear = true;
        public bool eclipseyear = false;
        //the count of days in each of the 12 months in the year
        private int[] monthLengthArray = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        //names of all the months
        private string[] monthNameArray = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

        //the current date in space. Is constantly Updated inside the Update Function
        public Date currentDate { get; private set; }

        //used to check whether Space is Manually simulating an eclipse
        [SerializeField] private SetupViewAfterEclipseChosen setupViewAfterEclipseChosen;
        //the Manual Simulation Controller which will inform this script whether
        //space is being Manually Simulated or not
        //[SerializeField] private ManualSimulationController manualSimulationController;

        void Update()
        {
            //date should be updated only during Automatic Simulation, during setting up an eclipse
            //or when space is being Manually SImulated using the Backward and Forward buttons ( || manualSimulationController.isSpaceBeingManuallySimulated == true )
            if (!SimulationMode.IsGameModeManual() || setupViewAfterEclipseChosen.shouldSetupSpaceToSimulateEclipse)
            {
                ////Debug.Log($"{CalculateDayNumber()}th Day of the Year");
                //var date = CalculateDateBasedOnDayNumber();
                //Debug.Log($"{date.day} of {date.month}");

                //getting the current date
                UpdateCurrentDate();
            }

        }

        //updating the date
        private void UpdateCurrentDate()
        {
            currentDate = CalculateDateBasedOnDayNumber();
        }

        //this method returns the n-th day of the Year.
        //getPreciseValue should be set to True only when one needs to use a precise
        //number of day to count the angle around the Sun

        public float CalculateDayNumberBasedOnAngleAroundSun(bool getPreciseValue)
        {
            //will be used to calculate the angle around the Sun
            dayNumber_precise = (EarthAngleAroundSun.earthAngleAroundSun / 0.98561009f) + startDay;
            //the Day of The Year
            dayNumber = (int)((int)EarthAngleAroundSun.earthAngleAroundSun / 0.98561009f) + startDay;

            if (dayNumber > 365)
            {
                dayNumber = dayNumber - 365;
            }
            //is a precise value needed?
            if (getPreciseValue == false)
            {
                return dayNumber;
            }
            //returning the rounded value rather than a precise one
            return dayNumber_precise;
        }

        //this method returns the current Date based on the Day Number of the Year
        public Date CalculateDateBasedOnDayNumber()
        {
            //getting the current Day of the Year
            int dayNumberOfYear = (int)CalculateDayNumberBasedOnAngleAroundSun(getPreciseValue: false);
            Date date;
            int numbOfDaysTemp = 1;

            for (int i = 0; i < monthLengthArray.Length; i++)
            {
                numbOfDaysTemp += monthLengthArray[i];
                if (numbOfDaysTemp > dayNumberOfYear)
                {
                    //adding 1 to make 0-11 1-12
                    monthNumber = i + 1;
                    //getting the Month's name based on its index
                    month = monthNameArray[i];
                    //Debug.Log($"Month {month} , {monthNumber}");
                    //calculating the Day of Monthe
                    day = monthLengthArray[i] - (numbOfDaysTemp - dayNumberOfYear) + 1;
                    if(day == 1 && monthNameArray[i] == "January" && nextyear && !eclipseyear)
                    {
                        year++;
                        nextyear = false;
                    }
                    else
                    {
                        nextyear = true;

                    }
                    //Debug.Log($"Day {day}");

                    date = new Date(month: month, day: day, year: year);
                    return date;
                }
            }

            date = new Date(month: month, day: day, year: year);
            return date;
        }
        public void ChoosenEclipseAndNeedChangeYear (int year)
        {
            eclipseyear = true;
            this.year = year;
        }
        public void EclipseYearForFalse ()
        {
            eclipseyear = false;
        }
        //takes a Date and Calculates the Number Of Day Of The Year
        //for example, June 21th is the 172nd day of the year
        public int CalculateDayNumberBasedOnDate(Date date)
        {
            //this will hold the final number of Days that have passed since the 
            //beginning of the year
            int numbOfDays = 0;

            //gets the index of the Month from the Array which contains all Month Names
            int indexOfMonth = Array.IndexOf(monthNameArray, date.month);

            //total number of days of all months that have already passed COMPLETELY
            for (int j = 0; j < indexOfMonth; j++)
            {
                numbOfDays += monthLengthArray[j];
            }

            //adding the Day of the Month to get the final Day Number
            numbOfDays += date.day;

            return numbOfDays;
        }
    }

    public class Months
    {
        public const string January = "January";
        public const string February = "February";
        public const string March = "March";
        public const string April = "April";
        public const string May = "May";
        public const string June = "June";
        public const string July = "July";
        public const string August = "August";
        public const string September = "September";
        public const string October = "October";
        public const string November = "November";
        public const string December = "December";
    }

    //the structure in which the Date is returned
    public class Date
    {
        public string month;
        public int day;
        public int year;

        public Date(string month, int day, int year)
        {
            this.month = month;
            this.day = day;
            this.year = year;
        }
    }
}



