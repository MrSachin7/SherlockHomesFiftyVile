using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fiftyville.Model
{
    [Table("passengers")]
    public class Passenger
    {
        [Column("flight_id")]public int FlightId { get; set; }
        [ Column("passport_number")]public long? PassportNumber { get; set; }
        [Column("seat")]public string Seat { get; set; }

        public override string ToString() {
            return $"FlightId : {FlightId} \t PassportNumber : {PassportNumber} \t Seat : {Seat}";
        }

    }
}