using System.Collections.Generic;

namespace Bangk.Common.Dto
{
    public class ClientGEADto
    {
        public List<ClientDatosDto> clientes { get; set; }
    }

    public class ClientDatosDto
    {
        public DataGEADto datos { get; set; }
        public DataGEAExtrasMedDto datos_extras { get; set; }
    }

    public class DataGEAExtrasMedDto
    {
        public string identificacion_pagador { get; set; }
        public string nombre_pagador { get; set; }
    }

    public class ClientGEAPetDto
    {
        public List<DatosWrapperDto> clientes { get; set; }
    }

    public class DatosWrapperDto
    {
        public DataGEADto datos { get; set; }
        public DataGEAExtrasDto datosExtras { get; set; }
    }

    public class DataGEADto
    {
        public string identificacion;

        public int tipo_identificacion;
        public string cveafiliado { get; set; }
        public int tipo_cveafiliado { get; set; }
        public int cuenta { get; set; }
        public int plan { get; set; }
        public string fecha_inicio_vigencia { get; set; }
        public string fecha_fin_vigencia { get; set; }
        public string estado { get; set; }
        public string nombres { get; set; }
        public string email { get; set; }
        public string telefono_personal { get; set; }
    }

    public class DataGEAExtrasDto
    {
        public string tipo_mascota { get; set; }
        public int edad_mascota { get; set; }
        public string raza_mascota { get; set; }
        public string color_mascota { get; set; }
        public string nombre_mascota { get; set; }
    }
}
