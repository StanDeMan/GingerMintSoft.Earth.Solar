using System.Collections;

namespace GingerMintSoft.Earth.Location.Solar
{
    public class Roofs
    {
        public List<Roof>? Read { get; set; } = [];

        public void Add(Roof roof)
        {
            Read!.Add(roof);
        }

        public void Clear()
        {
            Read!.Clear();
        }

        public void Remove(Roof roof)
        {
            Read!.Remove(roof);
        }
    }
}
