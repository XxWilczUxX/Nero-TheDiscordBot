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

            return $"Floor:{Height}-Contains:Nothing";
        
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

            int empty;

            Navigation = CreateBranch((Floor)Navigation, Branches, out empty);

            Navigation = ReturnToHeight((Floor)Navigation, 0);

            this.ID = DetermineID(guildID);
            
        }


        private Floor CreateBranch(Floor floor, int branches, out int branchesLeft, int iteration = 0) { // It works now but it's complicated, iteration is used to prevent one branch from having all the branches overall. It's a bit of a mess but it works.
            
            Random random = new Random();

            //Console.WriteLine($"Height: {floor.Height} Branches: {branches}; floor.Height: {floor.Height} Size: {Size}");

            if(floor.Height < Size-1) {

                floor.AddNext();

                if(random.Next(1, 11) >= 7 && random.Next(1,11) >= 5 + iteration && branches > 0 && floor.Height < Size-2 && floor.Height > 1){
                    floor.AddNext();
                    branches--;
                    floor.Branch = CreateBranch((Floor)floor.Right(), branches, out branches, iteration+1);
                }
                floor.Child = CreateBranch((Floor)floor.Down(), branches, out branches, iteration);

            }
            branchesLeft = branches;
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
            
            NetworkArchitecture network = new NetworkArchitecture(Difficulty.Level.Unset);
            
            try {
                // Try to load the network from temp folder
                Console.WriteLine("Loading network.");

                network = SaveableFactory.LoadTemp<NetworkArchitecture>((ulong)command.GuildId!, command.User.Id);
                
            }
            catch(Exception e) {
                // Create the network

                Console.WriteLine($"{e.Message}\n Creating new network.");

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

                network = new NetworkArchitecture(level, size, branches, (ulong)command.GuildId!);

                Console.WriteLine($"Network Created: {level} {network.Size} {network.Branches}");
                network.PreorderConsole((network.Navigation as Floor)!);
                Console.WriteLine("\n");

            }
            finally {
                var navigationActions = new NavigationActions();
                await navigationActions.Respond(command, network.Navigation);
                SaveableFactory.SaveTemp(network, (ulong)command.GuildId!, command.User.Id);
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