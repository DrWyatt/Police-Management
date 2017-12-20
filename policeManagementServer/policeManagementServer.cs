using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace policeManagementServer
{
    public class PoliceManagementServer : BaseScript
    {
        Database database = new Database("resources/[policepack]/database.db", true);
        List<Cop> cops = new List<Cop>();
        List<Admin> admins = new List<Admin>();
        List<Department> departments = new List<Department>();

        public PoliceManagementServer()
        {
            //Add Commands
            RegisterCommand("pmadddepartment", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { AddDepartment(source, args, rawCommand); }), true);
            RegisterCommand("pmaddadmin", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { AddAdmin(source, args, rawCommand); }), true);
            RegisterCommand("pmaddcop", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { AddCop(source, args, rawCommand); }), true);

            //List Commands
            RegisterCommand("pmlistdepartments", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { ListDepartments(source, args, rawCommand); }), true);
            RegisterCommand("pmlistadmins", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { ListAdmins(source, args, rawCommand); }), true);
            RegisterCommand("pmlistcops", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { ListCops(source, args, rawCommand); }), true);

            //Remove Commands
            RegisterCommand("pmremovedepartment", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { RemoveDepartment(source, args, rawCommand); }), true);
            RegisterCommand("pmremoveadmin", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { RemoveAdmin(source, args, rawCommand); }), true);
            RegisterCommand("pmremovecop", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { RemoveCop(source, args, rawCommand); }), true);

            //Clean Commands
            RegisterCommand("pmcleardb", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { ClearDBC(source, args, rawCommand); }), true);
            RegisterCommand("pmcleardepartments", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { ClearDep(source, args, rawCommand); }), true);
            RegisterCommand("pmclearadmins", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { ClearAdm(source, args, rawCommand); }), true);            
            RegisterCommand("pmclearcops", new Action<int, List<dynamic>, string>((source, args, rawCommand) => { ClearCops(source, args, rawCommand); }), true);

            //Default Events
            EventHandlers.Add("chatMessage", new Action<int, int, string, string>(ChatMessage));

            //Custom Events
            EventHandlers.Add("pm:isAdmin", new Action<int, string, string>(EIsAdmin));
            EventHandlers.Add("pm:isCop", new Action<int, string, string>(EIsCop));
            EventHandlers.Add("pm:triggerToAllCops", new Action<string, string>(TAllCops));
            EventHandlers.Add("pm:triggerToAllAdmins", new Action<string, string>(TAllAdmins));
            EventHandlers.Add("pm:triggerToAllDepartment", new Action<int, string, string>(TAllDepartment));

            if (database.Read() == null)
                ClearDB();

            Tuple<List<Cop>, List<Admin>, List<Department>> tuple = database.Read();
            cops = tuple.Item1;
            admins = tuple.Item2;
            departments = tuple.Item3;            
        }

        private void TAllDepartment(int depID, string eventName, string message)
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
                    if(departments.IndexOf(cop.Department) == depID)
                    {
                        if (message != null)
                            TriggerClientEvent(GetPlayerFromSID(id), eventName, message);
                        else
                            TriggerClientEvent(GetPlayerFromSID(id), eventName);
                    }
                }                    
            }
        }

        private void TAllAdmins(string eventName, string message)
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
                    if (message != null)
                        TriggerClientEvent(GetPlayerFromSID(id), eventName, message);
                    else
                        TriggerClientEvent(GetPlayerFromSID(id), eventName);
                }
            }
        }

        private void TAllCops(string eventName, string message)
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
                    if(message != null)
                        TriggerClientEvent(GetPlayerFromSID(id), eventName, message);
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

        private void EIsAdmin(int sourceSID, string posEventName, string negEventName)
        {
            if (IsAdmin(sourceSID))
                TriggerClientEvent(posEventName);
            else
                TriggerClientEvent(negEventName);
        }

        private void ChatMessage([FromSource]int sourceCID, int sourceSID, string sourceName, string message)
        {
            string[] splitMessage = message.Split(' ');
            if(splitMessage[0] == "/addcop")
            {
                if (IsAdmin(sourceSID))
                    AddCop(sourceSID, new List<dynamic> { splitMessage.ToList() }, message);
            }
            else if (splitMessage[0] == "/adddepartment")
            {
                if (IsAdmin(sourceSID))
                    AddDepartment(sourceSID, new List<dynamic> { splitMessage.ToList() }, message);
            }
            else if (splitMessage[0] == "/listadmins")
            {
                if (IsAdmin(sourceSID))
                    ListAdmins(sourceSID, new List<dynamic> { splitMessage.ToList() }, message);
            }
            else if (splitMessage[0] == "/listcops")
            {
                if (IsAdmin(sourceSID))
                    ListCops(sourceSID, new List<dynamic> { splitMessage.ToList() }, message);
            }
            else if (splitMessage[0] == "/listdepartments")
            {
                if (IsAdmin(sourceSID))
                    ListDepartments(sourceSID, new List<dynamic> { splitMessage.ToList() }, message);
            }
            else if (splitMessage[0] == "/removecop")
            {
                if (IsAdmin(sourceSID))
                    RemoveCop(sourceSID, new List<dynamic> { splitMessage.ToList() }, message);
            }
            else if (splitMessage[0] == "/removedepartment")
            {
                if (IsAdmin(sourceSID))
                    RemoveDepartment(sourceSID, new List<dynamic> { splitMessage.ToList() }, message);
            }
            else if (splitMessage[0] == "/cleardb")
            {
                if (IsAdmin(sourceSID))
                    ClearDBC(sourceSID, new List<dynamic> { splitMessage.ToList() }, message);
            }
            else if (splitMessage[0] == "/clearcops")
            {
                if (IsAdmin(sourceSID))
                    ClearCops(sourceSID, new List<dynamic> { splitMessage.ToList() }, message);
            }
            else if (splitMessage[0] == "/cleardepartments")
            {
                if (IsAdmin(sourceSID))
                    ClearDep(sourceSID, new List<dynamic> { splitMessage.ToList() }, message);
            }
            else if (splitMessage[0] == "/od" || splitMessage[0] == "/onduty")
            {
                if (IsCop(sourceSID)){
                    Cop cop = GetCopFromID(sourceSID);
                    if (cop.OnDuty)
                    {
                        TriggerClientEvent(GetPlayerFromSID(sourceSID), "chatMessage", "DISPATCH", new[] { 255, 0, 0 }, "You are already on-duty!");
                        CancelEvent();
                        return;
                    }
                    GetCopFromID(sourceSID).OnDuty = true;
                    TriggerClientEvent(GetPlayerFromSID(sourceSID), "chatMessage", "DISPATCH", new[] { 255, 0, 0 }, "You are now on-duty!");
                }
                else
                {
                    TriggerClientEvent(GetPlayerFromSID(sourceSID), "chatMessage", "", new[] { 255, 0, 0 }, "You are not a Police Officer!");
                }
            }
            else if (splitMessage[0] == "/ofd" || splitMessage[0] == "/offduty")
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
            CancelEvent();
        }

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

        private void ClearDBC(int sourceID, List<dynamic> args, string rawCommand)
        {
            ClearDB();
            Debug.WriteLine("Database Cleared!");
        }

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
                Debug.WriteLine("Invalid Syntax, use: pmadddepartment <Acronym> <Department Name>");
            }
        }
        
        private void ClearDB()
        {
            cops.Clear();
            admins.Clear();
            departments.Clear();
            Cop cop = new Cop
            {
                Hex = "test"
            };
            Admin admin = new Admin
            {
                Hex = "test"
            };
            Department department = new Department
            {
                Name = "test"
            };
            cops.Add(cop);
            admins.Add(admin);
            departments.Add(department);
            Tuple<List<Cop>, List<Admin>, List<Department>> write = new Tuple<List<Cop>, List<Admin>, List<Department>>(cops, admins, departments);
            database.Write(write);
        }

        private void DatabaseSave()
        {
            var write = new Tuple<List<Cop>, List<Admin>, List<Department>>(cops, admins, departments);
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

    }
}
