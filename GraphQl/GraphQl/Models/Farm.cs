using System.ComponentModel.DataAnnotations.Schema;

namespace GraphQl.Models
{
    public class Farm
    {
        public int Id { get; set; }
        public string FarmName { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public int FarmableArea { get; set; }
        public string CropType { get; set; }
        public string LegalDescription { get; set; }

        public ICollection<Activity> Activities { get; set; }
        [NotMapped]
        public ICollection<FarmableAcresRecord> FarmableAcresProgress { get; set; }

        public Farm()
        {

        }
    }
}
