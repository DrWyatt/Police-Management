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
pmaddpd <Department's Acronym> <Department's Name>   | Adds a new Police Department
pmaddfd <Department's Acronym> <Department's Name>   | Adds a new Fire Department
pmremovepd <Department's ID>  | Removes an existing Police Department
pmremovefd <Department's ID>  | Removes an existing Fire Department
pmlistpds  | Lists all Police Departments
pmlistfds  | Lists all Fire Departments
pmclearpds | Removes all exiting Departments
pmclearfds | Removes all exiting Departments
pmaddcop <Player's ID> <Department's ID> <Callsign> | Adds a new Cop
pmaddff <Player's ID> <Department's ID> <Callsign> | Adds a new Firefighter
pmremovecop <Cop's ID> | Removes an existing Cop
pmremoveff <Firefighter's ID> | Removes an existing Firefighter
pmlistcops | Lists all Cops
pmlistffs | Lists all Firefighters
pmclearcops | Removes all exiting Cops
pmclearffs | Removes all exiting Cops
pmaddadmin <Player's ID> | Adds a new Admin
pmremoveadmin <Admin's ID> | Removes an existing Admin
pmlistadmins | Lists all Admins
pmclearadmins | Removes all existing Admins
pmcleardb | Flushes Database
  
## In-Game Commands
All in-game commands are the same as the RCON ones with a "/" in front of it, "pmaddadmin" and "pmremoveadmin" are not available in-game.


Command  | Function
------------- | -------------
/cops | Lists all on-duty cops
/firefighters | Lists all on-duty firefighters
/odc or /ondutycop | Get on-duty as a Police Officer
/odf or /ondutyfire | Get on-duty as a firefighter
/ofd or /offduty | Get off-duty
/funitid [Callsign] | Change your FD callsign
/punitid [Callsign] | Change your PD callsign


## Events (For Developers)

Event's Name  | Event's Arguments | Event's Description
------------- | ------------- | -------------
"pm:isAdmin" | int playerSID, string eventName | If player is an Admin, trigger eventName
"pm:isCop" | int playerSID, string eventName | If player is a Cop, trigger eventName
"pm:isFire" | int playerSID, string eventName | If player is a Cop, trigger eventName
"pm:triggerToAllCops" | string eventName, List<object> arguments | Triggers eventName using arguments to all on-duty cops
"pm:triggerToAllFire" | string eventName, List<object> arguments | Triggers eventName using arguments to all on-duty firefighters
"pm:triggerToAllAdmins" | string eventName, List<object> arguments | Triggers eventName using arguments to all admins
"pm:triggerToAllOnDuty" | string eventName, List<object> arguments | Triggers eventName using arguments to all on-duty cops and firefighters
"pm:triggerToAllDepartment" | int departmentID, string eventName, List<object> arguments | Triggers eventName using arguments to whole department

## WIP
**>** ??? (Send suggestions as Enhancements in [here](https://github.com/DrWyatt/Police-Management/issues)