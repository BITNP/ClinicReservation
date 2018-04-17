using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using ClinicReservation.Models.Data;
using ClinicReservation.Services.Database;

namespace ClinicReservation.Validates
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ReservationAttribute : ValidationSetResultAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is int reservation))
                return new ValidationResult("The input value is not a validate int");

            IDbQuery query = validationContext.GetService<IDbQuery>();
            Reservation reservationInstance = query.TryGetReservation(reservation);
            if (reservationInstance == null)
                return new ValidationResult($"The requested reservation with id {reservation} is not found in database");
            TrySetResult(validationContext, reservationInstance);
            return ValidationResult.Success;
        }
    }
}
