using ORMi;
using PSLNLExportUtility.Logic.Services.ExcelDataReader.Models;
using PSLNLExportUtility.Logic.Services.LebelWMI.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PSLNLExportUtility.Logic.Services.LebelWMI
{
    public class LenelService
    {
        private readonly WMIHelper _wmi;

        public LenelService()
        {
            _wmi = new WMIHelper("root\\onGuard");
        }

        public IEnumerable<Cardholder> GetCardholders()
        {
            return _wmi.Query<Cardholder>();
        }

        public void UpsertEmployee(Employee employee)
        {
        }
    }
}
