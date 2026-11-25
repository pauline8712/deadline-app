# Kravspec: "Vi missar deadlines hela tiden"

> **Om det här dokumentet.** Varje krav ska gå att spåra till en källa:
>
> - **Ärendet**: Saras mejl och buggrapport i [`docs/arendet.md`](arendet.md).
> - **Lektionen**: det Sara sa på lektionen (svensk tid, "fredag betyder hela fredagen", 60 % → 85 %,
>   under 200 ms).
> - **Kursmaterialet**: övningarna och läsmaterialet för vecka 2.
>
> Allt som är **markerat med ⚠️ ANTAGANDE** har Sara inte bekräftat. Det ska stämmas av med henne och
> ändras eller strykas om hon säger något annat. En samlad lista finns i [avsnitt 8](#8-antaganden-att-stämma-av-med-sara).

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
kunna se i tid vilka uppgifter som är försenade eller ska vara klara idag, så att de hinner agera.
Listan måste gå att lita på, och i dag gör den inte det, eftersom den markerar uppgifter som försenade
samma dag som de ska vara klara.

---

## 2. Mål, och hur vi vet att vi har nått dem

| | Mätvärde: andel uppgifter som blir klara i tid |
|---|---|
| **Nuläge** | Ungefär 60 % (lektionen) |
| **Mål** | 85 % (lektionen) |
| **Tidsram** | Fyra veckor (lektionen) |
| **Hur vi mäter** | `GET /api/todos/stats` (kursmaterialet, övning 8), se US-4 |

**Definitioner**

- En uppgift är **klar i tid** om den markerades som klar senast på sitt förfallodatum, i svensk tid.
  Den är **klar sent** om den markerades som klar efter förfallodatumet. Det följer av "fredag betyder
  hela fredagen".
- **Andelen klar i tid** = klara i tid ÷ (klara i tid + klara sent).
  ⚠️ ANTAGANDE: uppgifter som är försenade men inte klara räknas inte in i andelen. De visas som en egen
  siffra, "försenade just nu".
- ⚠️ ANTAGANDE: uppgifter **utan förfallodatum** räknas inte med, eftersom de inte kan bli sena.
- Finns det ingen klar uppgift med förfallodatum än är svaret **"ingen data än"**, inte 0 %
  (kursmaterialet).

**Uppföljning** ⚠️ ANTAGANDE: Jag och Sara läser av `GET /api/todos/stats` tillsammans en gång i veckan
under de fyra veckorna, och jämför med nuläget 60 %. Efter fyra veckor har vi ett uppföljningsmöte där vi
bestämmer om målet är nått. Exakt dag bestäms med Sara. Avsnittet skrivs klart i övning 8.

---

## 3. Frågor vi ställde, och svaren

| # | Fråga | Svar |
|---|---|---|
| 1 | Hur många använder appen, och hur ofta? | Åtta personer i teamet, varje dag (ärendet). |
| 2 | Vad händer i dag när en deadline missas? | Ingen märker det förrän kunden ringer och är arg (ärendet). |
| 3 | När räknas en uppgift som försenad? Om den ska vara klar på fredag, när är den sen? | "Fredag" betyder hela fredagen. Den är försenad först när fredagen är slut (lektionen). |
| 4 | Vilken tid gäller, om servern står i ett annat land? | Svensk tid, Europe/Stockholm (lektionen). |
| 5 | Hur vet du om en månad att det har blivit bättre? | Andelen uppgifter som blir klara i tid ska öka från cirka 60 % till 85 % på fyra veckor (lektionen). |
| 6 | Hur snabbt måste listan vara? | Under 200 ms (lektionen), även när alla åtta öppnar den samtidigt på morgonen (kursmaterialet). |
| 7 | Var ska uppgifter utan förfallodatum visas i listan? | **Obesvarad.** ⚠️ ANTAGANDE: sist. |
| 8 | Hur och hur ofta vill du följa upp mätvärdet? | **Obesvarad.** ⚠️ ANTAGANDE: en gång i veckan, se avsnitt 2. |
| 9 | Vad ska hända när någon markerar en försenad uppgift som klar? | **Obesvarad.** ⚠️ ANTAGANDE: den försvinner från "försenade" och räknas som "klar sent". |
| 10 | Behöver teamet påminnelser, eller räcker det att listan visar försenat och "idag"? | **Obesvarad.** ⚠️ ANTAGANDE: listan räcker för nu, se avsnitt 7. |
| 11 | Ska uppgifter kunna tilldelas en viss person? | **Obesvarad.** ⚠️ ANTAGANDE: inte nu, se avsnitt 7. |

---

## 4. User stories med acceptanskriterier

Exemplen använder måndag **2026-11-16** som "idag". I november gäller vintertid, så svensk tid är
UTC+1. Klockslagen är svensk tid om inget annat står. Varje kriterium testas med ett test märkt
`[Trait("Krav", "AK-x.y")]`, och `dotnet test --filter "Krav=AK-x.y"` ska hitta det.

### US-1: Sätta ett förfallodatum

**Som** teammedlem **vill jag** kunna ange vilken dag en uppgift ska vara klar, **så att** appen kan
hålla koll på deadlinen åt mig.

| Id | Acceptanskriterium |
|---|---|
| AK-1.1 | **Givet** att det är 2026-11-16 kl. 10:00, **när** jag skapar en uppgift med förfallodatum 2026-11-20, **så** svarar API:t 201 Created och uppgiften har förfallodatum `2026-11-20`, ett datum utan klockslag. |
| AK-1.2 | **Givet** att det är 2026-11-16 kl. 10:00, **när** jag skapar en uppgift med förfallodatum 2026-11-15, **så** svarar API:t 400 Bad Request och ingen uppgift sparas (kursmaterialet, övning 6). |
| AK-1.3 | **Givet** att det är 2026-11-16 kl. 10:00, **när** jag skapar en uppgift med förfallodatum 2026-11-16, alltså idag, **så** svarar API:t 201 Created. |

### US-2: Se vilka uppgifter som är försenade

**Som** teammedlem **vill jag** se vilka uppgifter som är försenade, **så att** jag kan agera innan
kunden ringer.

Hit hör Saras buggrapport.

| Id | Acceptanskriterium |
|---|---|
| **AK-2.1** | **Givet** en ej klar uppgift "Skicka offert" med förfallodatum 2026-11-16, **när** klockan är 09:00 den 2026-11-16 och jag hämtar försenade uppgifter, **så** finns den **inte** med. *(Kims bugg.)* |
| AK-2.2 | **Givet** en ej klar uppgift med förfallodatum 2026-11-16, **när** klockan är 00:00 den 2026-11-17 och jag hämtar försenade uppgifter, **så** finns den med och är försenad 1 dag. |
| AK-2.3 | **Givet** en uppgift med förfallodatum 2026-11-15 som är markerad som klar, **när** jag hämtar försenade uppgifter den 2026-11-16, **så** finns den **inte** med. |

Hur förseningen räknas i svensk tid, oavsett serverns tid, testas under IK-3.

### US-3: Planera dagen

**Som** teammedlem **vill jag** se en arbetslista i den ordning jag bör ta mig an uppgifterna, **så att**
jag vet vad jag ska göra först när jag börjar dagen.

Arbetslistan är `GET /api/todos`.

| Id | Acceptanskriterium |
|---|---|
| AK-3.1 | **Givet** att det är 2026-11-16 och det finns ej klara uppgifter med förfallodatum 2026-11-17, 2026-11-13 och 2026-11-16, **när** jag hämtar arbetslistan, **så** kommer de i ordningen 2026-11-13 (försenad), 2026-11-16 (idag), 2026-11-17. ⚠️ ANTAGANDE: tidigast förfallodatum först. |
| AK-3.2 | **Givet** en ej klar uppgift utan förfallodatum och en ej klar uppgift med förfallodatum 2026-11-20, **när** jag hämtar arbetslistan, **så** kommer uppgiften utan förfallodatum efter. ⚠️ ANTAGANDE (fråga 7). |
| AK-3.3 | **Givet** en klar uppgift med förfallodatum 2026-11-13 och en ej klar uppgift utan förfallodatum, **när** jag hämtar arbetslistan, **så** kommer den klara uppgiften sist. ⚠️ ANTAGANDE: klara uppgifter visas, men sist. |
| AK-3.4 | **Givet** att det inte finns någon uppgift med id 999, **när** jag markerar uppgift 999 som klar, **så** svarar API:t 404 Not Found. *(Lades till från undantag U2.)* |
| AK-3.5 | **Givet** att API:t inte svarar, **när** jag öppnar appen, **så** visar appen ett felmeddelande i stället för en tom lista. *(Lades till från undantag U1. Testas i frontend med Vitest. Testnamnet innehåller "AK-3.5".)* |

### US-4: Följa upp målet

**Som** teamledare **vill jag** se hur stor andel av uppgifterna som blir klara i tid, **så att** jag vet
om teamet missar färre deadlines.

Källa: målet i avsnitt 2 och kursmaterialet, övning 8.

| Id | Acceptanskriterium |
|---|---|
| AK-4.1 | **Givet** en uppgift med förfallodatum 2026-11-16, **när** den markeras som klar 2026-11-16 kl. 23:00, **så** sparas tidpunkten när den blev klar, och `GET /api/todos/stats` räknar den som **klar i tid**. |
| AK-4.2 | **Givet** en uppgift med förfallodatum 2026-11-16, **när** den markeras som klar 2026-11-17 kl. 08:00, **så** räknar `GET /api/todos/stats` den som **klar sent**. |
| AK-4.3 | **Givet** 3 uppgifter klara i tid, 1 klar sent och 2 ej klara med förfallodatum 2026-11-13, **när** jag hämtar `GET /api/todos/stats` den 2026-11-16, **så** svarar API:t med klara i tid 3, klara sent 1, försenade just nu 2 och andel klara i tid 0,75. |
| AK-4.4 | **Givet** att ingen uppgift med förfallodatum har markerats som klar, **när** jag hämtar `GET /api/todos/stats`, **så** är andelen klara i tid `null` ("ingen data än"), inte 0. |

### Granskning mot INVEST

| Story | I (oberoende) | N (förhandlingsbar) | V (värdefull) | E (går att uppskatta) | S (liten) | T (testbar) |
|---|---|---|---|---|---|---|
| US-1 | ✅ | ✅ | ✅ Grunden för allt annat | ✅ | ✅ | ✅ Konkreta datum |
| US-2 | ⚠️ Behöver förfallodatum från US-1 | ✅ | ✅ Löser buggrapporten | ✅ | ✅ | ✅ Fast klocka i testerna |
| US-3 | ⚠️ Behöver US-1, och US-2:s regel för "försenad" | ✅ Ordningen är ett antagande | ✅ "Vad gör jag först?" | ✅ | ⚠️ Fem kriterier, varav ett i frontend | ✅ |
| US-4 | ⚠️ Behöver US-1 | ✅ Uppföljningen är ett antagande | ✅ Mäter Saras mål | ✅ | ✅ | ✅ |

US-2, US-3 och US-4 hänger på US-1. Det är medvetet: utan förfallodatum finns ingen deadline att
missa. US-1 byggs därför först.

**Två-utvecklartestet:** varje kriterium har ett fast datum, ett fast klockslag och ett exakt förväntat
svar (statuskod, ordning eller siffra). "Klockan nio" är skrivet som 09:00 den 2026-11-16, och inte
som "idag på morgonen".

---

## 5. Use case: Planera dagen

**Aktör:** Teammedlem
**Mål:** Veta vad som ska göras först idag.
**Förutsättning:** Det finns uppgifter i appen.

### Huvudflöde

1. Teammedlemmen öppnar appen på morgonen.
2. Appen hämtar arbetslistan från API:t (`GET /api/todos`).
3. API:t tar reda på vilken dag det är i svensk tid.
4. API:t sorterar uppgifterna: försenade först, sedan de som ska vara klara idag, sedan resten (AK-3.1).
5. Appen visar listan i den ordningen.
6. Teammedlemmen börjar med den första uppgiften och markerar den som klar när hen är färdig.

### Alternativa flöden

| Id | Flöde | Täcks av |
|---|---|---|
| A1 | **Inget är försenat.** I steg 4 finns inga försenade uppgifter. Listan börjar med dagens uppgifter. | AK-3.1 |
| A2 | **Uppgiften ska vara klar idag och det är fortfarande idag.** Klockan är 09:00. Uppgiften är inte försenad. Det här är Kims bugg. | AK-2.1 |
| A3 | **En uppgift blir klar för sent.** I steg 6 markerar teammedlemmen en försenad uppgift som klar. Den finns inte längre bland de försenade och räknas som "klar sent". | AK-2.3, AK-4.2 |
| A4 | **Uppgift utan förfallodatum.** Den visas efter uppgifterna med datum. | AK-3.2 ⚠️ |

### Undantagsflöden

| Id | Flöde | Täcks av |
|---|---|---|
| U1 | **API:t svarar inte.** I steg 2 får appen inget svar. Appen visar ett felmeddelande i stället för en tom lista. | 🕳️ **Lucka hittad:** inget kriterium täckte detta. Kriteriet **AK-3.5** lades till. |
| U2 | **Uppgiften finns inte.** I steg 6 har uppgiften tagits bort eller fått fel id. API:t svarar 404 Not Found. | 🕳️ **Lucka hittad:** inget kriterium täckte detta. Kriteriet **AK-3.4** lades till. |

---

## 6. Icke-funktionella krav

| Id | Krav | Källa | Hur vi mäter |
|---|---|---|---|
| IK-1 | Arbetslistan (`GET /api/todos`) svarar på **under 200 ms** med 1 000 uppgifter och 20 samtidiga användare under 10 sekunder, utan fel. | 200 ms: lektionen. 1 000 uppgifter och 20 användare: kursmaterialet, övning 7. | Lasttest med `autocannon`. Resultatet hamnar i [`docs/matning.md`](matning.md). Lektionen sa p95, men autocannon visar inte p95, så vi jämför **p99** med 200 ms. Håller p99 så håller också p95. |
| IK-2 | Sorteringen av arbetslistan tar **under 1 ms** för 1 000 uppgifter. Den mäts också för 100 och 10 000 uppgifter. | Kursmaterialet (torsdag och övning 7). | BenchmarkDotNet i Release, med fast seed. |
| IK-3 | All logik för datum och försening använder **svensk tid** (Europe/Stockholm), oavsett var servern står. Exempel: en ej klar uppgift med förfallodatum 2026-11-16 är försenad kl. 23:30 UTC den 2026-11-16, eftersom klockan då är 00:30 den 2026-11-17 i Sverige. | Lektionen. | Test märkt `[Trait("Krav", "IK-3")]`, med `FakeTimeProvider`. |
| IK-4 | Försenade uppgifter filtreras fram **i databasen**, inte genom att hämta alla till minnet. | Kursmaterialet, övning 5, steg 4. | Kodgranskning: filtret är en `Where` som körs innan `ToListAsync`. |

---

## 7. Utanför scope

Det här gör vi medvetet **inte** nu:

- Förfallodatum med klockslag. Ett förfallodatum är alltid en hel dag (lektionen).
- ⚠️ ANTAGANDE: påminnelser via mejl, SMS eller push-notiser (fråga 10).
- ⚠️ ANTAGANDE: att tilldela uppgifter till en viss person (fråga 11).
- ⚠️ ANTAGANDE: inloggning och behörigheter.
- Att markera "Försenad" i frontendens lista. Det är en extra-övning i kursen.
- Paginering av arbetslistan. Det är en extra-övning, och tas upp igen om lasttestet visar att den behövs.

---

## 8. Antaganden att stämma av med Sara

| # | Antagande | Var |
|---|---|---|
| 1 | Uppgifter utan förfallodatum visas sist i arbetslistan. | AK-3.2, A4 |
| 2 | Arbetslistan sorteras med tidigast förfallodatum först. | AK-3.1 |
| 3 | Klara uppgifter visas i arbetslistan, men sist. | AK-3.3 |
| 4 | Andelen klar i tid räknas bara på klara uppgifter. Försenade som inte är klara visas som en egen siffra. | Avsnitt 2 |
| 5 | Uppgifter utan förfallodatum räknas inte i mätvärdet. | Avsnitt 2 |
| 6 | Uppföljning en gång i veckan, och ett möte efter fyra veckor. | Avsnitt 2 |
| 7 | En försenad uppgift som markeras klar räknas som "klar sent". | A3, AK-4.2 |
| 8 | Påminnelser, tilldelning och inloggning behövs inte nu. | Avsnitt 7 |
