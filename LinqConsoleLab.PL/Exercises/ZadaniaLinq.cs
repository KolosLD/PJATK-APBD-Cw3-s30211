using LinqConsoleLab.PL.Data;

namespace LinqConsoleLab.PL.Exercises;

public sealed class ZadaniaLinq
{
   
    public IEnumerable<string> Zadanie01_StudenciZWarszawy()
    {
        return DaneUczelni.Studenci
            .Where(s => s.Miasto == "Warsaw")
            .Select(s => $"{s.NumerIndeksu} {s.Imie} {s.Nazwisko} {s.Miasto}");
    }

    
    public IEnumerable<string> Zadanie02_AdresyEmailStudentow()
    {
        return DaneUczelni.Studenci
            .Select(s => s.Email);

    }

    
    public IEnumerable<string> Zadanie03_StudenciPosortowani()
    {
        return DaneUczelni.Studenci 
            .OrderBy(s => s.Nazwisko)
            .ThenBy(s => s.Imie)
            .Select(s => $"{s.NumerIndeksu} {s.Imie} {s.Nazwisko}");
    }

    
    public IEnumerable<string> Zadanie04_PierwszyPrzedmiotAnalityczny()
    {
        var przedmiot = DaneUczelni.Przedmioty
            .FirstOrDefault(p => p.Kategoria == "Analytics");
        
        if (przedmiot == null)
        {
            return new List<string> { "Nie znaleziono przedmiotu z kategorii Analytics." };
        }

        return new[] { przedmiot.Nazwa };
        
    }
    
    public IEnumerable<string> Zadanie05_CzyIstniejeNieaktywneZapisanie()
    {
        bool czyIstnieje = DaneUczelni.Zapisy.Any(z => !z.CzyAktywny);
        string odpowiedz = czyIstnieje ? "Tak" : "Nie";
        return new[] { $"Czy istnieje nieaktywne zapisanie? {odpowiedz}" };
    }
    
    
    public IEnumerable<string> Zadanie06_CzyWszyscyProwadzacyMajaKatedre()
    {
        bool czyWszyscyMaja = DaneUczelni.Prowadzacy.All(p => !string.IsNullOrWhiteSpace(p.Katedra));
        
        string odpowiedz = czyWszyscyMaja ? "Tak" : "Nie";
        return new[] { $"Czy wszyscy prowadzący mają przypisaną katedrę? {odpowiedz}" };
    }

    
    public IEnumerable<string> Zadanie07_LiczbaAktywnychZapisow()
    {
        int liczbaAktywnych = DaneUczelni.Zapisy.Count(z => z.CzyAktywny);
        return new[] { $"Liczba aktywnych zapisów: {liczbaAktywnych}" };
    }

    
    public IEnumerable<string> Zadanie08_UnikalneMiastaStudentow()
    {
        return DaneUczelni.Studenci
            .Select(s => s.Miasto)
            .Distinct()
            .OrderBy(miasto => miasto);
    }

    
    public IEnumerable<string> Zadanie09_TrzyNajnowszeZapisy()
    {
        return DaneUczelni.Zapisy
            .OrderByDescending(z => z.DataZapisu)
            .Take(3)
            .Select(z => $"Data: {z.DataZapisu:yyyy-MM-dd}, Student: {z.StudentId}, Przedmiot: {z.PrzedmiotId}");
    }

    
    public IEnumerable<string> Zadanie10_DrugaStronaPrzedmiotow()
    {
        const int pageSize = 2; 
        const int pageNumber = 2; 
        
        return DaneUczelni.Przedmioty
            .OrderBy(p => p.Nazwa)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => $"{p.Nazwa} [{p.Kategoria}]");
    }

    
    public IEnumerable<string> Zadanie11_PolaczStudentowIZapisy()
    {
        var wynik = from student in DaneUczelni.Studenci
            join zapis in DaneUczelni.Zapisy on student.Id equals zapis.StudentId
            select $"{student.Imie} {student.Nazwisko} - Data: {zapis.DataZapisu:yyyy-MM-dd}";

        return wynik;
    }

    
    public IEnumerable<string> Zadanie12_ParyStudentPrzedmiot()
    {
        var wynik = from zapis in DaneUczelni.Zapisy
            join student in DaneUczelni.Studenci on zapis.StudentId equals student.Id
            join przedmiot in DaneUczelni.Przedmioty on zapis.PrzedmiotId equals przedmiot.Id
            select $"{student.Imie} {student.Nazwisko} - {przedmiot.Nazwa}";

        return wynik;
    }

   
    public IEnumerable<string> Zadanie13_GrupowanieZapisowWedlugPrzedmiotu()
    {
        var wynik = from zapis in DaneUczelni.Zapisy
            join przedmiot in DaneUczelni.Przedmioty on zapis.PrzedmiotId equals przedmiot.Id
            group zapis by przedmiot.Nazwa into grupa
            select $"{grupa.Key}: {grupa.Count()} zapisów";

        return wynik;
    }

    
    public IEnumerable<string> Zadanie14_SredniaOcenaNaPrzedmiot()
    {
        var wynik = from zapis in DaneUczelni.Zapisy
            join przedmiot in DaneUczelni.Przedmioty on zapis.PrzedmiotId equals przedmiot.Id
            where zapis.OcenaKoncowa.HasValue
            group zapis by przedmiot.Nazwa into grupa
            select $"{grupa.Key}: {grupa.Average(z => z.OcenaKoncowa.Value):F2}";

        return wynik;
    }

    
    public IEnumerable<string> Zadanie15_ProwadzacyILiczbaPrzedmiotow()
    {
        var wynik = DaneUczelni.Prowadzacy.GroupJoin(
                DaneUczelni.Przedmioty,
                prowadzacy => prowadzacy.Id,
                przedmiot => przedmiot.ProwadzacyId,
                (prowadzacy, przedmiotyProwadzacego) => new
                {
                    ImieNazwisko = $"{prowadzacy.Imie} {prowadzacy.Nazwisko}",
                    LiczbaPrzedmiotow = przedmiotyProwadzacego.Count()
                })
            .Select(x => $"{x.ImieNazwisko}: {x.LiczbaPrzedmiotow} przedmiotów");

        return wynik;
    }

    /// <summary>
    /// Zadanie:
    /// Dla każdego studenta znajdź jego najwyższą ocenę końcową.
    /// Pomiń studentów, którzy nie mają jeszcze żadnej oceny.
    ///
    /// SQL:
    /// SELECT s.Imie, s.Nazwisko, MAX(z.OcenaKoncowa)
    /// FROM Studenci s
    /// JOIN Zapisy z ON s.Id = z.StudentId
    /// WHERE z.OcenaKoncowa IS NOT NULL
    /// GROUP BY s.Imie, s.Nazwisko;
    /// </summary>
    public IEnumerable<string> Zadanie16_NajwyzszaOcenaKazdegoStudenta()
    {
        throw Niezaimplementowano(nameof(Zadanie16_NajwyzszaOcenaKazdegoStudenta));
    }

    /// <summary>
    /// Wyzwanie:
    /// Znajdź studentów, którzy mają więcej niż jeden aktywny zapis.
    /// Zwróć pełne imię i nazwisko oraz liczbę aktywnych przedmiotów.
    ///
    /// SQL:
    /// SELECT s.Imie, s.Nazwisko, COUNT(*)
    /// FROM Studenci s
    /// JOIN Zapisy z ON s.Id = z.StudentId
    /// WHERE z.CzyAktywny = 1
    /// GROUP BY s.Imie, s.Nazwisko
    /// HAVING COUNT(*) > 1;
    /// </summary>
    public IEnumerable<string> Wyzwanie01_StudenciZWiecejNizJednymAktywnymPrzedmiotem()
    {
        throw Niezaimplementowano(nameof(Wyzwanie01_StudenciZWiecejNizJednymAktywnymPrzedmiotem));
    }

    /// <summary>
    /// Wyzwanie:
    /// Wypisz przedmioty startujące w kwietniu 2026, dla których żaden zapis nie ma jeszcze oceny końcowej.
    ///
    /// SQL:
    /// SELECT p.Nazwa
    /// FROM Przedmioty p
    /// JOIN Zapisy z ON p.Id = z.PrzedmiotId
    /// WHERE MONTH(p.DataStartu) = 4 AND YEAR(p.DataStartu) = 2026
    /// GROUP BY p.Nazwa
    /// HAVING SUM(CASE WHEN z.OcenaKoncowa IS NOT NULL THEN 1 ELSE 0 END) = 0;
    /// </summary>
    public IEnumerable<string> Wyzwanie02_PrzedmiotyStartujaceWKwietniuBezOcenKoncowych()
    {
        throw Niezaimplementowano(nameof(Wyzwanie02_PrzedmiotyStartujaceWKwietniuBezOcenKoncowych));
    }

    /// <summary>
    /// Wyzwanie:
    /// Oblicz średnią ocen końcowych dla każdego prowadzącego na podstawie wszystkich jego przedmiotów.
    /// Pomiń brakujące oceny, ale pozostaw samych prowadzących w wyniku.
    ///
    /// SQL:
    /// SELECT pr.Imie, pr.Nazwisko, AVG(z.OcenaKoncowa)
    /// FROM Prowadzacy pr
    /// LEFT JOIN Przedmioty p ON p.ProwadzacyId = pr.Id
    /// LEFT JOIN Zapisy z ON z.PrzedmiotId = p.Id
    /// WHERE z.OcenaKoncowa IS NOT NULL
    /// GROUP BY pr.Imie, pr.Nazwisko;
    /// </summary>
    public IEnumerable<string> Wyzwanie03_ProwadzacyISredniaOcenNaIchPrzedmiotach()
    {
        throw Niezaimplementowano(nameof(Wyzwanie03_ProwadzacyISredniaOcenNaIchPrzedmiotach));
    }

    /// <summary>
    /// Wyzwanie:
    /// Pokaż miasta studentów oraz liczbę aktywnych zapisów wykonanych przez studentów z danego miasta.
    /// Posortuj wynik malejąco po liczbie aktywnych zapisów.
    ///
    /// SQL:
    /// SELECT s.Miasto, COUNT(*)
    /// FROM Studenci s
    /// JOIN Zapisy z ON s.Id = z.StudentId
    /// WHERE z.CzyAktywny = 1
    /// GROUP BY s.Miasto
    /// ORDER BY COUNT(*) DESC;
    /// </summary>
    public IEnumerable<string> Wyzwanie04_MiastaILiczbaAktywnychZapisow()
    {
        throw Niezaimplementowano(nameof(Wyzwanie04_MiastaILiczbaAktywnychZapisow));
    }

    private static NotImplementedException Niezaimplementowano(string nazwaMetody)
    {
        return new NotImplementedException(
            $"Uzupełnij metodę {nazwaMetody} w pliku Exercises/ZadaniaLinq.cs i uruchom polecenie ponownie.");
    }
}
