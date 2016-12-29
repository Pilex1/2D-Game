using Game.Util;
using System;
using System.Collections.Generic;

namespace Game.Items {

    class Inventory {

        public Item[,] Items;

        public Inventory(Vector2i v) : this(v.x, v.y) { }
        public Inventory(int x, int y) {
            Items = new Item[x, y];
            Clear();
        }

        public void Clear() {
            for (int i = 0; i < Items.GetLength(0); i++) {
                for (int j = 0; j < Items.GetLength(1); j++) {
                    Items[i, j] = new Item(RawItem.None, 0);
                }
            }
        }

        public Vector2i[] GetAllItemLocations(RawItem item) {
            var l = new List<Vector2i>();
            for (int j = 0; j < Items.GetLength(1); j++) {
                for (int i = 0; i < Items.GetLength(0); i++) {
                    if (Items[i, j].rawitem.id == item.id)
                        l.Add(new Vector2i(i, j));
                }
            }
            return l.ToArray();
        }

        public Vector2i[] GetAllNonFullItemLocations(RawItem item) {
            var l = new List<Vector2i>();
            for (int j = 0; j < Items.GetLength(1); j++) {
                for (int i = 0; i < Items.GetLength(0); i++) {
                    if (Items[i, j].rawitem.id == item.id && Items[i, j].amt < Items[i, j].rawitem.attribs.stackSize)
                        l.Add(new Vector2i(i, j));
                }
            }
            return l.ToArray();
        }

        public Vector2i? GetFirstNonFullItemLocations(RawItem item) {
            var v = GetAllNonFullItemLocations(item);
            if (v.Length == 0) return null;
            return v[0];
        }

        public Vector2i[] GetAllEmptyItemLocations() {
            return GetAllItemLocations(RawItem.None);
        }

        public Vector2i? GetFirstEmptyItemLocation() {
            var v = GetAllEmptyItemLocations();
            if (v.Length == 0) return null;
            return v[0];
        }

        public void SwapItems(Vector2i u, Vector2i v) {
            Item i = Items[u.x, u.y];
            Items[u.x, u.y] = Items[v.x, v.y];
            Items[v.x, v.y] = i;
        }

        public bool AddItem(Item i) {
            if (i.rawitem.id == ItemID.None) return false;
            if (i.amt > i.rawitem.attribs.stackSize) {
                bool b1 = AddItem(new Item(i.rawitem, i.rawitem.attribs.stackSize));
                bool b2 = AddItem(new Item(i.rawitem, i.amt - i.rawitem.attribs.stackSize));
                return b1 && b2;
            }
            var l = GetFirstNonFullItemLocations(i.rawitem);
            if (l == null) {
                var e = GetFirstEmptyItemLocation();
                //no more space in inventory
                if (e == null) return false;
                var e2 = (Vector2i)e;
                Items[e2.x, e2.y] = i;
                return true;
            } else {
                var l2 = (Vector2i)l;
                return AddAmount(l2.x, l2.y, i.amt);
            }
        }

        private bool AddAmount(int x, int y, int amt) {
            Items[x, y].amt += amt;
            if (Items[x, y].amt > Items[x, y].rawitem.attribs.stackSize) {
                var l = GetFirstEmptyItemLocation();
                if (l == null) {
                    Items[x, y].amt -= amt;
                    return false;
                }
                var l2 = (Vector2i)l;
                Items[l2.x, l2.y] = new Item(Items[x, y].rawitem, Items[x, y].amt - Items[x, y].rawitem.attribs.stackSize);
                Items[x, y].amt = Items[x, y].rawitem.attribs.stackSize;
            }
            return true;
        }

        public bool AddItem(RawItem i) {
            return AddItem(new Item(i, 1));
        }

        public bool RemoveItem(Item i) {
            var l = GetAllItemLocations(i.rawitem);
            if (Items[l[0].x, l[0].y].amt < i.amt) return false;
            Items[l[0].x, l[0].y].amt -= i.amt;
            if (Items[l[0].x, l[0].y].amt == 0) Items[l[0].x, l[0].y] = new Item(RawItem.None, 0);
            return true;
        }

        public bool RemoveItem(RawItem i) {
            return RemoveItem(new Item(i, 1));
        }

        public bool RemoveItem(int x, int y, int amt) {
            if (Items[x, y].amt < amt) return false;
            Items[x, y].amt -= amt;
            if (Items[x, y].amt == 0) Items[x, y] = new Item(RawItem.None, 0);
            return true;
        }

        public bool RemoveItem(int x, int y) {
            return RemoveItem(x, y, 1);
        }

        public void LoadItems(Item[,] items) {
            if (items.GetLength(0) > Items.GetLength(0) || items.GetLength(1) > Items.GetLength(1)) {
                throw new ArgumentException("Invalid inventory dimensions");
            }
            for (int i = 0; i < Items.GetLength(0); i++) {
                for (int j = 0; j < Items.GetLength(1); j++) {
                    Items[i, j] = i >= items.GetLength(0) || j >= items.GetLength(1) ? new Item(RawItem.None, 0) : items[i, j];
                }
            }
        }

    }


}
