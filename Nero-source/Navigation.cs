using System;

namespace Nero {

    public interface INavigation {
        public abstract object Right();
        public abstract object Left();
        public abstract object Up();
        public abstract object Down();
    }

}