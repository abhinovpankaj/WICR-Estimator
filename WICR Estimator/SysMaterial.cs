namespace WICR_Estimator
{
    public class SysMaterial
    {
        
        public int? ProjectId { get; set; }
        
        public string MaterialName { get; set; }
        
        public decimal? MaterialPrice { get; set; }
        public int? Coverage { get; set; }
        public decimal? MaterialWeight { get; set; }
        
        public decimal? HorProdRate { get; set; }
        
        public decimal? VertProdRate { get; set; }
        
        public decimal? StairProdRate { get; set; }
        
        public decimal? MinChargePerHr { get; set; }
       
        public int MaterialId { get; set; }
    }
}