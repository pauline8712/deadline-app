# Mätning: håller vi prestandakraven?

**Kort svar:** Ja, för det som kraven gäller.

| Krav | Mål | Uppmätt | Håller? |
|---|---|---|---|
| IK-2: sortera arbetslistan, 1 000 uppgifter | under 1 ms | **0,34 ms** | ✅ |
| IK-1: `GET /api/todos`, 1 000 uppgifter, 20 samtidiga användare | p99 under 200 ms, 0 fel | **p99 94–163 ms**, 0 fel | ✅, men med liten marginal i den första, kalla körningen |
| Utanför kravet: samma lasttest med 10 000 uppgifter | (p99 under 200 ms) | **p99 972 ms** | ❌ |

Mätt 2026-11-26.

## Datorn jag mätte på

| | |
|---|---|
| Processor | Intel Core i5-8350U, 1,70 GHz (max 1,90 GHz under mätningen), 4 kärnor / 8 trådar |
| Minne | 16 GB |
| OS | Windows 11 Pro (10.0.26200) |
| .NET | SDK 10.0.103, runtime 10.0.3, Release |
| Node | 24.13.1, autocannon 8 |

Det är en bärbar dator från 2018, och API:t och lasttestet körde på samma maskin. Siffrorna gäller den här datorn
och är inte ett löfte om hur en server beter sig. De duger för att svara på om kravet håller med marginal.

---

## IK-2: sortering av arbetslistan (BenchmarkDotNet)

**Krav:** Sorteringen av arbetslistan tar under 1 ms för 1 000 uppgifter (kursmaterialet).

**Hur jag mätte:** `Starter.Benchmarks/WorkListOrderingBenchmarks.cs` mäter `TodoOrdering.ForWorkList`, samma
metod som `GET /api/todos` använder. Slumpdatan har en fast seed (42), så varje körning mäter samma data.
Ungefär var fjärde uppgift är klar och var femte saknar förfallodatum.

```bash
cd backend
dotnet run -c Release --project Starter.Benchmarks -- --filter "*" --job short
```

**Resultat:**

| Uppgifter | Mean | StdDev | Allocated |
|---:|---:|---:|---:|
| 100 | 19,06 µs | 0,37 µs | 3,59 KB |
| 1 000 | **341,73 µs** | 10,69 µs | 29,95 KB |
| 10 000 | 4 479,18 µs | 48,97 µs | 293,63 KB |

**Håller kravet?** Ja. 0,34 ms är en knapp tredjedel av gränsen på 1 ms.

**Jämfört med läraren:** Lärarens dator tog cirka 0,08 ms för 1 000 och 1,9 ms för 10 000 uppgifter. Min
dator är ungefär 4 gånger långsammare, men förhållandet är detsamma.

**Varför tar 10 gånger fler uppgifter 13 gånger längre tid?** Sortering kostar ungefär *n · log n*, inte *n*.
Med 10 gånger fler element blir varje element också inblandat i fler jämförelser. Dessutom passar 10 000
objekt sämre i processorns cache än 1 000. Minnet växer däremot linjärt (30 KB → 294 KB), eftersom
`OrderBy` bara behöver en nyckel och en plats per element.

---

## IK-1: API:t under last (autocannon)

**Krav:** Arbetslistan svarar på under 200 ms (lektionen) med 1 000 uppgifter och 20 samtidiga användare
(kursmaterialet). Lektionen sa p95, men autocannon visar inte p95. Därför jämför vi **p99**, som är strängare:
håller p99 så håller också p95.

**Hur jag mätte** (torsdagens material):

```bash
# Terminal 1: API:t i Release, med en ny, tom databas
dotnet run -c Release --project Starter.Api

# Terminal 2
node perf/seed.mjs 1000
npx autocannon -c 20 -d 10 http://localhost:5069/api/todos
```

**Resultat:**

| Körning | Uppgifter | Svar per anrop | p50 | p97,5 | **p99** | Max | Anrop på 10 s | Fel |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 (direkt efter start) | 1 000 | 117 KB | 56 ms | 143 ms | **163 ms** | 340 ms | ~3 000 | 0 |
| 2 (varm) | 1 000 | 117 KB | 45 ms | 84 ms | **94 ms** | 195 ms | ~4 000 | 0 |
| 3 | 10 000 | 1,19 MB | 522 ms | 897 ms | **972 ms** | 998 ms | 377 | 0 |

**Håller kravet?** Ja. p99 var 163 ms i första körningen och 94 ms när API:t var varmt, och inga anrop
misslyckades. Den första körningen är långsammare eftersom .NET kompilerar koden (JIT) och SQLite värmer upp sin
cache under de första anropen. Marginalen i den kalla körningen är bara 37 ms.

**Vad händer med 10 000 uppgifter?** Kravet bryts tydligt: p99 blir nästan en sekund. Varje anrop skickar då
hela listan, 1,19 MB JSON, och servern hinner bara med cirka 38 anrop per sekund. Det är inte sorteringen som
är problemet (4,5 ms enligt benchmarken), utan att hela tabellen läses, görs om till JSON och skickas varje gång.

## Slutsats och nästa steg

- Med de volymer som kraven gäller, 1 000 uppgifter och 20 användare, håller båda kraven.
- Sorteringen är billig. Kostnaden i API:t ligger i databasen, JSON och mängden data.
- Om teamen får många fler uppgifter är **paginering** (extra-övningen) det första att prova. Därefter mäter
  vi igen, för att se att det blev bättre i stället för att gissa.

---

## Testpyramiden (torsdagen, del 1)

| Nivå | Antal | Var |
|---|---:|---|
| Enhetstester (regler, service, sortering) | 14 | `TodoDeadlinesTests`, `TodoServiceTests`, `OverdueTests`, `WorkListTests` |
| API-tester (hela appen i minnet) | 6 | `TodosApiTests` |
| Frontend-komponenttester | 3 | `TodoList.test.tsx`, `TodosPage.test.tsx` |

Det är många snabba enhetstester i botten och färre API-tester ovanpå, så det ser ut som en pyramid. Det finns
inga tester av hela systemet (webbläsare, API och databas tillsammans).
