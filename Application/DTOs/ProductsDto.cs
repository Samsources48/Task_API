using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class ProductsDto : AuditProperties
    {
        public int? IdProducto { get; set; }
        //[Required]
        public string NombreProducto { get; set; }
        public string? Descripcion { get; set; }
        //public decimal Precio { get; set; }
        //public int Stock { get; set; }
    }

    public class SaveProductsDto
    {
        public int? IdProducto { get; set; }
        public string? NombreProducto { get; set; }
        public string? Descripcion { get; set; }
    }
}
