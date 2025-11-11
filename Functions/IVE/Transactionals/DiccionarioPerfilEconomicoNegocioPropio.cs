using System;

namespace HeroServer
{
	public class DiccionarioPerfilEconomicoNegocioPropio
	{
		public String NombreComercial { get; set; }
		public String PrincipalActividadEconomica { get; set; }
		public String FechaInscripcionNegocio { get; set; }
		public int? NumeroRegistro { get; set; }
		public int? Folio { get; set; }
		public int? Libro { get; set; }
		public String DireccionNegocio { get; set; }
		public DiccionarioLugar Lugar { get; set; }
		public DiccionarioIngresos Ingresos { get; set; }



		public DiccionarioPerfilEconomicoNegocioPropio(String nombreComercial, String principalActividadEconomica, String fechaInscripcionNegocio, int? numeroRegistro,
													   int? folio, int? libro, String direccionNegocio, DiccionarioLugar lugar, DiccionarioIngresos ingresos)
		{
			NombreComercial = nombreComercial;
			PrincipalActividadEconomica = principalActividadEconomica;
			FechaInscripcionNegocio = fechaInscripcionNegocio;
			NumeroRegistro = numeroRegistro;
			Folio = folio;
			Libro = libro;
			DireccionNegocio = direccionNegocio;
			Lugar = lugar;
			Ingresos = ingresos;
		}

		public DiccionarioPerfilEconomicoNegocioPropio(Negocio negocio, DiccionarioLugar lugar, DiccionarioIngresos ingresos)
		{
			NombreComercial = negocio.NombreComercial;
			PrincipalActividadEconomica = negocio.PrincipalActividadEconomica;
			FechaInscripcionNegocio = negocio.FechaInscripcionNegocio == null? null : IveHelper.ConvertDateTimeToString(Convert.ToDateTime(negocio.FechaInscripcionNegocio));
			NumeroRegistro = negocio.NumeroRegistro;
			Folio = negocio.Folio;
			Libro = negocio.Libro;
			DireccionNegocio = negocio.DireccionNegocio;
			Lugar = lugar;
			Ingresos = ingresos;
		}
	}
}
