using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Consulting.Models
{
    [ModelMetadataType(typeof(WorkSessionMetaData))]
    public partial class WorkSession : IValidatableObject
    {
        ConsultingContext _context = new ConsultingContext();
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
           // ConsultingContext _context = new ConsultingContext();
            var contract = _context.Contract.Where(a => a.ContractId == ContractId).FirstOrDefault();
            //   var contract = _context.Contract.Where(a => a.ContractId == ContractId).FirstOrDefault();
            if (contract != null)
            {
                if (contract.Closed)
                {
                    yield return new ValidationResult("Contract can not be closed.", new[] { "ContractID" });
                }
            }
            else
            {
                yield return new ValidationResult("No contract found.", new[] { "ContractID" });
            }
            
           
            var consultant = _context.Consultant.Where(a => a.ConsultantId == ConsultantId).FirstOrDefault();
            if(consultant == null)
            {
                yield return new ValidationResult("No consultant found.", new[] { "ConsultantId" });
            }

            if(DateWorked != null)
            {
                if(DateWorked > DateTime.Now)
                {
                    yield return new ValidationResult("Date Worked Can not be future.", new[] { "DateWorked" });
                }
                if(contract != null && DateWorked < contract.StartDate)
                {
                    yield return new ValidationResult("Date Worked Can not be before contract start Date.", new[] { "DateWorked" });
                }
            }

            if(HoursWorked <= 0)
            {
                yield return new ValidationResult("Hours Worked Can not be > 0", new[] { "HoursWorked" });
            }

            var totalHoursWorked = _context.WorkSession.Where(a => a.ConsultantId == ConsultantId &&
                                                                    a.DateWorked == DateWorked)
                                                                .Sum(a => a.HoursWorked);

            if(totalHoursWorked + HoursWorked > 24)
            {
                yield return new ValidationResult("Hours Worked in a Day should not exeed 24", new[] { "HoursWorked" });
            }

            if(WorkSessionId == 0)
            {
                HourlyRate = consultant.HourlyRate;
            }
            else
            {
                if(HourlyRate <= 0)
                    yield return new ValidationResult("Hours Rate cannot be 0 or less", new[] { "HourlyRate" });
                else if(HourlyRate * 1.5 > consultant.HourlyRate)
                {
                    yield return new ValidationResult("Hours Rate cannot be exeeding 1.5", new[] { "HourlyRate" });
                }
            }
            TotalChargeBeforeTax = HourlyRate * HoursWorked;

            var customer = _context.Customer.Where(a => a.CustomerId == contract.CustomerId).FirstOrDefault();
            var province = _context.Province.Where(a => a.ProvinceCode == customer.ProvinceCode).FirstOrDefault();

            ProvincialTax = TotalChargeBeforeTax * province.ProvincialTax;
        }
    }
    public class WorkSessionMetaData
    {
        public int WorkSessionId { get; set; }
        public int ContractId { get; set; }
        [Required]
        public DateTime DateWorked { get; set; }
        public int ConsultantId { get; set; }
        public double HoursWorked { get; set; }
        public string WorkDescription { get; set; }
        public double HourlyRate { get; set; }
        public double ProvincialTax { get; set; }
        public double TotalChargeBeforeTax { get; set; }

        public virtual Consultant Consultant { get; set; }
        public virtual Contract Contract { get; set; }
    }
}
