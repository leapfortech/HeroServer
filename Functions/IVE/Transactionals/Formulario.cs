namespace HeroServer
{
	public class Formulario
	{
		public DiccionarioCamposMinimos[] Titulares { get; set; }
		public DiccionarioProductoServicio[] Productos { get; set; }
		public DiccionarioPerfilEconomicoTransaccional PerfilEconomico { get; set; }

		public Formulario(DiccionarioCamposMinimos[] titulares,
							 DiccionarioProductoServicio[] productos,
							 DiccionarioPerfilEconomicoTransaccional perfilEconomico)
		{
			Titulares = titulares;
			Productos = productos;
			PerfilEconomico = perfilEconomico;
		}
	}
}
