using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAplication.Models;

namespace WebAplication.Services
{
    public class ServicesS : IDbService
    {
        private readonly string _connection = "Data Source=db-mssql;Initial Catalog=s18621;Integrated Security=True";

        public async Task<PrescriptionRequest> GetPrescriptionById(int Id_Prescription)
        {
                var leki = new List<Medicament>();
                using (var client = new SqlConnection(_connection))
                using (var command = new SqlCommand())
                {
                    command.Connection = client;
                    command.CommandText = @"SELECT * FROM Medicament m INNER JOIN Prescription_medicament pm ON pm.IdMedicament = m.IdMedicament WHERE pm.IdPrescription = @IdPres";
                    command.Parameters.AddWithValue("IdPres", Id_Prescription);

                    client.Open();

                    var dataReader = await command.ExecuteReaderAsync();
                    while (await dataReader.ReadAsync())
                    {
                        var medicament = new Medicament();
                        medicament.IdMedicament = int.Parse(dataReader["IdMedicament"].ToString());
                        medicament.Name = dataReader["Name"].ToString();
                        medicament.Description = dataReader["Description"].ToString();
                        medicament.Type = dataReader["Type"].ToString();
                        leki.Add(medicament);
                    }
                    client.Close();

                }

                using (var client = new SqlConnection(_connection))
                using (var command = new SqlCommand())
                {
                    command.Connection = client;
                    command.CommandText = "SELECT 1 FROM Prescription WHERE IdPrescription = @IdPres";
                    command.Parameters.AddWithValue("IdPres", Id_Prescription);
                    client.Open();
                    var dr = await command.ExecuteReaderAsync();
                    while (await dr.ReadAsync())
                    {
                        var PRequest = new PrescriptionRequest();
                        PRequest.IdPrescription = int.Parse(dr["IdPrescription"].ToString());
                        PRequest.Date = DateTime.Parse(dr["Date"].ToString());
                        PRequest.DueDate = DateTime.Parse(dr["DueDate"].ToString());
                        PRequest.Leki = leki;

                        return PRequest;
                    }
                }
            return null;
           
        }
        public async Task<Doc> GetDoctor(int id)
        {
            using (var conn = new SqlConnection(_connection))
            {
                using (var com = new SqlCommand())
                {
                    com.Connection = conn;
                    com.CommandText = @"SELECT iddoctor
                                        FROM Doctor WHERE iddoctor = @doc";
                    com.Parameters.AddWithValue("doc", id);
                    conn.Open();
                    try
                    {
                        var reader = await com.ExecuteReaderAsync();
                        await reader.ReadAsync();
                        var doct = new Doc
                        {
                            IdDoctor = int.Parse(reader["iddoctor"].ToString()),
                        };
                        return doct;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            }
        }

        public async Task<Pat> GetPatient(int id)
        {
            using (var conn = new SqlConnection(_connection))
            {
                using (var com = new SqlCommand())
                {
                    com.Connection = conn;
                    com.CommandText = @"SELECT idpatient
                                        FROM Patient WHERE idpatient = @pat";
                    com.Parameters.AddWithValue("pat", id);
                    conn.Open();
                    try
                    {
                        var reader = await com.ExecuteReaderAsync();
                        await reader.ReadAsync();
                        var pati = new Pat
                        {
                            IdPatient = int.Parse(reader["idpatient"].ToString()),
                        };
                        return pati;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            }
        }
        public async Task Register(DateTime d, DateTime dued, int idpat, int iddoc)
        {
            using (var conn = new SqlConnection(_connection))
            {
                using (var com = new SqlCommand())
                {
                    conn.Open();
                    var transaction = conn.BeginTransaction();

                    com.Connection = conn;
                    com.Transaction = transaction;
                    com.CommandText =
                        @"
                            SET IDENTITY_INSERT Prescription ON
                            DECLARE @_maxid int
                          SELECT @_maxid = MAX(idprescription) + 1
                          FROM Presciption                       
                          INSERT INTO Prescription VALUES (@_maxid, @_d, @_dued, @_idpat, @_iddoc)";
                    com.Parameters.AddWithValue("_d", d);
                    com.Parameters.AddWithValue("_dued", dued);
                    com.Parameters.AddWithValue("_idpat", idpat);
                    com.Parameters.AddWithValue("_iddoc", iddoc);
                    await com.ExecuteNonQueryAsync();
                    try
                    {
                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                    }
                }
            }
        }
    }
}
