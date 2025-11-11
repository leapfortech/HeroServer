using System;

namespace HeroServer
{
	public class DiccionarioLugar
	{
		public String Pais { get; set; }
		public String Departamento { get; set; }
		public String Municipio { get; set; }

        public DiccionarioLugar()
		{
		}


        public DiccionarioLugar(String pais, String departamento, String municipio)
		{
			Pais = pais;
			if (departamento != null)
				Departamento = departamento;
			if (municipio != null)
			Municipio = municipio;
		}
	}
}
