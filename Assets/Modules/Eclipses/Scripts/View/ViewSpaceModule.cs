using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Eclipses
{
    public class ViewSpaceModule : SpaceModuleElement
    {
        //the planet Earth, its moon, and the Light(Earth and Moon GameObject in Scene)
        public PlanetsView _planetsView;

        //planet Earth(child Earth)
        public EarthView _earthView;

        //moon (parent Moon)
        public MoonView _moonView;

        public ViewSpaceModule(PlanetsView planetsView, EarthView earthView, MoonView moonView)
        {
            _planetsView = planetsView;
            _earthView = earthView;
            _moonView = moonView;
        }
    }
}
