using System;
using System.Net.NetworkInformation;
using Discord.WebSocket;

namespace Nero {

    public class Difficulty {
        
        public enum Level {
            Basic,
            Standard,
            Uncommon,
            Advanced
        }
        public int SecurityDV;
        public int NetrunnerInterface;
        public int NetrunnerInterfaceThatMightDie; 

    }

    public class Floor {

        public Floor? Parent;
        public Floor? Left;
        public Floor? Right;
        public int Height = 0;

        public Floor() {
            Parent = null;
            Height = 0;
            Left = null;
            Right = null;
        }

        public Floor(Floor parent) {
            Parent = parent;
            Height = parent.Height + 1;
            Left = null;
            Right = null;
        }

        public void AddNext() {
            if(Left == null) {
                Left = new Floor(this);
            } 
            else if(Right == null) {
                Right = new Floor(this);
            } 
            else {
                throw new Exception("Maximum branch limit reached. (2)");
            }
        }

    }
    public class NetworkArchitecture {

        private Difficulty Difficulty = new Difficulty(); 
        public int Size {get;}
        public int Branches {get;}
        public Floor RootFloor {get;}

        public NetworkArchitecture(Difficulty.Level level) {

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

            for(int i = 0; i < 3; i++) {
                Size += random.Next(1, 7);
            }

            while(random.Next(1, 11) >= 7) {
                Branches++;
            }

            RootFloor = new Floor();

            int branchesLeft; // Assign a variable to the 'out' parameter
            RootFloor = CreateBranch(RootFloor, Size, Branches, out branchesLeft);

            RootFloor = ReturnToHeight(RootFloor, 0);
            
        }

        private Floor CreateBranch(Floor floor, int maxHeight, int maxBranches, out int branchesLeft, bool isBranch = false) {
            
            Random random = new Random();

            if(isBranch? floor.Height < maxHeight-1 : floor.Height < maxHeight) {
                floor.AddNext();
                CreateBranch(floor.Left!, maxHeight, maxBranches, out branchesLeft);
                if(random.Next(1, 3) != 1 && branchesLeft > 0) {
                    floor.AddNext();
                    branchesLeft--;;
                    CreateBranch(floor.Right!, maxHeight, branchesLeft, out branchesLeft, true);
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
            if(floor == null) {
                return;
            }
            else {
                Console.Write(floor.Height + " ");
                PreorderConsole(floor.Left);
                PreorderConsole(floor.Right);
            }
        }

        public List<Floor>[] PreorderList(List<Floor>[] floors, Floor floor) {
            
            if(floor == null) {
                return floors;
            }
            else if(floor.Height < floors.Length) {
                if(floors[floor.Height] == null) {
                    floors[floor.Height] = new List<Floor>();
                }
                floors[floor.Height].Add(floor);
                floors = PreorderList(floors, floor.Left);
                floors = PreorderList(floors, floor.Right);
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
            NetworkArchitecture network = new NetworkArchitecture(Difficulty.Level.Basic);

            Console.WriteLine("\n" + network.Size + "\n" + network.Branches);
            network.PreorderConsole(network.RootFloor);
            await command.RespondAsync("Network Created but i don't have a idea how to display it. (Not implemented yet.)");
            //await command.RespondAsync(embed: new Embeds().NetworkArchitecture(network).Build());
        }

        public async Task DeleteNetwork(SocketSlashCommand command) {
            await command.RespondAsync("Not implemented yet.");
        }

        public async Task ListNetworks(SocketSlashCommand command) {
            await command.RespondAsync("Not implemented yet.");
        }

    }

}