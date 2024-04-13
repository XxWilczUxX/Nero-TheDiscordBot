using System;
using System.Net.NetworkInformation;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace Nero {

    public class Difficulty {
        
        public enum Level : ushort{
            Basic,
            Standard,
            Uncommon,
            Advanced,
            Unset
        }
        public int SecurityDV;
        public int NetrunnerInterface;
        public int NetrunnerInterfaceThatMightDie; 

    }

    public class Floor : INavigation {
        
        public Floor? Parent;
        public Floor? Child;
        public Floor? Branch;

        public int Height = 0;

        public Floor() {
            Parent = null;
            Child = null;
            Branch = null;
        }

        public Floor(Floor parent) {
            Parent = parent;
            Height = parent.Height + 1;
            Child = null;
            Branch = null;
        }

        public void AddNext() {
            if(Child == null) {
                Child = new Floor(this);
            } 
            else if(Branch == null) {
                Branch = new Floor(this);
            } 
            else {
                throw new Exception("Maximum branch limit reached. (2)");
            }
        
        }

        public object Up() {
            if(Parent != null) {
                return Parent;
            }
            else {
                return this;
            }
        
        }

        public object Down() {
            if(Child != null) {
                return Child;
            }
            else {
                return this;
            }
        }

        public object Right() {
            if(Branch != null) {
                return Branch;
            }
            else {
                return this;
            }
        }

        public object Left() {
            if(Branch != null) {
                return Branch;
            }
            else {
                return this;
            }
        }

        public string GetContents() {

            return $"{Height}";
        
        }

    }

    public class NetworkArchitecture : Saveable{

        public ulong ID {get; set;}
        public string Type {get;} = "NetworkArchitecture";
        public INavigation Navigation {get; set;}
        private Difficulty Difficulty = new Difficulty(); 
        public int Size {get; set;}
        public int Branches {get; set;}

        private ulong DetermineID(ulong guildID) {
            var path =  Path.Combine(Directory.GetCurrentDirectory(), ($"json\\guilds\\{guildID}\\{Type}"));
            ulong ID = 0;
            if(Path.Exists(path)) {
                
                var files = Directory.GetFiles(path);
                foreach(var file in files) {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    if (ulong.TryParse(fileName, out ulong result))
                    {
                        ID = Math.Max(ID, result+1);
                    }
                }

            }
            return ID;
        }

        public NetworkArchitecture(Difficulty.Level level, int size = 0, int branches = 0, ulong guildID = 0) {

            switch (level) {
                case Difficulty.Level.Basic:
                    Difficulty.SecurityDV = 6;
                    Difficulty.NetrunnerInterface = 2;
                    Difficulty.NetrunnerInterfaceThatMightDie = -1;
                    break;
                case Difficulty.Level.Standard:
                    Difficulty.SecurityDV = 8;
                    Difficulty.NetrunnerInterface = 4;
                    Difficulty.NetrunnerInterfaceThatMightDie = 2;
                    break;
                case Difficulty.Level.Uncommon:
                    Difficulty.SecurityDV = 10;
                    Difficulty.NetrunnerInterface = 6;
                    Difficulty.NetrunnerInterfaceThatMightDie = 4;
                    break;
                case Difficulty.Level.Advanced:
                    Difficulty.SecurityDV = 12;
                    Difficulty.NetrunnerInterface = 8;
                    Difficulty.NetrunnerInterfaceThatMightDie = 6;
                    break;
            }

            Random random = new Random();

            this.Size = size;
            this.Branches = branches;

            if(Size == 0){
                for(int i = 0; i < 3; i++) {
                    Size += random.Next(1, 7);
                }
            }
            
            if(Branches == 0){                
                while(random.Next(1, 11) >= 7) {
                    Branches++;
                }
            }
            Navigation = new Floor();

            int branchesLeft; // Assign a variable to the 'out' parameter
            Navigation = CreateBranch((Floor)Navigation, Size, Branches, out branchesLeft);

            Navigation = ReturnToHeight((Floor)Navigation, 0);

            this.ID = DetermineID(guildID);
            
        }


        private Floor CreateBranch(Floor floor, int maxHeight, int maxBranches, out int branchesLeft, bool isBranch = false) { //This doesn't work for creating side branches. It only works for creating the main branch. 
            
            Random random = new Random();

            if(isBranch? floor.Height < maxHeight-2 : floor.Height < maxHeight-1) {
                
                if(isBranch) {
                    if(random.Next(1, 3) == 1) {
                        branchesLeft = maxBranches;
                        return floor;
                    }
                }

                floor.AddNext();
                if((Floor)floor.Down() != floor) {
                    CreateBranch((Floor)floor.Down(), maxHeight, maxBranches, out branchesLeft, isBranch);
                }
                else {
                    branchesLeft = maxBranches;
                }
                if(random.Next(1, 3) != 1 && branchesLeft > 0 && floor.Height != maxHeight-2 && (Floor)floor.Right() != floor) {
                    floor.AddNext();
                    branchesLeft--;;
                    CreateBranch((Floor)floor.Right(), maxHeight, branchesLeft, out branchesLeft, true);
                }


            } else {
                branchesLeft = maxBranches;
            }

            return floor;
        }

        private Floor ReturnToHeight(Floor floor, int height) {
            while(floor.Height != height) {
                floor = floor.Parent!;
            }
            return floor;
        }

        public void PreorderConsole(Floor floor) {
            Console.WriteLine(floor.Height);

            if((Floor)floor.Down() != floor) {
                PreorderConsole((Floor)floor.Down());
            }
            if((Floor)floor.Right() != floor) {
                PreorderConsole((Floor)floor.Right());
            }
            return;
            
        }

        public List<Floor>[] PreorderList(List<Floor>[] floors, Floor floor) {

            if((Floor)floor.Down() == floor || (Floor)floor.Right() == floor) {
                return floors;
            }
            else if(floor.Height < floors.Length){

                if(floors[floor.Height] == null) {
                    floors[floor.Height] = new List<Floor>();
                }
                floors[floor.Height].Add(floor);
                if((Floor)floor.Down() != floor) {
                    floors = PreorderList(floors, (Floor)floor.Down());
                }
                if((Floor)floor.Right() != floor) {
                    floors = PreorderList(floors, (Floor)floor.Right());
                }

            }

            return floors;
        }

    }



    public class NetworkArchitectureCommands {
        
        public async Task CommandHandler(SocketSlashCommand command) {

            switch(command.Data.Options.First().Name) {
                case "create":
                    await CreateNetwork(command);
                    break;
                case "delete":
                    await DeleteNetwork(command);
                    break;
                case "list":
                    await ListNetworks(command);
                    break;
            }

        }

        public async Task CreateNetwork(SocketSlashCommand command) {
            
            
            try {
                // Try to load the network from temp folder

                SaveableFactory.LoadTemp((ulong)command.GuildId!, command.User.Id);
                
            }
            catch {
                // Create the network

                var options = command.Data.Options.First().Options.ToArray();

                Difficulty.Level level = Difficulty.Level.Unset;
                int size = 0;
                int branches = 0;

                for(int i = 0; i < options.Length; i++) {
                    if(options[i].Name == "difficulty") {
                        level = (Difficulty.Level)(ushort)(long)options[i].Value;
                    }
                    else if(options[i].Name == "size") {
                        size = Convert.ToInt32(options[i].Value);
                    }
                    else if(options[i].Name == "branches") {
                        branches = Convert.ToInt32(options[i].Value);
                    }
                }

                if(level == Difficulty.Level.Unset) {
                    level = (Difficulty.Level)new Random().Next(0, 4);
                }

                NetworkArchitecture network = new NetworkArchitecture(level, size, branches, (ulong)command.GuildId!);

                Console.WriteLine($"Network Created: {level} {network.Size} {network.Branches}");
                network.PreorderConsole((network.Navigation as Floor)!);
                Console.WriteLine("\n");

                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
            }
            finally {
                await command.RespondAsync("Network Created but i don't have a idea how to display it. (Not implemented yet.)");
            }   

        }

        public async Task DeleteNetwork(SocketSlashCommand command) {
            await command.RespondAsync("Not implemented yet.");
        }

        public async Task ListNetworks(SocketSlashCommand command) {
            await command.RespondAsync("Not implemented yet.");
        }

    }

}