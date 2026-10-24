# Lønnskalkulator

![Build and deploy](https://github.com/ZarrarAh/SalaryCalculator/actions/workflows/master_salarycalculator-zarrar.yml/badge.svg)

En webapp for å føre opp arbeidsvakter og regne ut lønn med kvelds-, helge- og helligdagstillegg. Laget med C#, .NET 8 og Blazor, og kjører i Azure.

**Live:** https://salarycalculator-zarrar-exccendefxf2fjh0.swedencentral-01.azurewebsites.net
*(Appen kjører på Azure sitt gratisnivå og går i dvale når den ikke er i bruk, så første lasting kan ta 20-30 sekunder.)*

## Bakgrunn

Jeg jobber som resepsjonist på hotell, og førte tidligere timene mine i et Excel-ark for å kontrollere lønnsslippen. Arket viste bare timer, ikke hva jeg faktisk skulle få utbetalt med tillegg. Denne appen erstatter arket: man velger vakttype for hver dag, og appen regner ut timer, tillegg og bruttolønn for lønnsperioden. Flere kolleger kan bruke den, og hver bruker har sine egne vakter og satser.

## Funksjonalitet

- **Vaktliste per lønnsperiode** (fra den 16. til den 15. neste måned), med én rad per dag, slik som i Excel-arket
- **Faste vakttyper:** 1v (07-15), mv (10-18), 2v (15-23), nv (23-07), eller egne tider
- **Automatisk lagring** når en vakt legges inn eller endres
- **Tillegg:**
  - Kveldstillegg for timer etter et valgfritt klokkeslett (standard 17:00)
  - Helgetillegg for alle timer lørdag og søndag
  - Helligdagstillegg som prosent av timelønn for røde dager
- **Nattevakter over midnatt** fordeles riktig på hver dag, f.eks. får en vakt fredag 23:00 til lørdag 07:00 helgetillegg for timene på lørdag
- **Lønnsoppsummering** som ligner på en lønnsslipp: timer, sats og beløp per linje, og brutto
- **Brukerkontoer** med egne innstillinger for timelønn og satser
- **Datasikkerhet:** brukere kan bare lese og endre sine egne data

## Teknologi

| Område | Teknologi |
|---|---|
| Språk og rammeverk | C#, .NET 8 |
| Frontend | Blazor Server (Interactive Server), Bootstrap |
| Innlogging | ASP.NET Core Identity |
| Database | Entity Framework Core med SQLite, migrasjoner |
| Testing | xUnit |
| CI/CD | GitHub Actions |
| Hosting | Azure App Service (Linux), deploy med OIDC (uten lagrede passord) |

## Hvordan lønnen beregnes

All logikk ligger i `PayCalculator`, som ikke har noen avhengigheter til database eller UI. Det gjør den enkel å teste.

Kjernen er én metode som finner hvor mange timer to tidsrom overlapper:

```
Vakt:          15:00 |=================| 23:00
Kveldsvindu:              17:00 |===============| 24:00
Overlapp:                 17:00 |======| 23:00   = 6 timer
```

Alle tillegg bruker samme mønster. For hver dato vakten berører (startdagen og dagen etter, siden en vakt kan gå over midnatt), lages et tidsvindu, og overlappen med vakten legges sammen:

- **Kveld:** fra kveldsstart til midnatt
- **Helg:** hele døgnet, men bare lørdag og søndag

Pause trekkes fra de betalte timene. Beløp lagres som `decimal` for å unngå avrundingsfeil.

## Prosjektstruktur

```
SalaryCalculator/
├── Components/
│   ├── Layout/          Felles layout og meny
│   ├── Pages/           Sider: Home, Vaktliste (Period), Innstillinger (Settings)
│   └── Shared/          Gjenbrukbare komponenter (PaySummary)
├── Data/                ApplicationDbContext og brukermodell
├── Migrations/          EF Core-migrasjoner
├── Models/              Shift, PaySettings, ShiftPay, PeriodPay, ShiftType
└── Services/            PayCalculator, ShiftService, SettingsService

SalaryCalculator.Tests/  Unit tester for PayCalculator
```

Sidene snakker aldri direkte med databasen. De går via `ShiftService` og `SettingsService`, som henter innlogget bruker fra innloggingen og sjekker eierskap før data endres eller slettes.

## Kjøre lokalt

Krever [.NET 8 SDK](https://dotnet.microsoft.com/download).

```bash
git clone https://github.com/ZarrarAh/SalaryCalculator.git
cd SalaryCalculator
dotnet run --project SalaryCalculator
```

Eller åpne `SalaryCalculator.sln` i Visual Studio og trykk F5.

Databasen (`salary.db`) og tabellene lages automatisk ved første oppstart. Registrer en bruker, legg inn timelønn under **Innstillinger**, og før opp vakter under **Vaktliste**.

## Tester

```bash
dotnet test
```

Testene dekker beregning av arbeidstimer, overlapp, kveldstimer, helgetimer og vakter som går over midnatt.

## CI/CD

Hver push til `master` kjører en GitHub Actions-pipeline som:

1. Bygger løsningen
2. Kjører alle testene, og stopper hvis noen feiler
3. Publiserer webprosjektet
4. Logger inn i Azure med OIDC og deployer til Azure App Service

I Azure ligger SQLite-databasen i `/home/data`, som beholdes mellom omstarter og nye deployer. Plasseringen settes med miljøvariabelen `ConnectionStrings__DefaultConnection`.

## Videre arbeid

- Nattillegg
- Flytte felles kode for innlogget bruker til en egen `CurrentUserService`
- Flere tester, blant annet for `PeriodPay` og helligdagstillegg
- Oppgradere til .NET 10

## Laget av

Zarrar Ahmed
