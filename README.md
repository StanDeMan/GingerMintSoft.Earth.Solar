In order to calculate the yields of a PV system (photovoltaic e.g. with east-west orientation) to the minute, we need the following information:

1. Input data:

   Geocoordinates (latitude and longitude): For calculating the position of the sun.
    Installed power (kWp): The nominal power of the PV system.
    Inclination angle of the modules: For example, 30° (commonly used angle).
    Orientation (east-west): Modules point in both directions.
    Weather and atmospheric data:
        Direct and diffuse radiation (W/m²).
        Temperature (°C), as the efficiency is temperature-dependent.
    System losses: A general value of around 10-15 % (e.g. due to inverters, cabling).


2.  Steps for the calculation:

    Calculate the position of the sun: Based on geocoordinates, date and time (solar altitude and azimuth).
    Determine irradiation on modules:
        Convert global radiation (direct + diffuse) per minute to the module surfaces.
        Take into account the angle dependence of the irradiation (cosine rule).
    Temperature-dependent efficiency: Calculate the reduction in output due to high temperatures.
    Yield calculation:
        Yield=irradiation on modules×efficiency×installed powerYield=irradiation on modules×efficiency×installed power.
    Aggregation: Add up minute-by-minute values to the daily yield.


Calculated yield of an east-west roof on 21.06. - data processed in Excel.


   ![image](https://github.com/user-attachments/assets/6b5fbc66-15e9-4159-ab83-48aa20c79371)

Configuration of the pv plant:
```json
{
  "Powerplants": [
    {
      "PlantId": "WxHCombjzTaw",
      "name": "PV Anlage Zuhause",
      "altitude": 232,
      "latitude": 48.1051268096319,
      "longitude": 7.9085366169705145,
      "Roofs": [
        {
          "name": "Ostdach",
          "tilt": 43.0,
          "azimuth": 90.0,
          "azimuthDeviation": 15.0,
          "panels": {
            "panel": [
              {
                "name": "JA Solar JAM54S31 LR 420Wp Full Black",
                "area": 1.78,
                "efficiency": 0.21
              },
              {
                "name": "JA Solar JAM54S31 LR 420Wp Full Black",
                "area": 1.78,
                "efficiency": 0.21
              },
              {
                "name": "JA Solar JAM54S31 LR 420Wp Full Black",
                "area": 1.78,
                "efficiency": 0.21
              },
              {
                "name": "JA Solar JAM54S31 LR 420Wp Full Black",
                "area": 1.78,
                "efficiency": 0.21
              }
            ]
          }
        },
        {
          "name": "Westdach",
          "tilt": 43.0,
          "azimuth": 270.0,
          "azimuthDeviation": 15.0,
          "panels": {
            "panel": [
              {
                "name": "JA Solar JAM54S31 LR 420Wp Full Black",
                "area": 1.78,
                "efficiency": 0.21
              },
              {
                "name": "JA Solar JAM54S31 LR 420Wp Full Black",
                "area": 1.78,
                "efficiency": 0.21
              },
              {
                "name": "JA Solar JAM54S31 LR 420Wp Full Black",
                "area": 1.78,
                "efficiency": 0.21
              },
              {
                "name": "JA Solar JAM54S31 LR 420Wp Full Black",
                "area": 1.78,
                "efficiency": 0.21
              }
            ]
          }
        }
      ]
    }
  ]
}
```
