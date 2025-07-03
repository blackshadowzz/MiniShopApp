namespace MiniShopApp.Models.Customers
{
    public class CustomerType : BaseEntity
    {
        public int Id { get; set; }
        public string? TypeName { get; set; }
        public double? DiscountRate { get; set; }
        public string? Description { get; set; }

        // Map to ViewCustomerType
        public ViewCustomerType ToViewCustomerType() => new ViewCustomerType
        {
            Id = this.Id,
            TypeName = this.TypeName,
            DiscountRate = this.DiscountRate,
            Description = this.Description,
            EditSeq = this.EditSeq,
            IsActive = this.IsActive,
            CreatedDT = this.CreatedDT,
            ModifiedDT = this.ModifiedDT,
            CreatedBy = this.CreatedBy,
            ModifiedBy = this.ModifiedBy
        };

        // Map from CustomerTypeDtoCreate
        public static CustomerType FromDtoCreate(CustomerTypeDtoCreate dto)
        {
            return new CustomerType
            {
                TypeName = dto.TypeName,
                DiscountRate = dto.DiscountRate,
                Description = dto.Description,
                EditSeq = dto.EditSeq,
                IsActive = dto.IsActive,
                CreatedDT = dto.CreatedDT,
                ModifiedDT = dto.ModifiedDT,
                CreatedBy = dto.CreatedBy,
                ModifiedBy = dto.ModifiedBy
            };
        }

        // Map from CustomerTypeDtoUpdate
        public static CustomerType FromDtoUpdate(CustomerTypeDtoUpdate dto)
        {
            return new CustomerType
            {
                Id = dto.Id,
                TypeName = dto.TypeName,
                DiscountRate = dto.DiscountRate,
                Description = dto.Description,
                EditSeq = dto.EditSeq,
                IsActive = dto.IsActive,
                CreatedDT = dto.CreatedDT,
                ModifiedDT = dto.ModifiedDT,
                CreatedBy = dto.CreatedBy,
                ModifiedBy = dto.ModifiedBy
            };
        }

        // Map to CustomerTypeDtoUpdate
        public CustomerTypeDtoUpdate ToDtoUpdate() => new CustomerTypeDtoUpdate
        {
            Id = this.Id,
            TypeName = this.TypeName,
            DiscountRate = this.DiscountRate,
            Description = this.Description,
            EditSeq = this.EditSeq,
            IsActive = this.IsActive,
            CreatedDT = this.CreatedDT,
            ModifiedDT = this.ModifiedDT,
            CreatedBy = this.CreatedBy,
            ModifiedBy = this.ModifiedBy
        };

        // Map to CustomerTypeDtoCreate
        public CustomerTypeDtoCreate ToDtoCreate() => new CustomerTypeDtoCreate
        {
            TypeName = this.TypeName,
            DiscountRate = this.DiscountRate,
            Description = this.Description,
            EditSeq = this.EditSeq,
            IsActive = this.IsActive,
            CreatedDT = this.CreatedDT,
            ModifiedDT = this.ModifiedDT,
            CreatedBy = this.CreatedBy,
            ModifiedBy = this.ModifiedBy
        };
    }

    // View model for displaying customer type data
    public class ViewCustomerType : BaseEntity
    {
        public int Id { get; set; }
        public string? TypeName { get; set; }
        public double? DiscountRate { get; set; }
        public string? Description { get; set; }
    }

    // DTO for creating a new customer type
    public class CustomerTypeDtoCreate : BaseEntity
    {
        public string? TypeName { get; set; }
        public double? DiscountRate { get; set; }
        public string? Description { get; set; }
    }

    // DTO for updating an existing customer type
    public class CustomerTypeDtoUpdate : BaseEntity
    {
        public int Id { get; set; }
        public string? TypeName { get; set; }
        public double? DiscountRate { get; set; }
        public string? Description { get; set; }
    }
}
