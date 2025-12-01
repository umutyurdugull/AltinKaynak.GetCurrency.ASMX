using System;

namespace AltinKaynak.GetCurrency.ASMX.Model
{
    public class Kur
    {
        public int Id { get; set; }
        public double Alis { get; set; }
        public double Satis { get; set; }
        public string Aciklama { get; set; }
        public string KurTipi { get; set; }
        public string KurKodu { get; set; }
        public DateTime? GuncellemeZamani { get; set; }
    }
}
