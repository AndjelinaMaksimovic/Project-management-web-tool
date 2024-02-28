# Instalacija Git-a:
Prvo što trebate učiniti je instalirati Git na vaš računar. Git je dostupan za sve glavne operativne sisteme (Windows, macOS, Linux). Možete preuzeti instalacione datoteke sa zvanične Git veb stranice (https://git-scm.com/), a zatim pratiti uputstva za instalaciju za vaš operativni sistem.

# Konfiguracija Git-a:
Nakon instalacije, potrebno je konfigurisati Git sa vašim korisničkim imenom i email adresom. Ovo se može uraditi pomoću sledećih komandi:

`git config --global user.name "Vaše Ime"`
`git config --global user.email "vaš@email.com"`
Ovo se radi samo jednom nakon instalacije Git-a.

# Kreiranje novog repozitorijuma:
Da biste koristili Git za upravljanje vašim kodom, prvo morate kreirati novi repozitorijum. To možete uraditi na Git hosting platformi poput GitLab-a ili lokalno na vašem računaru. Na primer, da biste kreirali novi repozitorijum lokalno, možete izvršiti sledeće korake:

`mkdir novi_repozitorijum`
`cd novi_repozitorijum`
`git init`

# Dodavanje i snimanje promena:
Kada izmenite datoteke u svom projektu, možete koristiti Git da pratite ove promene. Koristite sledeće komande:

`git add . `             # Dodajte sve promene za snimanje
`git commit -m "Opis promena"`   # Snimite promene sa odgovarajućom porukom

# Kloniranje udaljenog repozitorijuma:
Koristite komandu git clone kako biste klonirali udaljeni repozitorijum na lokalni računar. Na primer:

`git clone url_repzitorijuma`

# Slanje promena na udaljeni repozitorijum:
Ako koristite udaljeni repozitorijum (npr. GitLab), možete slati svoje lokalne promene na server koristeći komandu git push. Na primer:

`git push origin master `  # Slanje promena na udaljeni repozitorijum

# Povlačenje promena sa udaljenog repozitorijuma:
Ako drugi korisnici ili vi na drugom uređaju napravite promene na repozitorijumu, možete ih povući na svoj lokalni računar koristeći komandu git pull. Na primer:

`git pull origin master `  # Povlačenje promena sa udaljenog repozitorijuma

# Grane

Git grane su sredstvo kojim se omogućava paralelno razvijanje različitih delova projekta. Ovo omogućava timu da istovremeno radi na različitim funkcijama, a da ne ometaju jedni druge.

## Kreiranje grana:
Možete kreirati novu granu pomoću komande `git branch <ime_grane>`. Na primer:

`git branch nova_grana`

Ova komanda će kreirati novu granu nazvanu "nova_grana", ali neće vas prebaciti na tu granu. Da biste se prebacili na novu granu, koristite komandu git checkout:

`git checkout nova_grana`

Možete kombinovati ove dve komande u jednu koristeći -b opciju:

`git checkout -b nova_grana`

Ovo će kreirati novu granu i automatski vas prebaciti na nju.

## Rad na granama:
Nakon što se prebacite na granu, možete raditi na projektu kao i obično. Sve promene koje napravite biće specifične za tu granu.

## Spajanje grana:
Kada završite rad na nekoj grani i želite da te promene spojite nazad u glavnu granu (najčešće master ili main), koristite komandu git merge. Na primer, da spojite sadržaj grane nova_grana u glavnu granu:

`git checkout master` # Prebacivanje na granu na koju želimo spajati

`git merge nova_grana`

Ova komanda će spojiti promene iz `nova_grana` u trenutnu granu (u ovom slučaju, `master`).

## Rešavanje konflikata:
Ponekad, kada pokušate da spojite dve grane, Git može otkriti konflikte, što znači da ista linija u istoj datoteci ima različite promene u obe grane. Da biste rešili konflikt, morate ručno izmeniti datoteku u tekst editor-u kako biste odabrali ispravnu verziju. Nakon što ručno rešite konflikte, mozete nastaviti proces spajanja.

# Korisni linkovi:

https://www.youtube.com/watch?v=CvUiKWv2-C0

https://www.youtube.com/watch?v=8JJ101D3knE