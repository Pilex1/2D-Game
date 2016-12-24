using System;

namespace Game.Items {
    [Serializable]
    class Item {

        public RawItem rawitem;
        public int amt;

        public Item(RawItem rawitem, int amt) {
            this.rawitem = rawitem;
            this.amt = amt;
        }
        public Item(RawItem rawitem) : this(rawitem, 1) { }

        public override string ToString() {
            return rawitem.ToString() + ": " + amt;
        }
    }
}
