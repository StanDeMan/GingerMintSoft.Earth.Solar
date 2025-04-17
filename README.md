# Calculation of solar energy system earnings

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


Calculated yield of an east-west roof on 21.06. (yearly maximun) - data processed in Excel.


   ![image](https://github.com/user-attachments/assets/6b5fbc66-15e9-4159-ab83-48aa20c79371)

Configuration of the pv plant:
```json
{
  "Powerplants": [
    {
      "PlantId": "WxHCombjzTaw",
      "name": "PV Anlage Zuhause",
      "altitude": 232,
      "latitude": 48.0,
      "longitude": 7.91,
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
Calculated yield of an east-west roof on 6th of April 2025 - data shown in Chart.js.
![Screenshot 2025-04-06 122528](https://github.com/user-attachments/assets/d0b341f6-ec03-4732-b3f3-e98e363b47e2)

Calculated yield of an east roof on 6th of April 2025 - data shown in Chart.js.
![Screenshot 2025-04-06 121959](https://github.com/user-attachments/assets/09d3a1c2-a20a-428b-9dce-c070cdc0a267)

Calculated yield of an west roof on 6th of April 2025 - data shown in Chart.js.
![Screenshot 2025-04-06 122203](https://github.com/user-attachments/assets/f9edf6ee-8118-450f-bb81-ace19cc0a528)

Calculated yield of an east-west roof on 6th of April 2025 as envelope - data shown in Chart.js.
![Screenshot 2025-04-06 122432](https://github.com/user-attachments/assets/2a13ef53-d251-49fe-90a4-9c1f6d4011eb)

Calculated yield of an east-west roof on 6th of April 2025 from both roofs that get added to get the graph from above - data shown in Chart.js.
![Screenshot 2025-04-06 122323](https://github.com/user-attachments/assets/13273f2f-460d-4e25-8ba0-5a94930e3891)

Calculated earnings (kWh) of an east-west roof at my location on 18th of April 2025
![2025 04 18 EastAndWest-Roof-EnergyDay](https://github.com/user-attachments/assets/385b8ca7-4a74-4670-87b0-e7240dafee03)


