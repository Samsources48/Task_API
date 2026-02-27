using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Utilidades
{
    public static class GlobalUtils
    {
        public static string ObtenerPrimerValorValido(params string[] valores)
        {
            return valores.FirstOrDefault(valor => !string.IsNullOrEmpty(valor)) ?? "";
        }
        public static long? ObtenerPrimerValorValido(params long?[] valores)
        {
            return valores.FirstOrDefault(valor => valor.HasValue) ?? null;
        }
    }
}
