namespace Game.Terrains.Lighting {
    interface IMultiLight {
        ILight[] Lights();
        int State { get; set; }
    }
}
