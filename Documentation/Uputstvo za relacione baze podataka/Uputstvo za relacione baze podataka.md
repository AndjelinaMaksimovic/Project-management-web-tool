# Relacione baze podataka

## sadržaj

[Uvod u relacione baze podataka](#_toc159885896)

[MYSQL](#_mysqlskok)

[instalacija](#_instalacijaMYSQL)

[konekcija](#__konekcijaMYSQl)

[kreiranje tabele i vršenje upita](#_kreiranjeMYSQL)

[SQLITE](#_sqliteskok)

[instalacija](#_instalacijaSQLite)

[konekcija](#__konekcijaSQlite)

[kreiranje tabele i vršenje upita](#_kreiranjeSQLite)

[RESURSI I KORISNI LINKOVI	](#_toc159885903)

<br>

<a name="_toc159885896"></a> Uvod u relacione baze podataka

Relaciona baza podataka je tip baze podataka koji organizuje podatke u tabele koje su povezane ili "relacionisane" putem ključeva. Ova vrsta baze podataka koristi se široko u različitim aplikacijama i sistemima za čuvanje, upravljanje i manipulaciju strukturiranim podacima.

Osnovni koncepti relacione baze podataka uključuju:

Tabele: Relaciona baza podataka sastoji se od više tabela. Svaka tabela predstavlja entitet ili objekat koji sadrži određene vrste podataka.

Ključevi: Ključevi su kolone u tabelama koje jedinstveno identifikuju svaki red veze: Relacione baze podataka koriste veze ili odnose (relationships) između tabela kako bi se organizovali podaci i omogućilo efikasno upravljanje njima. 

SQL: Structured Query Language (SQL) je jezik za upravljanje relacionim bazama podataka. SQL se koristi za kreiranje, čitanje, ažuriranje i brisanje podataka iz baze podataka.

Normalizacija: Normalizacija je proces organizovanja podataka u tabelama kako bi se smanjila redundancija podataka i osigurala doslednost i integritet podataka.

<a name="_mysqlskok"></a> MYSQL

 <a name="_instalacijaMYSQL"></a> instalacija

Potrebno je preuzeti MySQL Installer na zvaničnoj MySQL web stranici (https://dev.mysql.com/downloads/mysql/).  Nakon preuzimanja, pokrenite instalacioni fajl (.msi). To će pokrenuti MySQL Installer koji će vam omogućiti da instalirate MySQL Server i druge povezane alate.Pratite uputstva na ekranu kako biste konfigurisali MySQL Server, uključujući postavljanje lozinke za root korisnika. Kada završite sa konfiguracijom, završite instalaciju. To će instalirati MySQL Server na vašem računaru.

<a name="_konekcijaMYSQL"></a> konekcija

možete koristiti SqlConnection klasu iz System.Data.SqlClient namespace-a da biste se povezali sa MySQL bazom podataka putem ADO.NET-a. Evo kako to možete uraditi koristeći konekcioni string:

	string connectionString = "Server=hostname;Database=database_name;Uid=username;Pwd=password;";
 
	using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                // Otvorite konekciju
                connection.Open();

                // Ako je konekcija uspešna, ispišite poruku
                Console.WriteLine("Successfully connected to MySQL database!");

                // Rad sa bazom podataka
            }
            catch (Exception ex)
            {
                // U slučaju greške pri konekciji, ispišite grešku
                Console.WriteLine("Error connecting to MySQL database: " + ex.Message);
            }
        }

U ovom primeru, hostname, username, password i database_name treba da budu zamenjeni sa odgovarajućim vrednostima za vaš MySQL server i bazu podataka.

 <a name="_kreiranjeMYSQL"></a>kreiranje tabele i vršenje upita

Koristite SqlCommand klasu za kreiranje SQL naredbi koje želite da izvršite nad bazom podataka, kao što su kreiranje tabele ili izvršavanje SELECT upita.

 string createTableQuery = "CREATE TABLE IF NOT EXISTS MyTable (Id INT PRIMARY KEY, Name VARCHAR(255))";

Izvršite SQL naredbu: Koristite ExecuteNonQuery metodu SqlCommand objekta za izvršavanje SQL naredbi koje ne vraćaju rezultate (kao što je kreiranje tabele).

 	using (SqlCommand command = new SqlCommand(createTableQuery, connection))
                {
                    // Izvršite SQL naredbu za kreiranje tabele
                    command.ExecuteNonQuery();
                    
                }

<a name="_sqliteskok"></a> SQLITE 

<a name="_instalacijaSQLite"></a> instalacija

Posetite zvaničnu SQLite web stranicu (https://www.sqlite.org/download.html) i preuzmite odgovarajuću verziju.

 Nakon preuzimanja, raspakujte arhivu u željeni direktorijum.

 Ovaj korak nije obavezan, ali možete dodati putanju do direktorijuma sa SQLite binarnim datotekama u sistemsku promenljivu PATH kako biste mogli da pristupite sqlite3.exe iz bilo kog direktorijuma u komandnoj liniji. 
Nakon instalacije, možete pokrenuti SQLite Shell (komandna linija) tako što ćete otvoriti terminal i pokrenuti sqlite3 komandu.

<a name="_konekcijaSQLite"></a> konekcija

 Kreirajte konekcioni string koji sadrži putanju do SQLite baze podataka. Primer konekcionog stringa može izgledati ovako:

	string connectionString = @"Data Source=C:\putanja\do\baze\moja_baza.db;Version=3;";

Koristite SQLiteConnection klasu iz System.Data.SQLite namespace-a da biste se povezali sa SQLite bazom podataka.

	 using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            try
            {
                connection.Open();

                // Ako je konekcija uspešna, ispišite poruku
                Console.WriteLine("Successfully connected to SQLite database!");

            }
            catch (Exception ex)
            {
                // U slučaju greške pri konekciji, ispišite grešku
                Console.WriteLine("Error connecting to SQLite database: " + ex.Message);
            }
        }

<a name="_kreiranjeSQLite"></a>kreiranje tabele i vršenje upita

Da biste kreirali tabelu, koristite SQLiteCommand objekat sa CREATE naredbom.

	string createTableQuery = @"CREATE TABLE IF NOT EXISTS MyTable (
                            Id INTEGER PRIMARY KEY,
                            Name TEXT,
                            Age INTEGER)";

	using (SQLiteCommand command = new SQLiteCommand(createTableQuery, connection))
	{
    		command.ExecuteNonQuery();
	}


Da biste izvršili upite nad bazom podataka, koristite SQLiteCommand objekat sa odgovarajućim SQL naredbama i koristite SQLiteDataReader za čitanje rezultata.
	
	string selectQuery = "SELECT * FROM MyTable";

	using (SQLiteCommand command = new SQLiteCommand(selectQuery, connection))
	{
    	    using (SQLiteDataReader reader = command.ExecuteReader())
   	 {
       	     while (reader.Read())
         {
            int id = reader.GetInt32(0);
            string name = reader.GetString(1);
            int age = reader.GetInt32(2);

            Console.WriteLine($"Id: {id}, Name: {name}, Age: {age}");
        }
    }
}

<br><br>
<a name="_toc159885903"></a>RESURSI I KORISNI LINKOVI
<br>

https://www.sqlitetutorial.net/

https://www.youtube.com/watch?v=GMHK-0TKRVk

https://www.mysqltutorial.org/

https://www.youtube.com/watch?v=7S_tz1z_5bA