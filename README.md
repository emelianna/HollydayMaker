# Holiday Maker


## Innehåll

| Fil / mapp | Beskrivning |
|---|---|
| `HollydayMaker/` | C#-projektet |
| `database/holiday_maker.sql` | Skapar databasen, tabellerna och testdata |
| `docs/ER-diagram.png` | ER-diagram |
| `docs/ER-diagram - jämförelse.pdf` | Gruppens första ER-diagram jämfört med den färdiga databasen |
| `docs/Reflektion.md` | Individuell reflektion, inklusive hur lösningen testats |

## Kom igång

MySQL körs i Docker på port **3307**:

```bash
docker run -d --name mysql-kurs -e MYSQL_ROOT_PASSWORD=kurslosenord -p 127.0.0.1:3307:3306 -v mysql-kurs-data:/var/lib/mysql mysql:8.4
```

1. Kör `database/holiday_maker.sql` (skapar databasen `holiday_maker` med testdata).
2. Kontrollera anslutningssträngen överst i `HollydayMaker/Program.cs` (port 3307, lösenord `kurslosenord`).
3. Starta från mappen med `HolidayMaker.sln`:
   ```bash
   dotnet run --project HollydayMaker
   ```

## Användning

Vanligt flöde: **1** välj/registrera kund → **2** sök lediga rum → **3** boka med rum-id från sökningen. Val **4–6** visar, ändrar antal personer på och avbokar befintliga bokningar.

Sökningen kan filtreras på krav på pool, krav på restaurang, max avstånd till strand och till centrum (kombinerbara), och sorteras på pris eller omdöme.


