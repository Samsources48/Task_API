using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Utilidades
{
    public static class ParametersTypesValues
    {
        public static bool ConvertToBool(string valor)
        {
            try
            {
                var value = bool.TryParse(valor, out var result);
                return result;
            }
            catch (Exception)
            {

                //throw;
                return false;
            }
        }
        public static int ConvertToInt(string valor)
        {
            try
            {
                var value = int.TryParse(valor, out var result);
                return result;
            }
            catch (Exception)
            {
                //throw;
                return 0;
            }
        }
        public static long ConvertToLong(string valor)
        {
            try
            {
                var value = long.TryParse(valor, out var result);
                return result;
            }
            catch (Exception)
            {
                //throw;
                return 0;
            }
        }
        public static decimal ConvertToDecimal(string valor)
        {
            try
            {
                var value = decimal.TryParse(valor, out var result);
                return result;
            }
            catch (Exception)
            {
                //throw;
                return 0;
            }
        }
        public static DateTime ConvertToDateTime(string valor)
        {
            try
            {
                var value = DateTime.TryParse(valor, out var result);
                return result;
            }
            catch (Exception)
            {
                //throw;
                return DateTime.MinValue;
            }
        }
    }
}
