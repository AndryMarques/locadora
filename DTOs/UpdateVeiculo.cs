namespace locadora.DTOs
{
    public class UpdateVeiculo
    {
        public string Modelo { get; set; }
        public int Ano { get; set; }
        public double KM { get; set; }
        public int IdFabricante { get; set; }
    }
}
