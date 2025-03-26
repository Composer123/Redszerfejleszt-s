namespace Raktar.DataContext.DataTransferObjects
{
    public interface IAddressDTO
    {
        public int AddressId { get; set; }

    }

    public interface IAddressCreateDTO
    {
        public SettlementDTO Settlement { get; set; }
    }
}
