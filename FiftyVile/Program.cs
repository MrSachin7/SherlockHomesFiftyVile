// See https://aka.ms/new-console-template for more information

using System.Data;
using System.Runtime.ExceptionServices;
using System.Xml.XPath;
using FiftyVile.DataAccess;
using Fiftyville.Model;
using Fiftyville.PrintUtil;
using Microsoft.EntityFrameworkCore;

FiftyVilleContext ctx = new FiftyVilleContext();


// Get all crime scenes report at July 28, 2020
 IQueryable<CrimeSceneReport> crimeSceneReports = ctx.CrimeSceneReports.Where(report => report.Day == 28 && report.Month == 7 && report.Year == 2020);

 // Filter by chamberlin street
 crimeSceneReports=crimeSceneReports.Where(report => report.Street.Equals("Chamberlin Street"));


 // Printer.PrettyPrint<CrimeSceneReport>(crimeSceneReports.ToList()); 
// Here, we found out that the crime happened at 10.15 am and all the three witnesses interview transcripts mentions the courthouse.


// Lets take all the interviews that mentioned courthouse
 // IQueryable<Interview> interviews = ctx.Interviews.Where(interview => interview.Transcript.Contains("courthouse") && interview.Day==28 && interview.Month==7);
 // Printer.PrettyPrint(interviews.ToList());

// So, lets find out all the logs in between 10-11 on courthouse at July 28 .2020

// All logs at that day..
// IQueryable<CourtHouseSecurityLog> courtHouseSecurityLogs = ctx.CourtHouseSecurityLogs.Where(log => log.Year==2020 && log.Month==7 && log.Day==28);
// // Logs from 10-11
// courtHouseSecurityLogs = courtHouseSecurityLogs.Where(log => log.Hour > 9 && log.Hour < 11);
//
// Printer.PrettyPrint(courtHouseSecurityLogs.ToList());


                           // Logs of that day
IQueryable<CourtHouseSecurityLog> securityLogs = ctx.CourtHouseSecurityLogs.Where(log => log.Year == 2020 && log.Day == 28 && log.Month == 7);
securityLogs = securityLogs.Where(log => log.Hour==10 && log.Minute>=15 && log.Minute<=25 && log.Activity.Equals("exit"));
List<CourtHouseSecurityLog> securityLogsSuspect = securityLogs.ToList();
// Printer.PrettyPrint(securityLogsSuspect);


// ATM logs of all withdrawls
// IQueryable<AtmTransaction> atmTransactions = ctx.AtmTransactions.Where(transaction => transaction.TransactionType.Equals("withdraw"));
// atmTransactions = atmTransactions.Where(transaction => transaction.Year==2020 && transaction.Month==7 && transaction.Day==28);
// atmTransactions=atmTransactions.Where(transaction => transaction.AtmLocation.Equals("Fifer Street"));
// Printer.PrettyPrint(atmTransactions.ToList());

// Phone calls
IQueryable<PhoneCall> phoneCalls = ctx.PhoneCalls.Where(call => call.Year==2020 && call.Month==7 && call.Day==28);
phoneCalls = phoneCalls.Where(call => call.Duration<60);
List<PhoneCall> phoneCallsSus = phoneCalls.ToList();




IQueryable<Flight> flights = ctx.Flights.Where(flight => flight.Year==2020 && flight.Month==7 && flight.Day==29);

Airport airport = await ctx.Airports.FirstAsync(airport => airport.FullName.Equals("Fiftyville Regional Airport"));
flights = flights.Where(flight => flight.OriginAirportId == airport.Id);
flights = flights.OrderBy(flight => flight.Hour);
Flight flightWithTheTheftFlewAway = flights.First();
// Console.WriteLine(flightWithTheTheftFlewAway.Id);

// Printer.PrettyPrint(flights.ToList());

IQueryable<Passenger> passengers = ctx.Passengers.Where(passenger => passenger.FlightId ==flightWithTheTheftFlewAway.Id);
// passengers = passengers.Where(passenger => passenger.Seat.Equals("3B"));

List<Passenger> passengersSuspect = passengers.ToList();
// Printer.PrettyPrint(passengersSuspect);
 // passengers.ToList().ForEach(Console.WriteLine);

// Todo show troels on how the key was messed up and I literally lost 2 days googling...


// Make a new list of persons matching the information from the passenger
List<Person> personsOfPassengers = new List<Person>();
foreach (Passenger passenger in passengersSuspect) {
 Person person = ctx.People.First(person => person.PassportNumber.Equals(passenger.PassportNumber));
 personsOfPassengers.Add(person);
}
// Printer.PrettyPrint(personsOfPassengers);

// make a new list of persons matching the information of liscence plate

List<Person> personsLiscencePlate = new List<Person>();
foreach (CourtHouseSecurityLog securityLog in securityLogsSuspect) {
 Person person = ctx.People.First(person1 => person1.LicensePlate.Equals(securityLog.LicensePlate));
 personsLiscencePlate. Add(person);
}

// COmmon persons on persons of passengers and liscnece plate
List<Person> commonPersons = new List<Person>();

foreach (Person personLiscence in personsLiscencePlate) {
 foreach (Person personsOfPassenger in personsOfPassengers) {
  if (personLiscence.Equals(personsOfPassenger)) {
   commonPersons.Add(personLiscence);
  }
 }
}

// new list of persons matching the common suspicions to the phone calss

List<Person> personsCommonOnSuspiciosAndPhone = new List<Person>();

foreach (PhoneCall call in phoneCallsSus) {

 Person? person = commonPersons.Find(person1 => person1.PhoneNumber.Equals(call.Caller));
 if (person !=null) {
   personsCommonOnSuspiciosAndPhone.Add(person);
 }

}

Printer.PrettyPrint(personsCommonOnSuspiciosAndPhone);
Printer.PrettyPrint(phoneCallsSus);



