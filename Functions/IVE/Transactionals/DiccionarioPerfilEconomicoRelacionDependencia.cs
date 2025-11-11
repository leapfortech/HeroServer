using System;

namespace HeroServer
{
	public class DiccionarioPerfilEconomicoRelacionDependencia
	{
		public String Sector { get; set; }
		public String NombreEmpleador { get; set; }
		public String PrincipalActividadEconomicaEmpleador { get; set; }
		public String PuestoDesempenia { get; set; }
		public String DireccionEmpleador { get; set; }
		public DiccionarioLugar Lugar { get; set; }
		public DiccionarioIngresos Ingresos { get; set; }



		public DiccionarioPerfilEconomicoRelacionDependencia(String sector, String nombreEmpleador, String principalActividadEconomicaEmpleador, String puestoDesempenia,
														     String direccionEmpleador, String direccionNegocio, DiccionarioLugar lugar, DiccionarioIngresos ingresos)
		{
			Sector = sector;
			NombreEmpleador = nombreEmpleador;
			PrincipalActividadEconomicaEmpleador = principalActividadEconomicaEmpleador;
			PuestoDesempenia = puestoDesempenia;
			DireccionEmpleador = direccionEmpleador;
			Lugar = lugar;
			Ingresos = ingresos;
		}

		public DiccionarioPerfilEconomicoRelacionDependencia(Dependencia dependencia, String sector, DiccionarioLugar lugar, DiccionarioIngresos ingresos)
		{
			Sector = sector;
			NombreEmpleador = dependencia.NombreEmpleador;
			PrincipalActividadEconomicaEmpleador = dependencia.PrincipalActividadEconomicaEmpleador;
			PuestoDesempenia = dependencia.PuestoDesempenia;
			DireccionEmpleador = dependencia.DireccionEmpleador;
			Lugar = lugar;
			Ingresos = ingresos;
		}
	}
}
