using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Module.Eclipses
{
    public class DateTextController : MonoBehaviour
    {
        //will access info about the date from this controller
        [SerializeField] private DateController dateController;
        //the text to show the current date
        private Text _dateText;

        [SerializeField] private SetupViewAfterEclipseChosen setupViewAfterEclipseChosen;

        private void Awake()
        {
            _dateText = GetComponent<Text>();
        }

        void Update()
        {
            //updating Date Text
            UpdateDateText();
        }

        //this method updates the date text based on the date it gets from
        // inside the date controller
        private void UpdateDateText()
        {
            _dateText.text = $"{dateController.currentDate.month} {dateController.currentDate.day} {dateController.currentDate.year}".ToUpper();
        }
    }
}
