using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace policeManagementServer
{
    public class PoliceManagementServer : BaseScript
    {
        Database database = new Database("policemanagement.db", true);

        List<Cop> cops = new List<Cop>();
        List<Firefighter> firefighters = new List<Firefighter>();
        List<Admin> admins = new List<Admin>();
        List<Department> departments = new List<Department>();
        List<FDepartment> fdepartments = new List<FDepartment>();

        public PoliceManagementServer()
        {
            //Add Commands
            RegisterCommand("pmaddpd", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { AddDepartment(source, args, rawCommand); }), true);
            RegisterCommand("pmaddadmin", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { AddAdmin(source, args, rawCommand); }), true);
            RegisterCommand("pmaddcop", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { AddCop(source, args, rawCommand); }), true);
            RegisterCommand("pmaddfd", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { AddFDepartment(source, args, rawCommand); }), true);
            RegisterCommand("pmaddff", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { AddFF(source, args, rawCommand); }), true);

            //List Commands
            RegisterCommand("pmlistpds", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { ListDepartments(source, args, rawCommand); }), true);
            RegisterCommand("pmlistfds", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { ListFDepartments(source, args, rawCommand); }), true);
            RegisterCommand("pmlistadmins", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { ListAdmins(source, args, rawCommand); }), true);
            RegisterCommand("pmlistcops", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { ListCops(source, args, rawCommand); }), true);
            RegisterCommand("pmlistffs", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { ListFFs(source, args, rawCommand); }), true);

            //Remove Commands
            RegisterCommand("pmremovepd", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { RemoveDepartment(source, args, rawCommand); }), true);
            RegisterCommand("pmremovefd", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { RemoveFDepartment(source, args, rawCommand); }), true);
            RegisterCommand("pmremoveadmin", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { RemoveAdmin(source, args, rawCommand); }), true);
            RegisterCommand("pmremovecop", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { RemoveCop(source, args, rawCommand); }), true);
            RegisterCommand("pmremoveff", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { RemoveFF(source, args, rawCommand); }), true);

            //Clean Commands
            RegisterCommand("pmcleardb", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { ClearDBC(source, args, rawCommand); }), true);
            RegisterCommand("pmclearpds", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { ClearDep(source, args, rawCommand); }), true);
            RegisterCommand("pmclearfds", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { ClearFDep(source, args, rawCommand); }), true);
            RegisterCommand("pmclearadmins", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { ClearAdm(source, args, rawCommand); }), true);            
            RegisterCommand("pmclearcops", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { ClearCops(source, args, rawCommand); }), true);
            RegisterCommand("pmclearffs", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { ClearFFs(source, args, rawCommand); }), true);

            //Default Events
            EventHandlers.Add("chatMessage", new Action<int, int, string, string>(ChatMessage));

            //Custom Events
            EventHandlers.Add("pm:isAdmin", new Action<int, string, string>(EIsAdmin));
            EventHandlers.Add("pm:isCop", new Action<int, string, string>(EIsCop));
            EventHandlers.Add("pm:isFire", new Action<int, string, string>(EIsFire));
            EventHandlers.Add("pm:triggerToAllCops", new Action<string, List<object>>(TAllCops)); //PD
            EventHandlers.Add("pm:triggerToAllFire", new Action<string, List<object>>(TAllFire)); //FD
            EventHandlers.Add("pm:triggerToAllAdmins", new Action<string, List<object>>(TAllAdmins));
            EventHandlers.Add("pm:triggerToAllOnDuty", new Action<string, List<object>>(TAllOnDuty)); //PD + FD
            EventHandlers.Add("pm:triggerToAllDepartment", new Action<bool, int, string, List<object>>(TAllDepartment));

            if (database.Read() == null)
                ClearDB();

            Tuple<List<Cop>, List<Admin>, List<Firefighter>, List<Department>, List<FDepartment>> tuple = database.Read();
            cops = tuple.Item1;
            admins = tuple.Item2;
            firefighters = tuple.Item3;
            departments = tuple.Item4;
            fdepartments = tuple.Item5;
        }

#region CUSTOM_EVENTS
        private void TAllFire(string eventName, List<object> args)
        {
            foreach (Firefighter cop in firefighters)
            {
                int id = 0;
                foreach (Player player in new PlayerList())
                {
                    if (cop.Hex == player.Identifiers.First().ToString())
                        id = Convert.ToInt32(player.Handle);

                }
                if (cop.OnDuty)
                {
                    if (args.Count >= 1)
                        TriggerClientEvent(GetPlayerFromSID(id), eventName, args);
                    else
                        TriggerClientEvent(GetPlayerFromSID(id), eventName);
                }
            }
        }

        private void TAllOnDuty(string eventName, List<object> args)
        {
            foreach (Cop cop in cops)
            {
                int id = 0;
                foreach (Player player in new PlayerList())
                {
                    if (cop.Hex == player.Identifiers.First().ToString())
                        id = Convert.ToInt32(player.Handle);

                }
                if (cop.OnDuty)
                {
                    if (args.Count >= 1)
                        TriggerClientEvent(GetPlayerFromSID(id), eventName, args);
                    else
                        TriggerClientEvent(GetPlayerFromSID(id), eventName);
                }
            }
            foreach (Firefighter cop in firefighters)
            {
                int id = 0;
                foreach (Player player in new PlayerList())
                {
                    if (cop.Hex == player.Identifiers.First().ToString())
                        id = Convert.ToInt32(player.Handle);

                }
                if (cop.OnDuty)
                {
                    if (args.Count >= 1)
                        TriggerClientEvent(GetPlayerFromSID(id), eventName, args);
                    else
                        TriggerClientEvent(GetPlayerFromSID(id), eventName);
                }
            }
        }

        private void TAllDepartment(bool isFire, int depID, string eventName, List<object> args)
        {
            if (!isFire)
            {
                foreach (Cop cop in cops)
                {
                    int id = 0;
                    foreach (Player player in new PlayerList())
                    {
                        if (cop.Hex == player.Identifiers.First().ToString())
                            id = Convert.ToInt32(player.Handle);

                    }
                    if (cop.OnDuty)
                    {
                        if (departments.IndexOf(cop.Department) == depID)
                        {
                            if (args.Count >= 1)
                                TriggerClientEvent(GetPlayerFromSID(id), eventName, args);
                            else
                                TriggerClientEvent(GetPlayerFromSID(id), eventName);
                        }
                    }
                }
            }
            else
            {
                foreach (Firefighter cop in firefighters)
                {
                    int id = 0;
                    foreach (Player player in new PlayerList())
                    {
                        if (cop.Hex == player.Identifiers.First().ToString())
                            id = Convert.ToInt32(player.Handle);

                    }
                    if (cop.OnDuty)
                    {
                        if (fdepartments.IndexOf(cop.Department) == depID)
                        {
                            if (args.Count >= 1)
                                TriggerClientEvent(GetPlayerFromSID(id), eventName, args);
                            else
                                TriggerClientEvent(GetPlayerFromSID(id), eventName);
                        }
                    }
                }
            }
        }

        private void TAllAdmins(string eventName, List<object> args)
        {
            foreach (Admin cop in admins)
            {
                int id = 0;
                bool isOn = false;
                foreach (Player player in new PlayerList())
                {
                    if (cop.Hex == player.Identifiers.First().ToString())
                    {
                        id = Convert.ToInt32(player.Handle);
                        isOn = true;
                    }
                }
                if (isOn)
                {
                    if (args.Count >= 1)
                        TriggerClientEvent(GetPlayerFromSID(id), eventName, args);
                    else
                        TriggerClientEvent(GetPlayerFromSID(id), eventName);
                }
            }
        }

        private void TAllCops(string eventName, List<object> args)
        {
            foreach (Cop cop in cops)
            {
                int id = 0;
                foreach(Player player in new PlayerList())
                {
                    if (cop.Hex == player.Identifiers.First().ToString())
                        id = Convert.ToInt32(player.Handle);
                }
                if (cop.OnDuty)
                {
                    if(args.Count >= 1)
                        TriggerClientEvent(GetPlayerFromSID(id), eventName, args);
                    else
                        TriggerClientEvent(GetPlayerFromSID(id), eventName);
                }
                    
            }
        }

        private void EIsCop(int sourceSID, string posEventName, string negEventName)
        {
            if (IsCop(sourceSID) && GetCopFromID(sourceSID).OnDuty)
                TriggerClientEvent(posEventName, GetCopFromID(sourceSID).Callsign, departments.IndexOf(GetCopFromID(sourceSID).Department));
            else
                TriggerClientEvent(negEventName);
        }

        private void EIsFire(int sourceSID, string posEventName, string negEventName)
        {
            if (IsFire(sourceSID) && GetFireFromID(sourceSID).OnDuty)
                TriggerClientEvent(posEventName, GetFireFromID(sourceSID).Callsign, fdepartments.IndexOf(GetFireFromID(sourceSID).Department));
            else
                TriggerClientEvent(negEventName);
        }

        private void EIsAdmin(int sourceSID, string posEventName, string negEventName)
        {
            if (IsAdmin(sourceSID))
                TriggerClientEvent(posEventName);
            else
                TriggerClientEvent(negEventName);
        }
#endregion

        private void ChatMessage([FromSource]int sourceCID, int sourceSID, string sourceName, string message)
        {
            string[] splitMessage = message.Split(' ');
            if (splitMessage[0] == "/odc" || splitMessage[0] == "/ondutycop")
            {
                if (IsCop(sourceSID)){
                    if (GetFireFromID(sourceSID) != null)
                        GetFireFromID(sourceSID).OnDuty = false;
                    Cop cop = GetCopFromID(sourceSID);
                    if (cop.OnDuty)
                    {
                        TriggerClientEvent(GetPlayerFromSID(sourceSID), "chatMessage", "DISPATCH", new[] { 255, 0, 0 }, "You are already on-duty as a Police Officer!");
                        CancelEvent();
                        return;
                    }
                    GetCopFromID(sourceSID).OnDuty = true;
                    TriggerClientEvent(GetPlayerFromSID(sourceSID), "chatMessage", "DISPATCH", new[] { 255, 0, 0 }, "You are now on-duty as a Police Officer!");
                }
                else
                {
                    TriggerClientEvent(GetPlayerFromSID(sourceSID), "chatMessage", "", new[] { 255, 0, 0 }, "You are not a Police Officer!");
                }
            }
            else if (splitMessage[0] == "/ofdc" || splitMessage[0] == "/offdutycop")
            {
                if (IsCop(sourceSID))
                {
                    Cop cop = GetCopFromID(sourceSID);
                    if (!cop.OnDuty)
                    {
                        TriggerClientEvent(GetPlayerFromSID(sourceSID), "chatMessage", "DISPATCH", new[] { 255, 0, 0 }, "You are already off-duty!");
                        CancelEvent();
                        return;
                    }
                    cop.OnDuty = false;
                    TriggerClientEvent(GetPlayerFromSID(sourceSID), "chatMessage", "DISPATCH", new[] { 255, 0, 0 }, "You are now off-duty!");
                }
                else
                {
                    TriggerClientEvent(GetPlayerFromSID(sourceSID), "chatMessage", "", new[] { 255, 0, 0 }, "You are not a Police Officer!");
                }
            }
            else if (splitMessage[0] == "/odf" || splitMessage[0] == "/ondutyfire")
            {
                if (IsFire(sourceSID))
                {
                    if (GetCopFromID(sourceSID) != null)
                        GetCopFromID(sourceSID).OnDuty = false;
                    Firefighter cop = GetFireFromID(sourceSID);
                    if (cop.OnDuty)
                    {
                        TriggerClientEvent(GetPlayerFromSID(sourceSID), "chatMessage", "DISPATCH", new[] { 255, 0, 0 }, "You are already on-duty as a Firefighter!");
                        CancelEvent();
                        return;
                    }
                    GetFireFromID(sourceSID).OnDuty = true;
                    TriggerClientEvent(GetPlayerFromSID(sourceSID), "chatMessage", "DISPATCH", new[] { 255, 0, 0 }, "You are now on-duty as a Firefighter!");
                }
                else
                {
                    TriggerClientEvent(GetPlayerFromSID(sourceSID), "chatMessage", "", new[] { 255, 0, 0 }, "You are not a Firefighter!");
                }
            }
            else if (splitMessage[0] == "/ofdf" || splitMessage[0] == "/offdutyfire")
            {
                if (IsFire(sourceSID))
                {
                    Firefighter cop = GetFireFromID(sourceSID);
                    if (!cop.OnDuty)
                    {
                        TriggerClientEvent(GetPlayerFromSID(sourceSID), "chatMessage", "DISPATCH", new[] { 255, 0, 0 }, "You are already off-duty!");
                        CancelEvent();
                        return;
                    }
                    cop.OnDuty = false;
                    TriggerClientEvent(GetPlayerFromSID(sourceSID), "chatMessage", "DISPATCH", new[] { 255, 0, 0 }, "You are now off-duty!");
                }
                else
                {
                    TriggerClientEvent(GetPlayerFromSID(sourceSID), "chatMessage", "", new[] { 255, 0, 0 }, "You are not a Firefighter!");
                }
            }
            else if (splitMessage[0] == "/cops")
            {
                List<Cop> onDCops = new List<Cop>();
                foreach(Cop cop in cops)
                {
                    if (cop.Hex != "test" && cop.OnDuty)
                    {
                        onDCops.Add(cop);
                    }
                }
                if (onDCops.Count != 0)
                {
                    TriggerClientEvent(GetPlayerFromSID(sourceSID), "chatMessage", "DISPATCH", new[] { 255, 0, 0 }, "The Following Officer(s) Are On-Duty:");
                    foreach (Cop cop in onDCops)
                    {
                        TriggerClientEvent(GetPlayerFromSID(sourceSID), "chatMessage", "", new[] { 255, 0, 0 }, cop.Name + " (" + cop.Callsign + ") - " + cop.Department.Acronym);
                    }
                }
                else
                {
                    TriggerClientEvent(GetPlayerFromSID(sourceSID), "chatMessage", "DISPATCH", new[] { 255, 0, 0 }, "No Cops On-Duty!");
                }
                    
            }
            else if (splitMessage[0] == "/firefighters")
            {
                List<Firefighter> onDCops = new List<Firefighter>();
                foreach (Firefighter cop in firefighters)
                {
                    if (cop.Hex != "test" && cop.OnDuty)
                    {
                        onDCops.Add(cop);
                    }
                }
                if (onDCops.Count != 0)
                {
                    TriggerClientEvent(GetPlayerFromSID(sourceSID), "chatMessage", "DISPATCH", new[] { 255, 0, 0 }, "The Following Officer(s) Are On-Duty:");
                    foreach (Firefighter cop in onDCops)
                    {
                        TriggerClientEvent(GetPlayerFromSID(sourceSID), "chatMessage", "", new[] { 255, 0, 0 }, cop.Name + " (" + cop.Callsign + ") - " + cop.Department.Acronym);
                    }
                }
                else
                {
                    TriggerClientEvent(GetPlayerFromSID(sourceSID), "chatMessage", "DISPATCH", new[] { 255, 0, 0 }, "No Cops On-Duty!");
                }

            }
            else if (splitMessage[0] == "/funitid")
            {
                if(IsFire(sourceSID))
                {
                    firefighters[firefighters.IndexOf(GetFireFromID(sourceSID))].Callsign = message.Replace("/funitid ", "");
                    DatabaseSave();
                }
                else
                {
                    TriggerClientEvent(GetPlayerFromSID(sourceSID), "chatMessage", "", new[] { 255, 0, 0 }, "You are not a Firefighter!");
                }
            }
            else if (splitMessage[0] == "/punitid")
            {
                if (IsCop(sourceSID))
                {
                    cops[cops.IndexOf(GetCopFromID(sourceSID))].Callsign = message.Replace("/punitid ", "");
                    DatabaseSave();
                }
                else
                {
                    TriggerClientEvent(GetPlayerFromSID(sourceSID), "chatMessage", "", new[] { 255, 0, 0 }, "You are not a Cop!");
                }
            }
            CancelEvent();
        }

#region CLEAR_THINGS
        private void ClearCops(int sourceID, List<dynamic> args, string rawCommand)
        {
            cops.Clear();
            Cop department = new Cop
            {
                Hex = "test"
            };
            cops.Add(department);
            DatabaseSave();
        }

        private void ClearFFs(int sourceID, List<dynamic> args, string rawCommand)
        {
            firefighters.Clear();
            Firefighter department = new Firefighter
            {
                Hex = "test"
            };
            firefighters.Add(department);
            DatabaseSave();
        }

        private void ClearAdm(int sourceID, List<dynamic> args, string rawCommand)
        {
            admins.Clear();
            Admin department = new Admin
            {
                Hex = "test"
            };
            admins.Add(department);
            DatabaseSave();
        }

        private void ClearDep(int sourceID, List<dynamic> args, string rawCommand)
        {
            departments.Clear();
            Department department = new Department
            {
                Name = "test"
            };
            departments.Add(department);
            DatabaseSave();
        }

        private void ClearFDep(int sourceID, List<dynamic> args, string rawCommand)
        {
            fdepartments.Clear();
            FDepartment department = new FDepartment
            {
                Name = "test"
            };
            fdepartments.Add(department);
            DatabaseSave();
        }

        private void ClearDBC(int sourceID, List<dynamic> args, string rawCommand)
        {
            ClearDB();
            Debug.WriteLine("Database Cleared!");
        }

        private void ClearDB()
        {
            cops.Clear();
            admins.Clear();
            departments.Clear();
            fdepartments.Clear();
            firefighters.Clear();
            Cop cop = new Cop
            {
                Hex = "test"
            };
            Admin admin = new Admin
            {
                Hex = "test"
            };
            Firefighter ff = new Firefighter
            {
                Hex = "test"
            };
            Department department = new Department
            {
                Name = "test"
            };
            FDepartment fdepartment = new FDepartment
            {
                Name = "test"
            };
            cops.Add(cop);
            admins.Add(admin);
            firefighters.Add(ff);
            departments.Add(department);
            fdepartments.Add(fdepartment);
            Tuple<List<Cop>, List<Admin>, List<Firefighter>, List<Department>, List<FDepartment>> write = new Tuple<List<Cop>, List<Admin>, List<Firefighter>, List<Department>, List<FDepartment>>(cops, admins, firefighters, departments, fdepartments);
            database.Write(write);
        }
#endregion

#region REMOVE_THINGS
        private void RemoveCop(int sourceID, List<dynamic> args, string rawCommand)
        {
            if (args.Count == 1)
            {
                Cop cop = null;
                if (cops.Count > Convert.ToInt32(args[0]))
                {
                    cop = cops[Convert.ToInt32(args[0])];
                }
                if (cop == null)
                {
                    Debug.WriteLine("Invalid Cop ID!");
                    return;
                }
                if (GetPlayerFromHex(cop.Hex) != null)
                {
                    TriggerClientEvent(GetPlayerFromHex(cop.Hex), "chatMessage", "", new[] { 255, 0, 0 }, "You are no longer a cop!");
                }
                cops.Remove(cop);
                DatabaseSave();
                Debug.WriteLine("Cop Deleted!");
                
            }
            else
            {
                Debug.WriteLine("Invalid Syntax, use: pmremovecop <Cop ID>");
                return;
            }
        }

        private void RemoveFF(int sourceID, List<dynamic> args, string rawCommand)
        {
            if (args.Count == 1)
            {
                Firefighter cop = null;
                if (firefighters.Count > Convert.ToInt32(args[0]))
                {
                    cop = firefighters[Convert.ToInt32(args[0])];
                }
                if (cop == null)
                {
                    Debug.WriteLine("Invalid Cop ID!");
                    return;
                }
                if (GetPlayerFromHex(cop.Hex) != null)
                {
                    TriggerClientEvent(GetPlayerFromHex(cop.Hex), "chatMessage", "", new[] { 255, 0, 0 }, "You are no longer a cop!");
                }
                firefighters.Remove(cop);
                DatabaseSave();
                Debug.WriteLine("Cop Deleted!");

            }
            else
            {
                Debug.WriteLine("Invalid Syntax, use: pmremoveff <Cop ID>");
                return;
            }
        }

        private void RemoveAdmin(int sourceID, List<dynamic> args, string rawCommand)
        {
            if (args.Count == 1)
            {
                Admin admin = null;
                if (admins.Count >= Convert.ToInt32(args[0]))
                {
                    admin = admins[Convert.ToInt32(args[0])];
                }
                if (admin == null)
                {
                    Debug.WriteLine("Invalid Admin ID!");
                    return;
                }
                if (GetPlayerFromHex(admin.Hex) != null)
                {
                    TriggerClientEvent(GetPlayerFromHex(admin.Hex), "chatMessage", "", new[] { 255, 0, 0 }, "You are no longer an admin!");
                }                
                admins.Remove(admin);
                DatabaseSave();
                Debug.WriteLine("Admin Deleted!");                
            }
            else
            {
                Debug.WriteLine("Invalid Syntax, use: pmremoveadmin <Admin ID>");
                return;
            }
        }

        private void RemoveDepartment(int sourceID, List<dynamic> args, string rawCommand)
        {
            if (args.Count == 1)
            {
                Department dep = null;
                if (departments.Count > Convert.ToInt32(args[0]))
                {
                    dep = departments[Convert.ToInt32(args[0])];
                }
                if (dep == null)
                {
                    Debug.WriteLine("Invalid Department ID!");
                    return;
                }
                departments.Remove(dep);
                DatabaseSave();
                Debug.WriteLine("Department Deleted!");
            }
            else
            {
                Debug.WriteLine("Invalid Syntax, use: pmremovedepartment <Department ID>");
                return;
            }
        }

        private void RemoveFDepartment(int sourceID, List<dynamic> args, string rawCommand)
        {
            if (args.Count == 1)
            {
                FDepartment dep = null;
                if (fdepartments.Count > Convert.ToInt32(args[0]))
                {
                    dep = fdepartments[Convert.ToInt32(args[0])];
                }
                if (dep == null)
                {
                    Debug.WriteLine("Invalid Department ID!");
                    return;
                }
                fdepartments.Remove(dep);
                DatabaseSave();
                Debug.WriteLine("Department Deleted!");
            }
            else
            {
                Debug.WriteLine("Invalid Syntax, use: pmremovefd <Department ID>");
                return;
            }
        }
        #endregion

#region LIST_THINGS
        private void ListCops(int sourceID, List<dynamic> args, string rawCommand)
        {
            foreach(Cop cop in cops)
            {
                if(cop.Hex != "test")
                    Debug.Write("[" + cops.IndexOf(cop) + "] - " + cop.Name + "(" + cop.Callsign + ") - " + cop.Department.Acronym);
            }
            if (cops.Count == 1)
                Debug.WriteLine("No Cops!");
        }

        private void ListFFs(int sourceID, List<dynamic> args, string rawCommand)
        {
            foreach (Firefighter cop in firefighters)
            {
                if (cop.Hex != "test")
                    Debug.Write("[" + firefighters.IndexOf(cop) + "] - " + cop.Name + "(" + cop.Callsign + ") - " + cop.Department.Acronym);
            }
            if (cops.Count == 1)
                Debug.WriteLine("No Firefighters!");
        }

        private void ListAdmins(int sourceID, List<dynamic> args, string rawCommand)
        {
            foreach(Admin admin in admins)
            {
                if(admin.Hex != "test")
                    Debug.WriteLine("[" + admins.IndexOf(admin) + "] - " + admin.Name);
            }
            if (admins.Count == 1)
                Debug.WriteLine("No Admins!");
        }

        private void ListDepartments(int sourceID, List<dynamic> args, string rawCommand)
        {
            foreach (Department department in departments)
            {
                if(department.Name != "test")
                    Debug.WriteLine("[" + departments.IndexOf(department) + "] - " + department.Name + " (" + department.officers.Count + ")");
            }
            if (departments.Count == 1)
                Debug.WriteLine("No Departments!");
        }

        private void ListFDepartments(int sourceID, List<dynamic> args, string rawCommand)
        {
            foreach (FDepartment department in fdepartments)
            {
                if (department.Name != "test")
                    Debug.WriteLine("[" + fdepartments.IndexOf(department) + "] - " + department.Name + " (" + department.firefighters.Count + ")");
            }
            if (departments.Count <= 1)
                Debug.WriteLine("No Fire Departments!");
        }
        #endregion

#region ADD_THINGS
        private void AddCop(int sourceID, List<dynamic> args, string rawComamnd)
        {
            if (args.Count >= 3)
            {
                if(GetPlayerFromSID(Convert.ToInt32(args[0])) != null)
                {                    
                    if (GetDepartmentFromID(Convert.ToInt32(args[1])) != null)
                    {
                        string[] splitArgs = args.Select(x => (string)x).ToArray();
                        Cop cop = new Cop();
                        Player player = GetPlayerFromSID(Convert.ToInt32(args[0]));
                        cop.Name = player.Name;
                        cop.Department = GetDepartmentFromID(Convert.ToInt32(args[1]));
                        cop.Callsign = string.Join(" ", splitArgs).Replace(splitArgs[0] + " ", "").Replace(splitArgs[1] + " ", "");
                        cop.Hex = player.Identifiers.First().ToString();
                        cops.Add(cop);
                        cop.Department.officers.Add(cop);
                        DatabaseSave();
                        Debug.WriteLine("\"" + cop.Name + "\" (" + cop.Callsign + ") was added to \"" + cop.Department.Name + "\"");
                        TriggerClientEvent(GetPlayerFromSID(Convert.ToInt32(args[0])), "chatMessage", "", new[] { 0, 255, 0 }, "Welcome to "+cop.Department.Name+", your callsign is: "+cop.Callsign);
                    }
                    else
                    {
                        Debug.WriteLine("Invalid Department");
                    }
                }
                else
                {
                    Debug.WriteLine("Invalid Player ID");
                }
            }
            else
            {
                Debug.WriteLine("Invalid syntax, use: pmaddcop <Player ID> <Department ID> <Callsign>");
            }
        }

        private void AddFF(int sourceID, List<dynamic> args, string rawComamnd)
        {
            if (args.Count >= 3)
            {
                if (GetPlayerFromSID(Convert.ToInt32(args[0])) != null)
                {
                    if (GetFDepartmentFromID(Convert.ToInt32(args[1])) != null)
                    {
                        string[] splitArgs = args.Select(x => (string)x).ToArray();
                        Firefighter cop = new Firefighter();
                        Player player = GetPlayerFromSID(Convert.ToInt32(args[0]));
                        cop.Name = player.Name;
                        cop.Department = GetFDepartmentFromID(Convert.ToInt32(args[1]));
                        cop.Callsign = string.Join(" ", splitArgs).Replace(splitArgs[0] + " ", "").Replace(splitArgs[1] + " ", "");
                        cop.Hex = player.Identifiers.First().ToString();
                        firefighters.Add(cop);
                        cop.Department.firefighters.Add(cop);
                        DatabaseSave();
                        Debug.WriteLine("\"" + cop.Name + "\" (" + cop.Callsign + ") was added to \"" + cop.Department.Name + "\"");
                        TriggerClientEvent(GetPlayerFromSID(Convert.ToInt32(args[0])), "chatMessage", "", new[] { 0, 255, 0 }, "Welcome to " + cop.Department.Name + ", your callsign is: " + cop.Callsign);
                    }
                    else
                    {
                        Debug.WriteLine("Invalid Department");
                    }
                }
                else
                {
                    Debug.WriteLine("Invalid Player ID");
                }
            }
            else
            {
                Debug.WriteLine("Invalid syntax, use: pmaddff <Player ID> <Department ID> <Callsign>");
            }
        }

        private void AddAdmin(int sourceID, List<dynamic> args, string rawComamnd)
        {
            if (args.Count != 0)
            {
                if(GetPlayerFromSID(Convert.ToInt32(args[0])) != null)
                {
                    Admin admin = new Admin();
                    Player player = GetPlayerFromSID(Convert.ToInt32(args[0]));
                    admin.Name = player.Name;
                    admin.Hex = player.Identifiers.First().ToString();
                    admins.Add(admin);
                    DatabaseSave();
                    Debug.WriteLine("\"" + admin.Name + "\" was added as an admin!");
                    TriggerClientEvent(GetPlayerFromSID(Convert.ToInt32(args[0])), "chatMessage", "", new[] { 0, 255, 0 }, "You are now an admin!");
                }
                else
                {
                    Debug.WriteLine("Invalid Player ID");
                }
            }
            else
            {
                Debug.WriteLine("Invalid Syntax, use: pmaddadmin <Player ID>");
            }
        }

        private void AddDepartment(int sourceID, List<dynamic> args, string rawCommand)
        {
            if (args.Count >= 2)
            {
                string[] splitArgs = args.Select(x => (string)x).ToArray();
                Department dep = new Department
                {
                    Name = string.Join(" ", splitArgs).Replace(splitArgs[0] + " ", ""),
                    Acronym = splitArgs[0].ToString().ToUpper(),
                    officers = new List<Cop>()
                };
                departments.Add(dep);
                DatabaseSave();
                Debug.WriteLine("\""+dep.Name+"\" ("+dep.Acronym+") was added as a Department!");
            }
            else
            {
                Debug.WriteLine("Invalid Syntax, use: pmaddpd <Acronym> <Department Name>");
            }
        }

        private void AddFDepartment(int sourceID, List<dynamic> args, string rawCommand)
        {
            if (args.Count >= 2)
            {
                string[] splitArgs = args.Select(x => (string)x).ToArray();
                FDepartment dep = new FDepartment
                {
                    Name = string.Join(" ", splitArgs).Replace(splitArgs[0] + " ", ""),
                    Acronym = splitArgs[0].ToString().ToUpper(),
                    firefighters = new List<Firefighter>()
                };
                fdepartments.Add(dep);
                DatabaseSave();
                Debug.WriteLine("\"" + dep.Name + "\" (" + dep.Acronym + ") was added as a Fire Department!");
            }
            else
            {
                Debug.WriteLine("Invalid Syntax, use: pmaddfd <Acronym> <Department Name>");
            }
        }
#endregion

#region OTHER_THINGS
        private void DatabaseSave()
        {
            var write = new Tuple<List<Cop>, List<Admin>, List<Firefighter>, List<Department>, List<FDepartment>>(cops, admins, firefighters, departments, fdepartments);
            database.Write(write);
        }

        private Player GetPlayerFromSID(int id)
        {
            Player playerToReturn = null;
            foreach (Player player in new PlayerList())
            {
                if (Convert.ToInt32(player.Handle) == id)
                    playerToReturn = player;
            }
            return playerToReturn;
        }

        private Player GetPlayerFromHex(string id)
        {
            Player playerToReturn = null;
            foreach (Player player in new PlayerList())
            {
                if (player.Identifiers.First().ToString() == id)
                    playerToReturn = player;
            }
            return playerToReturn;
        }

        private Department GetDepartmentFromID(int id)
        {
            Department department = null;
            foreach(Department dep in departments)
            {
                if (departments.IndexOf(dep) == id)
                    department = dep;
            }
            return department;
        }

        private FDepartment GetFDepartmentFromID(int id)
        {
            FDepartment department = null;
            foreach (FDepartment dep in fdepartments)
            {
                if (fdepartments.IndexOf(dep) == id)
                    department = dep;
            }
            return department;
        }

        private Cop GetCopFromID(int id)
        {
            Cop department = null;
            foreach (Cop dep in cops)
            {
                if (dep.Hex == GetPlayerFromSID(id).Identifiers.First().ToString())
                    department = dep;
            }
            return department;
        }

        private Firefighter GetFireFromID(int id)
        {
            Firefighter department = null;
            foreach (Firefighter dep in firefighters)
            {
                if (dep.Hex == GetPlayerFromSID(id).Identifiers.First().ToString())
                    department = dep;
            }
            return department;
        }

        private List<string> ListAdminHexes()
        {
            List<string> list = new List<string>();
            foreach (Admin admin in admins)
            {
                list.Add(admin.Hex);
            }
            return list;
        }

        private List<string> ListCopHexes()
        {
            List<string> list = new List<string>();
            foreach (Cop admin in cops)
            {
                list.Add(admin.Hex);
            }
            return list;
        }

        private List<string> ListFireHexes()
        {
            List<string> list = new List<string>();
            foreach (Firefighter admin in firefighters)
            {
                list.Add(admin.Hex);
            }
            return list;
        }

        private bool IsAdmin(int id)
        {
            PlayerList playerList = new PlayerList();
            Player selectedPlayer = null;
            string selectedHash = null;
            foreach (Player player in playerList)
            {
                if (Convert.ToInt32(player.Handle) == id)
                {
                    selectedPlayer = player;
                    selectedHash = player.Identifiers.First().ToString();
                }
            }
            if (selectedPlayer == null || selectedHash == null)
            {
                Debug.WriteLine("Invalid ID");
                return false;
            }
            if (ListAdminHexes().Contains(selectedHash))
                return true;
            else
                return false;
        }

        private bool IsCop(int id)
        {
            PlayerList playerList = new PlayerList();
            Player selectedPlayer = null;
            string selectedHash = null;
            foreach (Player player in playerList)
            {
                if (Convert.ToInt32(player.Handle) == id)
                {
                    selectedPlayer = player;
                    selectedHash = player.Identifiers.First().ToString();
                }
            }
            if (selectedPlayer == null || selectedHash == null)
            {
                Debug.WriteLine("Invalid ID");
                return false;
            }
            if (ListCopHexes().Contains(selectedHash))
                return true;
            else
                return false;
        }

        private bool IsFire(int id)
        {
            PlayerList playerList = new PlayerList();
            Player selectedPlayer = null;
            string selectedHash = null;
            foreach (Player player in playerList)
            {
                if (Convert.ToInt32(player.Handle) == id)
                {
                    selectedPlayer = player;
                    selectedHash = player.Identifiers.First().ToString();
                }
            }
            if (selectedPlayer == null || selectedHash == null)
            {
                Debug.WriteLine("Invalid ID");
                return false;
            }
            if (ListFireHexes().Contains(selectedHash))
                return true;
            else
                return false;
        }
#endregion
    }
}