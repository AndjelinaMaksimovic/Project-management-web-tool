

# Sadržaj

1. [Potrebne instalacije](#_instalacije)

2. [Pokretanje](#_pokretanje)


<a name="_instalacije"></a>
<h3>Instalacije</h3>
Radi pokretanja web aplikacije neophodno je instalirati sledeće:

<br> 

##### Node.js i npm
Mogu se preuzeti sa [zvaničnog Node.js sajta](https://nodejs.org/en). Kako bi proverili da li je instaliran npm, menadžer paketa za Node.js, u terminalu ukucati komandu `npm` . Ukoliko se vrati greška da ova komanda ne postoji istu instalirati komandom u terminalu `npm install`. 

##### Angular CLI
Instalirati pomoću npm komande `npm install -g @angular/cli`.


##### .NET Core SDK
Preuzeti sa [sajta](https://dotnet.microsoft.com/en-us/download) i instalirati.

<a name="_pokretanje"></a>

Klonirati repoziturijum aplikacije sa gitlaba sledećom komandom u terminalu: <br>
`git clone https://gitlab.pmf.kg.ac.rs/si2024/codedberries.git`


<h3> Pokretanje</h3>

###### Pokretanje backenda
Folder iz koga je izvršeno kloniranje će sadržati kloniran repozitorijum. Kako bi se pokrenuo backend aplikacije treba se locirati u terminalu na folder u kome je Program.cs sledećom komandom: 

`cd Codedberries/Src/BE/Codedberries/Codedberries`

Za pokretanje razvojne verzije otucati:

`git checkout dev`

Za pokretanje produkcione verzije umesto prethodne komande otkucati:

`git checkout master`

Sa ove lokacije pokrenuti backend komandom:

`dotnet run`

Build aplikacije koji se dešava pri pozivu ove komande može malo potrajati. Kada bude gotov kao prvi od info: odgovora trebalo bi da bude naznačeno na kom portu radi backend.

##### Pokretanje frontenda
Otvoriti jos jedan terminal te se i u njemu locirati na folder u koji je kloniran repozitorijum. 
To možete jednostavno uraditi otvaranjem foldera u kome je repozitorijum upotrebom File explorer-a. Kada se nalazite u njemu, na desni klik trebalo bi da se otvori opcija  *Open in terminal*. Klikom na nju će se otvoriti terminal prozor sa odgovarajući radnim direktorijumom.
Pristupite angular-app folderu komandom:

`cd Codedberries/Src/FE/angular-app`

Za pokretanje razvojne verzije otucati:

`git checkout dev`

Za pokretanje produkcione verzije umesto prethodne komande otkucati:

`git checkout master`


Kako bi se pokrenuo frontend otkucati komandu:

`ng serve`

Kao odgovor na ovu komandu, dobija se port na kome se nalazi aplikacija. Isti kopirati u web browser. Ovime aplikacija bi trebalo da je uspešno pokrenuta.