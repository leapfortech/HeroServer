using System;

namespace HeroServer
{
	public class DiccionarioInformacionEconomicaInicial
	{
		public double MontoIngresos { get; set; }
		public DiccionarioInformacionNegocioPropio[] NegocioPropio { get; set; }
		public DiccionarioInformacionRelacionDependencia[] RelacionDependencia { get; set; }
		public DiccionarioInformacionOtrosIngresos[] OtroIngresos { get; set; }
		public String PropositoRC { get; set; }

        public DiccionarioInformacionEconomicaInicial()
		{
		}


        public DiccionarioInformacionEconomicaInicial(double montoIngresos, DiccionarioInformacionNegocioPropio[] negocioPropio, 
													  DiccionarioInformacionRelacionDependencia[] relacionDependencia, 
													  DiccionarioInformacionOtrosIngresos[] otroIngresos, String propositoRC)
		{
			MontoIngresos = montoIngresos;
			NegocioPropio = negocioPropio;
			RelacionDependencia = relacionDependencia;
			OtroIngresos = otroIngresos;
			PropositoRC = propositoRC;
		}
	}
}
