using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeroServer
{
    public static class IveFunctions
    {
        public static async Task<Formulario> GetIve(IveRequest iveRequest)
        {
            long totalIncome = 0;

            List<DiccionarioInformacionRelacionDependencia> relacionesDependencia = [];
            List<DiccionarioInformacionNegocioPropio> negociosPropios = [];
            List<DiccionarioInformacionOtrosIngresos> otrosIngresos = [];

            for (int i = 0; i < iveRequest.Incomes.Length; i++)
            {
                totalIncome += Convert.ToInt64(iveRequest.Incomes[i].Amount);
                if (iveRequest.Incomes[i].TypeId == 1)
                    relacionesDependencia.Add(new DiccionarioInformacionRelacionDependencia(iveRequest.Incomes[i].Description));  // Relación de dependencia debe ser nombreEmpleador
                else if (iveRequest.Incomes[i].TypeId == 2)
                    negociosPropios.Add(new DiccionarioInformacionNegocioPropio(iveRequest.Incomes[i].Description));              // Negocio Propio debe ser nombreComercial
                else                                    
                    otrosIngresos.Add(new DiccionarioInformacionOtrosIngresos(iveRequest.Incomes[i].Description));                // Otros ingresos debe ser descripción
            }


            // Titulares

            DiccionarioCamposMinimos diccionarioCamposMinimos = new DiccionarioCamposMinimos
            {
                TipoActuacion = IveHelper.ConvertCR(1),            // 1=C=Cliente 2=R=Representante
                CalidadActua = null,                               // Si es R la descripción es obligatoria
                Lugar = new DiccionarioLugar
                {
                    Pais = await GenValuesFunctions.GetStringById("K-Country", 84, "Alpha2"),
                    Departamento = await GenValuesFunctions.GetStringById("K-State", 1, "Ive"),           // Obligatorio si Country = GT
                    Municipio = await GenValuesFunctions.GetStringById("K-City", 1, "Ive")                // Obligatorio si Country = GT
                },
                Fecha = IveHelper.ConvertDateTimeToString(DateTime.Today),
                Cliente = new DiccionarioDatosPersonales
                {
                    PrimerApellido = iveRequest.Identity.LastName1[..(iveRequest.Identity.LastName1.Length >= 15 ? 15 : iveRequest.Identity.LastName1.Length)],
                    SegundoApellido = iveRequest.Identity.LastName2[..(iveRequest.Identity.LastName2.Length >= 15 ? 15 : iveRequest.Identity.LastName2.Length)],
                    ApellidoCasada = iveRequest.Identity.LastNameMarried[..(iveRequest.Identity.LastNameMarried.Length >= 15 ? 15 : iveRequest.Identity.LastNameMarried.Length)],
                    PrimerNombre = iveRequest.Identity.FirstName1[..(iveRequest.Identity.FirstName1.Length >= 15 ? 15 : iveRequest.Identity.FirstName1.Length)],
                    SegundoNombre = iveRequest.Identity.FirstName2[..(iveRequest.Identity.FirstName2.Length >= 15 ? 15 : iveRequest.Identity.FirstName2.Length)],
                    OtrosNombres = iveRequest.Identity.FirstName3[..(iveRequest.Identity.FirstName3.Length >= 30 ? 30 : iveRequest.Identity.FirstName3.Length)],
                    FechaNacimiento = IveHelper.ConvertDateTimeToString(iveRequest.Identity.BirthDate),
                    Nacionalidades = await IveHelper.ConvertStringArray(iveRequest.Identity.NationalityIds),
                    Nacimiento = new DiccionarioLugar
                    {
                        Pais = await GenValuesFunctions.GetStringById("K-Country", iveRequest.Identity.BirthCountryId, "Alpha2"),
                        Departamento = iveRequest.Identity.BirthCountryId != 84 ? null : await GenValuesFunctions.GetStringById("K-State", iveRequest.Identity.BirthStateId, "Ive"),  // Obligatorio si Country = GT
                        Municipio = iveRequest.Identity.BirthCountryId != 84 ? null : await GenValuesFunctions.GetStringById("K-City", iveRequest.Identity.BirthCityId, "Ive")        // Obligatorio si Country = GT
                    },
                    CondicionMigratoria = null,          // Obligatorio según catálogo [MigratoryStatus] si pais nacimiento != GT
                    OtraCondicionMigratoria = null,      // Obligatorio si CondicionMigratoria = 8
                    Sexo = await GenValuesFunctions.GetStringById("K-Gender", iveRequest.Identity.GenderId, "Code"),
                    EstadoCivil = await GenValuesFunctions.GetStringById("K-MaritalStatus", iveRequest.Identity.MaritalStatusId, "Code"),
                    ProfesionOficio = null,              // Obligatoria hay que capturarla
                    TipoDocumentoIdentificacion = await GenValuesFunctions.GetStringById("K-IdentificationType", 1, "Code"),
                    NumeroDocumentoIdentificacion = iveRequest.Identity.DpiCui,
                    EmisionPasaporte = null,
                    Nit = null,
                    Telefonos = [ Convert.ToInt64(iveRequest.Phone.Replace("-", String.Empty)) ],
                    Email = iveRequest.Email,
                    DireccionResidencia = iveRequest.Residence.Address1,
                    Residencia = new DiccionarioLugar
                    {
                        Pais = await GenValuesFunctions.GetStringById("K-Country", iveRequest.Residence.CountryId, "Alpha2"),
                        Departamento = iveRequest.Residence.CountryId != 84 ? null : await GenValuesFunctions.GetStringById("K-State", iveRequest.Residence.StateId, "Ive"),         // Obligatorio si Country = GT
                        Municipio = iveRequest.Residence.CountryId != 84 ? null : await GenValuesFunctions.GetStringById("K-City", iveRequest.Residence.CityId, "Ive")               // Obligatorio si Country = GT
                    },
                    Pep = IveHelper.ConvertSN(2),
                    DatosPep = null,
                    ParienteAsociadoPep = IveHelper.ConvertSN(2),
                    DatosParienteAsociadoPep = null,
                    Cpe = IveHelper.ConvertSN(2)
                },
                Representante = null,
                InfoEconomica = new DiccionarioInformacionEconomicaInicial
                {
                    MontoIngresos = totalIncome,
                    NegocioPropio = negociosPropios.Count == 0 ? null : [..negociosPropios],
                    RelacionDependencia = relacionesDependencia.Count == 0 ? null : [..relacionesDependencia],
                    OtroIngresos = otrosIngresos.Count == 0 ? null : [..otrosIngresos],
                    PropositoRC = null      // Obligatorio
                }
            };

            // Productos

            // Perfil Economico

            return new Formulario([ diccionarioCamposMinimos ], null, null);
        }
    }
}