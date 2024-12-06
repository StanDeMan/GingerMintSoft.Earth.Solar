using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GingerMintSoft.Earth.Solar.PowerPlant
{
    public class Roof
    {
        public double Tilt { get; set; }
        public double Azimuth { get; set; }

        //private readonly double _roofTilt = 45; // Neigungswinkel des Dachs in Grad
        //private readonly double _roofAzimuthEast = 90; // Ausrichtung des Dachs in Grad (90° = Ost)
        //private readonly double _roofAzimuthWest = 270; // Ausrichtung des Dachs in Grad (270° = West)
    }
}
