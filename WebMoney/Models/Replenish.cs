using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebMoney.Models
{
    public class Replenish
    {
        [NotMapped]
        public string? AdditionalProperty { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get
            {
                return _id == 2 ? GenerateRandomId() : _id;
            }
            set { _id = value; }
        }
        private int _id;

        public string? IndificatorOrPhone { get; set; }
        public int? Amount { get; set; }
        public int UserId { get; set; }
        public DateTime Time { get; set; }
        public User User { get; set; }

        private int GenerateRandomId()
        {
            Random random = new Random();
            return random.Next(10000,9999999);
        }
    }


}
