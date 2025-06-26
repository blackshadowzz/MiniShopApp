namespace MiniShopApp.Models
{
    public class BaseEntity<TKeyType>
    {
        //Use to inherite tto model class
        public TKeyType Id { get; set; } = default!; //Provide table primary key type
        public int? EditSeq { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDT { get; set; }=DateTime.Now;
        public DateTime? ModifiedDT { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; } = string.Empty;
        public string? ModifiedBy { get; set; } = string.Empty;
    }
    public class BaseEntity
    {
        //Use to inherite tto model class
        public int? EditSeq { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDT { get; set; }
        public DateTime? ModifiedDT { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; } =string.Empty;
        public string? ModifiedBy { get; set; } = string.Empty;
    }
    public class BaseHelpEntity
    {
        //Use to inherite tto model class
        public int? EditSeq { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDT { get; set; } = DateTime.Now;
        public DateTime? ModifiedDT { get; set; } = DateTime.Now;
    }
}
