using System.Collections;

namespace GingerMintSoft.Earth.Location.Solar
{
    public class Roofs
    {
        public List<Roof>? Roof { get; set; } = [];

        public void Add(Roof roof)
        {
            Roof!.Add(roof);
        }

        public void Clear()
        {
            Roof!.Clear();
        }

        public void Remove(Roof roof)
        {
            Roof!.Remove(roof);
        }
    }
}
