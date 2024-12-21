Um die Erträge einer PV-Anlage (Photovoltaik z.B. mit Ost-West-Ausrichtung) minutengenau zu berechnen, benötigen wir folgende Informationen:

1. Eingangsdaten:

    Geokoordinaten (Breite und Länge): Für die Sonnenstandsberechnung.
    Installierte Leistung (kWp): Die Nennleistung der PV-Anlage.
    Neigungswinkel der Module: Zum Beispiel 30° (häufig verwendeter Winkel).
    Ausrichtung (Ost-West): Module zeigen in beide Richtungen.
    Wetter- und Atmosphärendaten:
        Direkte und diffuse Strahlung (W/m²).
        Temperatur (°C), da der Wirkungsgrad temperaturabhängig ist.
    Systemverluste: Ein pauschaler Wert von etwa 10-15 % (z. B. durch Wechselrichter, Verkabelung).

2. Schritte für die Berechnung:

    Sonnenstand berechnen: Basierend auf Geokoordinaten, Datum und Uhrzeit (Sonnenhöhe und Azimut).
    Einstrahlung auf Module bestimmen:
        Globalstrahlung (direkt + diffus) je Minute auf die Modulflächen umrechnen.
        Winkelabhängigkeit der Einstrahlung berücksichtigen (Cosinusregel).
    Temperaturabhängiger Wirkungsgrad: Berechnung der Leistungsminderung durch hohe Temperaturen.
    Ertragsberechnung:
        Ertrag=Einstrahlung auf Module×Wirkungsgrad×Installierte LeistungErtrag=Einstrahlung auf Module×Wirkungsgrad×Installierte Leistung.
    Aggregation: Minütliche Werte auf den Tagesertrag summieren.

Berechneter Ertrag eines Ost- Westdaches am 21.06.2024

   ![image](https://github.com/user-attachments/assets/6b5fbc66-15e9-4159-ab83-48aa20c79371)

