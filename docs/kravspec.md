# Kravspec: "Vi missar deadlines hela tiden"

> **Om det här dokumentet.** Det här är ett utkast. Allt som kommer direkt från Saras mejl, hennes
> buggrapport (`docs/arendet.md`) eller det hon sa på lektionen är skrivet som fakta. Allt som är
> **markerat med ⚠️ ANTAGANDE** har jag inte fått bekräftat av Sara. Det ska stämmas av med henne,
> och ändras eller strykas om hon säger något annat.

---

## 1. Problemet, med kundens egna ord

> "Vi är ett team på åtta personer som använder er uppgiftsapp varje dag. Problemet är att vi missar
> deadlines hela tiden, och ingen märker det förrän kunden ringer och är arg."
> /Sara, teamledare

Buggrapport, två dagar efter demon av Kims version:

> "Appen säger att saker är försenade samma dag som de ska vara klara. Klockan nio på morgonen står det
> att 'Skicka offert' är en dag sen, fast den ska vara klar idag. Folk har slutat lita på listan."

**Sammanfattning (tre meningar):**
Saras team på åtta personer missar deadlines, och det märks först när kunden hör av sig. De behöver
kunna se i tid vilka uppgifter som är försenade eller ska bli klara idag, så att de hinner agera.
Listan måste gå att lita på, och i dag gör den inte det eftersom den markerar uppgifter som försenade
samma dag som de ska vara klara.

---

## 2. Mål, och hur vi vet att vi har nått dem

| | Mätvärde: andel uppgifter som blir klara i tid |
|---|---|
| **Nuläge** | Ungefär 60 % |
| **Mål** | 85 % |
| **Tidsram** | Fyra veckor efter att funktionen är i drift |
| **Hur vi mäter** | `GET /api/todos/stats`: antal klara i tid ÷ (antal klara i tid + antal klara sent) |

- En uppgift räknas som **klar i tid** om den markerades som klar senast på sitt förfallodatum
  (svensk tid). Den räknas som **klar sent** om den markerades som klar efter det.
- Uppgifter **utan förfallodatum** räknas inte med i andelen.
- Finns det ingen avslutad uppgift med förfallodatum än svarar API:t att det **saknas data**, inte 0 %.

**Uppföljning:** ⚠️ ANTAGANDE: Jag läser av siffran tillsammans med Sara en gång i veckan under de fyra
veckorna, och jämför med nuläget 60 %. Exakt dag och vem som är med bestäms med Sara.

---

## 3. Frågor vi ställde, och svaren

> Fyll i med svaren från intervjun med Sara. Frågor där svaret redan framgår av mejlet, buggrapporten
> eller lektionen är ifyllda.

| # | Fråga | Svar |
|---|---|---|
| 1 | Hur många använder appen, och hur ofta? | Åtta personer i teamet, varje dag (mejlet). |
| 2 | Vad händer i dag när en deadline missas? | Ingen märker det förrän kunden ringer och är arg (mejlet). |
| 3 | När räknas en uppgift som försenad? Om den ska vara klar på fredag, när är den sen? | "Fredag" betyder hela fredagen. Den är försenad först när fredagen är slut (lektionen). |
| 4 | Vilken tid gäller, om servern står i ett annat land? | Svensk tid (Europe/Stockholm) (lektionen). |
| 5 | Hur vet du om en månad att det har blivit bättre? | Andelen uppgifter som blir klara i tid ska öka från cirka 60 % till 85 % på fyra veckor (lektionen). |
| 6 | Hur snabbt måste listan vara? | Under 200 ms, även när alla åtta använder den samtidigt (lektionen). |
| 7 | Vad är det första du vill veta när du öppnar appen på morgonen? | _Fyll i från intervjun._ |
| 8 | Vad ska hända med en uppgift som inte har något förfallodatum? | _Fyll i från intervjun._ |
| 9 | Vem ska kunna se en försenad uppgift, alla eller bara den som äger den? | _Fyll i från intervjun._ |
| 10 | Vad gör ni i dag när ni ser att något kommer bli sent? | _Fyll i från intervjun._ |

---

## 4. User stories med acceptanskriterier

### US-1: Sätta ett förfallodatum

**Som** teammedlem **vill jag** kunna ange vilken dag en uppgift ska vara klar, **så att** appen kan
hålla koll på deadlinen åt mig.

| Id | Acceptanskriterium |
|---|---|
| AK-1.1 | **Givet** att det är 2026-11-16 i Sverige, **när** jag skapar en uppgift med förfallodatum 2026-11-20, **så** sparas uppgiften med förfallodatum 2026-11-20 (bara datum, ingen tid). |
| AK-1.2 | **Givet** att det är 2026-11-16 i Sverige, **när** jag skapar en uppgift med förfallodatum 2026-11-15, **så** svarar API:t 400 Bad Request och uppgiften sparas inte. |
| AK-1.3 | **Givet** att jag skapar en uppgift utan förfallodatum, **när** den sparas, **så** har den inget förfallodatum och räknas aldrig som försenad. |

### US-2: Se vilka uppgifter som är försenade

**Som** teammedlem **vill jag** se vilka uppgifter som är försenade, **så att** jag kan agera innan
kunden ringer.

| Id | Acceptanskriterium |
|---|---|
| AK-2.1 | **Givet** en ej klar uppgift med förfallodatum idag, **när** klockan är 23:59 svensk tid, **så** är den **inte** försenad. |
| AK-2.2 | **Givet** en ej klar uppgift med förfallodatum igår, **när** klockan är 00:00 svensk tid idag, **så** är den försenad med 1 dag. |
| AK-2.3 | **Givet** en uppgift med förfallodatum igår som redan är markerad som klar, **när** jag hämtar försenade uppgifter, **så** finns den **inte** med. |
| AK-2.4 | **Givet** en uppgift med förfallodatum idag, **när** klockan är 23:30 UTC (00:30 nästa dag i Sverige under vintertid), **så** är den försenad, eftersom svensk tid gäller och inte serverns tid. |

### US-3: Planera dagen

**Som** teammedlem **vill jag** se en arbetslista i den ordning jag bör ta mig an uppgifterna, **så att**
jag vet vad jag ska göra först när jag börjar dagen.

| Id | Acceptanskriterium |
|---|---|
| AK-3.1 | **Givet** en försenad uppgift, en som ska vara klar idag och en som ska vara klar i morgon, **när** jag hämtar arbetslistan, **så** kommer de i ordningen: försenad, idag, i morgon. |
| AK-3.2 | **Givet** två försenade uppgifter, en med förfallodatum för tre dagar sedan och en för en dag sedan, **när** jag hämtar arbetslistan, **så** kommer den som är mest försenad först. |
| AK-3.3 | **Givet** att det finns uppgifter utan förfallodatum, **när** jag hämtar arbetslistan, **så** kommer de sist. ⚠️ ANTAGANDE: bekräfta med Sara (fråga 8). |
| AK-3.4 | **Givet** att alla uppgifter är klara, **när** jag hämtar arbetslistan, **så** är listan tom och svaret är 200 OK med en tom lista. |

---

## 5. Use case: Planera dagen

**Aktör:** Teammedlem
**Mål:** Veta vad som ska göras först idag.
**Förutsättning:** Det finns uppgifter i appen.

### Huvudflöde

1. Teammedlemmen öppnar appen på morgonen.
2. Appen hämtar arbetslistan från API:t.
3. API:t tar reda på vilken dag det är i svensk tid.
4. API:t sorterar ej klara uppgifter: försenade först, sedan de som ska vara klara idag, sedan resten.
5. Appen visar listan, med försenade uppgifter tydligt markerade.
6. Teammedlemmen börjar med den första uppgiften i listan.

### Alternativa flöden

| Id | Flöde | Täcks av |
|---|---|---|
| A1 | **Inget är försenat.** I steg 4 finns inga försenade uppgifter. Listan börjar med dagens uppgifter. | AK-3.1 |
| A2 | **Uppgiften ska vara klar idag och det är fortfarande idag.** Den visas som "idag", inte som försenad. Det här är Kims bugg. | AK-2.1 |
| A3 | **En uppgift blir klar för sent.** Teammedlemmen markerar en försenad uppgift som klar. Den försvinner ur listan och räknas som "klar sent" i statistiken. | AK-2.3, Mål (avsnitt 2) |
| A4 | **Allt är klart.** Listan är tom. | AK-3.4 |
| A5 | **Uppgift utan förfallodatum.** Den visas sist och markeras aldrig som försenad. | AK-1.3, AK-3.3 |

### Undantagsflöden

| Id | Flöde | Täcks av |
|---|---|---|
| U1 | **API:t svarar inte.** I steg 2 får appen inget svar. Appen visar ett felmeddelande, till exempel "Kunde inte hämta uppgifterna. Försök igen.", i stället för en tom lista. | ⚠️ **Saknar kriterium.** Ska läggas till: frontend-krav, se Utanför scope / nästa steg. |
| U2 | **Uppgiften har tagits bort.** Teammedlemmen försöker markera en uppgift som klar, men den har tagits bort av någon annan. API:t svarar 404 Not Found och appen visar att uppgiften inte finns längre. | ⚠️ **Saknar kriterium.** Ska läggas till som AK-2.5 eller en egen story. |

---

## 6. Icke-funktionella krav

| Id | Krav | Hur vi mäter |
|---|---|---|
| IK-1 | Arbetslistan laddas på **under 200 ms** (p95) med 1 000 uppgifter och 20 samtidiga användare. | Lasttest mot API:t (övning 7) |
| IK-2 | Sorteringen av arbetslistan klarar 10 000 uppgifter. ⚠️ ANTAGANDE: under 10 ms. | BenchmarkDotNet (övning 7) |
| IK-3 | All logik för datum och försening använder **svensk tid** (Europe/Stockholm), oavsett var servern står. | Tester med `FakeTimeProvider` (AK-2.4) |
| IK-4 | Försenade uppgifter filtreras fram **i databasen**, inte genom att hämta alla till minnet. | Kodgranskning, och lasttestet i IK-1 |

---

## 7. Utanför scope

Det här gör vi medvetet **inte** nu:

- Påminnelser via mejl, SMS eller push-notiser.
- Att tilldela uppgifter till en viss person.
- Förfallodatum med klockslag. Ett förfallodatum är alltid en hel dag.
- Återkommande uppgifter, till exempel "varje fredag".
- Inloggning och behörigheter.
- Felmeddelande i frontend när API:t inte svarar (U1). Det är noterat som en lucka och tas upp med Sara.
