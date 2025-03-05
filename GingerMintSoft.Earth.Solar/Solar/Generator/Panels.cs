using System.Collections;

namespace GingerMintSoft.Earth.Location.Solar.Generator
{
    public class Panels
    {
        public List<Panel> Panel { get; set; } = [];

        public void Add(Panel panel)
        {
            Panel.Add(panel);
        }

        public void Remove(Panel panel)
        {
            Panel.Remove(panel);
        }

        public void Clear()
        {
            Panel.Clear();
        }

        public IEnumerator GetEnumerator()
        {
            return Panel.GetEnumerator();
        }
    }
}
