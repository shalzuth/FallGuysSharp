using System;

namespace FallGuysMods
{
    public class ModBase
    {
        public String ModName { get; set; } = "Mod Name";
        public virtual Boolean HasConfig { get; set; } = false;
        public virtual Single SliderVal { get; set; } = 5;
        public virtual Single SliderMin { get; set; } = 0;
        public virtual Single SliderMax { get; set; } = 10;
        public Boolean Enabled { get; set; } = false;
        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void OnGUI() { }
        public virtual void OnDisable() { }
        public virtual void OnEnable() { }
    }
}