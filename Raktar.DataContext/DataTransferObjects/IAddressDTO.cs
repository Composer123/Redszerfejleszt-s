namespace Raktar.DataContext.DataTransferObjects
{
    public interface IAddressDTO
    {
        int AddressId { get; set; }
        string StreetName { get; set; }
        string City { get; set; }
        string State { get; set; }
        string PostalCode { get; set; }
        string Country { get; set; }
        SettlementDTO Settlement { get; set; }
    }

    public interface IAddressCreateDTO
    {
        SettlementDTO Settlement { get; set; }
    }
}
