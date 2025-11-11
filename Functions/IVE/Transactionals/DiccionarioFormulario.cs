using System.Text.Json.Serialization;

namespace HeroServer
{
	public class DiccionarioFormulario
	{
        public DiccionarioCamposMinimos[] Titulares { get; set; }
        public DiccionarioProductoServicio[] Productos { get; set; }
        public DiccionarioPerfilEconomicoTransaccional PerfilEconomico { get; set; }


		public DiccionarioFormulario(DiccionarioCamposMinimos[] titulares, DiccionarioProductoServicio[] productos, 
									 DiccionarioPerfilEconomicoTransaccional perfilEconomico)
		{
			Titulares = titulares;
			Productos = productos;
			PerfilEconomico = perfilEconomico;
		}
	}
}
