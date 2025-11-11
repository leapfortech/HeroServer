using System;

namespace HeroServer
{
	public class DiccionarioInformacionNegocioPropio
	{
		public String NombreComercial { get; set; }

        public DiccionarioInformacionNegocioPropio()
		{
		}

        public DiccionarioInformacionNegocioPropio(String nombreComercial)
		{
			NombreComercial = nombreComercial;
		}
	}
}
