using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Module.Eclipses
{
    public class ScrollViewRowController : MonoBehaviour
    {
        //the Row on the Scroll View
        private Button _rowButton;
        //used to choose and find the appropriate eclipse
        private EclipseChooseController eclipseChooseController;

        private void Awake()
        {
            //getting the button component from the Row
            _rowButton = gameObject.GetComponent<Button>();
            //adding an OnClick Listener to call a function
            _rowButton.onClick.AddListener(RowClickController);
            //getting the EclipseChooseController to Choose And Find an eclipse
            eclipseChooseController = GameObject.FindWithTag("controller").GetComponent<EclipseChooseController>();
            //getting the script to unclick the Position Choosing Button when an eclipse is chosen
            //_earthPositionChooserButton = GameObject.FindWithTag("EarthPositionChooserButton").GetComponent<EarthPositionChooserButton>();
        }

        //called when the row's OnClick Listener works
        //gets the index of the clicked row and finds the appropriate eclipse from the list of 
        //found eclipses using that index
        private void RowClickController()
        {
            //getting the index of the chosen eclipse in the scroll view
            int indexOfChoosenEclipse = gameObject.transform.GetSiblingIndex();
            //passing the index to the EclipseChooser to find and SAVE the needed eclipse
            eclipseChooseController.EclipseChooser(indexOfRowClicked: indexOfChoosenEclipse);

            //closing the scroll view containing info about
            //found eclipses after an item was clicked in the list
            CloseEclipseScrollView();

            //unclicking the Position Choosing Button
            //_earthPositionChooserButton.UnclickChoosePositionOnEarthButton();
        }

        //closes the scroll view after an eclipse is chosen from the list of found eclipses
        private void CloseEclipseScrollView()
        {
            //gameObject.transform.parent.gameObject.SetActive(false);
            GameObject.FindWithTag("EclipseScrollView").SetActive(false);
        }
    }
}
