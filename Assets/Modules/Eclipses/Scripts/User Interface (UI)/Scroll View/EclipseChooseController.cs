using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Eclipses
{
    public class EclipseChooseController : MonoBehaviour
    {
        //will be used to save the choosen eclipse
        public EclipseDataController.EclipseInfo choosenEclipse { get; private set; }
        //will be used to trigger space setup behavior which will move bodies to
        //positions where they should be during the selected eclipse
        [SerializeField] private SetupViewAfterEclipseChosen setupViewAfterEclipseChosen;
        [SerializeField] private MoonEclipseSetup moonEclipseSetup;
        [SerializeField] private DateController dateController;

        //called from inside the editor under the OnClick event of the "Row" Prefab
        public void EclipseChooser(int indexOfRowClicked)
        {

            //saving the choosen eclipse
            choosenEclipse = EclipseScrollViewController.foundEclipses[indexOfRowClicked - 1];
            int year = int.Parse(choosenEclipse.year);
            dateController.year = year;
            dateController.eclipseyear = true;
            moonEclipseSetup.eclipseType = choosenEclipse.eclipse;
            //Debug.Log($"{choosenEclipse.latitude} {choosenEclipse.longitude}");

            //setting up Space to position bodies where they should be during the eclipse
            setupViewAfterEclipseChosen.StartSettingUpSceneToSimulateEclipse();
        }
        public void SimulatedReallyYear ()
        {
            dateController.year = 2019;
        }
    }
}
