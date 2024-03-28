using System;
using System.Net.NetworkInformation;

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
    public class NetworkArchitecture {

        private Difficulty Difficulty = new Difficulty(); 

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

            
            
        }
    }
    public class NetworkArchitectureCommands {
        
    }

}