# ClinicReservation
##### PC Clinic Reservation System of BITNP
## Introduction
This is a PC Clinic Reservation System of NetPioneer Association in BIT.
## Features
* User: Apply repair reservation
* User: Write comments of reservation details
* User: SMS Notification
* Member: Accept repair reservation
* Member: Write comments of reservation details
* Member: SMS Notification
## Notice
* Clone all the project and add "appsettings.json" for the settings of environment and "ticket.txt" with ONE LINE as the key of creating clinic member account to "ClinicReservation" folder (not the root).  
An example of "appsettings.json":
```json
{
  "ConnectionStrings": {
    "reservationData": "YOUR OWN CONNECTION STRING"
  },
  "SessionName": ".CLINIC_RES",
  "SecurityKey": "YOUR OWN SECURITY KEY",
  "SMSUrl": "YOUR OWN SMS POST URL",
  "SMSApiKey": "YOUR OWN SMS API KEY",

  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Warning"
    }
  }
}
```