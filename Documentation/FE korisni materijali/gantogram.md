# dhtmlx-gantt  - [Demo](https://docs.dhtmlx.com/gantt/samples/02_extensions/24_click_drag.html) - [Docs](https://docs.dhtmlx.com/gantt/desktop__editions_comparison.html)
- Opterecenje radnika i resoursa postoji samo u placenoj PRO verziji
- Podprojekti su samo u placenoj PRO verziji
- Zaokruzuje task na ceo sat ili dan
- Jedino procentualno izvrsavanje taskova
- Nema nacina da se napravi novi prozor za editovanje taska pa mora da se koristi njihov (Jedine opcije su: Naslov, pocetak, tip i trajanje).

# Mermaid - [Demo](https://mermaid.live/edit#pako:eNp90cGOgyAQBuBXIZxtFbG29bbZ3fsmvXKZylhJEAyOTZrGd1_sto3xsHMBhu-HBO689hp5xS_giJQbsCbjHTv9jcp9-q63SKhZpb3DhMXSOIiE5ZkoNpnYZGXynh6U-4jBK7JnVfBYJo9QvgjtEya1cj8QwFq0TMz4lZqxTBg0hOF5m1jifI2Lf7Bc490CyxUu1rhc4GLGPOEdhg6Mjq92V44xxanFDhWv4lRjA6MlxZWbIh17DYTf2pAPvGrADphwGMmfbq7mFYURX-jLwCVA91bWg8YYunO69Y8vMgPFI2vvGnOZ-2Owsd0S9UOVpvP29mKoHc_b2nfpYHQLgdrrsUzLvDxALrHcS9hJqeuzOB6avBCN3mciBz5N0y_wxZ0J) - [Docs](https://mermaid.js.org/syntax/gantt.htm)
- Jedina dostupna interakcija sa chart-om je klik funkcija na task
- Sve funkcionalnosti moraju da se dodaju rucno

# gantt-schedule-timeline-calendar - [Demo](https://gantt-schedule-timeline-calendar.neuronet.io/gstc/examples/complex-1/index.html) - [Docs](https://gantt-schedule-timeline-calendar.neuronet.io/documentation/getting-started)
- Najlaksi za modifikaciju
- Podrzava akcije poput klika i update-a za bilo koji deo chart-a
- Koristi API, postoji besplatan trial koji mora da se obnavlja svaka 2 meseca
- Podrzava podprojekte ali pocetak i kraj taska roditelja je nepovezan sa potomcima (Task roditelj nema nikakve veze sa potomcima sem sto potomci mogu da se sakriju. Moguce je resiti preko akcija)

# Custom
- Vremenski zahtevno
- Zahteva dodatno istrazivanje biblioteka za grafiku poput [D3js](https://d3js.org/)
