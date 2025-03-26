namespace Raktar.DataContext.DataTransferObjects
{
    public interface IAddressDTO
    {
        int AddressId { get; set; }

    }

    public interface IAddressCreateDTO
    {
        SettlementDTO Settlement { get; set; }
    }
}
