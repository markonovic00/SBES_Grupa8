# SBES_Grupa8

Projektni zadatak 3.

Implementirati klijent-servis model takav da proizvoljan broj LocalDatabase aplikacija (koje imaju ulogu lokalnih baza podataka) pristupa centralnoj bazi podataka CentralDatabase. Baza podataka je implementirana kao tekstualni fajl koji sadrži sledeće informacije: 1) jedinstveni identifikator u bazi podataka 2) region 3) grad 4) godina 5) potrošnja el.energije za mesece u toku godine.

Prilikom startovanja, svaki LocalDatabase klijent definiše svoj(e) region(e) od interesa, i radiće nad svojom lokalnom kopijom baze koja sadrže samo željene regione. Sihnronizacija svih LocalDatabase klijenata sa CentralDatabase bazom podataka koja se nalazi na posebnom serveru se vrši svaki put kad se izmeni neka od kopija. Dakle, svaki LocalDatabase može nezavisno da vrši obradu podataka nad lokalnom kopijom, dok se prilikom bilo kakve izmene automatski radi sinhronizacija sa CentralDatabase komponentom kako bi ostali klijenti odmah dobili notifikaciju da je bilo izmena u bazi podataka i osvežili svoje podatke od interesa.

CentralDatabase se predstavlja self-signed sertifikatom, koji predstavlja CA za sertifikate LocalDatabase komponenti. Obostrana autentifikacija između glavne i lokalnih baza podataka vrši se custom validacijom – komponenta koja ima ulogu CentralDatabase proverava da li je Issuer sertifikata LocalDatabase on sam, a komponente LocalDatabase proveravaju da li SubjectName sertifikata odgovara njenom izdavaocu.

LocalDatabase imaju svoje klijente sa kojima komuniciraju preko Windows autentifikacionog protokola, dok je autorizacija zasnovana na RBAC modelu. Svaki klijent,u zavisnosti od dodeljenih privilegija, može da:

prikaže odgovarajuće informacije iz baze (Read)
izračuna srednju vrednost godišnje potrošnje za određeni grad ili region (Read, Calculate)
ažurira potrošnju za aktuelni mesec (Read, Modify)
doda nove ili briše postojeće entitete iz baze podataka (Read, Administrate).
Poruke koje se razmenjuju između LocalDatabase komponente i njenih klijenata treba da budu kripotvane AES algoritmom u CBC modu.

Sve akcije nad centralnom bazom podataka koje podrazumevaju izmene podataka je potrebno logovati u custom kreirani Windows Event Log (na lokalnoj mašini), uključujući i proces sinhronizacije glavne baze podataka sa lokalnim kopijama (na mašini gde je podignuta glavna baza podataka).

Sifre za sertifikate i naloge: 1234
