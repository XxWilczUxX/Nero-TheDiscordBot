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

        private NetworkArchitecture NetworkArchitecture;
        private Floor Left;
        private Floor Right;


    }
    public class NetworkArchitecture {

        private Difficulty Difficulty = new Difficulty(); 
        private int Size = 0;
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
            
        }

        public async Task DeleteNetwork(SocketSlashCommand command) {
            
        }

        public async Task ListNetworks(SocketSlashCommand command) {
            
        }

    }

}