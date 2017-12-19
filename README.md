# Police Management
For more scripts, beta releases and support join [my Discord](https://discord.me/drwyatt)!

## Brief Overview
"Police Management" is a response to all Police plugins available for FiveM that require MySQL, CouchDB or SQLite. "Police Management" doesn't require any outside database, as it includes its own database system that handles all information. It currently uses a very medieval form of triggering events, but it's all I could do, for now, will work on it in the future. "Police Management" is the base framework for plenty of plugins to come, including all my police-related scripts and others as Police Radios, 911 calls, etc...

## Installation
**1.** Download latest [release](https://github.com/DrWyatt/Police-Management/releases).

**2.** Copy the folder "[policepack]" that's located inside "In Resources" to your FiveM's Server RESOURCES folder.

**3.** Add "start policeManagement" to your server.cfg

## RCON Commands

Command  | Function
------------- | -------------
pmadddepartment <Department's Acronym> <Department's Name>   | Adds a new Department
pmremovedepartment <Department's ID>  | Removes an existing Department
pmlistdepartments  | Lists all Departments
pmcleardepartments | Removes all exiting Departments
pmaddcop <Player's ID> <Department's ID> <Callsign> | Adds a new Cop
pmremovecop <Cop's ID> | Removes an existing Cop
pmlistcops | Lists all Cops
pmclearcops | Removes all exiting Cops
pmaddadmin <Player's ID> | Adds a new Admin
pmremoveadmin <Admin's ID> | Removes an existing Admin
pmlistadmins | Lists all Admins
pmclearadmins | Removes all existing Admins
  
## In-Game Commands
All in-game commands are the same as the RCON ones with a "/" in front of it, "pmaddadmin" and "pmremoveadmin" are not available in-game.


Command  | Function
------------- | -------------
/cops | Lists all on-duty cops
/od or /onduty | Get on-duty as a Police Officer
/ofd or /offduty | Get off-duty as a Police Officer

## WIP
**>** ??? (Send suggestions as Enhancements in [here](https://github.com/DrWyatt/Police-Management/issues)
