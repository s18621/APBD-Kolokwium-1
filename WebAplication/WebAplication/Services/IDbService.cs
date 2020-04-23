using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAplication.Models;

namespace WebAplication.Services
{
    public interface IDbService
    {

        public Task<PrescriptionRequest> GetPrescriptionById(int Id_Prescription);
        public Task<Doc> GetDoctor(int id);
        public Task<Pat> GetPatient(int id);

        public Task Register(DateTime d, DateTime dued, int idpat, int iddoc);
    }
}
