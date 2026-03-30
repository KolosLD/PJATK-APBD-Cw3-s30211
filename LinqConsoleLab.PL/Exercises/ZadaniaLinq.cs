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

    
    public IEnumerable<string> Zadanie16_NajwyzszaOcenaKazdegoStudenta()
    {
        var wynik = from s in DaneUczelni.Studenci
            join z in DaneUczelni.Zapisy on s.Id equals z.StudentId
            where z.OcenaKoncowa.HasValue
            group z by new { s.Imie, s.Nazwisko } into grupa
            select $"{grupa.Key.Imie} {grupa.Key.Nazwisko}: Max ocena {grupa.Max(z => z.OcenaKoncowa.Value)}";

        return wynik;
    }

    
    public IEnumerable<string> Wyzwanie01_StudenciZWiecejNizJednymAktywnymPrzedmiotem()
    {
        var wynik = from s in DaneUczelni.Studenci
            join z in DaneUczelni.Zapisy on s.Id equals z.StudentId
            where z.CzyAktywny
            group z by new { s.Imie, s.Nazwisko } into grupa
            where grupa.Count() > 1
            select $"{grupa.Key.Imie} {grupa.Key.Nazwisko}: {grupa.Count()} aktywnych przedmiotów";

        return wynik;
    }

    
    public IEnumerable<string> Wyzwanie02_PrzedmiotyStartujaceWKwietniuBezOcenKoncowych()
    {
        var wynik = from p in DaneUczelni.Przedmioty
            where p.DataStartu.Year == 2026 && p.DataStartu.Month == 4
            join z in DaneUczelni.Zapisy on p.Id equals z.PrzedmiotId into zapisyPrzedmiotu
            where zapisyPrzedmiotu.All(z => !z.OcenaKoncowa.HasValue)
            select $"{p.Nazwa} (Start: {p.DataStartu:yyyy-MM-dd})";

        return wynik;
    }

    
    public IEnumerable<string> Wyzwanie03_ProwadzacyISredniaOcenNaIchPrzedmiotach()
    {
        var wynik = from pr in DaneUczelni.Prowadzacy
            join p in DaneUczelni.Przedmioty on pr.Id equals p.ProwadzacyId
            join z in DaneUczelni.Zapisy on p.Id equals z.PrzedmiotId
            where z.OcenaKoncowa.HasValue
            group z by new { pr.Imie, pr.Nazwisko } into grupa
            select $"{grupa.Key.Imie} {grupa.Key.Nazwisko}: Średnia ocen {grupa.Average(x => x.OcenaKoncowa.Value):F2}";

        return wynik;
    }

    
    public IEnumerable<string> Wyzwanie04_MiastaILiczbaAktywnychZapisow()
    {
        var wynik = from s in DaneUczelni.Studenci
            join z in DaneUczelni.Zapisy on s.Id equals z.StudentId
            where z.CzyAktywny
            group z by s.Miasto into grupa
            orderby grupa.Count() descending
            select $"{grupa.Key}: {grupa.Count()} aktywnych zapisów";

        return wynik;
    }

    private static NotImplementedException Niezaimplementowano(string nazwaMetody)
    {
        return new NotImplementedException(
            $"Uzupełnij metodę {nazwaMetody} w pliku Exercises/ZadaniaLinq.cs i uruchom polecenie ponownie.");
    }
}
