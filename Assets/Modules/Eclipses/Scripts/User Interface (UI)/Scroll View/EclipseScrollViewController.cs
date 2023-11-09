using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Module.Eclipses
{
    public class EclipseScrollViewController : MonoBehaviour
    {
        //this will hold the rows of eclipses
        [SerializeField] private GameObject _contentView;
        //the prefab row used to populate the _contentView
        [SerializeField] private GameObject _row;
        //used to get eclipses
        [SerializeField] private EclipseDataController eclipseDataController;
        //used to get the poistion which the user chooses on the screen
        [SerializeField] private ChosenPositionOnEarth chosenEclipsePosition;

        //this array holds all the eclipses found while searching by position on Earth
        public static List<EclipseDataController.EclipseInfo> foundEclipses { get; private set; }

        //this message will be shown every time the user wants to find eclipses
        //for a location on Earth where no eclipses have happened or will happen
        [SerializeField] private GameObject _noEclipsesFoundMessage;

        //used to close this panel if it's open when the scroll view appears
        [SerializeField] private GameObject _optionsPanel;

        private void OnEnable()
        {
            //getting all the eclipses by position
            foundEclipses = GetFoundEclipses();
            //Showing those eclipses in _contentView
            PopulateContentViewWithEclipseInfo();

            //closing the options panel if it is open so that it does not block the scroll view
            CloseOptionsPanelIfItIsOpen();
        }

        private void OnDisable()
        {
            //resetting the array
            foundEclipses = null;

            //deleting all the eclipse info rows from _contentView so that they 
            //do not add up every time the view is Enabled
            UnpopulateContentView();
        }

        //this method deletes all the rows except for the first one from
        //the _contentView. The first one stays because it's the headers
        private void UnpopulateContentView()
        {
            foreach (Transform _child in _contentView.transform)
            {
                //if the child object is not the row of HEADERS which is at index 0
                if (_child.GetSiblingIndex() != 0)
                {
                    Destroy(_child.gameObject);
                }
            }
        }

        //this method Adds all the eclipse info rows onto the _contentView
        private void PopulateContentViewWithEclipseInfo()
        {
            //if eclipses were found
            if (foundEclipses.Count > 0)
            {
                //creating as many rows as there are eclipses found
                for (int i = 0; i < foundEclipses.Count; i++)
                {
                    //referencing a row prefab to later instantiate it
                    var _rowToInstantiate = _row;

                    //getting the child count to correctly set up texts
                    int childCount = _rowToInstantiate.gameObject.transform.childCount;

                    for (int j = 0; j < childCount; j++)
                    {
                        //setting up texts correctly using text gameObjects' Tags
                        switch (_rowToInstantiate.gameObject.transform.GetChild(j).transform.GetChild(0).tag)
                        {
                            case "rowIndexText":
                                _rowToInstantiate.gameObject.transform.GetChild(j).transform.GetChild(0).GetComponent<Text>().text = $"{i + 1}";
                                break;
                            case "rowDateText":
                                _rowToInstantiate.gameObject.transform.GetChild(j).transform.GetChild(0).GetComponent<Text>().text = $"{foundEclipses[i].year} {foundEclipses[i].month} {foundEclipses[i].day}";
                                break;
                            case "rowEclipseText":
                                _rowToInstantiate.gameObject.transform.GetChild(j).transform.GetChild(0).GetComponent<Text>().text = $"{foundEclipses[i].eclipse}";
                                break;
                            case "rowLatitudeText":
                                _rowToInstantiate.gameObject.transform.GetChild(j).transform.GetChild(0).GetComponent<Text>().text = $"{foundEclipses[i].latitude}";
                                break;
                            case "rowLongitudeText":
                                _rowToInstantiate.gameObject.transform.GetChild(j).transform.GetChild(0).GetComponent<Text>().text = $"{foundEclipses[i].longitude}";
                                break;
                            case "rowTypeText":
                                _rowToInstantiate.gameObject.transform.GetChild(j).transform.GetChild(0).GetComponent<Text>().text = foundEclipses[i].type == "T" ? "Total" : foundEclipses[i].type == "P"
                                    ? "Partial" : foundEclipses[i].type == "A" ? "Annular" : foundEclipses[i].type == "Pb" ? "Partial" : foundEclipses[i].type == "Pe"
                                    ? "Partial" : foundEclipses[i].type == "Am" ? "Annular" : foundEclipses[i].type == "Tm" ? "Total" : foundEclipses[i].type == "An"
                                    ? "Annular" : foundEclipses[i].type == "A-" ? "Annular" : foundEclipses[i].type == "A+" ? "Annular" : foundEclipses[i].type == "H"
                                    ? "Partial" : foundEclipses[i].type == "As" ? "Annular" : foundEclipses[i].type == "T+" ? "Total" : foundEclipses[i].type == "T-"
                                    ? "Total" : foundEclipses[i].type == "H3" ? "Partial" : foundEclipses[i].type == "N" ? "Penumbral" : foundEclipses[i].type == "Nb"
                                    ? "Penumbral" : foundEclipses[i].type == "Ne" ? "Penumbral" : foundEclipses[i].type == "Nx"
                                    ? "Penumbral" : foundEclipses[i].type;
                                ;
                                break;
                            case "rowWidthText":
                                //if there is info about width
                                if (!string.IsNullOrEmpty(foundEclipses[i].width))
                                {
                                    _rowToInstantiate.gameObject.transform.GetChild(j).transform.GetChild(0).GetComponent<Text>().text = $"{foundEclipses[i].width}";
                                }
                                //if there is no info about width. just showing "-"
                                else
                                {
                                    _rowToInstantiate.gameObject.transform.GetChild(j).transform.GetChild(0).GetComponent<Text>().text = $" - ";
                                }
                                break;
                            case "rowDurationText":
                                //if there is info about time
                                if (!string.IsNullOrEmpty(foundEclipses[i].time))
                                {
                                    _rowToInstantiate.gameObject.transform.GetChild(j).transform.GetChild(0).GetComponent<Text>().text = $"{foundEclipses[i].time}";
                                }
                                //if there is no info about time. just showing "-"
                                else
                                {
                                    _rowToInstantiate.gameObject.transform.GetChild(j).transform.GetChild(0).GetComponent<Text>().text = $" - ";
                                }
                                break;
                            default:
                                Debug.Log("Sth went wrong, Bruh");
                                break;
                        }
                    }

                    //instantiating a new row
                    var _newRow = Instantiate(_rowToInstantiate, _contentView.transform.position, Quaternion.identity);
                    //setting _contentView as the parent of the newly instantiated row
                    //because all the rows must be on the _contentView
                    _newRow.transform.SetParent(_contentView.transform);
                    //stretching the row to fill the whole scroll view
                    _newRow.transform.localScale = new Vector3(1, 1, 1);
                }
            }
            //if there were no eclipses found
            else
            {
                //instantiating the warning message
                var _noEclipsesFoundText = Instantiate(_noEclipsesFoundMessage, _contentView.transform.position, Quaternion.identity);
                //setting it as the Scroll View's child
                _noEclipsesFoundText.transform.SetParent(_contentView.transform);
            }
        }

        //this method gets the Eclipses by position and returns them in an array
        private List<EclipseDataController.EclipseInfo> GetFoundEclipses()
        {
            List<EclipseDataController.EclipseInfo> foundEclipsesList = eclipseDataController.SearchForEclipse(listOfEclipses: eclipseDataController.eclipseInfoList, latitude: chosenEclipsePosition.chosenLatitude, longitude: chosenEclipsePosition.chosenLongitude);
            return foundEclipsesList;
        }

        //hides the scroll view
        public void HideEclipseScrollView()
        {
            gameObject.SetActive(false);
        }

        //close the options panel if it is open
        private void CloseOptionsPanelIfItIsOpen()
        {
            if (_optionsPanel.activeSelf == true)
            {
                _optionsPanel.SetActive(false);
            }
        }
    }
}
