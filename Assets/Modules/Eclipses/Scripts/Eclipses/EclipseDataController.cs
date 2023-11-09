using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;

namespace Module.Eclipses
{
    public class EclipseDataController : MonoBehaviour
    {
        //this list will hold information about all the eclipses
        //for 1000 years(it will read that information from a DataBase
        //file imported from NASA's websites)
        public List<EclipseInfo> eclipseInfoList = new List<EclipseInfo>();
        //this is how much the Search will be scattered when the user
        //wants to find an eclipse for a specific location
        private const float halfRange = 5f;

        public void Awake()
        {
            //reading all the eclipse info from the NASA file
            eclipseInfoList = ReadCSVFile();
        }

        //this method reads all the lines in the eclipse DataBase file
        //and adds those eclipses into a List so that later that list 
        //can be used to search for specific eclipses
        private List<EclipseInfo> ReadCSVFile()
        {
            //getting the DB file from its directory
            StreamReader strReader = new StreamReader(Application.streamingAssetsPath + "/DataSolarEclipse.csv");
            //this Boolean value will help determine whether the DB file has been read all the way through
            bool endOfFile = false;
            //this list will be returned at the end of this method. It will contain all the eclipse
            //info contained in the DB file
            List<EclipseInfo> eclipseInformation = new List<EclipseInfo>();

            //defining how the values should be parsed
            NumberFormatInfo format = new NumberFormatInfo();
            format.NumberGroupSeparator = ",";
            format.NumberDecimalSeparator = ".";
            //

            //while the end of the file has not been reached info
            //will continue to be extracted from inside the DB file
            while (!endOfFile)
            {
                //reads the consecutive line in the DB file
                string data_String = strReader.ReadLine();
                //if there are no more lines then it means that the end
                //of the file has been reached and that all the info has been extracted
                if (data_String == null)
                {
                    //signaling about the end of the file and finishing operations
                    endOfFile = true;
                    break;
                }
                //columns in the DB file are separated by semicolons(;)
                //this is how they will be split
                var data_values = data_String.Split(';');

                //the reason why these two field are being modified below before being added to 
                //the list is because they contain characters which need to be converted into
                //a more usable format to be used later in the app
                //namely, Latitude and Longitude values contain the letters W,E,N,S which mean
                //West, East, North, and South respectively. These indicators are being 
                //replaced with a negative sign(-)
                float latitudeHistorical;
                float longitudeHistorical;
                //S means South, it is replaced below with -
                if (data_values[4].Contains("S"))
                {
                    //removing the Letter at the end of the value
                    data_values[4] = data_values[4].Remove(data_values[4].Length - 1);
                    //parsing the value into Float using the corresponding format defined above
                    latitudeHistorical = float.Parse(data_values[4], format);
                    //making it negative to simulate South
                    latitudeHistorical *= -1;
                }
                //Below N is North, which is + and should NOT be replaced with -
                else
                {
                    //removing the Letter at the end of the value
                    data_values[4] = data_values[4].Remove(data_values[4].Length - 1);
                    //parsing the value into Float using the corresponding format defined above
                    latitudeHistorical = float.Parse(data_values[4], format);
                }
                //W means West, it is replaced below with -
                if (data_values[5].Contains("W"))
                {
                    //removing the Letter at the end of the value
                    data_values[5] = data_values[5].Remove(data_values[5].Length - 1);
                    //parsing the value into Float using the corresponding format defined above
                    longitudeHistorical = float.Parse(data_values[5], format);
                    //making it negative to simulate West
                    longitudeHistorical *= -1;
                }
                //Below E is East, which is + and should NOT be replaced with -
                else
                {
                    //removing the Letter at the end of the value
                    data_values[5] = data_values[5].Remove(data_values[5].Length - 1);
                    //parsing the value into Float using the corresponding format defined above
                    longitudeHistorical = float.Parse(data_values[5], format);
                }

                //creating an eclipse info object which holds information about the newly
                //parsed eclipse and slightly modified Latitude and Longitude values
                EclipseInfo eclipse = new EclipseInfo(year: data_values[0].ToString(),
                month: data_values[1].ToString(), day: data_values[2].ToString(),
                type: data_values[3].ToString(), latitude: latitudeHistorical,
                longitude: longitudeHistorical, width: data_values[6].ToString(),
                time: data_values[7].ToString(), eclipse: data_values[8]);

                //adding the Eclipse Info Object to the Array of all eclipses
                eclipseInformation.Add(eclipse);
            }

            //returns the array which contains all the eclipses
            return eclipseInformation;
        }

        //example how to use this method
        /*
            var a = SearchForEclipse(-40, -40);
            for (int i = 0; i < a.Count; i++)
            {
                Debug.Log(a[i].latitude + "      " + a[i].longitude);
            }
        */
        //this method takes two parameters in the form of Latitude and Longitude
        //and returns an Array containing all the eclipses whose positions coincide 
        //with the passed values. The method searches for eclipses in a range of values
        //because there are so many positions on Earth that the probability of finding an 
        //eclipse at a specific point is very low
        //INSTEAD the method finds eclipses which are more or less close to the chosen
        //position
        //this range can be changes by modifying the value of the constant "halfRange"
        public List<EclipseInfo> SearchForEclipse(List<EclipseInfo> listOfEclipses, float latitude, float longitude)
        {
            //this list will hold all the eclipses which are close to the chosen point on Earth
            List<EclipseInfo> foundElipses = new List<EclipseInfo>();

            //the code below finds eclipses in the specified range which depennds on the
            //position chosen on Earth
            if (latitude < 0)
            {
                //multiplying the value by -1 to make it easier to check whether the eclipse's
                //position is within the range. It's being done for the sake of the calculations' convenience
                latitude *= -1;

                for (int i = 0; i < listOfEclipses.Count; i++)
                {
                    //checking whether the current eclipse being considered is withing the range
                    if (listOfEclipses[i].latitude + latitude < halfRange && listOfEclipses[i].latitude + latitude > -halfRange)
                    {
                        if (longitude < 0)
                        {
                            //checking whether the current eclipse being considered is withing the range
                            if (listOfEclipses[i].longitude + longitude * -1 < halfRange && listOfEclipses[i].longitude + longitude * -1 > -halfRange)
                            {
                                //here an eclipse is found whose positions
                                //approximately coincide with the Latitude and Longitude
                                //values chosen on the surface of Earth
                                //Adding this eclipse to the array
                                foundElipses.Add(listOfEclipses[i]);
                            }
                        }
                        else
                        {
                            //checking whether the current eclipse being considered is withing the range
                            if (listOfEclipses[i].longitude - longitude > -halfRange && listOfEclipses[i].longitude - longitude < halfRange)
                            {
                                //here an eclipse is found whose positions
                                //approximately coincide with the Latitude and Longitude
                                //values chosen on the surface of Earth
                                //Adding this eclipse to the array
                                foundElipses.Add(listOfEclipses[i]);
                            }
                        }
                    }
                }
            }
            else
            {
                for (int j = 0; j < listOfEclipses.Count; j++)
                {
                    //checking whether the current eclipse being considered is withing the range
                    if (listOfEclipses[j].latitude - latitude > -halfRange && listOfEclipses[j].latitude - latitude < halfRange)
                    {
                        if (longitude < 0)
                        {
                            //checking whether the current eclipse being considered is withing the range
                            if (listOfEclipses[j].longitude + longitude * -1 < halfRange && listOfEclipses[j].longitude + longitude * -1 > -halfRange)
                            {
                                //here an eclipse is found whose positions
                                //approximately coincide with the Latitude and Longitude
                                //values chosen on the surface of Earth
                                //Adding this eclipse to the array
                                foundElipses.Add(listOfEclipses[j]);
                            }
                        }
                        else
                        {
                            //checking whether the current eclipse being considered is withing the range
                            if (listOfEclipses[j].longitude - longitude > -halfRange && listOfEclipses[j].longitude - longitude < halfRange)
                            {
                                //here an eclipse is found whose positions
                                //approximately coincide with the Latitude and Longitude
                                //values chosen on the surface of Earth
                                //Adding this eclipse to the array
                                foundElipses.Add(listOfEclipses[j]);
                            }
                        }
                    }
                }
            }
            //

            //returning the found eclipses at the specified position
            return foundElipses;
        }

        //this structure is used to store all the information that is 
        //being extracted from the DataBase file
        public class EclipseInfo
        {
            public string year;
            public string month;
            public string day;
            public string eclipse;
            public string type;
            public float latitude;
            public float longitude;
            public string width;
            public string time;

            public EclipseInfo(string year, string month, string day, string type, float latitude, float longitude, string width, string time, string eclipse)
            {
                this.year = year;
                this.month = month;
                this.day = day;
                this.eclipse = eclipse;
                this.type = type;
                this.latitude = latitude;
                this.longitude = longitude;
                this.width = width;
                this.time = time;
            }
        }
    }
}