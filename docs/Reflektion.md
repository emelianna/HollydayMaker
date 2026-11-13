# Reflektion - Holiday Maker

## Tekniska val

Mitt sätt att organisera databasen och relationerna mellan dem ger en logisk uppdelning mellan hotell, rum och kund. Tabellen med bokningarna fungerar som en kopplingstabell mellan customers och rooms, som har en många-till-många-relation. En kund kan ha många bokningar och ett rum kan ha många kunder över tid. Jag funderade över att även ha en tabell för cities. Jag beslöt mig dock för att orten passar bäst i hotels. Det beror på att det inte finns mycket data kopplat till cities, till exempel närmaste flygplats eller en beskrivning av orten, bara id och name. En extra tabell hade inneburit ytterligare en JOIN och troligtvis mer C#-kod.

Email är den enda kolumnen som får vara null. Tanken är att man inte ska behöva ha en emailadress för att göra en bokning. Ett alternativ är att dela den informationen via telefon. Det ökar tillgängligheten. Jag har medvetet valt bort att sätta unique på telefon. Det är inte vanligt, men det förekommer att det finns en hemtelefon och att mer än en i hushållet vill registrera sig.

Jag frångick gruppens ER-diagram bland annat genom att välja DATE i stället för DATETIME. En bokning gäller hela dagar. Krockregeln med < och > gör att en ny gäst kan checka in samma dag som en annan checkar ut. Att blanda in klockslag skulle göra jämförelsen mellan bokade datum och sökta datum svårare. DATE i databasen passar ihop med DateOnly i C#.


## Säkerhet och dataintegritet

Parametriserade frågor har använts i hela programmet. Inmatningen skickas som parametrar och klistras aldrig in i SQL-koden. Inmatningen tolkas på så sätt som ett värde, inte som ett kommando. Detta skyddar mot SQL-injektion. En begränsning är att en parameter bara kan vara ett värde, inte ett kolumnnamn. Det går till exempel inte att skriva ORDER BY @sortBy för att låta operatören välja sortering. Jag fick lära mig att använda ORDER BY CASE, där @sortBy bara jämförs med texten "rating". Är det samma text sorteras listan efter omdöme, annars efter pris. Kolumnnamnen står i SQL-koden och inmatningen skickas fortfarande som parameter. Se RoomRepository.cs, metoden SearchAvailableRoomsAsync. Alternativet hade varit att frångå parametriserade frågor och sätta ihop SQL-koden i C#, och därmed riskera SQL-injektion.

Det sker en kontroll precis innan en ny bokning görs. Men om programmet används av flera operatörer skulle det tekniskt sett kunna ske en dubbelbokning om bokningarna görs exakt samtidigt. Kontrollen och bokningen skulle kunna läggas i en transaktion där rummet låses medan det kontrolleras och bokas. Då får en annan operatör vänta och ser sedan att rummet är upptaget.

Databasen skyddar informationen med foreign keys, NOT NULL, UNIQUE och CHECK (tex omdöme 1–5 och utcheckning efter incheckning). Att sällskapet ryms i rummet kan CHECK inte kontrollera, eftersom det kräver tabellen rooms, så den kontrollen ligger i INSERT-frågan. Foreign keys gör att en bokning bara kan peka på en kund och ett rum som finns och att en kund eller ett rum med bokningar inte kan raderas av misstag.

Lösenordet står skrivet i koden, vilket givetvis är en säkerhetsrisk.


## Resultat och förbättringar

Alla krav är uppfyllda och har testats enligt uppgiftens testpunkter.
Efter att alla SQL-frågor (utom sorteringen) var färdiga testades de i DataGrip. Detta för att vara säker på att de fungerade innan de integrerades med C#. Sedan gjordes metoder i C# efter uppgiftens krav med hjälp av SQL-frågorna. Dessa testades i Program.cs. När programmet var så pass färdigt att tester kunde ske via menyn i konsolen skedde testerna på det sättet.

Om jag hade mer tid skulle jag göra det möjligt att boka direkt efter sökningen. Jag skulle också lägga till möjligheten att välja antal nätter. Jag skulle göra så att hela bokningen syns innan ändring/avbokning, inte bara bokningsnumret. Dessa förbättringar skulle jag göra för att underlätta för användaren och minska risken för fel.

En prisändring påverkar idag gamla bokningar, eftersom priset inte sparas i bokningen. Om betalningen sker på plats och inte direkt vid bokningen kan kunden alltså få ett annat pris än det som gällde när hon bokade. Om jag hade mer tid skulle jag spara priset i bokningen, så att det pris som gällde vid bokningen används. Den här förbättringen skulle jag prioritera först, eftersom den påverkar att informationen är korrekt. De andra gör programmet smidigare att använda.

Try/catch används idag för att fånga felaktiga inmatningar. Om det sker kommer användaren tillbaka till menyn. Om jag hade haft mer tid skulle jag ha använt TryParse för att ge användaren en chans till ny inmatning.


## AI och andra hjälpmedel

Vid kursens start startade jag ett Claude-projekt. Tydliga instruktioner i en fil förmedlar att jag är student och vill ha stöd i mitt lärande. Jag vill inte ha färdig kod, utan ledtrådar, förklaringar och upprepade kontrollfrågor. Kursplan, inlämningsuppgift och kursmaterial lades in så att jag kan få hänvisningar till sådant som gåtts igenom på lektionerna. Vid delar av uppgiften där upprepningar förekommit och jag hade koll på hur man gör har jag bett om färdig kod. Likaså ett skelett till menyn, som jag gjort flera gånger tidigare. Ibland har jag kört fast och fått mycket hjälp, men jag har varit noggrann med att få många förklaringar för att förstå.

Allt som Claude föreslog provkörde jag själv - SQL-frågorna i DataGrip och C#-koden i konsolen och jämförde resultatet med testdatan.

Claude genererade testdata som databasen fylldes med.

Claude har gjort större delen av README och varit behjälplig med SQL-skriptet för inlämning och testdata.

Claude har använts för struktur, både gällande arbetet i stort och för att påminna om pauser var 45:e minut och lunch.

Kursmaterial på bloggen, föreläsningar och diskussioner med klasskamrater har såklart också använts som hjälp.

## Så har jag testat

- Ledigt rum visas, upptaget filtreras bort: Malmö 10–13 dec, rum 7 (bokat) syns inte, rum 8 syns.
- Boka samma dag som någon checkar ut: rum 8, incheckning 13 dec ger 0 krockar.
- Överlapp nekas, även omslutande period: rum 8, 8–15 dec nekas.
- För stort sällskap nekas: rum 2 (2 pers), 4 pers nekas, 2 pers ger bokning 31.
- Kombinerade filter och båda sorteringarna: Malmö + restaurang + max 1000 m till centrum ger bara Hotell Centralen. Pris och omdöme testade.
- Ändrat antal följer kapaciteten: bokning 1 (2 pers), 3 pers ger 0 rader, 2 pers ger 1 rad.
- Avbokning frigör rummet: 6 → 7 lediga rum efter avbokning.
- Uppgifterna finns kvar efter omstart: programmet avslutat och startat igen, bokning 31 syns via val 4
