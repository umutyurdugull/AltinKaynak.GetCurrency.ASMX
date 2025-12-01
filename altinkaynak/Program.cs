using ServiceReference1;
using System.Globalization;
using AltinKaynak.GetCurrency.ASMX;
using System.Xml;
using AltinKaynak.GetCurrency.ASMX.Model;

namespace AltinKaynak.GetCurrency.ASMX
{
    class Program
    {
        static async Task Main(string[] args)
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
            var xmlstring = response.GetGoldResult;

            XmlDocument data = new XmlDocument();
            data.LoadXml(xmlstring);
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

                    var exists = db.Altinlar.FirstOrDefault(a => a.AltinTipi == kod);
                    if (exists != null)
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
            var xmlString = response.GetCurrencyResult;

            XmlDocument data = new XmlDocument();
            data.LoadXml(xmlString);
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
            }
        }
    }
}
