<h1> Predlog hierarhije uloga korisnika alata</h1>

Ideja je da alat sadrži uloge koje će biti ispod navedene. Svaka od uloga imaće podrazumevane permisije koje su navedene ispod svake od njih u daljem tekstu, ali će pored toga postojati funkcionalnost dodele različitih permisija razlićitim ulogama.

<h3> Project owner</h3>

- Kreira projekat
- Poziva putem mejla nove korisnike na učešće u projektu, i tom prilikom im dodeljuje jednu od uloga ( project manager, viewer, user )
- Ima uvid u sve tekuće projekte
- Projektima dodelje project manager-a(e)
- Ima uvid u opterećenost zaposlenih
- Viewer korisnicima dodeljuje koje u koje projekte smeju da imaju uvid
  
<h3> Project manager</h3>

- Dodeljuje zadatke članovima projekta
- Ima uvid u projekte koji su mu dodeljeni
- Ima uvid u opterećenost zaposlenih na svom projektu
- Po potrebi projekte deli na potprojekte

<h3> User </h3>

- Ima uvid samo u svoj projekat
- Dobija zadatke od project manager-a
- Uvek ima uvid u sve svoje zadatke i zadatke sa kojima su njegovi međuzavisni
- Može da pregleda i sve zadatke na projektu na kome radi
-  Ima dozvolu za rad na svojim zadacima, dok u navedene ima samo uvid


 <h3> Viewer </h3>

 - Ima isključivo uvid u projekte za koje mu ih je dodelio project owner


 <h3> Super user </h3>
 
 - Održava alata
 - Ima sve navedene permisije
 - Po zahtevu project ownera može proširivati funkcionalnosti (npr. potrebno je dodati novu ulogu, ili novu vrstu permisija)


