## Vad gör PR:en?

<!-- Beskriv kort vad som ändrats och varför. -->

## Vilken issue fixar den?

<!-- Skriv t.ex. "Fixes #3" så stängs issuen automatiskt när PR:en mergas. Skriv "Ingen" om det inte finns någon. -->

Fixes #

## Vilka acceptanskriterier gäller den?

<!-- T.ex. AK-2.1. Testerna ska vara märkta med [Trait("Krav", "AK-x.y")]. -->

## Hur testade du?

- [ ] Backend: alla tester går igenom (`dotnet test` i `backend/`)
- [ ] Frontend: alla tester går igenom (`npm test` i `frontend/`)
- [ ] Testat manuellt: <!-- beskriv vad du klickade på eller anropade -->

## Checklista

- [ ] Koden är formaterad (`dotnet format` i `backend/`, `npm run format` i `frontend/`)
- [ ] Inga lösenord, nycklar eller connection strings i koden
- [ ] README eller `docs/` är uppdaterade om något ändrats som påverkar hur man kör projektet
