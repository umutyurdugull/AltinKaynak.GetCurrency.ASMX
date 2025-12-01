using Microsoft.EntityFrameworkCore;
using ServiceReference1;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;


public class Kur
{
    /*
        Dönen xml dataları : 
           1. Kur 
           2. Kod
           3. Aciklama
           4. Alis
           5. Satis
           6. GuncellenmeZamani
        */
    public int Id { get; set; }
    public double Alis { get; set; }
    public double Satis { get; set; }
    public string Aciklama { get; set; }
    public string KurTipi { get; set; }
    public string KurKodu { get; set; }
    public DateTime? GuncellemeZamani { get; set; }

}
public class Altin
{
    public int Id { get; set; }
    public double Alis { get; set; }
    public double Satis { get; set; }
    public string Aciklama { get; set; }
    public string AltinTipi { get; set; }
}

public class KurContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=127.0.0.1,1433;Database=AltinKaynak1;User Id=sa;Password=Passw0rd;TrustServerCertificate=True;Encrypt=True;");

    }
    public DbSet<Kur> Kurlar { get; set; }
    public DbSet<Altin> Altinlar  { get;set;}
    class Program
    {

        static async Task Main(String[] args)
        {
            await GetCurrency();
            await GetGold();

        }
        private static async Task GetGold()
        {
            var client = new DataServiceSoapClient(DataServiceSoapClient.EndpointConfiguration.DataServiceSoap);
            var header = new AuthHeader
            {
                Password = "AltinkaynakWebServis",
                Username = "AltinkaynakWebServis",
            };
            var response = await client.GetGoldAsync(header);
            Console.WriteLine(response.GetGoldResult);
             /*
              Verilen outputlar :
             1 - Kur
             2 - Kod
             3 - Aciklama
             4 - Alis
             5 - Satis 
             6 - GuncellenmeZamani
              */
             var xmlstring = response.GetGoldResult;
             XmlDocument data = new XmlDocument();
            data.LoadXml(xmlstring);
            var altinlar = new List<dynamic>();
            XmlNodeList nodes = data.GetElementsByTagName("Kur");
            using(var db  = new KurContext() )
            {
                db.Database.EnsureCreated();
                foreach (XmlNode node in nodes)
                {
                   var kod = node["Kod"]?.InnerText;
                    var aciklama = node["Aciklama"]?.InnerText;
                    var alis = double.Parse(node["Alis"]?.InnerText ?? "0", CultureInfo.InvariantCulture);
                    var satis = double.Parse(node["Satis"]?.InnerText ?? "0", CultureInfo.InvariantCulture);
                    var exists = db.Altinlar.FirstOrDefault(a=>a.AltinTipi == kod);
                    if(exists != null)
                    {
                        exists.Alis = alis;
                        exists.Satis = satis;
                        exists.Aciklama = aciklama;
                    }
                    else
                    {
                        db.Altinlar.Add(new Altin
                        {
                            AltinTipi = kod,
                            Aciklama = aciklama,
                            Alis = alis,
                            Satis = satis
                        });
                    }
                }
                db.SaveChanges();
                Console.WriteLine("Altın Eklendi");
            }
        }
        private static async Task GetCurrency()
        {
            var client = new DataServiceSoapClient(DataServiceSoapClient.EndpointConfiguration.DataServiceSoap);

            var header = new AuthHeader
            {
                Username = "AltinkaynakWebServis",
                Password = "AltinkaynakWebServis"
            };
            var response = await client.GetCurrencyAsync(header);
            //XML olarak dönüyo
            //1. Deneme : https://stackoverflow.com/questions/55828/how-does-one-parse-xml-files
            /*
             Dönen xml dataları : 
                1. Kur 
                2. Kod
                3. Aciklama
                4. Alis
                5. Satis
                6. GuncellenmeZamani
             */
            var xmlString = response.GetCurrencyResult;
            XmlDocument data = new XmlDocument();
            data.LoadXml(xmlString);
            var kurlar = new List<dynamic>();
            XmlNodeList nodes = data.GetElementsByTagName("Kur");
            using (var db = new KurContext())
            {
                db.Database.EnsureCreated();
                foreach (XmlNode node in nodes)
                {
                    var kod = node["Kod"]?.InnerText;
                    var aciklama = node["Aciklama"]?.InnerText;
                    var alis = double.Parse(node["Alis"]?.InnerText ?? "0", CultureInfo.InvariantCulture);
                    var satis = double.Parse(node["Satis"]?.InnerText ?? "0", CultureInfo.InvariantCulture);

                    var guncelText = node["GuncellenmeZamani"]?.InnerText;
                    DateTime guncel = DateTime.MinValue;
                    if (!string.IsNullOrEmpty(guncelText))
                    {
                        bool parsed = DateTime.TryParseExact(
                            guncelText.Trim(),
                            "d.M.yyyy H:mm:ss",
                            CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None,
                            out guncel
                        );

                        if (!parsed)
                        {
                            Console.WriteLine($"GuncellemeZamani parse edilemedi: '{guncelText}'");
                        }
                    }

                    //Kodun burasını silmeyeceğim
                    //Yaptığım hata şurada, ben GuncellemeZamani olarak alirken dogrusu GuncellenmeZamani. Oradaki harfi yanlis okumam bana 15 dakika gibi bi süreye mal oldu.

                    var exists = db.Kurlar.FirstOrDefault(k => k.KurKodu == kod);
                    if (exists != null)
                    {
                        exists.Alis = alis;
                        exists.Satis = satis;
                        exists.Aciklama = aciklama;
                        exists.GuncellemeZamani = guncel;
                    }
                    else
                    {
                        db.Kurlar.Add(new Kur
                        {
                            KurKodu = kod,
                            Aciklama = aciklama,
                            Alis = alis,
                            Satis = satis,
                            KurTipi = "Döviz",
                            GuncellemeZamani = guncel
                        });
                    }
                }

                db.SaveChanges();
                Console.WriteLine("Döviz Eklendi");

            } //Şuan veri tabanında olan değerleri güncellemiyor, bunun yerine tabloda yeni satırlar açıyor.
              //Değerleri güncelliyor.
              //zamanı alırken null döndürüyor 
        }
    }
}