namespace Raktar.DataContext.DataTransferObjects
{
    /// <summary>
    /// Can be one of two states. <see cref="LandRegistryNumberDTO"/> or <see cref="SimpleAddressDTO"/>.
    /// </summary>
    public interface IAddressDTO
    {
        public int AddressId { get; set; }
    }
    /// <summary>
    /// Can be one of two states. <see cref="LandRegistryNumberCreateDTO"/> or <see cref="SimpleAddressCreateDTO"/>.
    /// </summary>
    public interface IAddressCreateDTO
    {
        public SettlementDTO Settlement { get; set; }
    }
}
