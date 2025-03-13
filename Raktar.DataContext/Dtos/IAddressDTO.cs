namespace Raktar.DataContext.Dtos
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
