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

        public Floor() {
            Parent = null;
            Left = null;
            Right = null;
        }

        public Floor(Floor parent) {
            Parent = parent;
            Left = null;
            Right = null;
        }

        public void AddNext() {
            if(Left == null) {
                Left = new Floor();
            } 
            else if(Right == null) {
                Right = new Floor();
            } 
            else {
                throw new Exception("Maximum branch limit reached. (2)");
            }
        }

    }
    public class NetworkArchitecture {

        private Difficulty Difficulty = new Difficulty(); 
        public int Size {get;}
        private int Branches = 0;
        private Floor RootFloor = new Floor();

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

            Floor TargetFloor = RootFloor;

            for(int i = 0; i < Size; i++) {
                if(i == 0) {
                    TargetFloor.AddNext();
                    TargetFloor = TargetFloor.Left!;
                }
                else {
                    TargetFloor.AddNext();
                    TargetFloor = TargetFloor.Left!;
                }

            }
            while(TargetFloor.Parent != null) {
                TargetFloor = TargetFloor.Parent!;
            }
            RootFloor = TargetFloor;


            
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

            await command.RespondAsync(embed: new Embeds().NetworkArchitecture(network).Build());
        }

        public async Task DeleteNetwork(SocketSlashCommand command) {
            await command.RespondAsync("Not implemented yet.");
        }

        public async Task ListNetworks(SocketSlashCommand command) {
            await command.RespondAsync("Not implemented yet.");
        }

    }

}